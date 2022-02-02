using System;
using System.Collections.Generic;
using System.Text;

namespace Cipher_Rabbit_Implementation
{
    class SystemState
    {
        private bool CounterCarryBit { get; set; }
        private List<uint> XVariables;
        private List<uint> CVariables;
        private ulong[] A = { 0x4D34D34D, 0xD34D34D3, 0x34D34D34, 0x4D34D34D, 0xD34D34D3, 0x34D34D34, 0x4D34D34D, 0xD34D34D3 };
        private ulong WORDSIZE = 0x100000000;
        public SystemState()
        {
            XVariables = new List<uint>();
            CVariables = new List<uint>();
            CounterCarryBit = false;
        }
        public override string ToString()
        {
            string result = "";
            if (CounterCarryBit) { result += "b=1 "; }
            else
            {
                result += "b=0 ";
            }
            string Xvalues = "X{0}={1:X} ";
            string Cvalues = "C{0}={1:X} ";
            for(int i=0;i<8;i++)
            {
                string value = "";
                value += String.Format(Xvalues, i, XVariables[i]) + String.Format(Cvalues,i,CVariables[i]);
                result += value;
            }
            return result;
        }
        public void InitializeVariables(byte[] Key)
        {
            // Key -> 16 bajtów ->
            uint[] k = new uint[8];
            for (int i = 0; i < 8; i++)
                k[i] = Convert.ToUInt32((Key[15 - 2*i])+(Key[15-2*i-1] << 8));
            for (int i = 0; i < 8; i++)
            {
                XVariables.Add((k[(i + 1) % 8] << 16) + k[i]);
                CVariables.Add((k[(i + 4) % 8] << 16) + k[(i + 5) % 8]);

                i++;

                XVariables.Add((k[(i + 5) % 8] << 16) + k[(i + 4) % 8]);
                CVariables.Add((k[i]<< 16) + k[(i + 1) % 8]);
            }
        }
        public void ModifyCounterVariables(string IV)
        {

        }

        public void ModifyCounterVariables(byte[] IV)
        {
            CVariables[0] ^= ((uint)IV[4] << 24) + ((uint)IV[5] << 16) + ((uint)IV[6] << 8) + (uint)IV[7];
            CVariables[1] ^= ((uint)IV[0] << 24) + ((uint)IV[1] << 16) + ((uint)IV[4] << 8) + (uint)IV[5];
            CVariables[2] ^= ((uint)IV[0] << 24) + ((uint)IV[1] << 16) + ((uint)IV[2] << 8) + (uint)IV[3];
            CVariables[3] ^= ((uint)IV[2] << 24) + ((uint)IV[3] << 16) + ((uint)IV[6] << 8) + (uint)IV[7];
            CVariables[4] ^= ((uint)IV[4] << 24) + ((uint)IV[5] << 16) + ((uint)IV[6] << 8) + (uint)IV[7];
            CVariables[5] ^= ((uint)IV[0] << 24) + ((uint)IV[1] << 16) + ((uint)IV[4] << 8) + (uint)IV[5];
            CVariables[6] ^= ((uint)IV[0] << 24) + ((uint)IV[1] << 16) + ((uint)IV[2] << 8) + (uint)IV[3];
            CVariables[7] ^= ((uint)IV[2] << 24) + ((uint)IV[3] << 16) + ((uint)IV[6] << 8) + (uint)IV[7];

        }

