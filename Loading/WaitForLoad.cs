#if UNITY_5_3_5
using UnityEngine;


namespace RocketWorks.Loading
{
    public class WaitForLoad : CustomYieldInstruction
    {
        private ILoadable item;

        public override bool keepWaiting {
            get {
                return item.Progress < 1f;
            }
        }

        public WaitForLoad(ILoadable loadable)
        {
            item = loadable;
        }

    }
}
#endif