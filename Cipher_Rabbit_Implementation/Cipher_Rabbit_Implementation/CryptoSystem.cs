using System;
using System.Collections.Generic;
using System.Text;

namespace Cipher_Rabbit_Implementation
{
    class CryptoSystem
    {
        //const long WORDSIZE = 0x100000000;
        //private string IV { get; set; }
        //private string Key { get; set; }
        private byte[] IV { get; set; }
        private byte[] Key { get; set; }
        private SystemState MasterState { get; set; }
        private SystemState CurrentState { get; set; }
        //private string KeyStream { get; set; }
        private byte[] KeyBlock { get; set; }
        private int BlockIndex;

        /*public CryptoSystem(string IV, string Key)
        {
            this.IV = IV;
            this.Key = Key;
            this.MasterState = KeySetup(this.Key);
            this.KeyStream = GenerateKeyStream();
        }*/
        public CryptoSystem(byte[] IV, byte[] Key)
        {
            if (Key.Length != 16 || IV.Length != 8)
                throw new Exception("Niepoprawny Klucz lub IV");
            this.IV = IV;
            this.Key = Key;
            MasterState = KeySetup(this.Key);
           // Console.WriteLine(MasterState.ToString());
            CurrentState = IVSetup(IV);
            //Console.WriteLine(CurrentState.ToString());
            KeyBlock = GenerateNextKeyBlock();
            BlockIndex = 0;
        }

        /*public string EnryptDecryptMessage(string Message)
        {
            string ReturnedText = ""; // XOR operation of input and KeyStream but in 128 bit blocks

            return ReturnedText;
        }*/
        /*public string GenerateKeyStream() 
        {
            SystemState EncryptionStartingState = IVSetup(IV);
            string KeyStream = EncryptionStartingState.ExtractionPhase();
            return KeyStream;
        }*/
        public byte[] EnryptDecryptMessage(string Message)
        {
            return EnryptDecryptMessage(Encoding.UTF8.GetBytes(Message));
        }

        public byte[] EnryptDecryptMessage(byte[] Message)
        {
            int MessageIndex = 0;
            int limit;
            int initLengthMessage = Message.Length;
            while (initLengthMessage*8 > (128 - 8*BlockIndex)) // dopóki długość wiadomości pozostała jest większa
            {
                //Console.WriteLine("oh baby, it is too big message...");
                limit = 16 - BlockIndex; // 1 blok=16 bajtów
                for (int i = 0; i < limit; i++)
                    Message[MessageIndex+i] ^= KeyBlock[BlockIndex++];
                KeyBlock = GenerateNextKeyBlock();
                //PrintBytes(KeyBlock);
                MessageIndex += limit;
                initLengthMessage -= limit;
            }
            for (int i = 0; i < initLengthMessage; i++)
            {
                Message[MessageIndex + i] ^= KeyBlock[BlockIndex++];
            }
            if (8*BlockIndex == 128) KeyBlock = GenerateNextKeyBlock();
            return Message;
        }

        private byte[] GenerateNextKeyBlock()
        {
            CurrentState.CounterUpdate();
            CurrentState.NextStateFunction();
            BlockIndex = 0;
            return CurrentState.ExtractionPhaseBytes();
        }

        public SystemState KeySetup(byte[] Key)
        {
            SystemState state = new SystemState();
            state.InitializeVariables(Key);
           // Console.WriteLine("After initalization of variables...");
            //Console.WriteLine(state.ToString());
            //Console.WriteLine();
            for (int i = 0; i < 4; i++)
            {
                
                state.CounterUpdate();
                state.NextStateFunction();
                //Console.WriteLine("After {0} iteration", i+1);
                //Console.WriteLine(state.ToString());
                //Console.WriteLine();
            }
            state.ReinitializeCounterVariables();

            return state;

        }

        public SystemState IVSetup(byte[] IV)
        {
            SystemState DerivState = MasterState.DeepCopy(); // copy of master State
            /*Console.WriteLine("Before Modyfying of variables...");
            Console.WriteLine(DerivState.ToString());
            Console.WriteLine();*/
            DerivState.ModifyCounterVariables(IV);
            /*Console.WriteLine("After Modyfying of variables...");
            Console.WriteLine(DerivState.ToString());
            Console.WriteLine();*/

            for (int i = 0; i < 4; i++)
            {
                DerivState.CounterUpdate();
                DerivState.NextStateFunction();
                /*Console.WriteLine("After {0} iteration", i + 1);
                Console.WriteLine(DerivState.ToString());
                Console.WriteLine();*/
            }
            return DerivState;
        }


    }
    
}
