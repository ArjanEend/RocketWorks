using System;
using System.Collections.Generic;

namespace RocketWorks.Commands
{
    public class Commander
    {
        protected Dictionary<Type, object> typeBindings;
        
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

    public class NetworkCommander : Commander
    {
        public void Execute(INetworkCommand command, int uid)
        {
            Type type = command.targetType;
            if (typeBindings.ContainsKey(type))
            {
                lock(typeBindings[type])
                {
                    command.Execute(typeBindings[type], uid);
                }
            }
        }
    }
}
