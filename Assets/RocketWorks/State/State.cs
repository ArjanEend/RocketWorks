using System;
public abstract class State<T>   {

	public delegate State<T> OnFinish(State<T> next);

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