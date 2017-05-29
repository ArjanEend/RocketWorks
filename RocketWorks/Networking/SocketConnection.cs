using RocketWorks.Commands;
using RocketWorks.Networking;
using RocketWorks.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace RocketWorks.Networking
{
    /*
    * wire protocol is a list of :   size   | payload
    *					            (short)   (buffer)
    */
    public class SocketConnection : IDisposable
    {
        ChannelBuffer[] m_Channels;
        INetworkCommand m_NetMsg;// = new NetworkMessage();
        NetworkWriter m_Writer;
        
        Action<INetworkCommand> m_MessageHandlers;
        
        const int k_MaxMessageLogSize = 150;

        public int hostId = -1;
        public int connectionId = -1;
        public bool isReady;
        public string address;
        public float lastMessageTime;
        public bool logNetworkMessages = false;
        public bool isConnected { get { return hostId != -1; }}

        private Socket socket;
        
        public enum Channels : int
        {
            DefaultReliable = 0,
            DefaultUnreliable = 1
        }

        public class PacketStat
        {
            public PacketStat()
            {
                msgType = 0;
                count = 0;
                bytes = 0;
            }

            public PacketStat(PacketStat s)
            {
                msgType = s.msgType;
                count = s.count;
                bytes = s.bytes;
            }

            public short msgType;
            public int count;
            public int bytes;

            public override string ToString()
            {
                return msgType + ": count=" + count + " bytes=" + bytes;
            }
        }

        Dictionary<short, PacketStat> m_PacketStats = new Dictionary<short, PacketStat>();
        internal Dictionary<short, PacketStat> packetStats { get { return m_PacketStats; }}

#if UNITY_EDITOR
        static int s_MaxPacketStats = 255;//the same as maximum message types
#endif

        public SocketConnection(Socket socket, int id)
        {
            this.connectionId = id;
            this.socket = socket;
            int numChannels = 2;

            m_Channels = new ChannelBuffer[numChannels];
            for (int i = 0; i < numChannels; i++)
            {
                int actualPacketSize = 1024;
                m_Channels[i] = new ChannelBuffer(this, actualPacketSize, (byte)i, i == 0, i == 0);
            }
        }

        // Track whether Dispose has been called.
        bool m_Disposed;

        ~SocketConnection()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            // Take yourself off the Finalization queue
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!m_Disposed && m_Channels != null)
            {
                for (int i = 0; i < m_Channels.Length; i++)
                {
                    m_Channels[i].Dispose();
                }
            }
            m_Channels = null;

            m_Disposed = true;
        }

        public SocketConnection()
        {
            m_Writer = new NetworkWriter();
        }

        public void Disconnect()
        {
            address = "";
            isReady = false;
            if (hostId == -1)
            {
                return;
            }
            byte error;
        }

        internal void SetHandlers(Action<INetworkCommand> handlers)
        {
            m_MessageHandlers = handlers;
        }

        public bool InvokeHandler(Type msgType, NetworkReader reader, int channelId)
        {
            /*if (m_MessageHandlersDict.ContainsKey(msgType))
            {
                m_MessageInfo.msgType = msgType;
                m_MessageInfo.conn = this;
                m_MessageInfo.reader = reader;
                m_MessageInfo.channelId = channelId;

                NetworkMessageDelegate msgDelegate = m_MessageHandlersDict[msgType];
                if (msgDelegate == null)
                {
                    if (LogFilter.logError) { Debug.LogError("NetworkConnection InvokeHandler no handler for " + msgType); }
                    return false;
                }
                msgDelegate(m_MessageInfo);
                return true;
            }*/
            return false;
        }

        public bool InvokeHandler(INetworkCommand netMsg)
        {
            /*if (m_MessageHandlersDict.ContainsKey(netMsg.msgType))
            {
                NetworkMessageDelegate msgDelegate = m_MessageHandlersDict[netMsg.msgType];
                msgDelegate(netMsg);
                return true;
            }*/
            return false;
        }

        internal void HandleFragment(NetworkReader reader, int channelId)
        {
            if (channelId < 0 || channelId >= m_Channels.Length)
            {
                return;
            }

            var channel = m_Channels[channelId];
        }
        
        public void FlushChannels()
        {
            if (m_Channels == null)
            {
                return;
            }
            for (int channelId = 0; channelId < m_Channels.Length; channelId++)
            {
                m_Channels[channelId].CheckInternalBuffer();
            }
        }

        public void SetMaxDelay(float seconds)
        {
            if (m_Channels == null)
            {
                return;
            }
            for (int channelId = 0; channelId < m_Channels.Length; channelId++)
            {
                m_Channels[channelId].maxDelay = seconds;
            }
        }

        public virtual bool Send(IRocketable msg)
        {
            return SendByChannel(msg, (int)Channels.DefaultReliable);
        }

        public virtual bool SendUnreliable(IRocketable msg)
        {
            return SendByChannel(msg, (int)Channels.DefaultUnreliable);
        }

        public virtual bool SendByChannel(IRocketable msg, int channelId)
        {
            m_Writer.StartMessage();
            msg.Rocketize(m_Writer);
            m_Writer.FinishMessage();
            return SendWriter(m_Writer, channelId);
        }

        public virtual bool SendBytes(byte[] bytes, int numBytes, int channelId)
        {
            if (logNetworkMessages)
            {
                LogSend(bytes);
            }
            return CheckChannel(channelId) && m_Channels[channelId].SendBytes(bytes, numBytes);
        }

        public virtual bool SendWriter(NetworkWriter writer, int channelId)
        {
            if (logNetworkMessages)
            {
                LogSend(writer.ToArray());
            }
            return CheckChannel(channelId) && m_Channels[channelId].SendWriter(writer);
        }

        private void Run()
        {
            try
            {
                // ShutdownEvent is a ManualResetEvent signaled by
                // Client when its time to close the socket.
                while (!ShutdownEvent.WaitOne(0))
                {
                    try
                    {
                        // We could use the ReadTimeout property and let Read()
                        // block.  However, if no data is received prior to the
                        // timeout period expiring, an IOException occurs.
                        // While this can be handled, it leads to problems when
                        // debugging if we are wanting to break when exceptions
                        // are thrown (unless we explicitly ignore IOException,
                        // which I always forget to do).
                        if (socket.Available < 0)
                        {
                            // Give up the remaining time slice.
                            Thread.Sleep(1);
                        }
                        else if (socket.Receive(_data, 0, _data.Length) > 0)
                        {
                            // Raise the DataReceived event w/ data...
                        }
                        else
                        {
                            // The connection has closed gracefully, so stop the
                            // thread.
                            ShutdownEvent.Set();
                        }
                    }
                    catch (IOException ex)
                    {
                        // Handle the exception...
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception...
            }
            finally
            {
                _stream.Close();
            }
        }

        void LogSend(byte[] bytes)
        {
            NetworkReader reader = new NetworkReader(bytes);
            var msgSize = reader.ReadUInt16();
            var msgId = reader.ReadUInt16();

            const int k_PayloadStartPosition = 4;

            StringBuilder msg = new StringBuilder();
            for (int i = k_PayloadStartPosition; i < k_PayloadStartPosition + msgSize; i++)
            {
                msg.AppendFormat("{0:X2}", bytes[i]);
                if (i > k_MaxMessageLogSize) break;
            }
            RocketLog.Log("ConnectionSend con:" + connectionId + " bytes:" + msgSize + " msgId:" + msgId + " " + msg);
        }

        bool CheckChannel(int channelId)
        {
            if (m_Channels == null)
            {
                RocketLog.Log("Channels not initialized sending on id '" + channelId);
                return false;
            }
            if (channelId < 0 || channelId >= m_Channels.Length)
            {
                RocketLog.Log("Invalid channel when sending buffered data, '" + channelId + "'. Current channel count is " + m_Channels.Length);
                return false;
            }
            return true;
        }

        public void ResetStats()
        {
#if UNITY_EDITOR
            for (short i = 0; i < s_MaxPacketStats; i++)
            {
                if (m_PacketStats.ContainsKey(i))
                {
                    var value = m_PacketStats[i];
                    value.count = 0;
                    value.bytes = 0;
                    NetworkTransport.SetPacketStat(0, i, 0, 0);
                    NetworkTransport.SetPacketStat(1, i, 0, 0);
                }
            }
#endif
        }

        protected void HandleBytes(
            byte[] buffer,
            int receivedSize,
            int channelId)
        {
            // build the stream form the buffer passed in
            NetworkReader reader = new NetworkReader(buffer);

            HandleReader(reader, receivedSize, channelId);
        }

        
        public virtual void GetStatsOut(out int numMsgs, out int numBufferedMsgs, out int numBytes, out int lastBufferedPerSecond)
        {
            numMsgs = 0;
            numBufferedMsgs = 0;
            numBytes = 0;
            lastBufferedPerSecond = 0;

            for (int channelId = 0; channelId < m_Channels.Length; channelId++)
            {
                var channel = m_Channels[channelId];
                numMsgs += channel.numMsgsOut;
                numBufferedMsgs += channel.numBufferedMsgsOut;
                numBytes += channel.numBytesOut;
                lastBufferedPerSecond += channel.lastBufferedPerSecond;
            }
        }

        public virtual void GetStatsIn(out int numMsgs, out int numBytes)
        {
            numMsgs = 0;
            numBytes = 0;

            for (int channelId = 0; channelId < m_Channels.Length; channelId++)
            {
                var channel = m_Channels[channelId];
                numMsgs += channel.numMsgsIn;
                numBytes += channel.numBytesIn;
            }
        }

        public override string ToString()
        {
            return string.Format("hostId: {0} connectionId: {1} isReady: {2} channel count: {3}", hostId, connectionId, isReady, (m_Channels != null ? m_Channels.Length : 0));
        }

        public virtual void TransportReceive(byte[] bytes, int numBytes, int channelId)
        {
            HandleBytes(bytes, numBytes, channelId);
        }

        [Obsolete("TransportRecieve has been deprecated. Use TransportReceive instead (UnityUpgradable) -> TransportReceive(*)", false)]
        public virtual void TransportRecieve(byte[] bytes, int numBytes, int channelId)
        {
            TransportReceive(bytes, numBytes, channelId);
        }

        /*public virtual bool TransportSend(byte[] bytes, int numBytes, int channelId, out byte error)
        {
            return socket.Send(bytes, numBytes);
        }*/
    }
}
