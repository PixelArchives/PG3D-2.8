using System.Collections;
using UnityEngine;

public class Player_move_c : MonoBehaviour
{
	public GUISkin MySkin;

	public Texture2D ammoTexture;

	public Texture2D scoreTexture;

	public Texture HeartTexture;

	public Texture buyTexture;

	public Texture2D AimTexture;

	public int AimTextureWidth = 50;

	public int AimTextureHeight = 50;

	private Transform GunFlash;

	public int BulletForce = 5000;

	public GameObject renderAllObjectPrefab;

	private Texture zaglushkaTexture;

	private GameObject _leftJoystick;

	private GameObject _rightJoystick;

	private int _curHealth;

	public int MaxHealth;

	public int AmmoBoxWidth = 100;

	public int AmmoBoxHeight = 100;

	public int AmmoBoxOffset = 10;

	public int ScoreBoxWidth = 100;

	public int ScoreBoxHeight = 100;

	public int ScoreBoxOffset = 10;

	private float GunFlashLifetime;

	public GUIStyle ScoreBox;

	public GUIStyle AmmoBox;

	public GUIStyle HealthBox;

	public GUIStyle pauseStyle;

	public GUIStyle pauseFonStyle;

	public GUIStyle resumeStyle;

	public GUIStyle menuStyle;

	public GUIStyle soundStyle;

	public GUIStyle buyStyle;

	public GUIStyle enemiesLeftStyle;

	private GameObject damage;

	private bool damageShown;

	public Texture pauseFon;

	private Pauser _pauser;

	public Texture pauseTitle;

	private GameObject _gameController;

	private bool _backWasPressed;

	private GameObject _player;

	private GameObject bullet;

	private GameObject _bulletSpawnPoint;

	private ZombieCreator _zombieCreator;

	public WeaponManager _weaponManager;

	private Vector2 leftFingerPos = Vector2.zero;

	private Vector2 leftFingerLastPos = Vector2.zero;

	private Vector2 leftFingerMovedBy = Vector2.zero;

	private bool canReceiveSwipes = true;

	public float slideMagnitudeX;

	public float slideMagnitudeY;

	public AudioClip ChangeWeaponClip;

	private float height = (float)Screen.height * 0.078f;

	private float _width = 125f;

	public static int MaxPlayerHealth
	{
		get
		{
			return 9;
		}
	}

	public int CurHealth
	{
		get
		{
			return _curHealth;
		}
		set
		{
			_curHealth = value;
		}
	}

	public int curHealthProp
	{
		get
		{
			return CurHealth;
		}
		set
		{
			if (CurHealth > value && !damageShown)
			{
				StartCoroutine(FlashWhenHit());
			}
			CurHealth = value;
		}
	}

	private void WalkAnimation()
	{
		_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>().CrossFade("Walk");
	}

	private void IdleAnimation()
	{
		_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>().CrossFade("Idle");
	}

	public void AddWeapon(GameObject weaponPrefab)
	{
		int score;
		if (_weaponManager.AddWeapon(weaponPrefab, out score))
		{
			ChangeWeapon(_weaponManager.CurrentWeaponIndex, false);
			return;
		}
		GlobalGameController.Score += score;
		base.gameObject.GetComponent<AudioSource>().PlayOneShot(ChangeWeaponClip);
	}

