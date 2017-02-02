using UnityEngine;
using System;
using System.Collections;
using System.Net.Sockets;
using System.IO;
using RocketWorks.Networking;
using RocketWorks.Pooling;
using System.Runtime.Serialization.Formatters.Binary;

public class NetSockets
{
    private bool socketReady = false;

    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;
    private BinaryFormatter formatter;
    private string host = "127.0.0.1";
    private int port = 8000;

    public void setupSocket()
    {
        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            formatter = new BinaryFormatter();
            socketReady = true;

            byte[] message = new byte[1024];
            int bytesRead;
            bytesRead = 0;

            try
            {
                bytesRead = stream.Read(message, 0, 1024);
            }
            catch
            {
            }
        }
        catch (Exception e)
        {
            Debug.Log("Socket error:" + e);
        }
    }

    public void WriteSocket(ICommand<EntityPool> command)
    {
        if (!socketReady)
            return;

        formatter.Serialize(stream, command);
    }

    public ICommand<EntityPool> ReadCommand()
    {
        if (!socketReady)
            return null;

        ICommand<EntityPool> command = formatter.Deserialize(stream) as ICommand<EntityPool>;
        return command;
    }

    public void closeSocket()
    {
        if (!socketReady)
            return;
        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }

    public void MaintainConnection()
    {
        if (!stream.CanRead)
        {
            setupSocket();
        }
    }
}