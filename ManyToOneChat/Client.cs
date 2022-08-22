using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ManyToOneChat
{
    public class Client
    {
        private static Client instance = null;
        private static readonly object objectLock = new object();

        public string clientUsername;

        private IPAddress serverIPAddress;
        private int serverPort;
        private byte[] privateKey;
        public string serverUsername;
        public TcpClient serverSocket;
        public NetworkStream serverStream;

        public bool isConnected;

        private List<Message> _messageList;

        public List<Message> messageList
        {
            get
            {
                return _messageList;
            }
            set
            {
                _messageList = value;
                //Broadcast update
            }
        }

        private Client()
        {

        }

        //-------- [ EVENTS ] -----------
        public static event Action<Client> onNewMessage;
        public static event Action onClientDisconnect;

        public void OnNewMessage(Client client)
        {
            onNewMessage?.Invoke(client);
        }
        public void OnClientDisconnect()
        {
            onClientDisconnect?.Invoke();
            isConnected = false;

        }

        public static Client Instance
        {
            get
            {
                lock(objectLock)
                {
                    if (instance == null)
                    {
                        instance = new Client();
                    }
                    return instance;
                }
            }
        }

        public void ConnnectToServer(string ipAddress, string portNumber, string clientUsername)
        {
            if (isConnected)
                return;

            this.clientUsername = clientUsername;
            messageList = new List<Message>();

            serverIPAddress = IPAddress.Parse(ipAddress);
            serverPort = int.Parse(portNumber);

            serverSocket = new TcpClient();
            serverSocket.Connect(serverIPAddress, serverPort);
            serverStream = serverSocket.GetStream();
            
            isConnected = true;


            //Receive server username

            byte[] receivedDataBytes = new byte[1000];
            serverStream.Read(receivedDataBytes, 0, receivedDataBytes.Length);

            byte[] receivedKey = new byte[32];
            Buffer.BlockCopy(receivedDataBytes, 0, receivedKey, 0, 32);
            privateKey = receivedKey;
            byte[] receivedMessage = new byte[receivedDataBytes.Length - 32];
            Buffer.BlockCopy(receivedDataBytes, 32, receivedMessage, 0, receivedDataBytes.Length - 32);

            Message receivedMsg = new Message(receivedMessage, MessageSender.CLIENT);
            serverUsername = receivedMsg.message;


            //Send client username
            Message outgoingMsg = new Message(MessageControlFlag.USERNAME, MessageSender.CLIENT, clientUsername);
            byte[] newOutgoingMsg = outgoingMsg.ConvertToByte();

            byte[] outStream = new byte[32 + newOutgoingMsg.Length];
            Buffer.BlockCopy(privateKey, 0, outStream, 0, 32);
            Buffer.BlockCopy(newOutgoingMsg, 0, outStream, 32, newOutgoingMsg.Length);

            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();


            Thread awaitMsgThread = new Thread(WaitMessage);
            awaitMsgThread.Start();
        }

        public void SendMessage(Message message)
        {
            byte[] outStream = AESEncryption.EncryptStream(message, privateKey);

            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
            messageList.Add(message);
            OnNewMessage(this);
            
        }

        public void WaitMessage()
        {
            try
            {
                while (isConnected)
                {
                    byte[] inStream = new byte[1000];
                    serverStream.Read(inStream, 0, inStream.Length);

                    byte[] messageByte = AESEncryption.DecryptStream(inStream, privateKey, out bool isDisconnectMsg);

                    Message message = new Message(messageByte, MessageSender.SERVER);

                    if (message.controlFlag == MessageControlFlag.DISCONNECT)
                    {
                        CloseConnection();
                        break;
                    }

                    if (message.controlFlag == MessageControlFlag.MESSAGE)
                    {
                        messageList.Add(message);
                        OnNewMessage(this);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void CloseConnection()
        {
            isConnected = false;

            serverStream.Close();
            serverSocket.Close();

            OnClientDisconnect();
        }

        //TODO: EVENTS
    }
}
