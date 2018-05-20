using System;
using RocketWorks.Entities;
using RocketWorks.Pooling;
using RocketWorks.Networking;

namespace RocketWorks.Commands
{
    [Serializable]
    public partial class SetUserIDCommand : NetworkCommandBase<SocketController>
    {
        public int uid;
        public DateTime timeStamp;

        public SetUserIDCommand()
        {

        }

        public SetUserIDCommand(int uid, DateTime timeStamp)
        {
            this.uid = uid;
            this.timeStamp = timeStamp;
        }

        public override void Execute(SocketController target, int uid)
        {
            target.SetUserID(this.uid);
            target.SetTimeStamp(timeStamp);
        }
    }
}