	public void ChangeWeapon(int index, bool shouldSetMaxAmmo = true)
	{
		Quaternion rotation = _player.transform.rotation;
		if ((bool)_weaponManager.currentWeaponSounds)
		{
			rotation = _weaponManager.currentWeaponSounds.gameObject.transform.rotation;
			_weaponManager.currentWeaponSounds.gameObject.transform.parent = null;
			Object.Destroy(_weaponManager.currentWeaponSounds.gameObject);
			_weaponManager.currentWeaponSounds = null;
		}
		GameObject gameObject = (GameObject)Object.Instantiate(((Weapon)_weaponManager.playerWeapons[index]).weaponPrefab, Vector3.zero, Quaternion.identity);
		gameObject.transform.parent = base.gameObject.transform;
		gameObject.transform.rotation = rotation;
		_weaponManager.CurrentWeaponIndex = index;
		_weaponManager.currentWeaponSounds = gameObject.GetComponent<WeaponSounds>();
		gameObject.transform.position = gameObject.transform.parent.TransformPoint(_weaponManager.currentWeaponSounds.gunPosition);
		_rightJoystick.SendMessage("setSeriya", _weaponManager.currentWeaponSounds.isSerialShooting);
		if (shouldSetMaxAmmo)
		{
		}
		if (((Weapon)_weaponManager.playerWeapons[_weaponManager.CurrentWeaponIndex]).currentAmmoInClip > 0 || _weaponManager.currentWeaponSounds.isMelee)
		{
			_rightJoystick.SendMessage("HasAmmo");
		}
		else
		{
			_rightJoystick.SendMessage("NoAmmo");
		}
		_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>()["Reload"].layer = 1;
		_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>()["Shoot"].layer = 1;
		if (!_weaponManager.currentWeaponSounds.isMelee)
		{
			foreach (Transform item in _weaponManager.currentWeaponSounds.gameObject.transform)
			{
				if (item.name.Equals("BulletSpawnPoint"))
				{
					_bulletSpawnPoint = item.gameObject;
					break;
				}
			}
			GunFlash = GameObject.Find("GunFlash").transform;
		}
		SendSpeedModifier();
		base.gameObject.GetComponent<AudioSource>().PlayOneShot(ChangeWeaponClip);
	}

	private void SendSpeedModifier()
	{
		_player.SendMessage("SetSpeedModifier", _weaponManager.currentWeaponSounds.speedModifier);
	}

	public bool NeedAmmo()
	{
		int currentWeaponIndex = _weaponManager.CurrentWeaponIndex;
		Weapon weapon = (Weapon)_weaponManager.playerWeapons[currentWeaponIndex];
		return weapon.currentAmmoInBackpack < _weaponManager.currentWeaponSounds.MaxAmmoWithRespectToInApp;
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void Start()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        NewInput.mouseLocked = true;
#endif
		zaglushkaTexture = Resources.Load("zaglushka") as Texture;
		_player = GameObject.FindGameObjectWithTag("Player");
		_weaponManager = GameObject.FindGameObjectWithTag("WeaponManager").GetComponent<WeaponManager>();
		bullet = GameObject.Find("Bullet");
		bullet.GetComponent<Bullet>().enabled = false;
		damage = GameObject.Find("Blood");
		_gameController = GameObject.FindGameObjectWithTag("GameController");
		_zombieCreator = _gameController.GetComponent<ZombieCreator>();
		_pauser = GameObject.FindGameObjectWithTag("GameController").GetComponent<Pauser>();
		_leftJoystick = GameObject.Find("LeftTouchPad");
		_rightJoystick = GameObject.Find("RightTouchPad");
		ChangeWeapon(_weaponManager.CurrentWeaponIndex, false);
		_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>()["Reload"].layer = 1;
		_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>().Stop();
		_SetGunFlashActive(false);
		CurHealth = PlayerPrefs.GetInt(Defs.CurrentHealthSett, MaxPlayerHealth);
		Color color = damage.GetComponent<GUITexture>().color;
		color.a = 0f;
		damage.GetComponent<GUITexture>().color = color;
		Invoke("SendSpeedModifier", 0.5f);
		GameObject gameObject = (GameObject)Object.Instantiate(renderAllObjectPrefab, Vector3.zero, Quaternion.identity);
	}

	private void _SetGunFlashActive(bool state)
	{
		if (!_weaponManager.currentWeaponSounds.isMelee && GunFlash != null)
		{
			GunFlash.gameObject.active = state;
		}
	}

	private void ReloadPressed()
	{
		if (!_weaponManager.currentWeaponSounds.isMelee && _weaponManager.CurrentWeaponIndex >= 0 && _weaponManager.CurrentWeaponIndex < _weaponManager.playerWeapons.Count && ((Weapon)_weaponManager.playerWeapons[_weaponManager.CurrentWeaponIndex]).currentAmmoInBackpack > 0 && ((Weapon)_weaponManager.playerWeapons[_weaponManager.CurrentWeaponIndex]).currentAmmoInClip != _weaponManager.currentWeaponSounds.ammoInClip)
		{
			_weaponManager.Reload();
			base.GetComponent<AudioSource>().PlayOneShot(_weaponManager.currentWeaponSounds.reload);
			_rightJoystick.SendMessage("HasAmmo");
		}
	}

