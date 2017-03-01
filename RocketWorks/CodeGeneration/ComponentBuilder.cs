using System;
using System.Reflection;

namespace RocketWorks.CodeGeneration
{
    class ComponentBuilder : ClassBuilder
    {
        public ComponentBuilder(Type type)
        {

            BuildImports();
            BuildHeader(type.Namespace, type.Name, "IRocketable");
            

        }

    }
}
