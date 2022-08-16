using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ManyToOneChat.Interface
{
    public interface IServerChatController
    {
        void View_SendMessage(string message);
        void View_DisconnectServer();
        void View_EndConversation();
        void View_ChangeMessagePanel(object sender, MouseButtonEventArgs e);

    }
}