	private void ShotPressed()
	{
		if (_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>().IsPlaying("Shoot") || _weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>().IsPlaying("Reload"))
		{
			return;
		}
		_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>().Stop();
		if (_weaponManager.currentWeaponSounds.isMelee)
		{
			_Shot();
		}
		else if (((Weapon)_weaponManager.playerWeapons[_weaponManager.CurrentWeaponIndex]).currentAmmoInClip > 0)
		{
			((Weapon)_weaponManager.playerWeapons[_weaponManager.CurrentWeaponIndex]).currentAmmoInClip--;
			if (((Weapon)_weaponManager.playerWeapons[_weaponManager.CurrentWeaponIndex]).currentAmmoInClip == 0)
			{
				_rightJoystick.SendMessage("NoAmmo");
			}
			_Shot();
			_SetGunFlashActive(true);
			GunFlashLifetime = 0.1f;
		}
		else
		{
			_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>().Play("Empty");
			base.GetComponent<AudioSource>().PlayOneShot(_weaponManager.currentWeaponSounds.empty);
		}
	}

	private void _Shot()
	{
		_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>().Play("Shoot");
		shootS();
		base.GetComponent<AudioSource>().PlayOneShot(_weaponManager.currentWeaponSounds.shoot);
	}

	public void shootS()
	{
		if (!_weaponManager.currentWeaponSounds.isMelee)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(bullet, _bulletSpawnPoint.transform.position, Quaternion.LookRotation(Camera.main.transform.TransformDirection(Vector3.forward)));
			gameObject.GetComponent<Bullet>().enabled = true;
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f));
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 100f, -5) && (bool)hitInfo.collider.gameObject.transform.parent && hitInfo.collider.gameObject.transform.parent.CompareTag("Enemy"))
			{
				BotHealth component = hitInfo.collider.gameObject.transform.parent.GetComponent<BotHealth>();
				component.adjustHealth((float)(-_weaponManager.currentWeaponSounds.damage) + Random.Range(_weaponManager.currentWeaponSounds.damageRange.x, _weaponManager.currentWeaponSounds.damageRange.y), Camera.main.transform);
			}
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Enemy");
		GameObject gameObject2 = null;
		float num = float.PositiveInfinity;
		GameObject[] array2 = array;
		foreach (GameObject gameObject3 in array2)
		{
			Vector3 to = gameObject3.transform.position - _player.transform.position;
			float magnitude = to.magnitude;
			if (magnitude < num && ((magnitude < _weaponManager.currentWeaponSounds.range && Vector3.Angle(_player.transform.forward, to) < 45f) || magnitude < 1.5f))
			{
				num = magnitude;
				gameObject2 = gameObject3;
			}
		}
		if ((bool)gameObject2)
		{
			StartCoroutine(HitByMelee(gameObject2));
		}
	}

	private IEnumerator HitByMelee(GameObject enemyToHit)
	{
		yield return new WaitForSeconds(_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>()["Shoot"].length * 0.57f);
		enemyToHit.GetComponent<BotHealth>().adjustHealth((float)(-_weaponManager.currentWeaponSounds.damage) + Random.Range(_weaponManager.currentWeaponSounds.damageRange.x, _weaponManager.currentWeaponSounds.damageRange.y), Camera.main.transform);
	}

	private IEnumerator Fade(float start, float end, float length, GameObject currentObject)
	{
		for (float i = 0f; i < 1f; i += Time.deltaTime / length)
		{
			Color rgba = currentObject.GetComponent<GUITexture>().color;
			rgba.a = Mathf.Lerp(start, end, i);
			currentObject.GetComponent<GUITexture>().color = rgba;
			yield return 0;
			Color rgba_ = currentObject.GetComponent<GUITexture>().color;
			rgba_.a = end;
			currentObject.GetComponent<GUITexture>().color = rgba_;
		}
	}

	private IEnumerator FlashWhenHit()
	{
		damageShown = true;
		Color rgba = damage.GetComponent<GUITexture>().color;
		rgba.a = 0f;
		damage.GetComponent<GUITexture>().color = rgba;
		float danageTime = 0.15f;
		yield return StartCoroutine(Fade(0f, 1f, danageTime, damage));
		yield return new WaitForSeconds(0.01f);
		yield return StartCoroutine(Fade(1f, 0f, danageTime, damage));
		damageShown = false;
	}

	private IEnumerator SetCanReceiveSwipes()
	{
		yield return new WaitForSeconds(0.1f);
		canReceiveSwipes = true;
	}

	private void Update()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        if (NewInput.pause) SetPause();
