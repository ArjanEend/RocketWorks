using RocketWorks.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace RocketWorks.CodeGeneration
{
    class ContextsBuilder : ClassBuilder
    {
        public ContextsBuilder(List<string> serializeableTypes)
        {
            BuildImports("Implementation.Components", "RocketWorks.Entities", "System.Collections.Generic");
            BuildHeader("", "Contexts", "", true);
            string lines = "";

            FieldInfo[] info = typeof(Contexts).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            //Let's assume the serializeabletypes are the same as the propertyInfo stuff
            for (int i = 0; i < serializeableTypes.Count; i++)
            {
                for(int j = 0; j < info.Length; j++)
                {
                    if(serializeableTypes[i].ToLower().Contains(info[j].Name.Replace("Context", "").ToLower()))
                    {
                        string typeName = serializeableTypes[i];
                        lines += string.Format("contexts.Add({0} = new {1}());", info[j].Name, typeName);
                    }
                }
            }
            BuildMethod("Populate", "partial", "void", lines);
            BuildEnding();
        }
    }
}
