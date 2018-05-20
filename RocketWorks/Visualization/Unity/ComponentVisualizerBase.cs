using System;
using RocketWorks.Entities;
using UnityEngine;

public abstract class ComponentVisualizerBase : MonoBehaviour, IComponentVisualizer
{
    public abstract void Init(IComponent component);
    public abstract void OnRemove(IComponent component);

    public void Init(params IComponent[] components)
    {
        for(int i = 0; i < components.Length; i++)
        {
            if(components[i] != null)
                Init(components[i]);
        }
    }
}

public abstract class ComponentVisualizerBase<T1> : ComponentVisualizerBase, IComponentVisualizer<T1> 
    where T1 : IComponent
{
    public abstract void Init(T1 component);
    public abstract void OnRemove(T1 component);
    public override void Init(IComponent component)
    {
        if (component is T1)
            Init((T1)component);
    }
    public override void OnRemove(IComponent component)
    {
        if (component is T1)
            OnRemove((T1)component);
    }
}

public abstract class ComponentUpdaterBase<T1> : ComponentVisualizerBase<T1>, IUpdateComponent<T1>
    where T1 : IComponent
{
    public virtual void OnUpdate(IComponent component)
    {
        if (component is T1)
            OnUpdate((T1)component);
    }

    public abstract void OnUpdate(T1 component);
}

public abstract class ComponentVisualizerBase<T1, T2> : ComponentUpdaterBase<T1>, IComponentVisualizer<T2> 
    where T1 : IComponent where T2 : IComponent
{
    public abstract void Init(T2 component);
    public abstract void OnRemove(T2 component);
    public override void Init(IComponent component)
    {
        base.Init(component);
        if (component is T2)
            Init((T2)component);
    }
    public override void OnRemove(IComponent component)
    {
        if (component is T2)
            OnRemove((T2)component);
    }
}

public abstract class ComponentUpdaterBase<T1, T2> : ComponentUpdaterBase<T1>, IUpdateComponent<T2>
    where T1 : IComponent where T2 : IComponent
{
    public abstract void Init(T2 component);
    public abstract void OnRemove(T2 component);
    public override void Init(IComponent component)
    {
        base.Init(component);
        if (component is T2)
            Init((T2)component);
    }
    public override void OnRemove(IComponent component)
    {
        if (component is T2)
            OnRemove((T2)component);
    }
    public override void OnUpdate(IComponent component)
    {
        base.OnUpdate(component);
        if (component is T2)
            OnUpdate((T2)component);
    }
    
    public abstract void OnUpdate(T2 component);
}

public abstract class ComponentVisualizerBase<T1, T2, T3> : ComponentVisualizerBase<T1, T2>, IComponentVisualizer<T3> 
    where T1 : IComponent where T2 : IComponent where T3 : IComponent
{
    public abstract void Init(T3 component);
    public abstract void OnRemove(T3 component);
    public override void Init(IComponent component)
    {
        base.Init(component);
        if (component is T3)
            Init((T3)component);
    }
    public override void OnRemove(IComponent component)
    {
        if (component is T3)
            OnRemove((T3)component);
    }
}

public abstract class ComponentUpdaterBase<T1, T2, T3> : ComponentUpdaterBase<T1, T2>, IUpdateComponent<T3>
    where T1 : IComponent where T2 : IComponent where T3 : IComponent
{
    public abstract void Init(T3 component);
    public abstract void OnRemove(T3 component);
    public override void Init(IComponent component)
    {
        base.Init(component);
        if (component is T3)
            Init((T3)component);
    }
    public override void OnRemove(IComponent component)
    {
        if (component is T3)
            OnRemove((T3)component);
    }
    public override void OnUpdate(IComponent component)
    {
        base.OnUpdate(component);
        if (component is T3)
            OnUpdate((T3)component);
    }

    public abstract void OnUpdate(T3 component);
}