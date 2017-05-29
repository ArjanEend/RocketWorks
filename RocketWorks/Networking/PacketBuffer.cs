using System;

namespace RocketWorks.Networking
{
    // This is used by the ChannelBuffer when buffering traffic.
    // Unreliable channels have a single ChannelPacket, Reliable channels have single "current" packet and a list of buffered ChannelPackets
    public struct PacketBuffer
    {
        int position;
        byte[] buffer;
        bool reliable;

        public PacketBuffer(int packetSize, bool isReliable)
        {
            position = 0;
            buffer = new byte[packetSize];
            reliable = isReliable;
        }

        public void Reset()
        {
            position = 0;
        }

        public bool IsEmpty()
        {
            return position == 0;
        }

        public void Write(byte[] bytes, int numBytes)
        {
            Array.Copy(bytes, 0, buffer, position, numBytes);
            position += numBytes;
        }

        public bool HasSpace(int numBytes)
        {
            return position + numBytes <= buffer.Length;
        }

        public bool SendToTransport(SocketConnection conn, int channelId)
        {
            byte error;

            bool result = true;
            /*if (!conn.TransportSend(buffer, (ushort)position, channelId, out error))
            {
                if (reliable)
                {
                    // handled below
                }
                else
                {
                    RocketLog.Log("Failed to send internal buffer channel:" + channelId + " bytesToSend:" + position);
                    result = false;
                }
            }
            if (error != 0)
            {
                if (reliable)
                {
                    // this packet will be buffered by the containing ChannelBuffer, so this is not an error

                    return false;
                }

                RocketLog.Log("Send Error: " + error + " channel:" + channelId + " bytesToSend:" + position);
                result = false;
            }*/
            position = 0;
            return result;
        }

    }
}
