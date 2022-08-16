using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ManyToOneChat
{
    public sealed class ServerClientManager
    {
        private static ServerClientManager instance = null;
        private static readonly object objectLock= new object();

        public bool isServerClientUp = false;

        private IPAddress serverIPAddress;
        private int serverPort;
        private TcpListener serverListener;
        private List<ClientHandler> clientList = new List<ClientHandler>();

        private string serverUsername;

        //Used when server is a client

        private ServerClientManager()
        {

        }

        // ------ [ Events ] ------
        public static event Action<ClientHandler> onNewClientConnect;
        public static event Action<ClientHandler> onNewMessage;
        public static event Action<ClientHandler> onClientDisconnect;
        public static event Action onServerDisconnect;
        public void OnNewClientConnect(ClientHandler client)
        {
            onNewClientConnect?.Invoke(client);
        }
        public void OnNewMessage(ClientHandler client)
        {
            onNewMessage?.Invoke(client);
        }
        public void OnClientDisconnect(ClientHandler client)
        {
            clientList.Remove(client);
            onClientDisconnect?.Invoke(client);
        }

        private void OnServerDisconnect()
        {
            onServerDisconnect?.Invoke();
        }


        public static ServerClientManager Instance
        {
            get
            {
                lock (objectLock)
                {
                    if (instance == null)
                    {
                        instance = new ServerClientManager();
                    }
                    return instance;
                }
            }
        }

        public void HostServer(string ipAddress, string portNumber, string serverUsername)
        {
            //Change Server client type
            if (isServerClientUp)
                return;

            this.serverUsername = serverUsername;
            clientList = new List<ClientHandler>();

            serverIPAddress = IPAddress.Parse(ipAddress);
            serverPort = int.Parse(portNumber);

            serverListener = new TcpListener(serverIPAddress, serverPort);
            serverListener.Start();

            Thread awaitConnectionThread = new Thread(AwaitConnections);
            awaitConnectionThread.Start();

            Console.WriteLine("[SERVER] Server hosted successfully on ip: " + serverIPAddress + " port: " + serverPort);
        }

        private void AwaitConnections()
        {
            try
            {
                while (true)
                {
                    TcpClient clientSocket = serverListener.AcceptTcpClient();
                    ClientHandler clientHandler = new ClientHandler(clientSocket, serverUsername);
                    clientList.Add(clientHandler);
                    OnNewClientConnect(clientHandler);

                }
            }
            catch(Exception e)
            {
                Console.WriteLine("[ERROR] " + e);
            }
            
        }

        public void CloseServer()
        {
            List<ClientHandler> clientListToClose = new List<ClientHandler>(clientList);
            for (int i = 0; i < clientListToClose.Count; i++)
            {
                clientListToClose[i].CloseConnection(); 
            }

            serverListener.Stop();

            OnServerDisconnect();
            isServerClientUp = false;

        }


        // ------- [ CLIENT HANDLING ] ---------

        
        public void DisconnectClient(ClientHandler client)
        {
            clientList.Remove(client);
            // Broadcast removal
        }

        public void SendMessageToClient(ClientHandler client, string message)
        {
            Message newMessage = new Message(MessageControlFlag.MESSAGE, MessageSender.SERVER, message);
            client.SendMessage(newMessage);

            //Broadcast updates
        }
    }
}
