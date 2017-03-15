using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RocketWorks.CodeGeneration
{
    public class ContextGenericBuilder : ClassBuilder
    {
        public ContextGenericBuilder(Type type, string contextName)
        {
            BuildImports("System", "RocketWorks.Entities");
            BuildHeader(type.Namespace, contextName + type.Name.Substring(0, type.Name.Length - 2), 
                type.ToGenericTypeString().Replace("<T>", "<" + contextName + ">"));

            string constructorLines = "";
            List<string> types = new List<string>();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < fields.Length; i++)
            {
                types.Add(fields[i].FieldType.Name);
                constructorLines += string.Format("this.{1} = var_{0};", fields[i].FieldType.Name.ToLower(), fields[i].Name);
            }

            BuildConstructor(constructorLines, types.ToArray());

            BuildEnding();

            RocketLog.Log(StringBuilder.ToString());
        }
    }
}
