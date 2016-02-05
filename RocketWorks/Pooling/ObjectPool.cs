using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool<T> where T : IPoolable, new()
{

	protected bool flexible = false;
	protected List<T> objects;

	private int currentIndex;

	public ObjectPool(){}
	public ObjectPool(int amount, bool flexible)
	{
		this.flexible = flexible;
		objects = new List<T>();
		GeneratePool(amount);
	}

	protected void GeneratePool(int amount)
	{
		for(int i = 0; i < amount; i++)
		{
			CreateObject();
		}
	}

	public virtual T GetObject()
	{
		int index;
		for(int i = 0; i < objects.Count; i++)
		{
			index = (i + currentIndex) % (objects.Count);
			if(!objects[index].Alive)
			{
				currentIndex = index;
				return objects[index];
			}
		}

		if(flexible)
		{
			Debug.Log ("[ObjectPool] flexible, spawning new object: " + typeof(T).ToString());
			return CreateObject();
		} else {
			Debug.Log ("[ObjectPool] non-flexible, recycling object: " + currentIndex);
			currentIndex++;
			currentIndex %= objects.Count;
			return objects[currentIndex];
		}
	}

	protected virtual T CreateObject()
	{
		T instance = new T();
		objects.Add(instance);
		return instance;
	}

}
