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
        private byte[] buffer;
        private MemoryStream sendBuffer;
        private SocketState state;
        public SocketState State { get { return state; } }
        private bool isReading = false;
        public bool IsReading { get { return isReading; } }
        private Socket socket;
        private int id;

        private int readCounter;

        public SocketConnection(Socket socket, int id)
        {
            state = SocketState.IDLE;
            this.socket = socket;
            this.id = id;
            sendBuffer = new MemoryStream();
            readStream = new MemoryStream();
            buffer = new byte[0];
        }

        public void Write(byte[] buffer, int v, int size)
        {
            sendBuffer.Write(buffer, v, size);
            if(state == SocketState.IDLE)
                state = SocketState.PENDING;
        }

        public void SendAsync()
        {
            WriteAsync(sendBuffer.GetBuffer(), (int)sendBuffer.Position, socket);
            sendBuffer = new MemoryStream();
        }

        private void WriteAsync(byte[] bytes, int size, Socket socket)
        {
            try
            {
                socket.BeginSend(bytes, 0, size, SocketFlags.None, WriteCompleted, socket);
                state = SocketState.SENDING;
            }
            catch
            {
                socket.Disconnect(false);
            }
        }


        private void WriteCompleted(IAsyncResult ar)
        {
            Socket sock = (Socket)ar.AsyncState;
            int sent = sock.EndSend(ar);
            state = SocketState.IDLE;
            //RocketLog.Log("Packet sent: " + sent + " bytes", this);
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
            this.buffer = buffer;
            socket.BeginReceive(buffer, 0, 4, SocketFlags.Partial, ReadPacketSize, promise);
        }

        private void ReadPacketSize(IAsyncResult ar)
        {
            IPromise<StreamResult> promise = (IPromise<StreamResult>)ar.AsyncState;
            
            try
            {
                int packets = socket.EndReceive(ar);
                if (packets > 0)
                {
                    BinaryReader reader = new BinaryReader(new MemoryStream(buffer));
                    int size = reader.ReadInt32();
                    buffer = new byte[size];
                    socket.BeginReceive(buffer, 0, size, SocketFlags.None, ReadCommandPacket, promise);
                }
                else
                {
                    RocketLog.Log("!!!!!!!!packet size failure", this);
                    buffer = new byte[0];
                    promise.Fail(new StreamResult(null, this));
                    promise.Complete(new StreamResult(null, this));
                    isReading = false;
                }
            }
            catch(Exception ex)
            {
                RocketLog.Log("ReadError: " + ex.Message, this);
                promise.Fail(new StreamResult(null, this));
                promise.Complete(new StreamResult(null, this));
                isReading = false;
            }
        }

        private void ReadCommandPacket(IAsyncResult ar)
        {
            IPromise<StreamResult> promise = (IPromise<StreamResult>)ar.AsyncState;
            int packets = socket.EndReceive(ar);

            try
            {
                if (packets > 0)
                {
                    MemoryStream mem = new MemoryStream(buffer);
                    promise.Succeed(new StreamResult(mem, this));
                }
                else
                {
                    RocketLog.Log("!!!!!!!!!!!!Fail command read", this);
                    buffer = new byte[0];
                    promise.Fail(new StreamResult(null, this));
                }
            }
            catch (Exception ex)
            {
                RocketLog.Log("ReadError: " + ex.Message + ex.StackTrace, this);

                promise.Fail(new StreamResult(null, this));
            }

            promise.Complete(new StreamResult(null, this));
            buffer = new byte[0];
            isReading = false;
        }

        public void Close()
        {
            socket.Close();
            state = SocketState.DISCONNECTED;
        }
    }
}
