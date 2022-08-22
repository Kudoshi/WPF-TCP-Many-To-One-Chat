using ManyToOneChat.Interface;
using ManyToOneChat.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ManyToOneChat.Controller
{
    public class ServerChatController :  IViewController, IServerChatController
    {
        public MainWindow PageManager { get ; set ; }
        public Page ViewPage { get ; set ; }
        public ServerChatView chatView;

        private List<ClientHandler> clientList = new List<ClientHandler>();
        private ClientHandler currentChatWith = null;

        public Page SetupController(MainWindow window)
        {
            PageManager = window;
            chatView = new ServerChatView(this);
            ViewPage = chatView;

            ServerClientManager.onNewClientConnect += OnNewClientConnect;
            ServerClientManager.onNewMessage += OnNewMessage;
            ServerClientManager.onClientDisconnect += OnClientDisconnect;
            ServerClientManager.onServerDisconnect += OnServerDisconnect;

            return ViewPage;
        }


        public void DisplayChatMessagePanel(ClientHandler client)
        {
            string username = client.clientUsername;
            List<Message> messageList = client.messageList;
            chatView.UpdateChatLogDisplay(username, messageList);
        }

        // ----------- [ SERVER EVENT ] ---------------

        private void OnClientDisconnect(ClientHandler obj)
        {
            Console.WriteLine("[CLIENT] Client {0} disconnected ", obj.clientUsername);
            clientList.Remove(obj);

            if (clientList.Count == 0)
            {
                chatView.UpdateChatLogDisplay(" ", new List<Message>());
            }
            else if (currentChatWith == obj)
            {
                currentChatWith = clientList[0];
                DisplayChatMessagePanel(clientList[0]);
            }

            chatView.UpdateChatListDisplay(clientList);
            
        }

        private void OnNewMessage(ClientHandler obj)
        {
            foreach(ClientHandler client in clientList)
            {
                if (client == currentChatWith)
                {
                    client.newMessageCount = 0;
                    DisplayChatMessagePanel(client);

                }
            }

            chatView.UpdateChatListDisplay(clientList);
        }

        private void OnNewClientConnect(ClientHandler obj)
        {

            Console.WriteLine("[CLIENT] New Client Connected: " + obj.clientUsername);

            clientList.Add(obj);

            if (clientList.Count == 1 && currentChatWith == null)
            {
                currentChatWith = obj;
                DisplayChatMessagePanel(obj);
            }
            chatView.UpdateChatListDisplay(clientList);

        }
        private void OnServerDisconnect()
        {
            ServerClientManager.onNewClientConnect -= OnNewClientConnect;
            ServerClientManager.onNewMessage -= OnNewMessage;
            ServerClientManager.onClientDisconnect -= OnClientDisconnect;
            ServerClientManager.onServerDisconnect -= OnServerDisconnect;

            PageManager.ChangeView(MainWindow.PageList.Home);
        }

        // ------- [ VIEW EVENTS ] ---------

        public void View_SendMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            if (currentChatWith == null)
            {
                string alertBoxText = "No client to send message to";
                string titleText = "No Client Found";

                MessageBox.Show(alertBoxText, titleText, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ServerClientManager.Instance.SendMessageToClient(currentChatWith, message);
        }

        public void View_DisconnectServer()
        {
            ServerClientManager.Instance.CloseServer();
        }

        public void View_EndConversation()
        {
            if (currentChatWith != null)
                currentChatWith.CloseConnection();
        }
        public void View_ChangeMessagePanel(object sender, MouseButtonEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)sender;
            currentChatWith = (ClientHandler)stackPanel.DataContext;
            currentChatWith.newMessageCount = 0;
            chatView.UpdateChatListDisplay(clientList);
            DisplayChatMessagePanel(currentChatWith);

        }
    }
}
