using System;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Collections.Generic;
using RocketWorks.Commands;
using RocketWorks.Serialization;

namespace RocketWorks.Networking
{
    public class SocketController
    {
        bool socketReady = false;
        
        private Socket socket;
        private List<Socket> connectedClients;
        private Dictionary<Socket, NetworkStream> streams;
        private NetworkCommander commander;
        
        private BinaryFormatter formatter;
        private BinaryWriter bWriter;
        private BinaryReader bReader;
        private MemoryStream memStream;

        private int userId = 0;
        public int UserId
        {
            get { return userId; }
        }

        public Action<uint> UserConnectedEvent = delegate { };

        public SocketController(NetworkCommander commander)
        {
            connectedClients = new List<Socket>();
            streams = new Dictionary<Socket, NetworkStream>();
            memStream = new MemoryStream();
            bWriter = new BinaryWriter(memStream);
            bReader = new BinaryReader(memStream);
            this.commander = commander;
            userId = new Random(DateTime.Now.Millisecond).Next(100);
        }

        public void SetupSocket(bool server = true, int port = 9001)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (server)
                {
                    IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
                    IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
                    socket.Bind(localEndPoint);
                    socket.Listen(10);
                    WaitForConnection(socket);
                }

                formatter = new BinaryFormatter();
                formatter.Binder = new UnityBinder();

                socketReady = true;
            }
            catch (Exception e)
            {
                RocketLog.Log("Socket error:" + e, this);
            }
        }

        public void Connect(string ip, int port)
        {
            IPAddress ipAddress = IPAddress.Parse(ip);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            socket.Connect(localEndPoint);
            connectedClients.Add(socket);
            streams.Add(socket, new NetworkStream(socket));
        }

        private void WaitForConnection(Socket socket)
        {
            socket.BeginAccept(OnNewConnection, socket);
        }

        private void OnNewConnection(IAsyncResult ar)
        {
            Socket newSocket = socket.EndAccept(ar);
            connectedClients.Add(newSocket);
            streams.Add(newSocket, new NetworkStream(newSocket));
            WaitForConnection(socket);
            RocketLog.Log("New connection accepted", this);

            UserConnectedEvent((uint)connectedClients.Count - 1);
        }

        public void WriteSocket<T>(ICommand<T> command, int toUser)
        {
            RocketLog.Log(connectedClients.Count + " : " + toUser, this);
            /*if (!connectedClients[toUser].Connected)
            {
                RemoveConnection(toUser);
                return;
            }*/
            try
            {
                WriteAsync(CreateBuffer(command), connectedClients[toUser]);
                /*NetworkStream writeStream = new NetworkStream(connectedClients[toUser]);
                
                formatter.Serialize(writeStream, command);
                writeStream.Close();
                writeStream = null;*/
            }
            catch(Exception ex)
            {
                RocketLog.Log("WriteException: " + ex.Message, this);
                RemoveConnection(toUser);
            }
        }

        public void WriteSocket<T>(ICommand<T> command)
        {
            if (!socketReady)
                return;

            for (int i = connectedClients.Count - 1; i >= 0; i--)
            {
                WriteSocket(command, i);
            }
        }

        public void RemoveConnection(int index)
        {
            connectedClients[index].Close();
            connectedClients.RemoveAt(index);
        }

        public void Update()
        {
            for(int i = 0; i < connectedClients.Count; i++)
            {
                ReadSocket(connectedClients[i]);
            }
        }

        private void WriteAsync(byte[] bytes, Socket socket)
        {
            socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, WriteCompleted, socket);
        }

        private void WriteCompleted(IAsyncResult ar)
        {
            Socket sock = (Socket)ar.AsyncState;
            sock.EndSend(ar);
        }

        private void ReadAsync(Socket socket)
        {
            byte[] buffer = new byte[4];
            int bSize = socket.Receive(buffer);
            if (bSize < 4)
                return;
            int size = bReader.Read(buffer, 0, 4);
            buffer = new byte[size];
            socket.Receive(buffer);

            MemoryStream mem = new MemoryStream(buffer);
            ICommand command = (ICommand)formatter.Deserialize(mem);
            commander.Execute(command);
        }

        private void Read(IAsyncResult ar)
        {
            /*Socket socket = (Socket)ar.AsyncState;
            int read = socket.EndReceive(ar);
            if (read > 0)
            {
                /*for (int i = 0; i < read; i++)
                {
                    status.TransmissionBuffer.Add(status.buffer[i]);
                }
                //we need to read again if this is true
                if (read == status.buffer.Length)
                {
                    //status.Socket.BeginReceive(status.buffer, 0, status.buffer.Length, SocketFlags.None, Receive, status);
                }
            }*/
        }

        private byte[] CreateBuffer(object obj)
        {
            memStream.Position = 4;
            formatter.Serialize(memStream, obj);
            byte[] returnValue = memStream.GetBuffer();

            memStream.Position = 0;
            bWriter.Write(returnValue.Length - 4);

            return returnValue;
        }


        private void ReadSocket(Socket socket)
        {
            if (!streams.ContainsKey(socket))
                return;
            NetworkStream stream = streams[socket];
            if (stream == null)
            {
                stream = new NetworkStream(socket);
            }

            ReadAsync(socket);

            //if (socket.re)
           // {
                /*
                INetworkCommand command = formatter.UnsafeDeserialize(stream, null) as INetworkCommand;
                if(command != null)
                    commander.Execute(command, 
                        (uint)connectedClients.IndexOf(socket) + 1);
                stream.Close();
                streams[socket] = new NetworkStream(socket);*/
           // }
        }

        public void CloseSocket()
        {
            if (!socketReady)
                return;

            socket.Close();
            socket.Disconnect(true);
            socketReady = false;
        }
    }
}
