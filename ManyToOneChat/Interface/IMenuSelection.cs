using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ManyToOneChat.Interface
{
    internal interface IMenuSelection
    {
        void HostServer(string username, string IPAddress, string PortNumber);
        void JoinServer(string username, string IPAddress, string PortNumber);
    }
}
