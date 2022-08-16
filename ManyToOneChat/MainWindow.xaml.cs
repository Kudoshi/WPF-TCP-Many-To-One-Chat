using ManyToOneChat.Controller;
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

namespace ManyToOneChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IViewController currentPage;

        public MainWindow()
        {
            InitializeComponent();
            LoadFirstPage();
        }

        //Page Manager

        public enum PageList
        {
            Home, ServerChat, ClientChat
        }

        public void ChangeView(PageList page)
        {
            this.Dispatcher.Invoke(() =>
            {
                switch (page)
                {
                    case PageList.Home:
                        currentPage = new MenuSelectionController();
                        break;
                    case PageList.ServerChat:
                        currentPage = new ServerChatController();
                        break;
                    case PageList.ClientChat:
                        currentPage = new ClientChatController();
                        break;
                }

                Page viewPage = currentPage.SetupController(this);

                FrameBox.Content = viewPage;
            });
        }

        private void LoadFirstPage()
        {
            ChangeView(PageList.Home);
            //ChangeView(PageList.ServerChat);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ServerClientManager.Instance.isServerClientUp)
            {
                ServerClientManager.Instance.CloseServer();
            }
            else if (Client.Instance.isConnected)
            {
                Client.Instance.CloseConnection();
            }
        }
    }
}
