﻿using RocketWorks.Entities;
using System.Collections.Generic;

public partial class Contexts
{
    protected List<EntityContext> contexts;

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
            if (contexts[i] is T)
                return contexts[i] as T;
        }
        return null;
    }

    public void ClearPools()
    {
        for(int i = 0; i < contexts.Count; i++)
        {
            //contexts[i].Pool.ResetAll();
        }
    }
}
