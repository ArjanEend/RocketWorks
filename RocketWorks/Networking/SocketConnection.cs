using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace RocketWorks.Networking
{
    public enum SocketState
    {
        DISCONNECTED = 0,
        IDLE = 1,
        PENDING = 2,
        SENDING = 3
    }

    public delegate void ResultDelegate(NetworkReader reader, int id);

    public class SocketConnection
    {
        private NetworkBuffer writeStream;
        private NetworkBuffer readStream;
        private NetworkBuffer readBuffer;
        private NetworkReader reader;
        private NetworkBuffer sendBuffer;
        private SocketState state;
        public SocketState State { get { return state; } }

        private Socket socket;
        private int id;

        private int packetsSent = 0;
        private int packetsRead = 0;

        private Thread sendThread;
        private Thread recieveThread;

        private int readCounter;

        public ResultDelegate RecieveResultDelegate;

        public bool Connected { get { return socket.Connected; } }

        public SocketConnection(Socket socket, int id)
        {
            state = SocketState.IDLE;
            this.socket = socket;
            this.id = id;
            sendBuffer = new NetworkBuffer();
            writeStream = new NetworkBuffer();
            readStream = new NetworkBuffer();
            readBuffer = new NetworkBuffer();
            reader = new NetworkReader();

            sendThread = new Thread(SendLoop);
            sendThread.Start();
            recieveThread = new Thread(RecieveLoop);
            recieveThread.Start();
        }

        private void SendLoop()
        {
            try
            {
                // ShutdownEvent is a ManualResetEvent signaled by
                // Client when its time to close the socket.
                bool connected = true;
                while (connected)
                {
                    //RocketLog.Log("SendLoop");
                    Thread.Sleep(1);
                    try
                    {
                        while (state != SocketState.PENDING)
                        {
                            // Give up the remaining time slice.
                            Thread.Sleep(1);
                        }
                        {
                            lock (sendBuffer)
                            {
                                var cache = writeStream;
                                writeStream = sendBuffer;
                                sendBuffer = cache;
                                sendBuffer.SeekZero();
                            }
                            state = SocketState.SENDING;
                            //RocketLog.Log("Sending: " + writeStream.Position);
                            uint targetPos = writeStream.Position;
                            uint pos = 0;
                            uint prevPos = 0;
                            while(pos < targetPos)
                            {
                                prevPos = pos;
                                pos = Math.Min(writeStream.Position, pos + 1024);
                                lock (socket)
                                {
                                    ArraySegment<Byte> seg = writeStream.AsArraySegment();
                                    if (socket.Send(seg.Array, (int)prevPos, (int)pos, SocketFlags.None) > 0)
                                    {
                                        RocketLog.Log("Packet sent: " + pos);
                                        state = SocketState.IDLE;
                                    }
                                    else
                                    {
                                        // The connection has closed gracefully, so stop the
                                        // thread.
                                        connected = false;
                                    }
                                }
                                Thread.Sleep(1);
                            }
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
            }
        }

        private void RecieveLoop()
        {
            try
            {

                Thread.Sleep(200);
                bool connected = true;
                while (connected)
                {
                    Thread.Sleep(1);
                    //RocketLog.Log("ReceiveLoop");
                    try
                    {
                        //while (socket.Available < 2 && socket.Connected)
                        //{
                            // Give up the remaining time slice.
                        //    Thread.Sleep(100);
                        //}
                        reader.SeekZero();
                        if (socket.Receive(readBuffer.AsArraySegment().Array, 2, SocketFlags.Partial) > 0)
                        {
                            uint size = reader.ReadUInt16();
                            reader.SeekZero();
                            //RocketLog.Log("Received packet size: " + size);
                            while (socket.Available < size)
                            {

                                //RocketLog.Log("Waiting for actual packet");
                                // Give up the remaining time slice.
                                Thread.Sleep(1);
                            }
                            if(socket.Receive(readBuffer.AsArraySegment().Array, (int)size, SocketFlags.Partial) > 0)
                            {
                                //RocketLog.Log("Received packet");
                                RecieveResultDelegate(reader, id);
                            } else
                            {
                                connected = false;
                            }
                        }
                        else
                        {
                            connected = false;
                        }
                    }
                    catch (IOException ex)
                    {
                        RocketLog.Log(ex.ToString(), ex);
                    }
                }
            }
            catch (Exception ex)
            {
                RocketLog.Log(ex.ToString(), ex);
            }
            finally
            {
            }
        }

        public void Write(byte[] buffer, int v, int size)
        {
            if (!Connected)
                return;
            sendBuffer.WriteBytesAtOffset(buffer, (ushort)sendBuffer.Position, (ushort)size);
            if(sendBuffer.Position != 0 && state == SocketState.IDLE)
                state = SocketState.PENDING;
        }

        public void Close()
        {
            socket.Close();
            state = SocketState.DISCONNECTED;
        }
    }
}
