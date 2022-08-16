using ManyToOneChat.Interface;
using ManyToOneChat.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ManyToOneChat.Controller
{
    public class ClientChatController : IViewController, IClientChatController
    {
        public MainWindow PageManager { get ; set ; }
        public Page ViewPage { get ; set ; }
        public ClientChatView chatView;

        private List<Message> messageList = new List<Message>();
        public Page SetupController(MainWindow window)
        {
            PageManager = window;
            chatView = new ClientChatView(this);
            ViewPage = chatView;

            Client.onNewMessage += OnNewMessage;
            Client.onClientDisconnect += OnClientDisconnect;

            SetUsername(Client.Instance.serverUsername);

            return ViewPage;
        }

        public void SetUsername(string username)
        {
            chatView.SetUsername(username);
        }

        public void SendMessage(string message)
        {
            Message sendMsg = new Message(MessageControlFlag.MESSAGE, MessageSender.CLIENT, message);
            Client.Instance.SendMessage(sendMsg);

        }


        // --------------- [ SERVER EVENTS ] -----------------

        private void OnClientDisconnect()
        {
            Client.onNewMessage -= OnNewMessage;
            Client.onClientDisconnect -= OnClientDisconnect;

            PageManager.ChangeView(MainWindow.PageList.Home);
        }

        private void OnNewMessage(Client obj)
        {
            messageList = obj.messageList;
            chatView.UpdateChatLogDisplay(messageList);
        }



        // ------------ [ VIEW EVENTS ] ---------------

        public void View_SendMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            SendMessage(message);
        }

        public void View_EndConversation()
        {
            Client.Instance.CloseConnection();
        }
    }
}
