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
        private bool receive = false;
        public Action<uint> UserConnectedEvent = delegate { };

        public SocketController(NetworkCommander commander)
        {
            connectedClients = new List<Socket>();
            streams = new Dictionary<Socket, NetworkStream>();
            buffers = new Dictionary<Socket, byte[]>();
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
            /*if (!connectedClients[toUser].Connected)
            {
                RemoveConnection(toUser);
                return;
            }*/
            int size = 0;
            byte[] buffer = CreateBuffer(command, out size);
            try
            {
                WriteAsync(buffer, size, connectedClients[toUser]);
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

        private void WriteAsync(byte[] bytes, int size, Socket socket)
        {
            socket.BeginSend(bytes, 0, size, SocketFlags.None, WriteCompleted, socket);
        }

        private void WriteCompleted(IAsyncResult ar)
        {
            Socket sock = (Socket)ar.AsyncState;
            int sent = sock.EndSend(ar);
            RocketLog.Log("Packet sent: " + sent + " bytes", this);
        }

        private void ReadAsync(Socket socket)
        {
            byte[] buffer = new byte[4];
            buffers[socket] = buffer;
            RocketLog.Log("BeginReceive", this);
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
            }

            }
            catch
            {
                RocketLog.Log("ReadError", this);
            }

        }

        private void ReadCommandPacket(IAsyncResult ar)
        {

            try
            {
                Socket socket = (Socket)ar.AsyncState;
                int packets = socket.EndReceive(ar);
                if (packets > 0)
                {
                    MemoryStream mem = new MemoryStream(buffers[socket]);
                    ICommand command = (ICommand)formatter.Deserialize(mem);
                    commander.Execute(command);
                }
                buffers[socket] = new byte[0];
            }
            catch(Exception ex)
            {
                RocketLog.Log("ReadError: " + ex.Message + ex.StackTrace, this);
            }
        }

        private byte[] CreateBuffer(object obj, out int size)
        {
            memStream.Position = 4;
            formatter.Serialize(memStream, obj);
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

            socket.Close();
            socket.Disconnect(true);
            socketReady = false;
        }
    }
}
