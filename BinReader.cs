using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Carto
{
    /// <summary>
    /// Extension of .NET BinaryReader that can swap bytes
    /// to/from BigEndian to/from LittleEndian
    /// </summary>
    public class BinReader
    {

        private long mFilePos; //Always zero based.
        private Encoding mCharEncoding;
        private string FullPath;
        private FileStream BaseStr;
        private BinaryReader BaseRdr;
        private int mFileBase; //Usually zero or one, depending usually on document

        public long UserFilePos  //Adjusted for base of file (usually zero or one)
        {
            get { return mFilePos + mFileBase; }
        }

        public long RdrFilePos  //Always zero based
        {
            get { return mFilePos; }
        }

        public Encoding CharEncoding
        {
            get { return mCharEncoding; }
        }

        public long FileBase
        {
            get { return mFileBase; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="FullPath"> Full path to file name, including extension</param>
        public BinReader(string FullPath, Encoding CharEncoding, int FileBase)
        { 
            BaseStr = new FileStream(FullPath, FileMode.Open,FileAccess.Read);
            BaseRdr = new BinaryReader(BaseStr, CharEncoding);
            this.mCharEncoding = CharEncoding;
            this.FullPath = FullPath;
            this.mFileBase = FileBase;
            mFilePos = 0;
        }

        
        /// <summary>
        /// Read a 4 byte integer, swapping bytes if necessary
        /// </summary>
        /// <param name="Order">Whether the bytes are DEC or PC order</param>
        /// <returns></returns>
        public Int32 ReadInt32(ByteOrder Order = ByteOrder.PC)
        {
            int Value = BaseRdr.ReadInt32();
            if (Order == ByteOrder.DEC)
                Utils.SwapBytes(ref Value);
            mFilePos += 4;
            return Value;
        }

        /// <summary>
        /// Read a 2 byte integer, swapping bytes if necessary
        /// </summary>
        /// <param name="Order">Whether the bytes are DEC or PC order</param>
        /// <returns></returns>
        public Int16 ReadInt16(ByteOrder Order = ByteOrder.PC)
        {
            Int16 Value = BaseRdr.ReadInt16();
            if (Order == ByteOrder.DEC)
                Utils.SwapBytes(ref Value);
            mFilePos += 2;
            return Value;
        }

        /// <summary>
        /// Read an 8 byte floating point number, swapping bytes if necessary
        /// </summary>
        /// <param name="Order">Whether the bytes are DEC or PC order</param>
        /// <returns></returns>
        public double ReadDouble(ByteOrder Order = ByteOrder.PC)
        {
            double Value = BaseRdr.ReadDouble();
            if (Order == ByteOrder.DEC)
                Utils.SwapBytes(ref Value);
            mFilePos += 8;
            return Value;
        }

        public ushort ReadUShort(ByteOrder Order = ByteOrder.PC)
        {
            ushort Value = BaseRdr.ReadUInt16();
            if (Order == ByteOrder.DEC)
                Utils.SwapBytes(ref Value);
            mFilePos += 2;
            return Value;
        }


        /// <summary>
        /// Read a 4 byte floating point number, swapping bytes if necessary
        /// </summary>
        /// <param name="Order">Whether the bytes are DEC or PC order</param>
        /// <returns></returns>
        public float ReadSingle(ByteOrder Order = ByteOrder.PC)
        {
            float Value = BaseRdr.ReadSingle();
            if (Order == ByteOrder.DEC)
                Utils.SwapBytes(ref Value);
            mFilePos += 4;
            return Value;
        }

        public char ReadCharEight()
        {
            byte Value = BaseRdr.ReadByte();
            mFilePos += 1;
            return (char) Value;
        }

        public char ReadChar16(ByteOrder Order = ByteOrder.PC)
        {
            ushort Value = ReadUShort(Order);
            return (char) Value;
        }

        public char [] ReadCharArray(int FilePos, int HowMany)
        {
            MoveTo(FilePos);
            string OutString = ReadChars(HowMany);
            char[] OutChars = OutString.ToCharArray();
            return OutChars;
        }

        public string ReadString(int StartPos, int EndPos)
        {
            int Count = EndPos - StartPos + 1;
            MoveTo(StartPos);
            string OutStr = ReadChars(Count);
            return OutStr;
        }

        public string ReadChars(int HowMany)
        {
            StringBuilder Str = new StringBuilder(HowMany);
            if (CharEncoding == Encoding.UTF8)
            {
                for (int i = 0; i < HowMany; i++)
                {
                    char NewChar = ReadCharEight();
                    Str.Append(NewChar);
                }
            }
            else if (CharEncoding == Encoding.Unicode)
            {
                for (int i = 0; i < HowMany; i++)
                {
                    char NewChar = ReadChar16();
                    Str.Append(NewChar);
                }
            }
            else
            {
                throw new Exception("BinReader can only read characters that are UTF-8 or Unicode.");
            }
            string OutString = Str.ToString();
            return OutString;
        }

        public void Rewind()
        {
            BaseRdr.Close();
            BaseRdr = null;
            BaseStr.Close();
            BaseStr = null;
            BaseStr = new FileStream(FullPath, FileMode.Open, FileAccess.Read);
            BaseRdr = new BinaryReader(BaseStr, CharEncoding);
            mFilePos = 0;
        }

        public void MoveTo(long BytePosition)
        {
            long ZeroBaseBytePosition = BytePosition - FileBase;
            long FastForwardCount = 0;
            if (BytePosition > mFilePos)
            {
                FastForwardCount = ZeroBaseBytePosition - mFilePos;
            }
            else
            {
                Rewind();
                FastForwardCount = ZeroBaseBytePosition;
            }
            BaseRdr.ReadBytes((int)FastForwardCount);
            mFilePos = ZeroBaseBytePosition;
        }

        public void AdvanceFile(long NumBytesToAdvance)
        {
            MoveTo(mFilePos + NumBytesToAdvance);
        }

        public void BackupFile(long NumBytesToMoveBack)
        {
            MoveTo(mFilePos - NumBytesToMoveBack);
        }


    

        /// <summary>
        /// Disposer closes and disposes of underlying stream
        /// </summary>
        public void Cleanup()
        {
            if (BaseRdr != null) 
            {
                BaseRdr.Close();
                BaseRdr.Dispose();
                BaseRdr = null;
            }

            if (BaseStr != null)
            {
                BaseStr.Close();
                BaseStr.Dispose();
                BaseStr = null;
            }
        }

        ~BinReader()
        {
            this.Cleanup();
        }


    }
}
