#if UNITY_EDITOR || UNITY_5
using UnityEngine;
using System;

namespace RocketWorks.Base
{
    public class UnityEvents : MonoBehaviour
    {
        public Action<float> OnUpdate;
        public Action OnFixedUpdate;

        private void Update()
        {
            if (OnUpdate != null)
                OnUpdate(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (OnFixedUpdate != null)
                OnFixedUpdate();
        }

    }
}
#endif