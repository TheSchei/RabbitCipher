using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace Cipher_Rabbit_Implementation
{
    class Program
    {
        public static byte [] CreateMD5Hash(string input, int byte_limit)
        {
            // Step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            byte[] output = new byte[byte_limit];
            // Step 2, convert byte array to hex string
            for (int i = 0; i <byte_limit; i++)
            {
                output[i] = hashBytes[i];
            }
            return output;
        }
        static void Main(string[] args)
        {
            string [] arguments=Environment.GetCommandLineArgs();

            if (arguments.Length != 4)
            {
                Console.WriteLine("Poprawne wywołanie wymaga argumentów [filename] [IV_String] [Key_String]");
                return;
            }

            string inputFile = arguments[1];
            string IV_str = arguments[2];
            string Key_str = arguments[3];
            byte [] IV = CreateMD5Hash(IV_str,8);
            byte [] Key = CreateMD5Hash(Key_str,16);

            if (!File.Exists(inputFile))
            {
                Console.WriteLine("Brak dostępu do {0}", inputFile);
                return;
            }

            //byte [] IV = {0xc3, 0x73, 0xf5, 0x75, 0xc1, 0x26, 0x7e, 0x59};
            //byte [] Key = {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};

            /*string Message = "12345678901234asdasd56789012345678901234567890";
            Console.WriteLine("Message to sent {0}",Message);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Message);

            CryptoSystem encryptSystem = new CryptoSystem(IV, Key);
            byte[] cipher = encryptSystem.EnryptDecryptMessage(buffer);
            
            Console.WriteLine("Cipher text -> " +System.Text.Encoding.UTF8.GetString(cipher, 0, cipher.Length));

            CryptoSystem decryptSystem = new CryptoSystem(IV, Key);

            byte[] decryption = decryptSystem.EnryptDecryptMessage(cipher);

            Console.WriteLine("Decrypted text -> " + System.Text.Encoding.UTF8.GetString(decryption, 0, decryption.Length));
            */
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();


            byte[] buffer = File.ReadAllBytes(inputFile);
            CryptoSystem encryptSystem = new CryptoSystem(IV, Key);
            sw.Start();
            byte[] cipher = encryptSystem.EnryptDecryptMessage(buffer);
            sw.Stop();
            Console.WriteLine("RABBIT - Czas wykonania={0}", sw.Elapsed);

            File.WriteAllBytes(Path.GetDirectoryName(Path.GetFullPath(inputFile)) + "\\output.txt", cipher);
        }
    }
}
