using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyToOneChat.Interface
{
    public interface IClientChatController
    {
        void View_SendMessage(string message);
        void View_EndConversation();

    }
}
