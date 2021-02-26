using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RocketWorks.Unity.DataDisplay
{
    public sealed class ListDisplay : DataDisplayBase<IList>
    {
        [SerializeField]
        private DisplayRoot prefab;

        private List<DisplayRoot> displays = new List<DisplayRoot>();
        public List<DisplayRoot> Displays => displays;
        private List<DisplayRoot> inactiveDisplays = new List<DisplayRoot>();

        public override void OnSetData()
        {
            prefab.gameObject.SetActive(false);
            for (int i = 0; i < Data.Count; i++)
            {
                DisplayRoot instance;

                if (inactiveDisplays.Count > 0)
                {
                    instance = inactiveDisplays[0];
                    inactiveDisplays.RemoveAt(0);
                    instance.transform.SetSiblingIndex(i);
                }
                else
                {
                    instance = Instantiate(prefab, transform);
                }

                instance.gameObject.SetActive(true);
                displays.Add(instance);
                displays[i].SetData(Data[i]);
            }
        }

        public override void OnDisplay()
        {
            for (int i = 0; i < displays.Count; i++)
            {
                displays[i].Display();
            }
        }

        protected override void OnClear()
        {
            for (int i = 0; i < displays.Count; i++)
            {
                displays[i].gameObject.SetActive(false);
                displays[i].Clear();
            }
            inactiveDisplays.AddRange(displays);
            displays.Clear();
        }
    }
}