#endif
        if (!_pauser.paused && canReceiveSwipes)
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            if (NewInput.mouseLocked)
            {
                if (NewInput.Shooting(_weaponManager)) ShotPressed();
                if (NewInput.reload) ReloadPressed();
            }
			if (canReceiveSwipes && NewInput.mouseScroll != 0)
			{
				if (NewInput.mouseScroll >= 0.1f)
				{
					_weaponManager.CurrentWeaponIndex++;
					int count = _weaponManager.playerWeapons.Count;
					count = ((count == 0) ? 1 : count);
					_weaponManager.CurrentWeaponIndex %= count;
					ChangeWeapon(_weaponManager.CurrentWeaponIndex, false);
					canReceiveSwipes = false;
					StartCoroutine(SetCanReceiveSwipes());
				}
				else if (NewInput.mouseScroll <= -0.1f)
				{
					_weaponManager.CurrentWeaponIndex--;
					if (_weaponManager.CurrentWeaponIndex < 0)
					{
						_weaponManager.CurrentWeaponIndex = _weaponManager.playerWeapons.Count - 1;
					}
					_weaponManager.CurrentWeaponIndex %= _weaponManager.playerWeapons.Count;
					ChangeWeapon(_weaponManager.CurrentWeaponIndex, false);
					canReceiveSwipes = false;
					StartCoroutine(SetCanReceiveSwipes());
				}
			}
#else
            Rect rect = new Rect((float)Screen.width - 264f * (float)Screen.width / 1024f, (float)Screen.height - 94f * (float)Screen.width / 1024f - 95f * (float)Screen.width / 1024f, 264f * (float)Screen.width / 1024f, 95f * (float)Screen.width / 1024f);
			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++)
			{
				Touch touch = touches[i];
				if (!rect.Contains(touch.position))
				{
					continue;
				}
				if (touch.phase == TouchPhase.Began)
				{
					leftFingerPos = Vector2.zero;
					leftFingerLastPos = Vector2.zero;
					leftFingerMovedBy = Vector2.zero;
					slideMagnitudeX = 0f;
					slideMagnitudeY = 0f;
					leftFingerPos = touch.position;
				}
				else if (touch.phase == TouchPhase.Moved)
				{
					leftFingerMovedBy = touch.position - leftFingerPos;
					leftFingerLastPos = leftFingerPos;
					leftFingerPos = touch.position;
					slideMagnitudeX = leftFingerMovedBy.x / (float)Screen.width;
					//Debug.Log("slideMagnitudeX = " + slideMagnitudeX);
                    float num = 0.02f;
					if (slideMagnitudeX > num)
                    {
                        _weaponManager.CurrentWeaponIndex++;
						int count = _weaponManager.playerWeapons.Count;
						count = ((count == 0) ? 1 : count);
						_weaponManager.CurrentWeaponIndex %= count;
						ChangeWeapon(_weaponManager.CurrentWeaponIndex, false);
						canReceiveSwipes = false;
						StartCoroutine(SetCanReceiveSwipes());
						leftFingerLastPos = leftFingerPos;
						leftFingerPos = touch.position;
						slideMagnitudeX = 0f;
                    }
                    else if (slideMagnitudeX < 0f - num)
                    {
                        _weaponManager.CurrentWeaponIndex--;
						if (_weaponManager.CurrentWeaponIndex < 0)
						{
							_weaponManager.CurrentWeaponIndex = _weaponManager.playerWeapons.Count - 1;
						}
						_weaponManager.CurrentWeaponIndex %= _weaponManager.playerWeapons.Count;
						ChangeWeapon(_weaponManager.CurrentWeaponIndex, false);
						canReceiveSwipes = false;
						StartCoroutine(SetCanReceiveSwipes());
						leftFingerLastPos = leftFingerPos;
						leftFingerPos = touch.position;
						slideMagnitudeX = 0f;
					}
					slideMagnitudeY = leftFingerMovedBy.y / (float)Screen.height;
				}
				else if (touch.phase == TouchPhase.Stationary)
				{
					leftFingerLastPos = leftFingerPos;
					leftFingerPos = touch.position;
					slideMagnitudeX = 0f;
					slideMagnitudeY = 0f;
				}
				else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
				{
					slideMagnitudeX = 0f;
					slideMagnitudeY = 0f;
				}
			}
