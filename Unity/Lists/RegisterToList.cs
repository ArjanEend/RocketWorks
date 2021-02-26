using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RocketWorks.Unity.Lists
{
    [DefaultExecutionOrder(-99)]
    public class RegisterToList : MonoBehaviour
    {
        [SerializeField] private ObjectList list;
        [SerializeField] private Component behaviour;
        [SerializeField] private bool ghostMode = true;

        public Component Behaviour
        {
            set => behaviour = value;
        }

        private void OnEnable()
        {
            if(!list.Contains(behaviour))
                list.Add(behaviour);
        }

        private void OnDestroy()
        {
            list.Remove(behaviour);
        }

        private void OnDisable()
        {
            if(!ghostMode)
                list.Remove(behaviour);
        }
    }
}
