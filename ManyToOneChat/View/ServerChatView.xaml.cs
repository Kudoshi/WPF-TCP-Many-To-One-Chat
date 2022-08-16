using ManyToOneChat.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ManyToOneChat.View
{
    /// <summary>
    /// Interaction logic for ServerChatView.xaml
    /// </summary>
    public partial class ServerChatView : Page
    {
        private IServerChatController controller;
        public ServerChatView(IServerChatController controller)
        {
            InitializeComponent();
            this.controller = controller;
        }
        


        // --- Conversation View - Left side ---

        public void UpdateChatListDisplay(List<ClientHandler> clientList)
        {

            this.Dispatcher.Invoke(() =>
            {
                ChatListView.ItemsSource = new List<ClientHandler>();
                ChatListView.ItemsSource = clientList;
            });
        }

        public void UpdateChatLogDisplay(string username, List<Message> messageList)
        {
            this.Dispatcher.Invoke(() =>
            {
                UsernameLabel.Text = username;
                ChatLogWindow.ItemsSource = new List<Message>();
                ChatLogWindow.ItemsSource = messageList;
                NewMessageInput.Text = "";

                ScrollViewer scrollViewer = (ScrollViewer) ChatLogWindow.Parent;


                if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
                {
                    scrollViewer.ScrollToBottom();
                }
            });
            
        }
        

        //----- [EVENTS VIEW] ------
        private void DisconnectServerBtn_Click(object sender, RoutedEventArgs e)
        {
            controller.View_DisconnectServer();
        }

        private void EndConversationBtn_Click(object sender, RoutedEventArgs e)
        {
            controller.View_EndConversation();
        }

        private void SendMsgBtn_Click(object sender, RoutedEventArgs e)
        {
            controller.View_SendMessage(NewMessageInput.Text);
        }

        private void ChatList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("[CLIENT] Chat list clicked ");
            controller.View_ChangeMessagePanel(sender, e);
        }
    }
}
