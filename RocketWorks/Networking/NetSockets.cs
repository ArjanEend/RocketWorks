using System;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Collections.Generic;
using RocketWorks.Commands;

namespace RocketWorks.Networking
{

    public class NetSockets
    {
        bool socketReady = false;
        
        private Socket socket;
        private Socket connection;
        private List<Socket> connectedClients;
        private Dictionary<Socket, NetworkStream> streams;
        private Commander commander;

        private NetworkStream stream;
        private BinaryFormatter formatter;

        private string Host = "127.0.0.1";
        private int Port = 8000;

        private int userId = 0;
        public int UserId
        {
            get { return userId; }
        }

        public NetSockets(Commander commander)
        {
            connectedClients = new List<Socket>();
            streams = new Dictionary<Socket, NetworkStream>();
            this.commander = commander;
            userId = new Random(DateTime.Now.Millisecond).Next(100);
        }

        public void SetupSocket(bool server = true)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(Host);
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Port);

                socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                if (server)
                {
                    socket.Bind(localEndPoint);
                    socket.Listen(10);
                    WaitForConnection(socket);
                } else
                {
                    socket.Connect(localEndPoint);
                    connectedClients.Add(socket);
                    streams.Add(socket, new NetworkStream(socket));
                }

                formatter = new BinaryFormatter();

                socketReady = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Socket error:" + e);
            }
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
        }

        public void WriteSocket<T>(ICommand<T> command)
        {
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
                ICommand command = formatter.Deserialize(stream) as ICommand;
                if(command != null)
                    commander.Execute(command);
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

        public void MaintainConnection()
        {
            if (!stream.CanRead)
            {
                SetupSocket();
            }
        }
    }
}
