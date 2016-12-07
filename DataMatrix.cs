using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Carto
{
    public class DataMatrix
    {
        public enum DataType {Nothing, Numerical, String, Other};

        public string[] ColLabel = null;
        public string[] RowLabel = null;

        virtual public double Get(int Row, int Col) { return ErrFlagFlt; }

        virtual public int Set(int Row, int Col, double InValue) { return ErrFlagInt; }

        virtual protected double ReadBinaryValue(BinaryReader Rdr) { return ErrFlagFlt; }

        virtual protected int WriteBinaryValue(BinaryWriter Wrtr, double Value) { return ErrFlagInt; }

        public int Rows { get; set; }

        public int Columns { get; set; }

        public double ErrFlagFlt { get; set; }

        public int ErrFlagInt { get; set; }

        public double MissingDataValue { get; set; }

        public DataType TypeIdentifier { get; set; }

        public string TypeName { get; set; }
       
        public int SetString(int Row, int Col, string InStr) 
        {
            double DblValue;
            bool OK = double.TryParse(InStr, out DblValue);
            if (!OK) return ErrFlagInt; 
            double Res = Set(Row, Col, DblValue);
            if (Res != 0.0) return ErrFlagInt;
            return 0;
        }

        public string GetString(int Row, int Col)
        {
            double Res = Get(Row, Col);
            if (Res == ErrFlagFlt) return "BAD ROW OR COLUMN";
            return Res.ToString();
        }

        public int GetInteger(int Row, int Col)
        {
            try
            {
              double Res = Get(Row, Col);
              if (Res == ErrFlagFlt) return ErrFlagInt;
              Res = Math.Round(Res);
              int IntRes = (int)Res;
              return IntRes;
            }
            catch
            {
                return ErrFlagInt;
            }
        }

        public void SetupLabels( bool HasRowLabels, bool HasColLabels)
        {
            if (HasRowLabels) RowLabel = new string[Rows]; else RowLabel = null;
            if (HasColLabels) ColLabel = new string[Columns]; else ColLabel = null;
        }

        public int ReadBinary(string InFileName)
        {
            if (!File.Exists(InFileName)) return -1;

            using (FileStream Fs = new System.IO.FileStream(InFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                using (BinaryReader Rdr = new System.IO.BinaryReader(Fs))
                {
                    try
                    {
                        for (int r = 0; r < Rows; r++)
                            for (int c = 0; c < Columns; c++) {
                                double DblVal = ReadBinaryValue(Rdr);
                                if (DblVal == ErrFlagFlt)
                                {
                                    return -8;
                                }
                                int Res = Set(r, c, DblVal);
                            }
                    }
                    catch
                    {
                        return -9;
                    }

                }
            }

            return 0;
        }

        public int ReadDelimited(string InFileName, char Delimiter) { return Int32.MinValue; }

        public int WriteDelimited(string OutFileName, char Delimiter, bool WriteLabels) { return Int32.MinValue; }

        public int WriteBinary(string OutFileName)
        {

            using (FileStream Fs = new System.IO.FileStream(OutFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                using (BinaryWriter Wrtr = new System.IO.BinaryWriter(Fs))
                {
                    try
                    {
                        for (int r = 0; r < Rows; r++)
                            for (int c = 0; c < Columns; c++)
                            {
                                double Value = Get(r, c);
                                int Res = WriteBinaryValue(Wrtr, Value);
                                if (Res == ErrFlagInt) return -8;
                            }
                    }
                    catch
                    {
                        return -9;
                    }

                }
            }

            return 0;
        }
            
        public bool BadRow(int Row)
        {
            if (Row < 0) return true;
            if (Row > Rows - 1) return true;
            return false;
        }

        public bool BadCol(int Col)
        {
            if (Col < 0) return true;
            if (Col > Columns - 1) return true;
            return false;
        }

        public bool GoodRC(int Row, int Col)
        {
            return !BadRC(Row, Col);
        }

        public bool BadRC(int Row, int Col) 
        {
           if (Row < 0) return true;
           if (Row > Rows - 1) return true;
           if (Col < 0) return true;
           if (Col > Columns - 1) return true;
           return false;
        }

        public int ColLabelIndex(string Label)
        {
            if (Columns < 1) return -2;
            if (Utils.BadString(Label)) return -3;
            if (ColLabel == null) return -4;
            Label = Label.ToUpper().Trim();
            for (int i = 0; i < Columns; i++)
            {
                if (Label == ColLabel[i].ToUpper()) return i;
            }
                return -1;
        }

        public int RowLabelIndex(string Label)
        {
            if (Rows < 1) return -2;
            if (Utils.BadString(Label)) return -3;
            if (RowLabel == null) return -4;
            Label = Label.ToUpper().Trim();
            for (int i = 0; i < Rows; i++)
            {
                if (Label == RowLabel[i].ToUpper()) return i;
            }
            return -1;
        }

        public double Mean()
        {
            double Sum = 0.0;
            double Count = 0.0;
            for (int r = 0; r < Rows; r++)
              for (int c = 0; c < Columns; c++)
              {
                  double Val = Get(r, c);
                  if (Val == MissingDataValue) continue;
                  Sum += Get(r, c);
                  Count += 1.0;
              }
            return Sum / Count;
        }

        public double RowMean(int RowIdx)
        {
            if (BadRow(RowIdx)) return ErrFlagFlt;
            double Sum = 0.0;
            double Count = 0.0;
            for (int c = 0; c < Columns; c++)
            {
                double Val = Get(RowIdx, c);
                if (Val == MissingDataValue) continue;
                Sum += Get(RowIdx, c);
                Count += 1.0;
            }
                return Sum / Count;
        }

        public double ColMean(int ColIdx)
        {
            if (BadCol(ColIdx)) return ErrFlagFlt;
            double Sum = 0.0;
            double Count = 0.0;
            for (int r = 0; r < Rows; r++)
            {
                double Value = Get(r, ColIdx);
                if (Value == MissingDataValue) continue;
                Sum += Value;
                Count += 1.0;
            }
            if (Count == 0) return ErrFlagFlt;
            return Sum / Count;
        }

        public double Stdv()
        {
            double Avg = Mean();
            if (Avg == ErrFlagFlt) return ErrFlagFlt;
            double Diff;
            double DiffSq;
            double SumDiffSq = 0.0;
            double Count = 0.0;
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++) {
                    double Value = Get(r, c);
                    if (Value == MissingDataValue) continue;
                    Diff = Avg - Value;
                    DiffSq = Diff * Diff;
                    SumDiffSq += DiffSq;
                    Count += 1.0;
                }
            double Quotient = SumDiffSq / Count;
            return Math.Sqrt(Quotient);
        }

     

        public int GetRow(int RowIdx, out double [] Values) 
        {
            Values = null;
            if (TypeIdentifier != DataType.Numerical) return ErrFlagInt; 
            Values = new double[Columns];
            for (int C = 0; C < Columns; C++) Values[C] = Get(RowIdx, C);
            return 0;
        }

        public int GetCol(int ColIdx, out double[] Values)
        {
            Values = null;
            if (TypeIdentifier != DataType.Numerical) return ErrFlagInt; 
            Values = new double[Rows];
            for (int R = 0; R < Rows; R++) Values[R] = Get(R, ColIdx);
            return 0;
        }

        public static int Regress(DataMatrix D1, DataMatrix D2, out double Intercept, out double Slope, out double R2, out double R, out int PairCount)
        {
            //Initialize and check to see if the two matrices appear to be corresponding
            Intercept = double.NegativeInfinity;
            Slope = double.NegativeInfinity;
            R2 = double.NegativeInfinity;
            R = double.NegativeInfinity;
            PairCount = 0;
            if (D1.Rows != D2.Rows) return -1;
            if (D1.Columns != D2.Columns) return -2;
            if (D1.TypeIdentifier != DataType.Numerical) return -3;
            if (D2.TypeIdentifier != DataType.Numerical) return -4;

            double SumX = 0.0;
            double SumY = 0.0;
            double SumXY = 0.0;
            double SumXSq = 0.0;
            double SumYSq = 0.0;
            for (int r = 0; r < D1.Rows; r++)
                for (int c = 0; c < D1.Columns; c++)
                {
                  double XVal = D1.Get(r,c);
                  double YVal = D2.Get(r,c);
                  if (XVal == D1.MissingDataValue) continue;
                  if (YVal == D2.MissingDataValue) continue;
                  PairCount++;
                  SumX = SumX + XVal;
                  SumY = SumY + YVal;
                  SumXY = SumXY + XVal * YVal;
                  SumXSq = SumXSq + XVal * XVal;
                  SumYSq = SumYSq + YVal * YVal;
                }
            
            //Calc slope and intercept
            double Denom = SumXSq - SumX * SumX / PairCount;
            if (Denom == 0.0)
            {
                Denom = double.NegativeInfinity;
                return -5;
            }
            Slope = (SumXY - SumX * SumY / PairCount) / Denom;
            Intercept = (SumY - Slope * SumX) / PairCount;
            
            //Calc correlation coeff
            Denom = SumYSq - SumY * SumY / PairCount;
            if (Denom == 0.0)
            {
                R2 = 1.0;
               
            } else {
                R2 = Slope * (SumXY - SumX * SumY / PairCount) / Denom; 
            }

            R = Math.Sqrt(R2);

            return 0;
        }

        public int RegressCols(int Col1, int Col2, out double Intercept, out double Slope, out double R2, out double R, out int PairCount)
        {
            Intercept = double.NegativeInfinity;
            Slope = double.NegativeInfinity;
            R2 = double.NegativeInfinity;
            R = double.NegativeInfinity;
            PairCount = 0;
            if (TypeIdentifier != DataType.Numerical) return -1;
           
            double SumX = 0.0;
            double SumY = 0.0;
            double SumXY = 0.0;
            double SumXSq = 0.0;
            double SumYSq = 0.0;
            for (int r = 0; r < Rows; r++)
            {
                double XVal = Get(r, Col1);
                double YVal = Get(r, Col2);
                if (XVal == MissingDataValue) continue;
                if (YVal == MissingDataValue) continue;
                PairCount++;
                SumX = SumX + XVal;
                SumY = SumY + YVal;
                SumXY = SumXY + XVal * YVal;
                SumXSq = SumXSq + XVal * XVal;
                SumYSq = SumYSq + YVal * YVal;
            }

            //Calc slope and intercept
            double Denom = SumXSq - SumX * SumX / PairCount;
            if (Denom == 0.0)
            {
                Denom = double.NegativeInfinity;
                return -2;
            }
            Slope = (SumXY - SumX * SumY / PairCount) / Denom;
            Intercept = (SumY - Slope * SumX) / PairCount;

            //Calc correlation coeff
            Denom = SumYSq - SumY * SumY / PairCount;
            if (Denom == 0.0)
            {
                R2 = 1.0;

            }
            else
            {
                R2 = Slope * (SumXY - SumX * SumY / PairCount) / Denom;
            }

            R = Math.Sqrt(R2);

            return 0;
        }

        public void Clear()
        {
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                    Set(r, c, MissingDataValue);
        }

        public int CloneTo(out DataMatrix OutWin)
        {  
            int NumRows = this.Rows;
            int NumCols = this.Columns;
            bool HasColLabels = (ColLabel != null);
            bool HasRowLabels = (RowLabel != null);
            double MissingDataValue = this.MissingDataValue;
            int ErrFlagInt = this.ErrFlagInt;
            double ErrFlagFlt = this.ErrFlagFlt;
            OutWin = null;
            switch (this.TypeName) 
            {
                case "FLOAT" :
                    OutWin = new FloatMatrix(NumRows, NumCols, HasRowLabels, HasColLabels, MissingDataValue, ErrFlagFlt, ErrFlagInt);
                    break;
                case "DOUBLE" :
                    OutWin = new DoubleMatrix(NumRows, NumCols, HasRowLabels, HasColLabels, MissingDataValue, ErrFlagFlt, ErrFlagInt);
                    break;
                case "INT16" :
                    //OutWin = new ShortMatrix(NumRows, NumCols, HasRowLabels, HasColLabels, MissingData);
                    break;
                case "INT32" :
                    OutWin = new LongMatrix(NumRows, NumCols, HasRowLabels, HasColLabels, MissingDataValue, ErrFlagFlt, ErrFlagInt);
                    break;
                case "BYTE" :
                    //OutWin = new ByteMatrix(NumRows, NumCols, HasRowLabels, HasColLabels, MissingData);
                    break;
            }

            for (int r = 0; r < NumRows; r++)
                for (int c = 0; c < NumCols; c++)
                { 
                    double Val = Get(r, c);
                    OutWin.Set(r, c, Val);
                }

            if (HasRowLabels)
                for (int i = 0; i < NumRows; i++) OutWin.RowLabel[i] = this.RowLabel[i];
           
            if (HasColLabels)
                for (int i = 0; i < NumCols; i++) OutWin.ColLabel[i] = this.ColLabel[i];

            return 0;
        }

        //This works, I just didn't bother to get the necessary references added
        //public void WriteBMP(byte MissingDataGrayLevel,
        //                     byte StartGrayLevel,
        //                     byte EndGrayLevel,
        //                     string OutFile)
        //{
        //    int NumRows = this.Rows;
        //    int NumCols = this.Columns;
        //    double Min = double.MaxValue;
        //    double Max = double.MinValue;
        //    for (int R = 0; R < NumRows; R++)
        //        for (int C = 0; C < NumCols; C++)
        //        {
        //            double Val = Get(R, C);
        //            if (Val == MissingDataValue) continue;
        //            if (Val < Min) Min = Val;
        //            if (Val > Max) Max = Val;
        //        }

        //    Color BackColor;
        //    Color ForeColor;
        //    BackColor = Color.FromArgb(MissingDataGrayLevel, MissingDataGrayLevel, MissingDataGrayLevel);

        //    Bitmap Bits = new Bitmap(NumRows, NumCols);
        //    LineEquation Scaler = new LineEquation(Min, StartGrayLevel, Max, EndGrayLevel);
        //    for (int R = 0; R < NumRows; R++)
        //        for (int C = 0; C < NumCols; C++)
        //        {
        //            double ImgVal = Get(R, C);
        //            if (ImgVal == this.MissingDataValue)
        //            {
        //                Bits.SetPixel(C, R, BackColor);
        //                continue;
        //            }

        //            double DigNumDbl = Scaler.Fwd(ImgVal);
        //            DigNumDbl = Math.Round(DigNumDbl);
        //            byte DigNum = (byte)DigNumDbl;

        //            ForeColor = Color.FromArgb(DigNum, DigNum, DigNum);
        //            Bits.SetPixel(C, R, ForeColor);
        //        }
            
        //    OutFile = OutFile.ToUpper();
        //    if (OutFile.EndsWith(".BMP")) OutFile = OutFile.Replace(".BMP", string.Empty);
        //    Bits.Save(OutFile, System.Drawing.Imaging.ImageFormat.Bmp);
        //}

        public int CountCells(double MatchValue, double MissingDataVal, out int MatchCount, out int MissingCount, out int OtherCount)
        {
            MatchCount = 0;
            MissingCount = 0;
            OtherCount = 0;
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++) {
                    double CellVal = Get(r, c);
                    if (CellVal == MatchValue)
                        MatchCount++;
                    else if (CellVal == MissingDataVal)
                        MissingCount++;
                    else
                        OtherCount++;
                }
            return 0;
        }

        public int CountMissing(out int Count)
        {
            Count = 0;
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                {
                    double CellVal = Get(r, c);
                    if (CellVal == MissingDataValue) Count++;
                }
            return 0;
        }

        public int ListGT(double CutValue, out List<double> OutVals)
        {
            OutVals = null;
            OutVals = new List<double>();
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                {
                    double Val = Get(r, c);
                    if (Val == MissingDataValue) continue;
                    if (Val > CutValue) OutVals.Add(Val);
                }
            return 0;
        }

        public int ListLT(double CutValue, out List<double> OutVals)
        {
            OutVals = null;
            OutVals = new List<double>();
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                {
                    double Val = Get(r, c);
                    if (Val == MissingDataValue) continue;
                    if (Val < CutValue) OutVals.Add(Val);
                }
            return 0;
        }

        public double Min(params double[] Ignore)
        {
            double TheMin = double.MaxValue;
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++){
                    double Value = Get(r, c);
                    for (int i = 0; i < Ignore.GetLength(0); i++) if (Value == Ignore[i]) Value = double.MaxValue;
                    if (Value < TheMin) TheMin = Value;
                }
            return TheMin;
        }

        public double Max(params double[] Ignore)
        {
            double TheMax = double.MinValue;
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                {
                    double Value = Get(r, c);
                    for (int i = 0; i < Ignore.GetLength(0); i++) if (Value == Ignore[i]) Value = double.MinValue;
                    if (Value > TheMax) TheMax = Value;
                }
            return TheMax;
        }

    }


    public class DoubleMatrix : DataMatrix
    {
        public double[,] Values = null;

        public DoubleMatrix(int Rows, int Columns, bool HasRowLabels, bool HasColLabels, double MissingDataValue, double ErrFlagFlt, int ErrFlagInt)
        {
            Setup(Rows, Columns, HasRowLabels, HasColLabels, MissingDataValue, ErrFlagFlt, ErrFlagInt);
        }
        
        public int Setup(int Rows, int Columns, bool HasRowLabels, bool HasColLabels, double MissingDataValue, double ErrFlagFlt, int ErrFlagInt)
        {
            TypeIdentifier = DataType.Numerical;
            TypeName = "DOUBLE";
            this.MissingDataValue = MissingDataValue;
            this.ErrFlagFlt = ErrFlagFlt;
            this.ErrFlagInt = ErrFlagInt;
            this.Rows = Rows;
            this.Columns = Columns;
            SetupLabels(HasRowLabels, HasColLabels);
            try
            {
                Values = new double[Rows, Columns];
            }
            catch
            {
                return ErrFlagInt;
            }
            return 0;
        }

        override public double Get(int Row, int Col)
        {
            if (BadRC(Row, Col)) return ErrFlagFlt;
            return Values[Row, Col];
        }

        override public int Set(int Row, int Col, double NewValue)
        {
            if (BadRC(Row, Col)) return ErrFlagInt;
            Values[Row, Col] = NewValue;
            return 0;
        }

        override protected double ReadBinaryValue(BinaryReader Rdr)
        {
            try
            {
                return Rdr.ReadDouble();
            }
            catch
            {
                return ErrFlagFlt;
            }
        }

        protected override int WriteBinaryValue(BinaryWriter Wrtr, double Value)
        {
            try
            {
                Wrtr.Write(Value);
            }
            catch
            {
                return ErrFlagInt;
            }
            return 0;
        }


    }


    public class FloatMatrix : DataMatrix
    {
        public float[,] Values = null;

        public FloatMatrix(int Rows, int Columns, bool HasRowLabels, bool HasColLabels, double MissingDataValue, double ErrFlagFlt, int ErrFlagInt)
        {
            Setup(Rows, Columns, HasRowLabels, HasColLabels, MissingDataValue, ErrFlagFlt, ErrFlagInt);
        }

        public int Setup(int Rows, int Columns, bool HasRowLabels, bool HasColLabels, double MissingData, double ErrFlagFlt, int ErrFlagInt)
        {
            TypeIdentifier = DataType.Numerical;
            TypeName = "FLOAT";
            this.MissingDataValue = MissingDataValue;
            this.ErrFlagFlt = ErrFlagFlt;
            this.ErrFlagInt = ErrFlagInt;
            this.Rows = Rows;
            this.Columns = Columns;
            SetupLabels(HasRowLabels, HasColLabels);
            try
            {
                Values = new float[Rows, Columns];
            }
            catch
            {
                return ErrFlagInt;
            }
            return 0;
        }

        override public double Get(int Row, int Col)
        {
            if (BadRC(Row, Col)) return ErrFlagFlt;
            return Values[Row, Col];
        }

        override public int Set(int Row, int Col, double NewValue)
        {
            if (BadRC(Row, Col)) return ErrFlagInt;
            if (NewValue > float.MaxValue) return -2;
            if (NewValue < float.MinValue) return -3;
            Values[Row, Col] = (float)NewValue;
            return 0;
        }

        override protected double ReadBinaryValue(BinaryReader Rdr)
        {
            try
            {
                return Rdr.ReadSingle();
            }
            catch
            {
                return ErrFlagFlt;
            }
        }

        protected override int WriteBinaryValue(BinaryWriter Wrtr, double Value)
        {
            try
            {
                if (Value > float.MaxValue) return ErrFlagInt;
                if (Value < float.MinValue) return ErrFlagInt;
                float FloatValue = (float)Value;
                Wrtr.Write(FloatValue);
            }
            catch
            {
                return ErrFlagInt;
            }
            return 0;
        }

    }


    public class LongMatrix : DataMatrix
    {
        public int[,] Values = null;

        public LongMatrix(int Rows, int Columns, bool HasRowLabels, bool HasColLabels, double MissingDataValue, double ErrFlagFlt, int ErrFlagInt)
        {
            Setup(Rows, Columns, HasRowLabels, HasColLabels, MissingDataValue, ErrFlagFlt, ErrFlagInt);
        }

        public int Setup(int Rows, int Columns, bool HasRowLabels, bool HasColLabels, double MissingData, double ErrFlagFlt, int ErrFlagInt)
        {
            TypeIdentifier = DataType.Numerical;
            TypeName = "INT32";
            this.MissingDataValue = MissingDataValue;
            this.ErrFlagFlt = ErrFlagFlt;
            this.ErrFlagInt = ErrFlagInt;
            this.Rows = Rows;
            this.Columns = Columns;
            SetupLabels(HasRowLabels, HasColLabels);
            try
            {
                Values = new int[Rows, Columns];
            }
            catch
            {
                return ErrFlagInt;
            }
            return 0;
        }

        override public double Get(int Row, int Col)
        {
            if (BadRC(Row, Col)) return ErrFlagFlt;
            return (double) Values[Row, Col];
        }

        override public int Set(int Row, int Col, double NewValue)
        {
            if (BadRC(Row, Col)) return ErrFlagInt;
            if (NewValue > int.MaxValue) return -2;
            if (NewValue < int.MinValue) return -3;
            Values[Row, Col] = (int) NewValue;
            return 0;
        }

        override protected double ReadBinaryValue(BinaryReader Rdr)
        {
            try
            {
                return Rdr.ReadInt32();
            }
            catch
            {
                return ErrFlagFlt;
            }
        }

        protected override int WriteBinaryValue(BinaryWriter Wrtr, double Value)
        {
            try
            {
                if (Value > int.MaxValue) return ErrFlagInt;
                if (Value < int.MinValue) return ErrFlagInt;
                int IntValue = (int)Value;
                Wrtr.Write(IntValue);
            }
            catch
            {
                return ErrFlagInt;
            }
            return 0;
        }

    }





}
