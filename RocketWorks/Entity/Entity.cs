using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace RocketWorks.Entity
{
    public class Entity : IPoolable
    {
        private bool enabled;
        public bool Enabled
        {
            get { return enabled; }
        }

        private bool alive;
        public bool Alive
        {
            get { return alive; }
        }

        private Dictionary<Type, IComponent> components;

        public Entity()
        {
			components = new Dictionary<Type, IComponent>();
        }

        public T GetComponent<T>() where T : IComponent
        {
            if (!components.ContainsKey(typeof(T)))
                return default(T);

            return (T)components[typeof(T)];
        }

        public T AddComponent<T>(T component) where T : IComponent
        {
			components.Add(typeof(T), component);
            return component;
        }

		public T AddComponent<T>() where T : IComponent, new()
        {
			return AddComponent<T>(new T());
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            if (!components.ContainsKey(typeof(T)))
                return;
            else
            {
				components.Remove(typeof(T));
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
