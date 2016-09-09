﻿using RocketWorks.Entities;
using System;
using System.Collections.Generic;

namespace RocketWorks.Grouping
{
    public class Group
    {
        private int composition;
        public int Composition
        {
            get { return composition; }
            internal set { composition = value; }
        }

        private Dictionary<Type, List<IComponent>> componentBindings = 
            new Dictionary<Type, List<IComponent>>();

        public Group(params Type[] types)
        {
            for(int i = 0; i < types.Length; i++)
            {
                componentBindings.Add(types[i], new List<IComponent>());
            }
        }

        public void AddComponent(IComponent component)
        {
            Type type = component.GetType();
            componentBindings[type].Add(component);
        }

        public bool HasComponents(int components)
        {
            return (composition & components) != 0;
        }

        public List<IComponent> GetComponents(System.Type type)
        {
            return componentBindings[type];
        }

    }
}
