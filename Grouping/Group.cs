using RocketWorks.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RocketWorks.Grouping
{
    public interface IGroup
    {
        int Count { get; }
        Entity this[int index] { get; }
        int Composition { get; }
        Action<Entity> OnEntityAdded { get; set; }
        Action<Entity> OnEntityRemoved { get; set; }
        void RemoveEntity(Entity entity);
        bool HasComponents(int components);
    }

    public class Group<T> : IGroup where T : Entity
    {
        public int Count
        {
            get { return entities.Count; }
        }
        public T this[int index]
        {
            get { return entities[index]; }
        }
        Entity IGroup.this[int index] { get { return this[index] as Entity; } }

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

        public void RemoveEntity(Entity entity)
        {
            RemoveEntity(entity as T);
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
