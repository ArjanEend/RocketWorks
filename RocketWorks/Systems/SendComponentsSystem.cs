using System;
using RocketWorks.Entities;
using RocketWorks.Pooling;
using RocketWorks.Networking;
using RocketWorks.Grouping;
using RocketWorks.Commands;

namespace RocketWorks.Systems
{
    public class SendComponentsSystem<T, S> : SystemBase where T : IComponent where S : EntityContext
    {
        private SocketController socket;
        private Group componentGroup;
        private int compId;

        public SendComponentsSystem(SocketController socket) : base()
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

        public override void Execute()
        {
            for(int i = 0; i < componentGroup.Count; i++)
            {
                socket.WriteSocket(new MainContextUpdateComponentCommand(componentGroup[i].GetComponent<T>(compId), componentGroup[i].CreationIndex));
            }
        }
    }
}
