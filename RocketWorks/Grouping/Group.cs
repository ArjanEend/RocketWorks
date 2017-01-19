using RocketWorks.Entities;
using System;
using System.Collections.Generic;

namespace RocketWorks.Grouping
{
    public class Group
    {
        public int Count
        {
            get { return entities.Count; }
        }
        public Entity this[int index]
        {
            get { return entities[index]; }
        }

        private int composition;
        public int Composition
        {
            get { return composition; }
            internal set { composition = value; }
        }

        private Action<Entity> onEntityAdded;
        public Action<Entity> OnEntityAdded
        {
            get { return onEntityAdded; }
            set { onEntityAdded = value; }
        }

        private Action<Entity> onEntityRemoved;
        public Action<Entity> OnEntityRemoved
        {
            get { return onEntityRemoved; }
            set { onEntityRemoved = value; }
        }

        private List<Entity> entities;
        public List<Entity> Entities
        {
            get { return entities; }
            set { entities = value; }
        }

       // private Dictionary<Type, List<IComponent>> componentBindings = 
       //     new Dictionary<Type, List<IComponent>>();

        public Group(params Type[] types)
        {
            entities = new List<Entity>();
            for(int i = 0; i < types.Length; i++)
            {
        //        componentBindings.Add(types[i], new List<IComponent>());
            }
        }

        public void AddEntity(Entity entity)
        {
            entities.Add(entity);
            //entity.CompositionChangeEvent += OnEntityChanged;
        }

       // private int OnEntityChanged(IComponent comp, Entity entity)
        //{
            //if (!entity.HasComponents(composition))
            //    entities.Remove(entity);

           // return -1;
        //}

        public bool HasComponents(int components)
        {
            return (composition & components) != 0;
        }

        public void RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
        }
    }
}
