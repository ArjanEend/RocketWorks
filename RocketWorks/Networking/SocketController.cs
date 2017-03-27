using System;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Collections.Generic;
using RocketWorks.Commands;
using RocketWorks.Serialization;
using RocketWorks.Promises;

namespace RocketWorks.Networking
{
    public class SocketController
    {
        bool socketReady = false;
        
        private Socket socket;
        private List<SocketConnection> connectedClients;

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

        private bool addingCommand;

        public SocketController(NetworkCommander commander, Rocketizer rocketizer)
        {
            connectedClients = new List<SocketConnection>();
            
            memStream = new MemoryStream(2048);
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

                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
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
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            socket.NoDelay = true;

            connectedClients.Add(new SocketConnection(socket, connectedClients.Count));
        }

        private void WaitForConnection(Socket socket)
        {
            socket.BeginAccept(OnNewConnection, socket);
        }

        private void OnNewConnection(IAsyncResult ar)
        {
            Socket newSocket = socket.EndAccept(ar);
            newSocket.NoDelay = true;
            newSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            SocketConnection connection = new SocketConnection(newSocket, connectedClients.Count);
            connectedClients.Add(connection);
            WaitForConnection(socket);
            RocketLog.Log("New connection accepted", this);

            int uid = connectedClients.IndexOf(connection);

            WriteSocket(new SetUserIDCommand(uid), uid);

            UserConnectedEvent(uid);
        }

        public void WriteSocket<T>(ICommand<T> command, int toUser)
        {
            if (toUser == -1)
                return;
            
            int size = 0;
            byte[] buffer = CreateBuffer(command, out size);
            try
            {
                connectedClients[toUser].Write(buffer, 0, size);
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

        public void WriteSocket(SocketConnection socket)
        {
            if(!socket.Connected)
            {
                connectedClients.Remove(socket);
                return;
            }
            if (socket.CanWrite)
                socket.SendAsync();
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
            while (!addingCommand && commandQueue.Count != 0)
                commander.Execute(commandQueue.Dequeue(), clientQueue.Dequeue());
        }

        private byte[] CreateBuffer(object obj, out int size)
        {
            rocketizer.SetWriteStream(memStream);

            memStream.Position = 4;
            rocketizer.WriteObject(obj);
            size = (int)memStream.Position;
            memStream.SetLength(size);
            byte[] returnValue = memStream.GetBuffer();

            memStream.Position = 0;
            bWriter.Write(size - 4);
            bWriter.Flush();

            return returnValue;
        }


        private void ReadSocket(SocketConnection socket)
        {
            if(!socket.IsReading)
            socket.StartRead(
                new Promise<StreamResult>().OnSucces(ReadCommand).OnFail(ReadFail).OnComplete(ReadComplete)
                );
        }

        private void ReadCommand(StreamResult stream)
        {
            addingCommand = true;
            INetworkCommand command = rocketizer.ReadObject<INetworkCommand>(stream.stream);
            if (command == null)
                throw new Exception("Command could not be read...");

            commandQueue.Enqueue(command);
            clientQueue.Enqueue(connectedClients.IndexOf(stream.id));
            addingCommand = false;
        }

        private void ReadFail(StreamResult result)
        {
            //Log some error here?
            RocketLog.Log("Read failed", this);
        }

        private void ReadComplete(StreamResult result)
        {
            ReadSocket(result.id);
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
