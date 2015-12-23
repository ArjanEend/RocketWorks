using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RocketWorks.Scene
{
    public class SceneBindings : ScriptableObject
    {
        private Dictionary<System.Type, string> typeToScene;
    }
}

#if UNITY_EDITOR
public class SceneBindingsDrawer : CustomPropertyDrawer
{

}
#endif