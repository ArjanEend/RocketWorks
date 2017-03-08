using System;
using RocketWorks.Entities;
using RocketWorks.Pooling;
using RocketWorks.Networking;

namespace RocketWorks.Commands
{
    [Serializable]
    public partial class SetUserIDCommand : NetworkCommandBase<SocketController>
    {
        private int uid;

        public SetUserIDCommand()
        {

        }

        public SetUserIDCommand(int uid)
        {
            this.uid = uid;
        }

        public override void Execute(SocketController target, uint uid)
        {
            RocketLog.Log("Excute command", this);
            target.SetUserID(this.uid);
        }
    }
}