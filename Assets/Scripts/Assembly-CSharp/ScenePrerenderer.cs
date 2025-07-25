using UnityEngine;

public class ScenePrerenderer : MonoBehaviour
{
	public Camera activeCamera;

	private RenderTexture _rt;

	public bool FinishPrerendering;

	public GameObject[] objsToRender;

	private GameObject _enemiesToRender;

	private void Awake()
	{
		_rt = new RenderTexture(32, 32, 24);
		_rt.Create();
		activeCamera.targetTexture = _rt;
		activeCamera.useOcclusionCulling = false;
	}

	private void Start()
	{
		Render_();
	}

	private void Render_()
	{
		Transform[] zombiePrefabs = GameObject.FindGameObjectWithTag("GameController").GetComponent<ZombieCreator>().zombiePrefabs;
		GameObject[] array = new GameObject[zombiePrefabs.Length];
		int num = 0;
		Transform[] array2 = zombiePrefabs;
		foreach (Transform transform in array2)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(transform.GetChild(0).gameObject, new Vector3(base.transform.position.x, base.transform.position.y - 20f, base.transform.position.z), transform.GetChild(0).gameObject.transform.rotation);
			string text = "(Clone)";
			int num2 = gameObject.name.IndexOf(text);
			gameObject.name = ((num2 >= 0) ? gameObject.name.Remove(num2, text.Length) : gameObject.name);
			gameObject.transform.parent = base.transform.parent;
			BotHealth.SetSkinForObj(gameObject);
			array[num] = gameObject;
			num++;
		}
		activeCamera.Render();
		RenderTexture.active = _rt;
		activeCamera.targetTexture = null;
		RenderTexture.active = null;
		GameObject[] array3 = objsToRender;
		foreach (GameObject obj in array3)
		{
			Object.Destroy(obj);
		}
		Object.Destroy(base.transform.parent.gameObject);
		Object.Destroy(activeCamera);
	}
}
