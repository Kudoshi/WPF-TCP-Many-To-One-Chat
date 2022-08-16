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
    /// Interaction logic for ClientChatView.xaml
    /// </summary>
    public partial class ClientChatView : Page
    {
        IClientChatController controller;
        public ClientChatView(IClientChatController controller)
        {
            InitializeComponent();
            this.controller = controller;
        }

        public void UpdateChatLogDisplay(List<Message> messageList)
        {
            this.Dispatcher.Invoke(() =>
            {
                ChatLogWindow.ItemsSource = new List<Message>();
                ChatLogWindow.ItemsSource = messageList;
                NewMessageInput.Text = "";
            });
            
        }

        public void SetUsername(string username)
        {
            UsernameLabel.Text = username;
        }


        // -------- [ VIEW EVENTS ] ----------
        private void SendMsgBtn_Click(object sender, RoutedEventArgs e)
        {
            controller.View_SendMessage(NewMessageInput.Text);
        }

        private void EndConversationBtn_Click(object sender, RoutedEventArgs e)
        {
            controller.View_EndConversation();
        }
    }
}
