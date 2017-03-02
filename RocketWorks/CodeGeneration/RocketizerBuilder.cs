using RocketWorks.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

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
                constructorLines += string.Format("idToType.Add({0}, typeof({1})); typeToId.Add(typeof({1}), {0});", i, serializeableTypes[i].FullName);
            }
            BuildConstructor(constructorLines);
            BuildEnding();
        }
    }
}