#endif
		}
		if (GunFlashLifetime > 0f)
		{
			GunFlashLifetime -= Time.deltaTime;
		}
		if (GunFlashLifetime <= 0f)
		{
			_SetGunFlashActive(false);
		}
		if (CurHealth <= 0)
		{
			if (GlobalGameController.Score > PlayerPrefs.GetInt(Defs.BestScoreSett, 0))
			{
				PlayerPrefs.SetInt(Defs.BestScoreSett, GlobalGameController.Score);
				PlayerPrefs.Save();
			}
			Application.LoadLevel("GameOver");
		}
	}

	private void OnGUI()
	{
		GUI.skin = MySkin;
		GUI.DrawTexture(new Rect((float)(Screen.width / 2) - (float)Screen.height * 0.023f, (float)(Screen.height / 2) - (float)Screen.height * 0.023f, (float)Screen.height * 0.046f, (float)Screen.height * 0.046f), AimTexture);
		Rect rect = new Rect((float)Screen.width - (float)Screen.width * 0.208f, 0f, (float)Screen.width * 0.208f, (float)Screen.height * 0.078f);
		AmmoBox.fontSize = Mathf.RoundToInt(18f * (float)Screen.width / 1024f);
		if (_weaponManager.CurrentWeaponIndex >= 0 && _weaponManager.CurrentWeaponIndex < _weaponManager.playerWeapons.Count)
		{
			GUI.DrawTexture(new Rect((float)Screen.width - 172f * (float)Screen.width / 1024f, 186f * (float)Screen.width / 1024f, (float)(40 * Screen.width) / 1024f, (float)(28 * Screen.width) / 1024f), ammoTexture);
			GUI.Box(new Rect((float)Screen.width - 135f * (float)Screen.width / 1024f, 186f * (float)Screen.width / 1024f, 130f * (float)Screen.width / 1024f, (float)(28 * Screen.width) / 1024f), ((Weapon)_weaponManager.playerWeapons[_weaponManager.CurrentWeaponIndex]).currentAmmoInClip + "/" + ((Weapon)_weaponManager.playerWeapons[_weaponManager.CurrentWeaponIndex]).currentAmmoInBackpack, AmmoBox);
		}
		else
		{
			Debug.Log("_weaponManager.CurrentWeaponIndex: " + _weaponManager.CurrentWeaponIndex);
		}
		ScoreBox.fontSize = Mathf.RoundToInt((float)Screen.height * 0.035f);
		GUI.DrawTexture(new Rect(73f * (float)Screen.width / 1024f, 0f, 173f * (float)Screen.width / 1024f, 73f * (float)Screen.width / 1024f), scoreTexture);
		GUI.Box(new Rect(73f * (float)Screen.width / 1024f, 35f * (float)Screen.width / 1024f, 173f * (float)Screen.width / 1024f, (float)Screen.height * 0.035f), string.Empty + GlobalGameController.Score, ScoreBox);
		bool flag = true;
		float left = (float)Screen.width * 0.271f;
		float width = (float)Screen.width * 0.521f;
		GUI.Box(new Rect(246f * (float)Screen.width / 1024f, 0f, 636f * (float)Screen.width / 1024f, 73f * (float)Screen.width / 1024f), string.Empty, HealthBox);
		float num = 45.4f - (float)HeartTexture.width;
		float num2 = (float)Screen.height * 0.068f - (float)Screen.width * 0.521f * 0.07f;
		for (int i = 0; i < CurHealth; i++)
		{
			GUI.DrawTexture(new Rect(257f * (float)Screen.width / 1024f + (float)i * (70.5f * (float)Screen.width / 1024f), 12f * (float)Screen.width / 1024f, 52f * (float)Screen.width / 1024f, 49f * (float)Screen.width / 1024f), HeartTexture);
		}
		GUI.DrawTexture(new Rect((float)Screen.width - 264f * (float)Screen.width / 1024f, 94f * (float)Screen.width / 1024f, 264f * (float)Screen.width / 1024f, 95f * (float)Screen.width / 1024f), _weaponManager.currentWeaponSounds.preview);
		if ((bool)_weaponManager && _weaponManager.playerWeapons != null && _weaponManager.playerWeapons.Count > 1)
		{
			GUI.Box(new Rect((float)Screen.width - 153f * (float)Screen.width / 1024f, 94f * (float)Screen.width / 1024f, 153f * (float)Screen.width / 1024f, 23f * (float)Screen.width / 1024f), "<SWIPE>", ScoreBox);
		}
		bool flag2 = false;
		if (Application.platform == RuntimePlatform.Android)
		{
			if (Input.GetKey(KeyCode.Escape))
			{
				_backWasPressed = true;
				Debug.Log("407 androidBackPressed " + flag2);
			}
			else
			{
				if (_backWasPressed)
				{
					flag2 = true;
				}
				_backWasPressed = false;
			}
        }
//#if !UNITY_STANDALONE
		if ((GUI.Button(new Rect(0f, 0f, 73f * (float)Screen.width / 1024f, 73f * (float)Screen.width / 1024f), string.Empty, pauseStyle) || (flag2 && !_pauser.paused)) && !_pauser.paused)
		{
			flag2 = false;
			if (CurHealth > 0)
			{
				SetPause();
			}
		}
//#endif
		int num3 = GlobalGameController.EnemiesToKill - _zombieCreator.NumOfDeadZombies;
		string text = ((num3 <= 0) ? "Enter the Portal..." : ("Enemies left: " + num3));
		enemiesLeftStyle.fontSize = Mathf.RoundToInt((float)Screen.height * 0.035f);
		GUI.Box(new Rect(left, 90f * (float)Screen.width / 1024f, width, height), text, enemiesLeftStyle);
		Rect position = new Rect(882f * (float)Screen.width / 1024f, 0f, 142f * (float)Screen.width / 1024f, 73f * (float)Screen.width / 1024f);
		GUI.DrawTexture(position, zaglushkaTexture);
		if ((bool)_pauser && _pauser.paused)
		{
			Rect position2 = new Rect(((float)Screen.width - 2048f * (float)Screen.height / 1154f) / 2f, 0f, 2048f * (float)Screen.height / 1154f, Screen.height);
			GUI.DrawTexture(position2, pauseFon, ScaleMode.StretchToFill);
			float num4 = 15f;
			Vector2 vector = new Vector2(176f, 150f - num4);
			GUI.DrawTexture(new Rect((float)Screen.width * 0.57f, (float)Screen.height * 0.2f, (float)Screen.height * 0.4781f, (float)Screen.height * 0.1343f), pauseTitle);
			if (GUI.Button(new Rect((float)Screen.width * 0.57f + (float)Screen.height * 0.129f, (float)Screen.height * 0.38f, (float)Screen.height * 0.22f, (float)Screen.height * 0.078f), string.Empty, resumeStyle) || flag2)
			{
				SetPause();
			}
			if (GUI.Button(new Rect((float)Screen.width * 0.57f + (float)Screen.height * 0.129f, (float)Screen.height * 0.5f, (float)Screen.height * 0.22f, (float)Screen.height * 0.078f), string.Empty, menuStyle))
			{
				Time.timeScale = 1f;
				Application.LoadLevel("Restart");
			}
			float num5 = 15f;
			bool @bool = PlayerPrefsX.GetBool(PlayerPrefsX.SndSetting, true);
			@bool = GUI.Toggle(new Rect((float)Screen.width * 0.05f, (float)Screen.height * 0.923f - (float)Screen.height * 0.0525f, (float)Screen.height * 0.105f, (float)Screen.height * 0.105f), @bool, string.Empty, soundStyle);
			AudioListener.volume = (@bool ? 1 : 0);
			PlayerPrefsX.SetBool(PlayerPrefsX.SndSetting, @bool);
			PlayerPrefs.Save();
		}
	}

	private void SetPause()
	{
		_pauser.paused = !_pauser.paused;

#if UNITY_EDITOR || UNITY_STANDALONE
        NewInput.mouseLocked = !_pauser.paused;
#endif
        if (_pauser.paused)
		{
			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = 1f;
		}
	}

#if UNITY_EDITOR || UNITY_STANDALONE
	void OnDestroy()
	{
		NewInput.mouseLocked = false;
    }
#endif
}
