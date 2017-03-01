

using RocketWorks.Entities;
using System;
using System.Reflection;

namespace RocketWorks.CodeGeneration
{
    public class ContextGenerator
    {
        private Contexts contexts;
        
        public ContextGenerator()
        {
            contexts = new Contexts();

            PropertyInfo[] info = contexts.GetType().GetProperties();
            for(int i = 0; i < info.Length; i++)
            {
                RocketLog.Log("FieldInfo: " + info[i].Name);
                EntityContext context = info[i].GetValue(contexts) as EntityContext;
                Type contextType = context.GetType();
                Type[] componentsToUse = contextType.GetGenericArguments();
                for(int j = 0; j < componentsToUse.Length; j++)
                {
                    RocketLog.Log("  Component: " + componentsToUse[j].Name);
                    Type componentType = componentsToUse[j];
                    new ComponentBuilder(componentType);
                }

            }
        }

    }
}
