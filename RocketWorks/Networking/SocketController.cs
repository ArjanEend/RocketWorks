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
                Console.WriteLine("Socket error:" + e);
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
            Console.WriteLine("New connection accepted");

            UserConnectedEvent((uint)connectedClients.Count - 1);
        }

        public void WriteSocket<T>(ICommand<T> command)
        {
            Console.WriteLine(command.GetType().AssemblyQualifiedName);
            if (!socketReady)
                return;

            for (int i = 0; i < connectedClients.Count; i++)
            {
                NetworkStream writeStream = new NetworkStream(connectedClients[i]);

                formatter.Serialize(writeStream, command);
                writeStream.Close();
                writeStream = null;
            }
        }

        public void Update()
        {
            for(int i = 0; i < connectedClients.Count; i++)
            {
                ReadSocket(connectedClients[i]);
            }
           
        }

        private void ReadSocket(Socket socket)
        {
            NetworkStream stream = streams[socket];
            if (stream == null)
            {
                stream = new NetworkStream(socket);
            }

            if (stream.DataAvailable)
            {
                INetworkCommand command = formatter.UnsafeDeserialize(stream, null) as INetworkCommand;
                if(command != null)
                    commander.Execute(command, 
                        (uint)connectedClients.IndexOf(socket) + 1);
                stream.Close();
                streams[socket] = new NetworkStream(socket);
            }
        }

        public void CloseSocket()
        {
            if (!socketReady)
                return;
            socket.Close();
            socketReady = false;
        }
    }
}
