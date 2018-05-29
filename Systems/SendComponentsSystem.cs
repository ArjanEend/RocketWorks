using System;
using RocketWorks.Entities;
using RocketWorks.Pooling;
using RocketWorks.Networking;
using RocketWorks.Grouping;
using RocketWorks.Commands;
using System.Collections.Generic;

namespace RocketWorks.Systems
{
    public class SendComponentsSystem<T, E, S> : SystemBase where T : IComponent where S : EntityContext where E : Entity, new()
    {
        private SocketController socket;
        private Group<E> componentGroup;
        private int compId;

        private bool newOnly;

        public SendComponentsSystem(SocketController socket, bool newOnly = false) : base()
        {
            this.socket = socket;
            this.tickRate = .2f;
            this.newOnly = newOnly;
        }

        public override void Initialize(Contexts contexts)
        {
            compId = contexts.GetContext<S>().GetIndexOf(typeof(T));
            componentGroup = contexts.GetContext<S>().GetPool<E>().GetGroup(typeof(T));
        }

        public override void Destroy()
        {
            throw new NotImplementedException();
        }

        public override void Execute(float deltaTime)
        {
            List<E> entities = newOnly ? componentGroup.NewEntities : componentGroup.Entities;
            for(int i = 0; i < entities.Count; i++)
            {
                socket.WriteSocket(new UpdateComponentCommand<S>(entities[i].GetComponent<T>(compId), entities[i].CreationIndex));
            }
        }
    }
}
