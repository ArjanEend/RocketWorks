using System;
using System.Net.Sockets;
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
        private List<SocketConnection> connectedClients;

        private NetworkCommander commander;
        
        private NetworkWriter bWriter;
        private NetworkReader bReader;

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
        public Action DisconnectEvent = delegate { };

        private bool addingCommand;

        public SocketController(NetworkCommander commander, Rocketizer rocketizer)
        {
            SetTimeStamp(DateTime.UtcNow);

            connectedClients = new List<SocketConnection>();
            
            bWriter = new NetworkWriter();
            bReader = new NetworkReader();
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

        public void SetTimeStamp(DateTime timeStamp)
        {
            ServerTimeStamp.SetServerTime(timeStamp);
        }

        public void SetupSocket(bool server = true, string serverIP = "127.0.0.1", int port = 9001, bool waitforIP = false)
        {
            if (waitforIP)
            {
                Console.WriteLine("Please enter your desired IP (to which users will be connecting) or leave empty for localhost");
                string newIP = Console.ReadLine();
                if (!string.IsNullOrEmpty(newIP))
                    serverIP = newIP;

                serverIP.Replace("\n", "");

                Console.WriteLine("Setting up server at " + serverIP);
            }
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (server)
                {

                    socket.LingerState = new LingerOption(false, 0);
                    IPAddress ipAddress = serverIP == "127.0.0.1" ? IPAddress.Any : IPAddress.Parse(serverIP);
                    IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
                    socket.Bind(localEndPoint);
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                    socket.Listen(10);

                    WaitForConnection(socket);
                }

                socketReady = true;
            }
            catch (Exception e)
            {
                RocketLog.Log("Error setting up server please restart. \nSocket error:" + e, this);
            }
        }

        public void Connect(string ip, int port)
        {
            IPAddress ipAddress = null;
            IPAddress.TryParse(ip, out ipAddress);
            if(ipAddress == null)
            {
                IPHostEntry entry = Dns.GetHostEntry(ip);
                ipAddress = entry.AddressList[0];

            }
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            
            socket.Connect(localEndPoint);
            
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            socket.NoDelay = true;

            connectedClients.Add(new SocketConnection(socket, connectedClients.Count));
            connectedClients[connectedClients.Count - 1].RecieveResultDelegate += ReadCommand;
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
            connectedClients[connectedClients.Count - 1].RecieveResultDelegate += ReadCommand;

            WaitForConnection(socket);
            RocketLog.Log("New connection accepted", this);

            int uid = connectedClients.IndexOf(connection);

            WriteSocket(new SetUserIDCommand(uid, DateTime.UtcNow), uid);

            UserConnectedEvent(uid);
        }

        public void WriteSocket<T>(ICommand<T> command, int toUser)
        {
            if (toUser == -1)
                return;
            
            ushort size = 0;
            byte[] buffer = CreateBuffer(command, out size);
            try
            {
                SocketConnection connection = connectedClients[toUser];
                connection.Write(buffer, 0, size);
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

        public void RemoveConnection(int index)
        {
            connectedClients[index].Close();
            connectedClients.RemoveAt(index);
        }

        public void Update()
        {
            HandleCommands();
        }

        private void HandleCommands()
        {
            while (!addingCommand && commandQueue.Count != 0)
                commander.Execute(commandQueue.Dequeue(), clientQueue.Dequeue());
        }

        private byte[] CreateBuffer(object obj, out ushort size)
        {
            bWriter.StartMessage();
            rocketizer.WriteObject(obj, bWriter);
            size = (ushort)bWriter.Position;
            bWriter.FinishMessage();

            return bWriter.AsArray();
        }

        private void ReadCommand(NetworkReader reader, int id)
        {
            if (addingCommand)
                throw new Exception("Can't add 2 commands at the same time");
            addingCommand = true;
            INetworkCommand command = null;
            lock (reader)
            {
                command = rocketizer.ReadObject<INetworkCommand>(id, reader);
                if (command == null)
                    throw new Exception("Command could not be read...");
            }
            commander.Execute(command, id);
            addingCommand = false;
        }

        public void CloseSocket()
        {
            if (!socketReady)
                return;

            for(int i = 0; i < connectedClients.Count; i++)
            {
                connectedClients[i].Close();
            }

            socket.Close();
            socket = null;
            socketReady = false;
            DisconnectEvent();
        }
    }
}
