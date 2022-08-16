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
    /// Interaction logic for MenuSelection.xaml
    /// </summary>
    public partial class MenuSelection : Page
    {
        //--- Initialization ---
        private Window mainWindow;
        private IMenuSelection controller;
        public MenuSelection(object controller)
        {
            InitializeComponent();
            this.controller = (IMenuSelection)controller;
        }

        //--- Events ---

        private void HostServerBtn_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameInput.Text;
            string ipAddress = IPAddressInput.Text;
            string portNumber = PortNumberInput.Text;

            controller.HostServer(username, ipAddress, portNumber);
        }
        private void JoinServerBtn_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameInput.Text;
            string ipAddress = IPAddressInput.Text;
            string portNumber = PortNumberInput.Text;

            controller.JoinServer(username, ipAddress, portNumber);
        }

        
    }
}
