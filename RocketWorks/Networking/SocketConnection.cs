using RocketWorks.Promises;
using System;
using System.IO;
using System.Net.Sockets;

namespace RocketWorks.Networking
{
    public enum SocketState
    {
        DISCONNECTED = 0,
        IDLE = 1,
        PENDING = 2,
        SENDING =3
    }

    public class StreamResult
    {
        public MemoryStream stream;
        public SocketConnection id;
        public StreamResult(MemoryStream stream, SocketConnection id)
        {
            this.stream = stream; this.id = id;
        }
    }

    public class SocketConnection
    {
        private MemoryStream writeStream;
        private MemoryStream readStream;
        private byte[] readBuffer;
        private MemoryStream sendBuffer;
        private SocketState state;
        public SocketState State { get { return state; } }

        private IAsyncResult asyncResult;
        public bool CanWrite { get { return sendBuffer.Position != 0 && state != SocketState.SENDING; } }

        private bool isReading = false;
        public bool IsReading { get { return isReading; } }
        public bool CanRead { get { return socket.Connected && socket.Available >= 4; } }
        private Socket socket;
        private int id;

        private int packetsSent = 0;
        private int packetsRead = 0;

        private int readCounter;

        public bool Connected { get { return socket.Connected; } }

        public SocketConnection(Socket socket, int id)
        {
            state = SocketState.IDLE;
            this.socket = socket;
            this.id = id;
            sendBuffer = new MemoryStream();
            readStream = new MemoryStream();
            readBuffer = new byte[0];
        }

        public void Write(byte[] buffer, int v, int size)
        {
            sendBuffer.Write(buffer, v, size);
            if(sendBuffer.Position != 0 && state == SocketState.IDLE)
                state = SocketState.PENDING;
        }

        public void SendAsync()
        {
            WriteAsync(sendBuffer.GetBuffer(), (int)sendBuffer.Position, socket);
            sendBuffer = new MemoryStream();
        }

        private void WriteAsync(byte[] bytes, int size, Socket socket)
        {
            lock (this)
            {
                try
                {
                    SocketError err;
                    asyncResult = null;
                    socket.BeginSend(bytes, 0, size, SocketFlags.None, out err, WriteCompleted, socket);
                    
                    state = SocketState.SENDING;
                    if (err != SocketError.Success)
                    {
                        RocketLog.Log(err.ToString());
                        //Disconnect for now
                        socket.Disconnect(false);
                    }
                }
                catch (Exception ex)
                {
                    RocketLog.Log(ex.Message, this);
                    socket.Disconnect(false);
                }
            }
        }


        private void WriteCompleted(IAsyncResult ar)
        {
            lock (this)
            {
                asyncResult = ar;
                Socket sock = (Socket)ar.AsyncState;
                try
                {
                    int sent = sock.EndSend(ar);
                }
                catch
                {
                    return;
                }
                if (sendBuffer.Position != 0)
                {
                    //state = SocketState.PENDING;
                    SendAsync();
                }
                else
                    state = SocketState.IDLE;
            }
                
            //RocketLog.Log("Packet sent: " + sent + " bytes " + sock.Connected, this);
        }

        public void StartRead(IPromise<StreamResult> promise)
        {
            ReadAsync(promise);
        }

        private void ReadAsync(IPromise<StreamResult> promise)
        {
            if (!socket.Connected)
                return;
            if (socket.Available < 4)
                return;
            if(isReading)
            {
                RocketLog.Log("         THIS IS REALLY FAULTY", this);
                return;
            }
            isReading = true;
            byte[] buffer = new byte[4];
            this.readBuffer = buffer;
            SocketError err;
            socket.BeginReceive(buffer, 0, 4, SocketFlags.Partial, out err, ReadPacketSize, promise);
            if (err != null && err != SocketError.Success)
                RocketLog.Log(err.ToString());
        }

        private void ReadPacketSize(IAsyncResult ar)
        {
            IPromise<StreamResult> promise = (IPromise<StreamResult>)ar.AsyncState;
            lock (this)
            {
                try
                {
                    int packets = socket.EndReceive(ar);
                    if (packets > 0)
                    {
                        BinaryReader reader = new BinaryReader(new MemoryStream(readBuffer));
                        int size = reader.ReadInt32();
                        readBuffer = new byte[size];
                        socket.BeginReceive(readBuffer, 0, size, SocketFlags.Partial, ReadCommandPacket, promise);
                    }
                    else
                    {
                        RocketLog.Log("!!!!!!!!packet size failure", this);
                        readBuffer = new byte[0];
                        promise.Fail(new StreamResult(null, this));
                        promise.Complete(new StreamResult(null, this));
                        isReading = false;
                    }
                }
                catch (Exception ex)
                {
                    RocketLog.Log("ReadError: " + ex.Message, this);
                    promise.Fail(new StreamResult(null, this));
                    promise.Complete(new StreamResult(null, this));
                    isReading = false;
                }
            }
        }

        private void ReadCommandPacket(IAsyncResult ar)
        {
            IPromise<StreamResult> promise = (IPromise<StreamResult>)ar.AsyncState;
            int packets = socket.EndReceive(ar);

            lock (this)
            {
                try
                {
                    if (packets > 0)
                    {
                        MemoryStream mem = new MemoryStream(readBuffer);
                        promise.Succeed(new StreamResult(mem, this));
                    }
                    else
                    {
                        RocketLog.Log("!!!!!!!!!!!!Fail command read", this);
                        readBuffer = new byte[0];
                        promise.Fail(new StreamResult(null, this));
                    }
                }
                catch (Exception ex)
                {
                    RocketLog.Log("ReadError: " + ex.Message + ex.StackTrace, this);

                    promise.Fail(new StreamResult(null, this));
                }

                readBuffer = new byte[0];
                isReading = false;
                promise.Complete(new StreamResult(null, this));
            }
        }

        public void Close()
        {
            socket.Close();
            state = SocketState.DISCONNECTED;
        }
    }
}