        public string ExtractionPhase()
        {
            string KeyStream = "";

            return KeyStream;
        }
        public byte[] ExtractionPhaseBytes()
        {
            byte[] S = new byte[16];
            ushort temp;

            temp = (ushort) ((XVariables[0] >>  0) ^ (XVariables[5] >> 16));
            S[15] = (byte)temp;
            S[14] = (byte)(temp >> 8);

            temp = (ushort)((XVariables[0] >> 16) ^ (XVariables[3] >>  0));
            S[13] = (byte)temp;
            S[12] = (byte)(temp >> 8);

            temp = (ushort)((XVariables[2] >>  0) ^ (XVariables[7] >> 16));
            S[11] = (byte)temp;
            S[10] = (byte)(temp >> 8);

            temp = (ushort)((XVariables[2] >> 16) ^ (XVariables[5] >>  0));
            S[9] = (byte)temp;
            S[8] = (byte)(temp >> 8);

            temp = (ushort)((XVariables[4] >>  0) ^ (XVariables[1] >> 16));
            S[7] = (byte)temp;
            S[6] = (byte)(temp >> 8);

            temp = (ushort)((XVariables[4] >> 16) ^ (XVariables[7] >>  0));
            S[5] = (byte)temp;
            S[4] = (byte)(temp >> 8);

            temp = (ushort)((XVariables[6] >>  0) ^ (XVariables[3] >> 16));
            S[3] = (byte)temp;
            S[2] = (byte)(temp >> 8);

            temp = (ushort)((XVariables[6] >> 16) ^ (XVariables[1] >>  0));
            S[1] = (byte)temp;
            S[0] = (byte)(temp >> 8);

            /*Console.WriteLine();
            Console.WriteLine();
            string Svalues = "S{0}={1:X} ";
            string result = "";
            for (int i = 0; i < 16; i++)
            {
                string value = "";
                value += String.Format(Svalues, i, S[i]);
                result += value;
            }
            Console.WriteLine(result);
            Console.WriteLine();
            */
            return S;
        }
     /*   public string PrintSVariables(byte [] S)
        {
            int len = S.Length;
            foreach(var el in S)
            {

            }

            return result;
        }
*/
        private uint GFunction(uint u, uint v)
        {
            uint SUM = u + v;
            ulong square = (ulong)(SUM % WORDSIZE) * (ulong)(SUM % WORDSIZE);
            return (uint)(square ^ (square >> 32)); //to też wygląda na git jeżeli uint ucina pierwsze 32 bity
        }

        public void CounterUpdate() // wydaje się git
        {
            ulong temp;
            for (int i = 0; i < 8; i++)
            {
                if (CounterCarryBit) temp = CVariables[i] + A[i] + 1;
                else temp = CVariables[i] + A[i];

                //if (temp / WORDSIZE >= 1) CounterCarryBit = true;
                if (temp >= WORDSIZE) CounterCarryBit = true;
                else CounterCarryBit = false;

                CVariables[i] = (uint)(temp % WORDSIZE);
                //CVariables[i] = (uint)temp;
            }
            
        }
        public uint LeftRotatation(uint value, int n)
        {
            uint temp = (value << n) | (value >> (32 - n));
            return temp;
        }
        public void NextStateFunction()
        {
            uint[] G = new uint[8];
            for (int i = 0; i < 8; i++)
                G[i] = GFunction(XVariables[i], CVariables[i]);

            XVariables[0] = (uint)((G[0] + LeftRotatation(G[7],16) + LeftRotatation(G[6],16)) % WORDSIZE);
            XVariables[1] = (uint)((G[1] + LeftRotatation(G[0],8) +  G[7]) % WORDSIZE);
            XVariables[2] = (uint)((G[2] + LeftRotatation(G[1],16) + LeftRotatation(G[0],16)) % WORDSIZE);
            XVariables[3] = (uint)((G[3] + LeftRotatation(G[2],8) +  G[1]) % WORDSIZE);
            XVariables[4] = (uint)((G[4] + LeftRotatation(G[3],16) + LeftRotatation(G[2],16)) % WORDSIZE);
            XVariables[5] = (uint)((G[5] + LeftRotatation(G[4],8)+  G[3]) % WORDSIZE);
            XVariables[6] = (uint)((G[6] + LeftRotatation(G[5],16) + LeftRotatation(G[4],16)) % WORDSIZE);
            XVariables[7] = (uint)((G[7] + LeftRotatation(G[6],8) +  G[5]) % WORDSIZE);
        }
        public void ReinitializeCounterVariables()
        {
            for (int i = 0; i < 8; i++)
                CVariables[i] ^= XVariables[(i + 4) % 8];
        }

        public SystemState DeepCopy()
        {
            SystemState Copy = new SystemState();
            Copy.CounterCarryBit = CounterCarryBit;
            foreach (uint X in XVariables)
                Copy.XVariables.Add(X);
            foreach (uint C in CVariables)
                Copy.CVariables.Add(C);
            return Copy;
        }
    }
}
