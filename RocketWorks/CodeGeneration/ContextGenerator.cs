using RocketWorks.Commands;
using RocketWorks.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RocketWorks.CodeGeneration
{
    public class ContextGenerator
    {
        private Contexts contexts;

        private List<ClassBuilder> builders;
        public List<ClassBuilder> Builders { get { return builders; } }

        private List<Type> types;

        public ContextGenerator()
        {
            contexts = new Contexts();
            builders = new List<ClassBuilder>();
            types = new List<Type>();

            PropertyInfo[] info = contexts.GetType().GetProperties();
            for(int i = 0; i < info.Length; i++)
            {
                RocketLog.Log("FieldInfo: " + info[i].Name);
                EntityContext context = info[i].GetValue(contexts, null) as EntityContext;
                Type contextType = context.GetType();
                Type[] componentsToUse = contextType.GetGenericArguments();
                
                for(int j = 0; j < componentsToUse.Length; j++)
                {
                    Type componentType = componentsToUse[j];
                    ParseType(componentType);
                    RocketLog.Log("  Component: " + componentsToUse[j].Name);
                }
            }

            Type type = typeof(ICommand);
            var commandsTypes = AppDomain.CurrentDomain.GetAssemblies().
                SelectMany(s => s.GetTypes()).
                Where(p => type.IsAssignableFrom(p));

            foreach(Type t in commandsTypes)
            {
                ParseType(t);
            }

            types.Add(typeof(Entity));

            builders.Add(new RocketizerBuilder(types));
        }

        private void ParseType(Type type)
        {
            if (type.IsInterface || type.IsAbstract)
                return;
            if (types.Contains(type) || type.GetInterface("IRocketable") != null || (!string.IsNullOrEmpty(type.Namespace) && !type.Namespace.Contains("RocketWorks") && !type.Namespace.Contains("Implementation")))
                return;
            types.Add(type);
            builders.Add(new ComponentBuilder(type));
            FieldInfo[] fields = type.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].FieldType.Name == "String")
                {
                }
                else if (fields[i].FieldType.IsPrimitive)
                {
                }
                else
                {
                    ParseType(fields[i].FieldType);
                }
            }
        }

    }
}
