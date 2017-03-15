using RocketWorks.Serialization;
using System;
using System.Collections.Generic;

namespace RocketWorks.CodeGeneration
{
    class RocketizerBuilder : ClassBuilder
    {
        public RocketizerBuilder(List<Type> serializeableTypes)
        {
            BuildImports("RocketWorks.Serialization");
            BuildHeader(typeof(Rocketizer).Namespace, typeof(Rocketizer).Name, "", true);
            string constructorLines = "";
            for(int i = 0; i < serializeableTypes.Count; i++)
            {
                string typeName = serializeableTypes[i].ToGenericTypeString(true);
                constructorLines += string.Format("idToType.Add({0}, typeof({1})); typeToId.Add(typeof({1}), {0});", i, typeName);
            }
            BuildConstructor(constructorLines);
            BuildEnding();
        }
    }
}
