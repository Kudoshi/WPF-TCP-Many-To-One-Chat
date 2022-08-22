using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ManyToOneChat
{
    public class AESEncryption
    {
        /// <summary>
        /// Used to decrypt incoming message
        /// </summary>
        /// <param name="inStream">The incoming network stream</param>
        /// <param name="privateKey">The private key used to encrypt/decrypt</param>
        /// <param name="isDisconnectMsg">Flag on whether the message is a disconnect message</param>
        /// <returns>The decrypted byte message without the IV</returns>
        public static byte[] DecryptStream(byte[] inStream, byte[] privateKey, out bool isDisconnectMsg)
        {
            byte[] messageByte = new byte[inStream.Length - 16];


            isDisconnectMsg = inStream.All(x => x == 0);

            if (isDisconnectMsg)
            {
                byte[] disconnectByte = new byte[1];
                disconnectByte[0] = 0;
                return disconnectByte;
            }


            byte[] IV = new byte[16];
            Buffer.BlockCopy(inStream, 0, IV, 0, 16);
            byte[] cipherByte = new byte[inStream.Length - 16];
            Buffer.BlockCopy(inStream, 16, cipherByte, 0, cipherByte.Length);

            int offsetToRead = 16;

            using (MemoryStream mStream = new MemoryStream(cipherByte))
            {
                using (Aes aes = Aes.Create())
                using (ICryptoTransform decryptor = aes.CreateDecryptor(privateKey, IV))
                using (CryptoStream cStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read))
                {
                    //aes.Padding = PaddingMode.None;
                    int read = 1;
                    read = cStream.Read(messageByte, 0, messageByte.Length);

                    //while (read > 0)
                    //{
                    //    offsetToRead += read;
                    //}

                }
            }

            return messageByte;
        }

        /// <summary>
        /// Used to encrypt the message by using a key and IV
        /// Appends the 16 byte IV onto the front of the message
        /// </summary>
        /// <param name="message">Message to be encrypted</param>
        /// <param name="privateKey">The key to be used in the encryption</param>
        /// <returns>The encrypted byte along with the IV appended</returns>
        public static byte[] EncryptStream(Message message, byte[] privateKey)
        {
            byte[] IV = new byte[16];
            byte[] cipherText;

            using (MemoryStream mStream = new MemoryStream())
            {
                using (Aes aes = Aes.Create())
                using (ICryptoTransform encryptor = aes.CreateEncryptor(privateKey, aes.IV))
                using (CryptoStream cStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
                {
                    //aes.Padding = PaddingMode.None;
                    IV = aes.IV;
                    byte[] messageByte = message.ConvertToByte();
                    cStream.Write(messageByte, 0, messageByte.Length);

                }

                cipherText = mStream.ToArray();
            }

            byte[] outStream = new byte[IV.Length + cipherText.Length];
            Buffer.BlockCopy(IV, 0, outStream, 0, IV.Length);
            Buffer.BlockCopy(cipherText, 0, outStream, 16, cipherText.Length);

            return outStream;
        }
    }
}
