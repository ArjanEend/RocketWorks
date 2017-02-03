using System;
using System.Collections.Generic;

namespace RocketWorks.Commands
{
    public class Commander
    {
        private Dictionary<Type, object> typeBindings;
        
        public Commander()
        {
            typeBindings = new Dictionary<Type, object>();
        }

        public void AddObject(object obj)
        {
            typeBindings.Add(obj.GetType(), obj);
        }

        public void Execute(ICommand command)
        {
            Type type = command.targetType;
            if (typeBindings.ContainsKey(type))
                command.Execute(typeBindings[type]);
        }

    }
}
