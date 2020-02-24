using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using RocketWorks.Base;

namespace RocketWorks.CodeGeneration
{
    public class ContextBuilder : ClassBuilder
    {
        public ContextBuilder(Type type, string fieldName)
        {
            Type[] components = type.GetGenericArguments();
            for (int i = 0; i < components.Length; i++)
            {
                BuildImports(components[i].Namespace);
            }
            BuildImports("System", "RocketWorks.Pooling");
            fieldName = fieldName.Substring(0, 1).ToUpper() + fieldName.Substring(1, fieldName.Length - 1);
            BuildHeader(type.Namespace, fieldName + "Context", type.ToGenericTypeString());

            string variableName = fieldName.ToLower() + "Pool";

            BuildVariable(string.Format("EntityPool<{0}>", fieldName + "Entity"), variableName, true, false);

            BuildConstructor(string.Format("this.pool = this.{1} = new EntityPool<{0}>(GetIndexOf, components.Count);", fieldName + "Entity", variableName));

            string groupLines = "";
            groupLines += "return this.pool as EntityPool<T>;";

            BuildMethod("GetPool<T>", "public override", "EntityPool<T>", groupLines);

            BuildEnding();

            RocketLog.Log(StringBuilder.ToString());
        }
    }
}
