using System;
using RocketWorks.Entities;
using RocketWorks.Pooling;
using RocketWorks.Networking;
using RocketWorks.Grouping;
using RocketWorks.Commands;
using System.Collections.Generic;

namespace RocketWorks.Systems
{
    public class SendEntitiesSystem<T, S> : SystemBase where T : IComponent where S : EntityContext
    {
        private SocketController socket;
        private Group componentGroup;
        private int compId;

        public SendEntitiesSystem(SocketController socket) : base()
        {
            this.socket = socket;
            this.tickRate = .2f;
        }

        public override void Initialize(Contexts contexts)
        {
            compId = contexts.GetContext<S>().GetIndexOf(typeof(T));
            componentGroup = contexts.GetContext<S>().Pool.GetGroup(typeof(T));
        }

        public override void Destroy()
        {
            throw new NotImplementedException();
        }

        public override void Execute(float deltaTime)
        {
            List<Entity> entities = componentGroup.NewEntities;
            for (int i = 0; i < entities.Count; i++)
            {
                socket.WriteSocket(new MainContextCreateEntityCommand(entities[i]));
                //RocketLog.Log("Send entity over network", this);
                entities[i].Reset();
            }
        }
    }
}
