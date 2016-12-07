using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Carto
{
    //The generic ENVI image with all its methods
    public abstract class EnviImage
    {
        public long NumRows = -1;
        public long NumCols = -1;
        public int NumBands = -1;
        public long NumElements = -1;
        public int DataType = -1;
        public string FileFormat = "UNKNOWN";
        protected BinaryReader Rdr = null;
        protected BinaryWriter Wtr = null;
        public abstract double this[int CellIdx] { get; set; }
        public abstract void AllocateGrid();
        public abstract void ReadCell(int i);
        public abstract void WriteCell(int i);
        public abstract void Dispose();

        ~EnviImage()
        {
            Dispose();
        }


        #region Learning to read the EnviImage 

        //public static void PlayByteFile(string BasePath) 
        //{
        //    long NumRows = -1;
        //    long NumCols = -1;
        //    int NumBands = -1;
        //    int DataType = -1;
        //    string FileFormat = "UNKNOWN";
        //    ReadHeader(BasePath, out NumBands, out NumRows, out NumCols, out DataType, out FileFormat);
        //    long NumElements = NumBands * NumRows * NumCols;
        //    byte[] X;  //Type specific.  A holder for the data itself.  
        //               //This is a variable, so it will be a data member
        //    X = new byte[NumElements];  //Type specific.  Allocate the space for the grid after reading header
        //    BinaryReader Rdr = new BinaryReader(new FileStream(BasePath + ".bin", FileMode.Open));
        //    for (int i = 0; i < NumElements; i++){ 
        //        X[i] = Rdr.ReadByte();  //Type specific.  Reading the specific length of data into 
        //                                //  a specific array cell using a binary reader
        //                                //  We will call it ReadCell.  The reader will persist from
        //                                //  call to call, but the index will change.
        //    }
        //    Rdr.Close();
        //    Rdr.Dispose();
        //    Rdr = null;
        //    X = null;  //Type specific.  Dispose of the data when you are done with it. 
        //               //  We will call it dispose
        //}

        #endregion



        public static EnviImage Read(string BasePath)
        {
            int NumBands;
            long NumRows, NumCols;
            int DataType;
            string FileFormat;
            //EnviHeader.Read(BasePath, out NumBands, out NumRows, out NumCols, out DataType, out FileFormat);
            EnviImage Image = EnviImage.Create(DataType, NumBands, NumRows, NumCols, FileFormat);
            BinaryReader Rdr = new BinaryReader(new FileStream(BasePath + ".bin", FileMode.Open));
            Image.NumElements = NumRows * NumCols * NumBands;
            Image.Rdr = Rdr;
            for (int i = 0; i < Image.NumElements; i++) Image.ReadCell(i);
            Rdr.Close();
            Rdr.Dispose();
            Rdr = null;
            return Image;
        }

        public static EnviImage Create(int DataType, int NumBands, long NumRows, long NumCols, string FileFormat)
        {
            EnviImage Image = null;
            switch (DataType)
            {
                case 1:
                    Image = new ByteImage();
                    break;
                case 4:
                    Image = new FloatImage();
                    break;
            }
            Image.FileFormat = FileFormat;
            Image.DataType = DataType;
            Image.NumBands = NumBands;
            Image.NumCols = NumCols;
            Image.NumRows = NumRows;
            Image.NumElements = NumBands * NumRows * NumCols;
            Image.AllocateGrid();
            return Image;
        }

        //public static void ReadHeader(string BasePath, out int NumBands, out long NumRows, out long NumCols, out int DataType, out string FileFormat)
        //{
        //    //Setup some values so when I debug, I can see if
        //    //  something got missed
        //    NumBands = -1;
        //    NumRows = -1;
        //    NumCols = -1;
        //    DataType = -1;
        //    FileFormat = "UNKNOWN";

        //    //Open file and read it line by line,
        //    //  Ignore any lines without the desired header info
        //    string FullPath = BasePath + ".hdr";
        //    StreamReader Rdr = new StreamReader(FullPath);

        //    //The first line should read ENVI, but I will check
        //    // it when I revise this initial version
        //    string OneLine = Rdr.ReadLine().Trim();

        //    while (!Rdr.EndOfStream)
        //    {
        //        OneLine = Rdr.ReadLine();
        //        string[] Piece = OneLine.Split('=');
        //        string Key = Piece[0].Trim();
        //        string Value = Piece[1].Trim();
        //        switch (Key.ToUpper())
        //        {
        //            case "LINES":
        //                NumRows = Convert.ToInt32(Value);
        //                break;
        //            case "SAMPLES":
        //                NumCols = Convert.ToInt32(Value);
        //                break;
        //            case "DATA TYPE":
        //                DataType = Convert.ToInt32(Value);
        //                break;
        //            case "INTERLEAVE":
        //                FileFormat = Value;
        //                break;
        //            case "BANDS":
        //                NumBands = Convert.ToInt32(Value);
        //                break;
        //        }
        //    }

        //    //Clean up the file
        //    Rdr.Close();
        //    Rdr.Dispose();
        //    Rdr = null;
        //}


        //public static void WriteHeader(string BasePath, int NumBands, long NumRows, long NumCols, int DataType, string FileFormat)
        //{
        //    string FullPath = BasePath + ".hdr";
        //    StreamWriter Wtr = new StreamWriter(FullPath);
        //    Wtr.WriteLine("ENVI");
        //    Wtr.WriteLine("lines = " + NumRows.ToString());
        //    Wtr.WriteLine("samples = " + NumCols.ToString());
        //    Wtr.WriteLine("data type = " + DataType.ToString());
        //    Wtr.WriteLine("interleave = " + FileFormat);
        //    Wtr.WriteLine("bands = " + NumBands.ToString());
        //    Wtr.Flush();
        //    Wtr.Close();
        //    Wtr.Dispose();
        //    Wtr = null;
        //}


        public void Write(string BasePath)
        {
            throw new Exception("Next commented line needs to be reincluded and fixed.");
            //EnviHeader.WriteHeader(BasePath,(uint) NumBands, (uint) NumRows, (uint) NumCols, DataType, FileFormat);
            Wtr = new BinaryWriter(new FileStream(BasePath + ".bin", FileMode.Create));
            long NumElements = NumRows * NumCols * NumBands;
            for (int i = 0; i < NumElements; i++) WriteCell(i);
            Wtr.Close();
            Wtr.Dispose();
            Wtr = null;
        }

    }

    //An ENVI image that holds byte values (unsigned)
    public class ByteImage : EnviImage
    {
        public byte[] X;


        public override void Dispose()
        {
            X = null;
        }

        public override double this[int CellIdx]
        {
            get
            {
                return X[CellIdx];
            }
            set
            {
                X[CellIdx] = (byte)value;
            }
        }

        public override void AllocateGrid()
        {
            X = new byte[NumElements];
        }

        public override void ReadCell(int i)
        {
            X[i] = Rdr.ReadByte();
        }

        public override void WriteCell(int i)
        {
            Wtr.Write(X[i]);
        }
    }

    //An ENVI image that holds single precision reals (floats)
    public class FloatImage : EnviImage
    {
        public float[] X;

        public override void Dispose()
        {
            X = null;
        }

        public override double this[int CellIdx]
        {
            get
            {
                return X[CellIdx];
            }
            set
            {
                X[CellIdx] = (float)value;
            }
        }

        public override void AllocateGrid()
        {
            X = new float[NumElements];
        }

        public override void ReadCell(int i)
        {
            X[i] = Rdr.ReadSingle();
        }

        public override void WriteCell(int i)
        {
            Wtr.Write(X[i]);
        }
    }

    //The generic RunList with all its methods
    public abstract class RunList
    {
        public long NumRows = -1;
        public long NumCols = -1;
        public int NumBands = -1;
        public long NumRuns = -1;
        public int DataType =  -1;
        public long NumGridElements = -1;
        public string FileFormat = "UNKNOWN";
        public string HeaderLine = "UNKNOWN";
        protected BinaryReader Rdr = null;
        protected BinaryWriter Wtr = null;
        public const int MaxRunLength = 255;  //Set to 255 when out of debugging mode, 4 otherwise
        public abstract void AllocateRuns(long RunCount);
        public abstract long DataElementSize { get; set; }
        public abstract double GetCell(long ImagePos);
        public abstract void WriteRun(int RunLength, double RunValue);
        public abstract void ReadRun(long RunIndex);
        public abstract void WriteGrid(long RunIndex);
        public abstract void Reference(EnviImage Image);
        public abstract void Dispose();

        ~RunList()
        {
            Dispose();
        }

        public void CalcCompression(out long CompressedSize, out long UncompressedSize, out double Percentage)
        {
            if (NumRuns < 1) NumRuns = EncodeRuns(null);
            long NumBytesPerRun = DataElementSize + DataElementSize;
            CompressedSize = NumBytesPerRun * NumRuns;
            UncompressedSize = DataElementSize * NumGridElements;
            Percentage = CompressedSize * 100.0 / UncompressedSize;
        }

        public void ExpandToDisk(string BasePath, bool WriteEnviHeader = true)
        {
            throw new Exception("Next two lines need to be reincluded and fixed.");
            //if (WriteEnviHeader)
            //    EnviImage.WriteHeader(BasePath, NumBands, NumRows, NumCols, DataType, FileFormat);

            Wtr = new BinaryWriter(new FileStream(BasePath + ".bin", FileMode.Create));
            for (long i = 0; i < NumRuns; i++) WriteGrid(i);
            Wtr.Close();
            Wtr.Dispose();
            Wtr = null;
        }

        public static RunList Create(int DataType, int NumBands, long NumRows, long NumCols, string FileFormat, string HeaderLine, long NumRuns = -1)
        {
            RunList Runs = null;
            switch (DataType)
            {
                case 1:
                    Runs = new ByteRunList();
                    break;
                case 4:
                    Runs = new FloatRunList();
                    break;
            }

            Runs.HeaderLine = HeaderLine;
            Runs.DataType = DataType;
            Runs.NumBands = NumBands;
            Runs.NumRows = NumRows;
            Runs.NumCols = NumCols;
            Runs.NumRuns = NumRuns;
            Runs.NumGridElements = NumRows * NumCols * NumBands;
            Runs.FileFormat = FileFormat;
            return Runs;
        }

        public static RunList Read(string BasePath)
        {
            int DataType = RunList.ReadDataType(BasePath);
 
            BinaryReader Rdr = new BinaryReader(new FileStream(BasePath + ".ENVI.RLE", FileMode.Open));
            string HeaderLine = Rdr.ReadString();
            long NumRows = Rdr.ReadInt64();
            long NumCols = Rdr.ReadInt64();
            DataType = Rdr.ReadInt32();
            string FileFormat = Rdr.ReadString();
            int NumBands = Rdr.ReadInt32();
            long RunCount = Rdr.ReadInt64();
            RunList Runs = Create(DataType, NumBands, NumRows, NumCols, FileFormat, HeaderLine, RunCount);
            Runs.AllocateRuns(RunCount);
            Runs.Rdr = Rdr;
            for (long i = 0; i < RunCount; i++) Runs.ReadRun(i);
            return Runs;
        }

        public void CompressToDisk(string BasePath)
        {
            Wtr = new BinaryWriter(new FileStream(BasePath + ".ENVI.RLE", FileMode.Create));
            HeaderLine = "ENVI RLE 1.0";
            Wtr.Write(HeaderLine);
            Wtr.Write(NumRows);
            Wtr.Write(NumCols);
            Wtr.Write(DataType);
            Wtr.Write(FileFormat);
            Wtr.Write(NumBands);
            NumRuns = EncodeRuns(null);
            Wtr.Write(NumRuns);
            EncodeRuns(Wtr);
            Wtr.Close();
            Wtr.Dispose();
            Wtr = null;
        }

        public static string ReadHeaderLine(string BasePath)
        {
            BinaryReader Rdr = new BinaryReader(new FileStream(BasePath + ".ENVI.RLE", FileMode.Open));
            string HeadLine = Rdr.ReadString();
            Rdr.Close();
            Rdr.Dispose();
            Rdr = null;
            return HeadLine;
        }

        public static int ReadDataType(string BasePath)
        {
            BinaryReader Rdr = new BinaryReader(new FileStream(BasePath + ".ENVI.RLE", FileMode.Open));
            string HeadLine = Rdr.ReadString();
            long NotUsed = Rdr.ReadInt64();
            NotUsed = Rdr.ReadInt64();
            int DataType = Rdr.ReadInt32();
            Rdr.Close();
            Rdr.Dispose();
            Rdr = null;
            return DataType;
        }

        protected long EncodeRuns(BinaryWriter Wtr = null) //When set to null, just returns the count of runs
        {
            long RunIndex = -1;
            double CurrentValue = GetCell(0);
            int RunLength = 0;
            for (long i = 0; i < NumGridElements; i++)
            {
                double NewValue = GetCell(i);
                if ((NewValue != CurrentValue) || (RunLength == MaxRunLength))
                {
                    RunIndex++;
                    if (Wtr != null) WriteRun(RunLength, CurrentValue);
                    RunLength = 0;
                    CurrentValue = NewValue;
                }
                RunLength++;
            }

            //Encode the last run if there is one
            if (RunLength > 0) {
                RunIndex++;
                if (Wtr != null) WriteRun(RunLength, CurrentValue);
            }
            
            //Always return the number of runs encoded (or just counted)
            return RunIndex + 1;
        }
    }

    //A byte run list
    public class ByteRunList : RunList
    {
        public struct ByteRun
        {
            public byte Length;
            public byte Value;
        }

        public byte[] bX;
        public ByteRun[] bRuns;


        public override void Dispose()
        {
            bX = null;
            bRuns = null;
        }

        public override void WriteRun(int RunLength, double RunValue)
        {
            Wtr.Write((byte) RunLength);
            Wtr.Write((byte) RunValue);
        }

        public override long DataElementSize
        {
            get
            {
                return 1;
            }
            set { }
        }

        public override void Reference(EnviImage Image)
        {
            bX = (Image as ByteImage).X;
            NumGridElements = Image.NumElements;
        }

        public override void AllocateRuns(long RunCount)
        {
            bRuns = new ByteRun[RunCount];
        }

        public override double GetCell(long ImagePos)
        {
            return bX[ImagePos];
        }

        public override void ReadRun(long RunIndex)
        {
            bRuns[RunIndex].Length = Rdr.ReadByte();
            bRuns[RunIndex].Value = Rdr.ReadByte();
        }

        public override void WriteGrid(long RunIndex)
        {
            int Length = bRuns[RunIndex].Length;
            byte Value = bRuns[RunIndex].Value;
            for (int i = 0; i < Length; i++) Wtr.Write(Value);
        }
    }

    //A float run list
    public class FloatRunList : RunList
    {
        public struct FloatRun
        {
            public byte Length;
            public float Value;
        }

        public float[] fX;
        public FloatRun[] fRuns;

        public override void Dispose()
        {
            fX = null;
            fRuns = null;
        }

        public override long DataElementSize
        {
            get
            {
                return 4;
            }
            set { }
        }

        public override void WriteRun(int RunLength, double RunValue)
        {
            Wtr.Write((byte)RunLength);
            Wtr.Write((float)RunValue);
        }

        public override void Reference(EnviImage Image)
        {
            fX = (Image as FloatImage).X;
            NumGridElements = Image.NumElements;
        }

        public override void AllocateRuns(long RunCount)
        {
            fRuns = new FloatRun[RunCount];
        }

        public override double GetCell(long ImagePos)
        {
            return fX[ImagePos];
        }

        public override void ReadRun(long RunIndex)
        {
            fRuns[RunIndex].Length = Rdr.ReadByte();
            fRuns[RunIndex].Value = Rdr.ReadSingle();
        }

        public override void WriteGrid(long RunIndex)
        {
            int Length = fRuns[RunIndex].Length;
            float Value = fRuns[RunIndex].Value;
            for (int i = 0; i < Length; i++) Wtr.Write(Value);
        }
    }

}

