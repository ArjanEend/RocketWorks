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

        private IComponent[] components;

        private int composition;

        public Entity() {}
        public Entity(int totalComponents)
        {
			components = new IComponent[totalComponents];
            composition = 0;
        }

        public T GetComponent<T>(int i) where T : IComponent
        {
            return (T)components[i];
        }

        public bool HasComponents(int components)
        {
            return (composition & components) == components;
        }

        public bool HasComposition(int composition)
        {
            return this.composition == composition;
        }

        public T AddComponent<T>(int i, T component) where T : IComponent
        {
			components[i] = component;
            return component;
        }

        public void RemoveComponent(int i)
        {
            components[i] = null;
        }

        public void Reset()
        {
            
        }
    }
}
