using RocketWorks.Entities;
using System.Collections.Generic;

public partial class Contexts
{
    private List<EntityContext> contexts;

    public Contexts()
    {
        contexts = new List<EntityContext>();
        Populate();
    }

    partial void Populate();

    public T GetContext<T>() where T : EntityContext
    {
        for(int i = 0; i < contexts.Count; i++)
        {
            if ((T)contexts[i] != null)
                return contexts[i] as T;
        }
        return null;
    }
}
