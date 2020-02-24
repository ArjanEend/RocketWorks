using System.Collections.Generic;
using RocketWorks.CodeGeneration;

namespace RocketWorks.Base
{
    public class ComponentBaseBuilder : ClassBuilder
    {
        public ComponentBaseBuilder(ComponentConfig config)
        {
            BuildImports("RocketWorks.Serialization", "System", "RocketWorks.Entities", "System.Collections.Generic");
            BuildHeader("", config.name, "IComponent", true);

            foreach (var varConf in config.Variables)
            {
                BuildVariable(varConf.typeName, varConf.fieldName, true, true);
            }

            BuildEnding();
        }
    }

    public class ContextBaseBuilder : ClassBuilder
    {
        public ContextBaseBuilder(List<ContextConfig> configs)
        {
            BuildImports("RocketWorks.Entities", "System.Collections.Generic");
            BuildHeader("", "Contexts", "", true);

            foreach (var context in configs)
            {
                var typeStr = "EntityContext<{0}>";
                var types = "";
                for (int i = 0; i < context.components.Count; i++)
                {
                    if (i != 0)
                        types += ",";
                    types += context.components[i].name;
                }

                typeStr = string.Format(typeStr, types);

                BuildVariable(typeStr, context.name + "Context", false, false, true);
            }

            BuildEnding();
        }
    }
}