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
            BuildImports("Implementation.Components", "RocketWorks.Entities", "System.Collections.Generic");
            BuildHeader("", contextName + "Entity", "Entity", true);

            for(int i = 0; i < components.Length; i++)
            {
                string lines = string.Format("return ({1})components[{0}];", i, components[i].Name);
                BuildMethod(components[i].Name, "public", components[i].Name, lines);
            }
            
            BuildEnding();
        }
    }
}
