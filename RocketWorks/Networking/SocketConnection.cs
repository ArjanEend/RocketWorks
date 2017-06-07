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
            reader = new NetworkReader(readBuffer);

            sendThread = new Thread(SendLoop);
            sendThread.Start();
            recieveThread = new Thread(RecieveLoop);
            recieveThread.Start();
        }

        private void SendLoop()
        {
            Thread.Sleep(200);
            try
            {
                // ShutdownEvent is a ManualResetEvent signaled by
                // Client when its time to close the socket.
                bool connected = true;
                while (connected && socket.Connected)
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
                            //while(pos < targetPos)
                            //{
                                //prevPos = pos;
                                //pos = Math.Min(writeStream.Position, pos + 1024);
                                lock (socket)
                                {
                                    ArraySegment<Byte> seg = writeStream.AsArraySegment();
                                    if (socket.Send(seg.Array, (int)prevPos, (int)targetPos, SocketFlags.Partial) > 0)
                                    {
                                        //RocketLog.Log("Packet sent: " + pos);
                                        state = SocketState.IDLE;
                                    }
                                    else
                                    {
                                        // The connection has closed gracefully, so stop the
                                        // thread.
                                        connected = false;
                                    }
                                }
                                //if(pos < targetPos)
                                 //   Thread.Sleep(5000);
                            //}
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
            Thread.Sleep(10);
            try
            {
                bool connected = true;
                while (connected && socket.Connected)
                {
                    try
                    {
                        while (socket.Available < 2)
                        {
                            // Give up the remaining time slice.
                            Thread.Sleep(1);
                        }
                        reader.SeekZero();
                        //RocketLog.Log("RecieveSocket: " + socket.Available);
                        if (socket.Receive(readBuffer.AsArraySegment().Array, 2, SocketFlags.Partial) > 0)
                        {
                            ushort size = reader.ReadUInt16();
                            //RocketLog.Log("Recieve size: " + size);
                            readBuffer.WriteCheckForSpace(size);
                            reader.SeekZero();
                            //RocketLog.Log("Received packet size: " + size);
                            while (socket.Available < size)
                            {
                                //RocketLog.Log("Waiting for actual packet");
                                // Give up the remaining time slice.
                                Thread.Sleep(1);
                            }
                            if(socket.Receive(readBuffer.AsArraySegment().Array, 0, (int)size, SocketFlags.Partial) > 0)
                            {
                                //RocketLog.Log("Received packet");
                                lock (reader)
                                {
                                    RecieveResultDelegate(reader, id);
                                }
                            } else
                            {
                                RocketLog.Log("Receive went wrong");
                                connected = false;
                            }
                        }
                        else
                        {
                            RocketLog.Log("Receive went wrong");
                            connected = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        RocketLog.Log("Read exception: " + ex, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                RocketLog.Log("Read exception: " + ex, ex);
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
            recieveThread.Abort();
            sendThread.Abort();
            state = SocketState.DISCONNECTED;
        }
    }
}
