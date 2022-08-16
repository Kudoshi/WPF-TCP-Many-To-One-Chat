using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyToOneChat
{
    public class Message
    {
        public MessageControlFlag controlFlag;
        public MessageSender sender { get; set; }
        public string message { get; set; }

        public Message(byte[] messageByte, MessageSender sender)
        {
            if (messageByte[0] == 0)
            {
                controlFlag = MessageControlFlag.DISCONNECT;
                return;
            }

            this.sender = sender;
            string decoded = Encoding.UTF8.GetString(messageByte);
            string[] messageArray = decoded.Split('Ÿ');

            Enum.TryParse(messageArray[0], out MessageControlFlag msgControlFlag);
            Enum.TryParse(messageArray[1], out MessageSender msgSender);

            controlFlag = msgControlFlag;
            sender = msgSender;
            message = messageArray[2];

        }

        public Message(MessageControlFlag controlFlag, MessageSender sender, string message)
        {
            this.controlFlag = controlFlag;
            this.sender=sender;
            this.message = message;
        }

        public byte[] ConvertToByte()
        {
            string editedMessage = controlFlag.ToString() + 'Ÿ' + sender + 'Ÿ' + message;
            byte[] outStream = Encoding.UTF8.GetBytes(editedMessage);
            return outStream;
        }
    }

    public enum MessageControlFlag
    {
        USERNAME, MESSAGE, DISCONNECT
    }

    public enum MessageSender
    {
        CLIENT, SERVER
    }
}
