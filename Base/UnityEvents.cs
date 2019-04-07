#if RW_UNITY
using UnityEngine;
using System;

namespace RocketWorks.Base
{
    [DefaultExecutionOrder(-1)]
    public class UnityEvents : MonoBehaviour
    {
        public Action<float> OnUpdate;
        public Action<float> OnFixedUpdate;

        private void Update()
        {
            if (OnUpdate != null)
                OnUpdate(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (OnFixedUpdate != null)
                OnFixedUpdate(Time.deltaTime);
        }

    }
}
#endif