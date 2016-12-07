using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Cartography
{
    public class Crypto
    {
        //For selecting the generator to use
        //Strongly couples generators to this class, when you add a new generator,
        //you have to fix the constructor too here.  That isn't best.
        public enum GeneratorType
        {
            RAN,
            RANQ1,
            RANQ2,
        }

        private RandomGenerator Generator;
        private uint Min;
        private uint Max;

        //Probably not the best way. Demonstrates how we have to, at some point
        //  actually make a new one of the concrete type somewhere.
        public Crypto(GeneratorType InGenerator, ulong Seed, uint Min, uint Max)
        {
            switch (InGenerator)  //The seed gets passed to the generator below
            {
                case GeneratorType.RAN:
                    Generator = new Ran(Seed);
                    break;
                case GeneratorType.RANQ1:
                    Generator = new Ranq1(Seed);
                    break;
                case GeneratorType.RANQ2:
                    Generator = new Ranq2(Seed);
                    break;
            }

            //The min and the max are used for the calls below
            this.Min = Min;
            this.Max = Max;
        }

        //Probably the better approach.  Would not have to change this class
        //  at all when a new RNG was made, as long as it descended from RandomGenerator
        //  class
        public Crypto(RandomGenerator InGenerator, uint Min, uint Max)
        {
            this.Min = Min;
            this.Max = Max;
            Generator = InGenerator;
        }


        ~Crypto() {
            Generator = null;
        }

        private uint EncodeChar(char InChar)
        {
            uint Val = (uint) InChar;
            uint XORValue = Generator.GetIntRange(Min, Max); //Polymorphic
            return (Val ^ XORValue);
        }

        private char DecodeNumber(uint InNumber)
        {
            uint XORValue = Generator.GetIntRange(Min, Max); //Polymorphic
            return (char)(InNumber ^ XORValue);
        }

        public uint [] Encode(string Msg) 
        {
            uint [] OutMessage = new uint [Msg.Length];
            for (int i = 0; i < Msg.Length; i++ ) OutMessage[i] = EncodeChar(Msg[i]);
            return OutMessage;
        }

        public string Decode(uint[] EncodedMsg)
        {
            StringBuilder OutMessage = new StringBuilder(EncodedMsg.Length);
            for (int i = 0; i < EncodedMsg.Length; i++) 
            {
                char DecodedChar = DecodeNumber(EncodedMsg[i]);
                OutMessage.Append(DecodedChar); 
            }
            return OutMessage.ToString();
        }

        public int Decode(string InFileName, string OutFileName) 
        {
            //Required for the InText.Split function syntax
            char [] Separator = new char [] {' '};

            //Check for the obvious error
            if (!File.Exists(InFileName))return -1;
            
            //Open up the output file
            StreamWriter Writer = new StreamWriter(OutFileName);

            //Get the input data and slam the StreamReader shut
            StreamReader Reader = new StreamReader(InFileName);
            string InText = Reader.ReadToEnd();
            Reader.Close();
            Reader = null;

            //Parse it into textual numbers like "147875654543"
            //Decode it to a letters like 'Y'
            //Compose the output file
            string[] EncodedLetters = InText.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            foreach (string Letter in EncodedLetters) 
            {
                uint Val = UInt32.Parse(Letter.ToString()); //Turn the string into a bonafide integer
                char OutChar = DecodeNumber(Val);           //Decode it 
                Writer.Write(OutChar);                      //Write it to the output file
            }
            Writer.Flush();
            Writer.Close();
            Writer = null;
            return 0;
        }
        
        public int Encode(string InFileName, string OutFileName)
        {
            //Check for the obvious error
            if (!File.Exists(InFileName))
            {
                return -1;
            }

            //Open up the output file
            StreamWriter Writer = new StreamWriter(OutFileName);

            //Get the input data and slam the StreamReader shut
            StreamReader Reader = new StreamReader(InFileName);
            string InText = Reader.ReadToEnd();
            Reader.Close();
            Reader = null;
            
            //Encode character by character
            foreach (char TheChar in InText)
            {
                uint EncodedChar = EncodeChar(TheChar);
                Writer.Write(EncodedChar);
                Writer.Write(" ");
            }   
            Writer.Flush();
            Writer.Close();
            Writer = null;
            return 0;
        }


    }
}