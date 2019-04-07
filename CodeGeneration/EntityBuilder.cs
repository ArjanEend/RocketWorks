using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocketWorks.CodeGeneration
{
    class EntityBuilder : ClassBuilder
    {
        public EntityBuilder(string contextName, Type[] components)
        {
            contextName = contextName.Replace("Context", "");
            for(int i = 0; i < components.Length; i++)
            {
                BuildImports(components[i].Namespace);
            }
            BuildImports("RocketWorks.Entities", "System.Collections.Generic");
            BuildHeader("", contextName + "Entity", "Entity", true);

            for(int i = 0; i < components.Length; i++)
            {
                string lines = string.Format("return ({1})components[{0}];", i, components[i].Name);
                BuildProperty(components[i].Name, components[i].Name, 
                    $"return ({components[i].Name})components[{i}];",
                    $"components[{i}] = ({components[i].Name})value;");
            }
            
            BuildEnding();
        }
    }
}
