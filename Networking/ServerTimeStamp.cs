using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocketWorks.Networking
{
    public class ServerTimeStamp
    {
        private static DateTime ServerTime;
        private static TimeSpan timeDifference;
        public static DateTime ServerNow
        {
            get { return DateTime.UtcNow - timeDifference; }
        }

        public static void SetServerTime(DateTime time)
        {
            ServerTime = time;
            timeDifference = DateTime.UtcNow - ServerTime;
        }
    }
}
