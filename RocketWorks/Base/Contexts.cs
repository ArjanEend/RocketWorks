using RocketWorks.Entities;
using System.Collections.Generic;

public partial class Contexts
{
    private List<EntityContext> contexts;

    public T GetContext<T>() where T : EntityContext
    {
        for(int i = 0; i < contexts.Count; i++)
        {
            if (contexts[i].GetType() == typeof(T))
                return contexts[i] as T;
        }
        return null;
    }
}
