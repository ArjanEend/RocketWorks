#if UNITY
using UnityEngine;
using System;

namespace RocketWorks.Base
{
    public class UnityEvents : MonoBehaviour
    {
        public Action OnUpdate;
        public Action OnFixedUpdate;

        private void Update()
        {
            if (OnUpdate != null)
                OnUpdate();
        }

        private void FixedUpdate()
        {
            if (OnFixedUpdate != null)
                OnFixedUpdate();
        }

    }
}
#endif