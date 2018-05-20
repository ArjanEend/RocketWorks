using System;
using System.IO;
using System.Net.Sockets;


struct TcpKeepAlive
{
    //[System.Runtime.InteropServices.FieldOffset(0)]
    public uint On_Off;

    //[System.Runtime.InteropServices.FieldOffset(4)]
    public uint KeepaLiveTime;

    //[System.Runtime.InteropServices.FieldOffset(8)]
    public uint KeepaLiveInterval;

    public int SetKeepAliveValues
      (
           System.Net.Sockets.Socket Socket,
           bool On_Off,
           uint KeepaLiveTime,
           uint KeepaLiveInterval
       )
    {
        int Result = -1;

        
        {
            TcpKeepAlive KeepAliveValues = new TcpKeepAlive();
            //KeepAliveValues.Bytes = new byte[12];

            KeepAliveValues.On_Off = Convert.ToUInt32(On_Off);
            KeepAliveValues.KeepaLiveTime = KeepaLiveTime;
            KeepAliveValues.KeepaLiveInterval = KeepaLiveInterval;

            byte[] InValue = new byte[12];
            MemoryStream memStream = new MemoryStream(InValue);
            BinaryWriter bWriter = new BinaryWriter(memStream);
            bWriter.Write(KeepAliveValues.On_Off);
            bWriter.Write(KeepAliveValues.KeepaLiveTime);
            bWriter.Write(KeepAliveValues.KeepaLiveInterval);
            //for (int I = 0; I < 12; I++)
            //    InValue[I] = KeepAliveValues.Bytes[I];

            Result = Socket.IOControl(IOControlCode.KeepAliveValues, InValue, null);
        }

        return Result;
    }

}
