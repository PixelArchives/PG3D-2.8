using UnityEngine;

public class GotToNextLevel : MonoBehaviour
{
	private GameObject _player;

	private GameObject _fpc;

	private Player_move_c _playerMoveC;

	private void Start()
	{
		_player = GameObject.FindGameObjectWithTag("Player");
		_playerMoveC = GameObject.FindGameObjectWithTag("PlayerGun").GetComponent<Player_move_c>();
	}

	private void Update()
	{
		if (Vector3.Distance(base.transform.position, _player.transform.position) < 1.5f)
		{
			PlayerPrefs.SetInt(Defs.CurrentHealthSett, _playerMoveC.CurHealth);
			AutoFade.LoadLevel("Loading", 0.5f, 0.5f, Color.white);
		}
	}
}
