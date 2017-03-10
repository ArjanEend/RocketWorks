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
        private Dictionary<Socket, byte[]> buffers;
        private Dictionary<Socket, MemoryStream> sendBuffers;

        private Dictionary<Socket, bool> sendStates;
        private NetworkCommander commander;
        
        private BinaryFormatter formatter;
        private BinaryWriter bWriter;
        private BinaryReader bReader;
        private MemoryStream memStream;

        private Rocketizer rocketizer;

        private Queue<INetworkCommand> commandQueue;
        private Queue<int> clientQueue;

        private int userId = -1;
        public int UserId
        {
            get { return userId; }
        }
        private bool receive = false;
        public Action<int> UserConnectedEvent = delegate { };
        public Action<int> UserIDSetEvent = delegate { };

        public SocketController(NetworkCommander commander, Rocketizer rocketizer)
        {
            connectedClients = new List<Socket>();
            streams = new Dictionary<Socket, NetworkStream>();
            buffers = new Dictionary<Socket, byte[]>();
            sendBuffers = new Dictionary<Socket, MemoryStream>();
            sendStates = new Dictionary<Socket, bool>();
            memStream = new MemoryStream();
            bWriter = new BinaryWriter(memStream);
            bReader = new BinaryReader(memStream);
            this.commander = commander;
            commander.AddObject(this);

            commandQueue = new Queue<INetworkCommand>();
            clientQueue = new Queue<int>();

            this.rocketizer = rocketizer;
        }

        public void SetUserID(int uid)
        {
            RocketLog.Log("My user ID is: " + uid);
            this.userId = uid;
            UserIDSetEvent(uid);
        }

        public void SetupSocket(bool server = true, int port = 9001)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (server)
                {
                    socket.LingerState = new LingerOption(false, 0);
                    IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
                    IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
                    socket.Bind(localEndPoint);
                    socket.Listen(10);
                    WaitForConnection(socket);
                }

                formatter = new BinaryFormatter();
                formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                formatter.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.XsdString;
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
            buffers.Add(socket, new byte[0]);
            sendBuffers.Add(socket, new MemoryStream());
            sendStates.Add(socket, false);
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
            buffers.Add(newSocket, new byte[0]);
            sendBuffers.Add(newSocket, new MemoryStream());
            sendStates.Add(newSocket, false);
            WaitForConnection(socket);
            RocketLog.Log("New connection accepted", this);

            int uid = connectedClients.IndexOf(newSocket);

            WriteSocket(new SetUserIDCommand(uid), uid);

            UserConnectedEvent(uid);
        }

        public void WriteSocket<T>(ICommand<T> command, int toUser)
        {
            if (toUser == -1)
                return;
            /*if (!connectedClients[toUser].Connected)
            {
                RemoveConnection(toUser);
                return;
            }*/
            int size = 0;
            byte[] buffer = CreateBuffer(command, out size);
            try
            {
                sendBuffers[connectedClients[toUser]].Write(buffer, 0, size);
                //WriteAsync(buffer, size, connectedClients[toUser]);
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
            {
                RocketLog.Log("Socket not initialized");
                return;
            }

            for (int i = connectedClients.Count - 1; i >= 0; i--)
            {
                //RocketLog.Log("Send to : " + i);
                WriteSocket(command, i);
            }
        }

        public void WriteSocket(Socket socket)
        {
            if (sendStates[socket] || sendBuffers[socket].Length == 0)
                return;

            WriteAsync(sendBuffers[socket].GetBuffer(), (int)sendBuffers[socket].Position, socket);
            sendBuffers[socket] = new MemoryStream();
        }

        public void RemoveConnection(int index)
        {
            connectedClients[index].Close();
            connectedClients.RemoveAt(index);
        }

        public void Update()
        {
            HandleCommands();
            for(int i = 0; i < connectedClients.Count; i++)
            {
                ReadSocket(connectedClients[i]);
                WriteSocket(connectedClients[i]);
            }
        }

        private void HandleCommands()
        {
            while (commandQueue.Count != 0)
                commander.Execute(commandQueue.Dequeue(), clientQueue.Dequeue());
        }

        private void WriteAsync(byte[] bytes, int size, Socket socket)
        {
            //RocketLog.Log("Packet send: " + size + " bytes", this);
            socket.BeginSend(bytes, 0, size, SocketFlags.None, WriteCompleted, socket);
            sendStates[socket] = true;
        }

        private void WriteCompleted(IAsyncResult ar)
        {
            Socket sock = (Socket)ar.AsyncState;
            int sent = sock.EndSend(ar);
            sendStates[sock] = false;
            //RocketLog.Log("Packet sent: " + sent + " bytes", this);
        }

        private void ReadAsync(Socket socket)
        {
            if (!socket.Connected)
                return;
            byte[] buffer = new byte[4];
            buffers[socket] = buffer;
            socket.BeginReceive(buffer, 0, 4, SocketFlags.None, ReadPacketSize, socket);
        }

        private void ReadPacketSize(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            int packets = socket.EndReceive(ar);

            try
            {
                if (packets > 0)
                {
                    BinaryReader reader = new BinaryReader(new MemoryStream(buffers[socket]));
                    int size = reader.ReadInt32();
                    buffers[socket] = new byte[size];
                    socket.BeginReceive(buffers[socket], 0, size, SocketFlags.None, ReadCommandPacket, socket);
                } else
                {
                    buffers[socket] = new byte[0];
                    ReadSocket(socket);
                }
            }
            catch
            {
                RocketLog.Log("ReadError", this);
            }

        }

        private void ReadCommandPacket(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            int packets = socket.EndReceive(ar);

            try
            {
                if (packets > 0)
                {
                    MemoryStream mem = new MemoryStream(buffers[socket]);
                    INetworkCommand command = rocketizer.ReadObject<INetworkCommand>(mem);//formatter.Deserialize(mem);
                    commandQueue.Enqueue(command);
                    clientQueue.Enqueue(connectedClients.IndexOf(socket));
                }
            }
            catch(Exception ex)
            {
                RocketLog.Log("ReadError: " + ex.Message + ex.StackTrace, this);
            }

            buffers[socket] = new byte[0];
            ReadSocket(socket);
        }

        private byte[] CreateBuffer(object obj, out int size)
        {
            rocketizer.SetStream(memStream);

            memStream.Position = 4;
            rocketizer.WriteObject(obj);
            //formatter.Serialize(memStream, obj);
            size = (int)memStream.Position;
            memStream.SetLength(size);
            byte[] returnValue = memStream.GetBuffer();

            memStream.Position = 0;
            bWriter.Write(size - 4);

            return returnValue;
        }


        private void ReadSocket(Socket socket)
        {
            if (!buffers.ContainsKey(socket))
                return;

            if(buffers[socket].Length == 0)
                ReadAsync(socket);
        }

        public void CloseSocket()
        {
            if (!socketReady)
                return;

            for(int i = 0; i < connectedClients.Count; i++)
            {
                connectedClients[i].Close();
                //connectedClients[i].Disconnect(false);
            }


            socket.Close();
            //socket.Disconnect(false);

            
            socket = null;

            socketReady = false;
        }
    }
}
