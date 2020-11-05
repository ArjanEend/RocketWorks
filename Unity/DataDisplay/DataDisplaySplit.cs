using UnityEngine;

namespace RocketWorks.Unity.DataDisplay
{
    public abstract class DataDisplaySplit<T> : DataDisplayBase<T>
    {
        [SerializeField] private DisplayRoot root;

        protected abstract object Convert(T Data);

        private void OnValidate()
        {
            if (root?.gameObject == gameObject)
            {
                root = null;
                Debug.LogError("Split needs to be at higher level");
            }
        }

        public override void OnSetData()
        {
            var data = Convert(Data);
            root.gameObject.SetActive(data != null);
            if (data != null)
                root.SetData(data);
        }

        public override void OnDisplay()
        {
            root.Display();
        }

        protected override void OnClear()
        {
            root.Clear();
        }
    }
}