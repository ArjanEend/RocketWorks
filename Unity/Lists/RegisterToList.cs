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

        public Component Behaviour
        {
            set => behaviour = value;
        }

        private void Start()
        {
            list.Add(behaviour);
        }

        private void OnDestroy()
        {
            list.Remove(behaviour);
        }
    }
}
