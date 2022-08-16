using ManyToOneChat.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ManyToOneChat.Controller
{
    public class MenuSelectionController : IMenuSelection, IViewController
    {
        public MainWindow PageManager { get; set; }
        public Page ViewPage { get ; set ; }

        // --- Setup IViewController ---

        public Page SetupController(MainWindow window)
        {
            PageManager = window;
            ViewPage = new MenuSelection(this);

            return ViewPage;
        }


        // --- IMenuSelection Methods ---

        public void HostServer(string username, string IPAddress, string PortNumber)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(IPAddress) || string.IsNullOrWhiteSpace(PortNumber))
            {
                Console.WriteLine("Inputs are empty");
                return;
            }
            try
            {
                ServerClientManager.Instance.HostServer(IPAddress, PortNumber, username);
                PageManager.ChangeView(MainWindow.PageList.ServerChat);

            }
            catch (Exception e)
            {
                string titleText = "Fail to host server";

                string alertBoxText = "Unable to host the server on the current ip address and port number";

                MessageBox.Show(alertBoxText, titleText, MessageBoxButton.OK, MessageBoxImage.Information);

                Console.WriteLine(e);

            }
        }

        public void JoinServer(string username, string IPAddress, string PortNumber)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(IPAddress) || string.IsNullOrWhiteSpace(PortNumber))
            {
                Console.WriteLine("Inputs are empty");
                return;
            }

            try
            {
                Client.Instance.ConnnectToServer(IPAddress, PortNumber, username);
                PageManager.ChangeView(MainWindow.PageList.ClientChat);

            }
            catch (Exception e)
            {
                string titleText = "Unable to join server";

                string alertBoxText = "No chat server by the ip address and port number found";

                MessageBox.Show(alertBoxText, titleText, MessageBoxButton.OK, MessageBoxImage.Information);
                Console.WriteLine(e);
            }

        }


        
    }
}
