using RocketWorks.Entities;

public interface IComponentVisualizer
{
    void Init(params IComponent[] components);
    void Init(IComponent component);
    void OnRemove(IComponent component);
}

public interface IUpdateComponent : IComponentVisualizer
{
    void OnUpdate(IComponent component);
}

public interface IComponentVisualizer<T> : IComponent where T : IComponent
{
    void Init(T component);
    void OnRemove(T component);
}

public interface IUpdateComponent<T> : IUpdateComponent where T : IComponent
{
    void OnUpdate(T component);
}