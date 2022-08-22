using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace ManyToOneChat
{
    /// <summary>
    /// Handles the communication from server to client
    /// </summary>
    public class ClientHandler
    {
        public string clientUsername { get; set; }
        public TcpClient clientSocket;
        public NetworkStream clientStream;
        private bool isConnected;

        private byte[] privateKey;

        //Message properties
        public int newMessageCount { get; set; }

        private List<Message> _messageList;
        public List<Message> messageList
        { get; private set; }


        public ClientHandler(TcpClient client, string serverUsername)
        {
            this.clientSocket = client;
            clientStream = clientSocket.GetStream();
            isConnected = true;
            messageList = new List<Message>();



            //Send server username


            Aes aes = Aes.Create();
            privateKey = aes.Key;

            Message outgoingMsg = new Message(MessageControlFlag.USERNAME, MessageSender.SERVER, serverUsername);
            byte[] newOutgoingMsg = outgoingMsg.ConvertToByte();
            

            byte[] outStream = new byte[32 + newOutgoingMsg.Length];
            Buffer.BlockCopy(privateKey, 0, outStream, 0, 32);
            Buffer.BlockCopy(newOutgoingMsg, 0, outStream, 32, newOutgoingMsg.Length);

            clientStream.Write(outStream, 0, outStream.Length);
            clientStream.Flush();



            //Get client Username
            byte[] receivedDataBytes = new byte[1000];
            clientStream.Read(receivedDataBytes, 0, receivedDataBytes.Length);

            byte[] receivedKey = new byte[32];
            Buffer.BlockCopy(receivedDataBytes, 0, receivedKey, 0, 32);
            byte[] receivedMessage = new byte[receivedDataBytes.Length - 32];
            Buffer.BlockCopy(receivedDataBytes, 32, receivedMessage, 0, receivedDataBytes.Length - 32);


            //If have extra time, make it such that the clienthandler auto unregisters itself from serverclientmanager
            if (receivedKey != privateKey)
                Console.WriteLine("[ERROR] Private key doesn't match..");

            Message message = new Message(receivedMessage, MessageSender.CLIENT);
            clientUsername = message.message;
            
            Thread awaitMsgThread = new Thread(WaitMessage);
            awaitMsgThread.Start();

            Console.WriteLine("[SERVER] Connected with user: " + clientUsername);
        }


        /// <summary>
        /// Can be used by controller
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(Message message)
        {
            byte[] outStream = AESEncryption.EncryptStream(message, privateKey);

            clientStream.Write(outStream, 0, outStream.Length);
            clientStream.Flush();
            messageList.Add(message);
            ServerClientManager.Instance.OnNewMessage(this);

        }

        /// <summary>
        /// Can be used by controller
        /// </summary>
        public void CloseConnection()
        {
            isConnected = false;

            clientStream.Close();
            clientSocket.Close();

            ServerClientManager.Instance.OnClientDisconnect(this);
        }

        public void WaitMessage()
        {
            try
            {
                while (isConnected)
                {
                    byte[] inStream = new byte[1000];

                    clientStream.Read(inStream, 0, inStream.Length);

                    byte[] messageByte = AESEncryption.DecryptStream(inStream, privateKey, out bool isDisconnectMsg);

                    Message message = new Message(messageByte, MessageSender.CLIENT);

                    if (isDisconnectMsg || message.controlFlag == MessageControlFlag.DISCONNECT)
                    {
                        CloseConnection();
                        break;
                    }

                    if (message.controlFlag == MessageControlFlag.MESSAGE)
                    {
                        messageList.Add(message);
                        newMessageCount += 1;
                        ServerClientManager.Instance.OnNewMessage(this);
                    }
                }
            
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}
