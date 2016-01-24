using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RocketWorks.Scene
{
    public class SceneBindings : ScriptableObject
    {
        private Dictionary<Type, string> typeToScene;
    }
}

#if UNITY_EDITOR
/*public class SceneBindingsDrawer : CustomPropertyDrawer
{
    
}*/
#endif