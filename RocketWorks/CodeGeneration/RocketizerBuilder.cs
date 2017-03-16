using RocketWorks.Serialization;
using System;
using System.Collections.Generic;

namespace RocketWorks.CodeGeneration
{
    class RocketizerBuilder : ClassBuilder
    {
        private List<string> generatedContexts;

        public RocketizerBuilder(List<Type> serializeableTypes, List<string> generatedContexts, List<string> generatedCommands)
        {
            BuildImports("RocketWorks.Serialization");
            BuildHeader(typeof(Rocketizer).Namespace, typeof(Rocketizer).Name, "", true);
            string constructorLines = "";
            int counter = 0;
            for (int i = 0; i < serializeableTypes.Count; i++)
            {
                string typeName = serializeableTypes[i].ToGenericTypeString(true);
                constructorLines += string.Format("idToType.Add({0}, typeof({1})); typeToId.Add(typeof({1}), {0});", counter, typeName);
                counter++;
            }
            for (int i = 0; i < generatedContexts.Count; i++)
            {
                constructorLines += string.Format("idToType.Add({0}, typeof({1})); typeToId.Add(typeof({1}), {0});", counter, "RocketWorks.Entities." + generatedContexts[i]);
                counter++; 
            }
            for (int i = 0; i < generatedCommands.Count; i++)
            {
                constructorLines += string.Format("idToType.Add({0}, typeof({1})); typeToId.Add(typeof({1}), {0});", counter, generatedCommands[i]);
                counter++;
            }
            BuildConstructor(constructorLines);
            BuildEnding();
        }
    }
}
