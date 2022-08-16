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
    /// <summary>
    /// Handles the communication from server to client
    /// </summary>
    public class ClientHandler
    {
        public string clientUsername { get; set; }
        public TcpClient clientSocket;
        public NetworkStream clientStream;
        private bool isConnected;

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

            //Get client Username
            byte[] receivedDataBytes = new byte[1000];
            clientStream.Read(receivedDataBytes, 0, receivedDataBytes.Length);

            messageList = new List<Message>();
            Message message = new Message(receivedDataBytes, MessageSender.CLIENT);
            clientUsername = message.message;
            
            //Send server username
            Message outgoingMsg = new Message(MessageControlFlag.USERNAME, MessageSender.SERVER, serverUsername);
            byte[] outStream = outgoingMsg.ConvertToByte();
            clientStream.Write(outStream, 0, outStream.Length);
            clientStream.Flush();

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
            string editedMessage = message.controlFlag + 'Ÿ' + message.message;

            byte[] outStream = message.ConvertToByte();

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

                    Message message = new Message(inStream, MessageSender.CLIENT);

                    if (message.controlFlag == MessageControlFlag.DISCONNECT)
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
