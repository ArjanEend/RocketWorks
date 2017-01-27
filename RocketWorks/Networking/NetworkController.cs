using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

namespace RocketWorks.Networking
{
    public class NetworkController : MonoBehaviour
    {

        int myReliableChannelId;
        int socketId;
        int socketPort = 8888;
        int connectionId = 0;
        int hostId = 0;
        int hostSocketId = 0;

        private bool host = false;

        private List<int> connectedClients = new List<int>();

        public void Init(bool host)
        {
            this.host = host;
            ConnectionConfig config = new ConnectionConfig();
            myReliableChannelId = config.AddChannel(QosType.Reliable);
            int maxConnections = 10;
            HostTopology topology = new HostTopology(config, maxConnections);

            NetworkTransport.Init();

            if (host)
                socketId = NetworkTransport.AddHost(topology, socketPort);
            else
                socketId = NetworkTransport.AddHost(topology);
            Debug.Log("Socket Open. SocketId is: " + socketId);
        }

        public void Connect()
        {
            byte error;
            connectionId = NetworkTransport.Connect(socketId, "127.0.0.1", socketPort, 0, out error);
            Debug.Log("Connected to server. ConnectionId: " + connectionId);
        }

        public void SendSocketMessage(ICommand command)
        {
            byte error;
            byte[] buffer = new byte[1024];
            Stream stream = new MemoryStream(buffer);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, command);

            int bufferSize = 1024;

            if (host)
            {
                for (int i = 0; i < connectedClients.Count; i++)
                {
                    NetworkTransport.Send(socketId, connectedClients[i], myReliableChannelId, buffer, bufferSize, out error);
                }
            }
            else
            {
                NetworkTransport.Send(socketId, connectionId, myReliableChannelId, buffer, bufferSize, out error);
            }
        }
    
        void Update()
        {
            int recHostId;
            int recConnectionId;
            int recChannelId;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            byte error;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);
            switch (recNetworkEvent)
            {
                case NetworkEventType.Nothing:
                    break;
                case NetworkEventType.ConnectEvent:
                    Debug.Log("incoming connection event received");
                    if (host)
                    {
                        connectedClients.Add(recConnectionId);
                    }
                    break;
                case NetworkEventType.DataEvent:
                    Stream stream = new MemoryStream(recBuffer);
                    BinaryFormatter formatter = new BinaryFormatter();
                    ICommand message = formatter.Deserialize(stream) as ICommand;
                    if (message != null)
                    {
                        //message.Execute(gameController);
                    }
                    Debug.Log("incoming message event received: " + message);
                    break;
                case NetworkEventType.DisconnectEvent:
                    Debug.Log("remote client event disconnected");
                    break;
            }
        }
    }
}