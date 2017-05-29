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

    public delegate void ResultDelegate(byte[] buffer, int id);

    public class SocketConnection
    {
        private MemoryStream writeStream;
        private MemoryStream readStream;
        private byte[] readBuffer;
        private MemoryStream sendBuffer;
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
            sendBuffer = new MemoryStream();
            readStream = new MemoryStream();
            readBuffer = new byte[0];

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
                        // We could use the ReadTimeout property and let Read()
                        // block.  However, if no data is received prior to the
                        // timeout period expiring, an IOException occurs.
                        // While this can be handled, it leads to problems when
                        // debugging if we are wanting to break when exceptions
                        // are thrown (unless we explicitly ignore IOException,
                        // which I always forget to do).
                        while (state != SocketState.PENDING)
                        {
                            // Give up the remaining time slice.
                            Thread.Sleep(1);
                        }
                        {
                            lock (sendBuffer)
                            {
                                writeStream = sendBuffer;
                                sendBuffer = new MemoryStream();
                            }
                            state = SocketState.SENDING;
                            //RocketLog.Log("Sending: " + writeStream.Position);
                            if (socket.Send(writeStream.GetBuffer(), (int)writeStream.Position, SocketFlags.None) > 0)
                            {
                                state = SocketState.IDLE;
                            }
                            else
                            {
                                // The connection has closed gracefully, so stop the
                                // thread.
                                connected = false;
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
                bool connected = true;
                while (connected)
                {
                    Thread.Sleep(1);
                    //RocketLog.Log("ReceiveLoop");
                    try
                    {
                        // We could use the ReadTimeout property and let Read()
                        // block.  However, if no data is received prior to the
                        // timeout period expiring, an IOException occurs.
                        // While this can be handled, it leads to problems when
                        // debugging if we are wanting to break when exceptions
                        // are thrown (unless we explicitly ignore IOException,
                        // which I always forget to do).
                        while (socket.Available < 4)
                        {
                            // Give up the remaining time slice.
                            Thread.Sleep(1);
                        }
                        readBuffer = new byte[4];
                        if (socket.Receive(readBuffer, 4, SocketFlags.Partial) > 0)
                        {
                            BinaryReader reader = new BinaryReader(new MemoryStream(readBuffer));
                            int size = reader.ReadInt32();
                            readBuffer = new byte[size];
                            //RocketLog.Log("Received packet size: " + size);
                            while (socket.Available < size)
                            {

                                //RocketLog.Log("Waiting for actual packet");
                                // Give up the remaining time slice.
                                Thread.Sleep(1);
                            }
                            if(socket.Receive(readBuffer, size, SocketFlags.Partial) > 0)
                            {
                                //RocketLog.Log("Received packet");
                                RecieveResultDelegate(readBuffer, id);
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
            sendBuffer.Write(buffer, v, size);
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
