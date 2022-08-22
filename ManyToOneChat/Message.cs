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


        /// <summary>
        /// Used when received message from 
        /// </summary>
        /// <param name="messageByte"></param>
        /// <param name="sender"></param>
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

        /// <summary>
        /// Used to craft message object to be sent
        /// </summary>
        /// <param name="controlFlag"></param>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        public Message(MessageControlFlag controlFlag, MessageSender sender, string message)
        {
            this.controlFlag = controlFlag;
            this.sender=sender;
            this.message = message;
            
        }

        /// <summary>
        /// Used to convert message object to byte to be sent 
        /// </summary>
        /// <returns></returns>
        public byte[] ConvertToByte()
        {
            //Add IV?
            string editedMessage = controlFlag.ToString() + 'Ÿ' + sender + 'Ÿ' + message + 'Ÿ';
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
