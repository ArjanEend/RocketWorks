using UnityEngine;
using System.Collections;

public class PrefabPoolWrapper : IPoolable {

	private GameObject go;

	public GameObject gameObject{
		get {
			return go;
		}
	}

	public bool active {
		get {
			return go.activeSelf;
		}
		set {
			go.SetActive(value);
		}
	}

	public PrefabPoolWrapper()
	{

	}

	public PrefabPoolWrapper(GameObject spawned)
	{
		spawned.SetActive(false);
		go = spawned;
	}

	public void Start()
	{
		go.SetActive(true);
		go.SendMessage("OnEnable", SendMessageOptions.DontRequireReceiver);
	}

}
