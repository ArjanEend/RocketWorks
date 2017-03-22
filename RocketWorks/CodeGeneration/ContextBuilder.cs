﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RocketWorks.CodeGeneration
{
    public class ContextBuilder : ClassBuilder
    { 
        public ContextBuilder(Type type, string fieldName)
        {
            BuildImports("System", "Implementation.Components", "RocketWorks.Pooling");
            BuildHeader(type.Namespace, fieldName + "Context", type.ToGenericTypeString());

            BuildConstructor(string.Format("this.pool = new EntityPool<{0}>(GetIndexOf, components.Count);", fieldName + "Entity"));
            

            BuildEnding();

            RocketLog.Log(StringBuilder.ToString());
        }
    }
}