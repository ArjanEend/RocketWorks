using System;
using RocketWorks.Entities;
using RocketWorks.Pooling;
using RocketWorks.Networking;
using RocketWorks.Grouping;
using RocketWorks.Commands;

namespace RocketWorks.Systems
{
    public class SendComponentsSystem<T> : SystemBase where T : IComponent
    {
        private SocketController socket;
        private Group componentGroup;
        private int compId;

        public SendComponentsSystem(SocketController socket) : base()
        {
            this.socket = socket;
            this.tickRate = .2f;
        }

        public override void Initialize(EntityPool pool)
        {
            compId = pool.GetIndexOf(typeof(T));
            componentGroup = pool.GetGroup(typeof(T));
        }

        public override void Destroy()
        {
            throw new NotImplementedException();
        }

        public override void Execute()
        {
            for(int i = 0; i < componentGroup.Count; i++)
            {
                socket.WriteSocket(new UpdateComponentCommand(componentGroup[i].GetComponent<T>(compId), componentGroup[i].CreationIndex));
            }
        }
    }
}
