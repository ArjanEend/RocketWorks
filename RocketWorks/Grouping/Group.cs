using RocketWorks.Entities;
using System;
using System.Collections.Generic;

namespace RocketWorks.Grouping
{
    public class Group : Group<Entity>
    {

    }

    public class Group<T> where T : Entity
    {
        public int Count
        {
            get { return entities.Count; }
        }
        public T this[int index]
        {
            get { return entities[index]; }
        }

        private int composition;
        public int Composition
        {
            get { return composition; }
            internal set { composition = value; }
        }

        private Action<T> onEntityAdded = delegate { };
        public Action<T> OnEntityAdded
        {
            get { return onEntityAdded; }
            set { onEntityAdded = value; }
        }

        private Action<T> onEntityRemoved = delegate { };
        public Action<T> OnEntityRemoved
        {
            get { return onEntityRemoved; }
            set { onEntityRemoved = value; }
        }

        private List<T> entities;
        public List<T> Entities
        {
            get { return entities; }
            set { entities = value; }
        }

        private List<T> newEntities;
        public List<T> NewEntities
        {
            get {
                List<T> returnValue = newEntities;
                newEntities = new List<T>();
                return returnValue;
            }
        }

        private bool mustMatch = false;

       // private Dictionary<Type, List<IComponent>> componentBindings = 
       //     new Dictionary<Type, List<IComponent>>();

        public Group()
        {
            entities = new List<T>();
            newEntities = new List<T>();
        }

        public Group<T> SetMatching(bool matching)
        {
            mustMatch = matching;
            return this;
        }
        
        public bool Contains(T ent)
        {
            return entities.Contains(ent);
        }

        public void DestroyAll()
        {
            for(int i = entities.Count - 1; i > 0; i--)
            {
                entities[i].Reset();
            }
        }

        public void AddEntity(T entity)
        {
            onEntityAdded(entity);
            if (entities.Contains(entity))
                return;
            entities.Add(entity);
            newEntities.Add(entity);
        }

        public bool HasComponents(int components)
        {
            if (mustMatch) return composition == components;
            return (composition & components) == composition;
        }

        public void RemoveEntity(T entity)
        {
            if (!entities.Contains(entity))
                return;
            onEntityRemoved(entity);
            entities.Remove(entity);
            newEntities.Remove(entity);
        }
    }
}
