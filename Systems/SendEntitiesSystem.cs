﻿using System;
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
        private IGroup componentGroup;
        private int compId;

        private bool autoDestroy;
        private bool sendDestroy;
        private bool sendCreate;

        public SendEntitiesSystem(SocketController socket, bool sendCreate = true, bool autoDestroy = false, bool sendDestroy = false) : base()
        {
            this.socket = socket;
            this.sendCreate = sendCreate;
            this.autoDestroy = autoDestroy;
            this.sendDestroy = sendDestroy;
        }

        public override void Initialize(Contexts contexts)
        {
            compId = contexts.GetContext<S>().GetIndexOf(typeof(T));
            componentGroup = contexts.GetContext<S>().Pool.GetGroup(typeof(T));
            if(sendCreate)
                componentGroup.OnEntityAdded += WriteEntity;
            if(sendDestroy)
                componentGroup.OnEntityRemoved += DestroyEntity;
        }

        private void DestroyEntity(Entity obj)
        {
            if(!obj.Alive)
                socket.WriteSocket(new DestroyEntityCommand<S>(obj));
        }

        private void WriteEntity(Entity obj)
        {
            socket.WriteSocket(new CreateEntityCommand<S>(obj));
            if (autoDestroy)
                obj.Reset();
        }

        public override void Destroy()
        {
        }
    }
}
