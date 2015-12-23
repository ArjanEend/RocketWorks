using System;
public abstract class StateBase<T>   {

	public delegate StateBase<T> OnFinish(StateBase<T> next);

	protected T entity;
	public OnFinish onFinish;
	
	public void RegisterState(T entity)
	{
		this.entity = entity;
	}
	
	virtual public void Initialize (){}
	
	virtual public void Update (){}
	
	virtual public void Exit(){}
}