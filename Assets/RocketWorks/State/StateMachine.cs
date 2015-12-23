﻿using UnityEngine;
using System.Collections;

public class StateMachine<T> {
	private T owner;

	private StateBase<T> currentState;
	private StateBase<T> previousState;

	public System.Type currentType
	{
		get{return currentState.GetType();}
	}
	
	public StateMachine(T owner) {
		this.owner = owner;
		currentState = null;
		previousState = null;
	}
	
	public void Update()
	{
	//	Debug.Log ("[StateMachine] Update: " + typeof(T));
		if (currentState != null)
			currentState.Update();
	}
	
	public StateBase<T> ChangeState(StateBase<T> newState)
	{
		previousState = currentState;
		
		if (currentState != null)
			currentState.Exit();
		
		currentState = newState;
		newState.onFinish += ChangeState;
		newState.RegisterState(owner);
		
		if (currentState != null)
			currentState.Initialize();

		return newState;
	}

	public void RevertToPreviousState()
	{
		if (previousState != null)
			ChangeState(previousState);
	}
}
