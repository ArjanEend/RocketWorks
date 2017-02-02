using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RocketWorks.Pooling;

public class PrefabPool : ObjectPool<PrefabPoolWrapper> {

	private GameObject prefab;

	public PrefabPool(GameObject prefab, int amount, bool flexible) : base(amount, flexible)
	{
		this.flexible = flexible;
		this.prefab = prefab;
		GeneratePool(amount);
	}

	public GameObject GetNext(bool autoStart = true)
	{
		PrefabPoolWrapper instance = GetObject();
		
		if(autoStart)
			instance.Reset();
		
		return instance.gameObject;
	}
	
	override protected PrefabPoolWrapper CreateObject()
	{
		GameObject go = (GameObject)GameObject.Instantiate(prefab);
		PrefabPoolWrapper instance = new PrefabPoolWrapper(go);
		activeObjects.Add(instance);
		return instance;
	}
	
}
