using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrefabPool : ObjectPool<PrefabPoolWrapper> {

	private GameObject prefab;

	public PrefabPool(){}

	public PrefabPool(GameObject prefab, int amount, bool flexible)
	{
		this.flexible = flexible;
		this.prefab = prefab;
		objects = new List<PrefabPoolWrapper>();
		GeneratePool(amount);
	}

	public GameObject GetNext()
	{
		PrefabPoolWrapper instance = GetObject();
		instance.Reset();
		return instance.gameObject;
	}
	
	override protected PrefabPoolWrapper CreateObject()
	{
		GameObject go = (GameObject)GameObject.Instantiate(prefab);
		PrefabPoolWrapper instance = new PrefabPoolWrapper(go);
		objects.Add(instance);
		return instance;
	}
	
}
