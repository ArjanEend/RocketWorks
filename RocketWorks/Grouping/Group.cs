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

        private Action<Entity> onEntityAdded = delegate { };
        public Action<Entity> OnEntityAdded
        {
            get { return onEntityAdded; }
            set { onEntityAdded = value; }
        }

        private Action<Entity> onEntityRemoved = delegate { };
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

        private List<Entity> newEntities;
        public List<Entity> NewEntities
        {
            get {
                List<Entity> returnValue = newEntities;
                newEntities = new List<Entity>();
                return returnValue;
            }
        }

        private bool mustMatch = false;

       // private Dictionary<Type, List<IComponent>> componentBindings = 
       //     new Dictionary<Type, List<IComponent>>();

        public Group(params Type[] types)
        {
            entities = new List<Entity>();
            newEntities = new List<Entity>();
            for(int i = 0; i < types.Length; i++)
            {
        //        componentBindings.Add(types[i], new List<IComponent>());
            }
        }

        public Group SetMatching(bool matching)
        {
            mustMatch = matching;
            return this;
        }

        public void AddEntity(Entity entity)
        {
            onEntityAdded(entity);
            if (entities.Contains(entity))
                return;
            entities.Add(entity);
            newEntities.Add(entity);
        }

       // private int OnEntityChanged(IComponent comp, Entity entity)
        //{
            //if (!entity.HasComponents(composition))
            //    entities.Remove(entity);

           // return -1;
        //}

        public bool HasComponents(int components)
        {
            if (mustMatch) return composition == components;
            return (composition & components) == composition;
        }

        public void RemoveEntity(Entity entity)
        {
            onEntityRemoved(entity);
            entities.Remove(entity);
            newEntities.Remove(entity);
        }
    }
}
