using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Carto
{

    public static class Utils
    {
        #region Math
        public static double D2R(double DegValue)
        {
            return DegValue * 0.01745329251994329576923690768489;
        }

        public static double R2D(double RadValue)
        {
            return RadValue * 57.295779513082320876798154814105;
        }

        public static double Sign(double Value)
        {
            if (Value == 0) return 0.0;
            if (Value < 0.0) return -1.0;
            if (Value > 0.0) return +1.0;
            return double.MaxValue;
        }

        public static double FlipSigns(double Value)
        {
            Value = Value * -1.0;
            return Value;
        }

        #endregion

        #region Array Utilities

        public static string BytesToString(byte[] InBytes)
        {
            StringBuilder Str = new StringBuilder(InBytes.Length);
            for (int i = 0; i < InBytes.Length; i++)
                Str.Append((char)InBytes[i]);
            string OutStr = Str.ToString();
            return OutStr;
        }

        public static void DestroyTwo(double[][] X)
        {
            for (int i = 0; i < X.Length; i++)
                X[i] = null;
            X = null;
        }

        public static double[] Setup(int nRows, double InitValue)
        {
            double[] T = new double[nRows];
            Assign(T, InitValue);
            return T;
        }

        public static int[] Setup(int nRows, int InitValue)
        {
            int[] T = new int[nRows];
            Assign(T, InitValue);
            return T;
        }

        public static double[,] SetupTwo(int nRows, int nCols, double InitVal = 0.0)
        {
            double[,] S = new double[nRows, nCols];
            Assign(S, InitVal);
            return S;
        }

        public static double[][] Setup(int nRows, int nCols, double InitVal = 0.0)
        {
            double[][] S = new double[nRows][];
            for (int i = 0; i < nRows; i++)
                S[i] = new double[nCols];
            Assign(S, InitVal);
            return S;
        }

        public static void Assign(int[] X, int InitValue)
        {
            for (int i = 0; i < X.Length; i++) X[i] = InitValue;
        }

        public static void Assign(bool[] X, bool InitValue)
        {
            for (int i = 0; i < X.Length; i++) X[i] = InitValue;
        }


        public static void Assign(double[] X, double InitValue)
        {
            for (int i = 0; i < X.Length; i++) X[i] = InitValue;
        }

        public static void Assign(double[,] X, double InitValue)
        {
            for (int i = 0; i < X.GetLength(0); i++)
                for (int j = 0; j < X.GetLength(1); j++)
                    X[i, j] = InitValue;
        }

        public static void Assign(double[][] X, double InitValue)
        {
            int NumRows = X.Length;
            int NumCols = X[0].Length;
            for (int i = 0; i < NumRows; i++)
                for (int j = 0; j < NumCols; j++)
                    X[i][j] = InitValue;
        }

        public static double[,] Convert(double[][] X)
        {
            int NumRows = X.Length;
            int NumCols = X[0].Length;
            double[,] T = new double[NumRows, NumCols];
            for (int i = 0; i < NumRows; i++)
            {
                for (int j = 0; j < NumCols; j++)
                    T[i, j] = X[i][j];
            }
            return T;

        }

        public static double[][] Convert(double[,] X)
        {
            int NumRows = X.GetLength(0);
            int NumCols = X.GetLength(1);
            double[][] T = new double[NumRows][];
            for (int i = 0; i < NumRows; i++)
            {
                T[i] = new double[NumCols];
                for (int j = 0; j < NumCols; j++)
                    T[i][j] = X[i, j];
            }
            return T;
        }

        /// <summary>
        /// Returns the maximum index for an array
        /// </summary>
        /// <param name="target">The array</param>
        /// <returns>The maximum index for that array</returns>
        public static int MaxIndex(this Array Target)
        {
            return Target.Length - 1;
        }

        /// <summary>
        /// Copy an array without instantiating it first
        /// </summary>
        /// <param name="Source">The array to copy</param>
        /// <returns>A copy of the array</returns>
        public static double[] CopyArray(double[] Source)
        {
            double[] NewArray = new double[Source.Length];
            Array.Copy(Source, NewArray, Source.Length);
            return NewArray;
        }


        /// <summary>
        /// Copy an array without instantiating it first
        /// </summary>
        /// <param name="Source">The array to copy</param>
        /// <returns>A copy of the array</returns>
        public static int[] CopyArray(int[] Source)
        {
            int[] NewArray = new int[Source.Length];
            Array.Copy(Source, NewArray, Source.Length);
            return NewArray;
        }

        /// <summary>
        /// Copy an array without instantiating it first
        /// </summary>
        /// <param name="Source">The array to copy</param>
        /// <returns>A copy of the array</returns>
        public static float[] CopyArray(float[] Source)
        {
            float[] NewArray = new float[Source.Length];
            Array.Copy(Source, NewArray, Source.Length);
            return NewArray;
        }

        /// <summary>
        /// Copy an array without instantiating it first
        /// </summary>
        /// <param name="Source">The array to copy</param>
        /// <returns>A copy of the array</returns>
        public static string[] CopyArray(string[] Source)
        {
            string[] NewArray = new string[Source.Length];
            Array.Copy(Source, NewArray, Source.Length);
            return NewArray;
        }

        /// <summary>
        /// Copy an array without instantiating it first
        /// </summary>
        /// <param name="Source">The array to copy</param>
        /// <returns>A copy of the array</returns>
        public static double[][] CopyArray(double[][] Source)
        {
            double[][] NewArray = new double[Source.Length][];
            for (int i = 0; i < Source.Length; i++)
            {
                NewArray[i] = CopyArray(Source[i]);
            }
            return NewArray;
        }

        #endregion

        #region Odd and Even
        /// <summary>
        /// Checks to see if an integer is odd
        /// </summary>
        /// <param name="X">The integer to check</param>
        /// <returns>True if the integer is odd, false otherwise</returns>
        public static bool IsOdd(int X)
        {
            int Rem = X % 2;
            return (Rem == 1);
        }

        /// <summary>
        /// Checks to see if an integer is even
        /// </summary>
        /// <param name="X">The integer to check</param>
        /// <returns>True if the integer is even, false otherwise</returns>
        public static bool IsEven(int X)
        {
            return !IsOdd(X);
        }


        #endregion

        #region Checking for Well Formed Tokens (e.g., strings, ints, etc.)
        /// <summary>
        /// Checks to see if a string is a well-formed positive integer
        /// </summary>
        /// <param name="WhatStr">The string to check</param>
        /// <returns>True if well formed, false otherwise</returns>
        public static bool GoodPositiveInt(string WhatStr)
        {
            return (!BadPositiveInt(WhatStr));
        }

        /// <summary>
        /// Checks to see if a string is a well-formed positive double
        /// </summary>
        /// <param name="WhatStr">The string to check</param>
        /// <returns>True if well formed, false otherwise</returns>
        public static bool GoodPositiveDouble(string WhatStr)
        {
            return (!BadPositiveDouble(WhatStr));
        }

        /// <summary>
        /// Checks to see if a string is incorrectly formed for a positive int
        /// </summary>
        /// <param name="WhatStr">The string to check</param>
        /// <returns>True if incorrectly formed, false otherwise</returns>
        public static bool BadPositiveInt(string WhatStr)
        {
            int Value = 0;
            bool Res = Int32.TryParse(WhatStr, out Value);
            if (Res == false) return true;
            if (Value < 0) return true;
            return false;
        }

        /// <summary>
        /// Checks to see if a string is incorrectly formed for a positive double
        /// </summary>
        /// <param name="WhatStr">The string to check</param>
        /// <returns>True if incorrectly formed, false otherwise</returns>
        public static bool BadPositiveDouble(string WhatStr)
        {
            double Value = 0.0;
            bool Res = Double.TryParse(WhatStr, out Value);
            if (Res == false) return true;
            if (Value < 0.0) return true;
            return false;
        }

        /// <summary>
        /// Checks to see if a string is good (i.e. not null or empty)
        /// </summary>
        /// <param name="WhatStr">The string to check</param>
        /// <returns>True if null or empty after trimming</returns>
        public static bool BadString(string WhatStr)
        {
            return !GoodString(WhatStr);
        }

        /// <summary>
        /// Checks to see if a string is good (i.e. not null or empty)
        /// </summary>
        /// <param name="WhatStr">The string to check</param>
        /// <returns>True if not null and not empty after trimming</returns>
        public static bool GoodString(string WhatStr)
        {
            if (WhatStr == null) return false;
            WhatStr = WhatStr.Trim();
            if (WhatStr == string.Empty) return false;
            return true;
        }

        public static void FixDoubleExponent(ref string Value)
        {
            StringBuilder Str = new StringBuilder(Value.Length);
            FixDoubleExponent(ref Str);
            Value = Str.ToString();
        }


        public static void FixDoubleExponent(ref StringBuilder Value)
        {
            for (int i = 0; i < Value.Length; i++)
            {
                switch (Value[i])
                {
                    case 'D':
                    case 'd':
                    case 'e':
                        Value[i] = 'E';
                        break;
                }
            }
        }


        #endregion

        #region Maximum and Minimum
        /// <summary>
        /// Return the maximum of two integers
        /// </summary>
        /// <param name="FirstInt">The first integer</param>
        /// <param name="SecInt">The second integer</param>
        /// <returns></returns>
        public static int Max(int FirstInt, int SecInt)
        {
            if (FirstInt > SecInt)
                return FirstInt;
            else
                return SecInt;
        }

        /// <summary>
        /// Return the maximum of two doubles
        /// </summary>
        /// <param name="FirstInt">The first integer</param>
        /// <param name="SecInt">The second integer</param>
        /// <returns></returns>
        public static double Max(double First, double Second)
        {
            if (First > Second)
                return First;
            else
                return Second;
        }

        /// <summary>
        /// Return the maximum of two integers
        /// </summary>
        /// <param name="FirstInt">The first integer</param>
        /// <param name="SecInt">The second integer</param>
        /// <returns></returns>
        public static int Min(int FirstInt, int SecInt)
        {
            if (FirstInt < SecInt)
                return FirstInt;
            else
                return SecInt;
        }

        #endregion

        #region Program Console Pause

        /// <summary>
        /// Provides a prompt for the end of a console program
        /// </summary>
        public static void ProgramDone()
        {
            Console.WriteLine();
            Console.Write("Program done.  Press return to exit... ");
            Console.ReadLine();
        }



        /// <summary>
        /// Provides a pause prompt for the end of a console program
        /// </summary>
        public static void ProgramPause()
        {
            Console.WriteLine();
            Console.Write("Program pause.  Press return to continue... ");
            Console.ReadLine();
        }

        #endregion

        #region Byte Wwapping

        /// <summary>
        /// Routine to swap bytes, converting from one endian representation to another
        /// </summary>
        /// <param name="X">The swapped representation</param>
        public static void SwapBytes(ref int X)
        {
            byte[] B = BitConverter.GetBytes(X);
            Array.Reverse(B);
            X = BitConverter.ToInt32(B, 0);
        }

        /// <summary>
        /// Routine to swap bytes, converting from one endian representation to another
        /// </summary>
        /// <param name="X">The swapped representation</param>
        public static void SwapBytes(ref Int16 X)
        {
            byte[] B = BitConverter.GetBytes(X);
            Array.Reverse(B);
            X = BitConverter.ToInt16(B, 0);
        }

        /// <summary>
        /// Routine to swap bytes, converting from one endian representation to another
        /// </summary>
        /// <param name="X">The swapped representation</param>
        public static void SwapBytes(ref UInt16 X)
        {
            byte[] B = BitConverter.GetBytes(X);
            Array.Reverse(B);
            X = BitConverter.ToUInt16(B, 0);
        }


        /// <summary>
        /// Routine to swap bytes, converting from one endian representation to another
        /// </summary>
        /// <param name="X">The swapped representation</param>
        public static void SwapBytes(ref double X)
        {
            byte[] B = BitConverter.GetBytes(X);
            Array.Reverse(B);
            X = BitConverter.ToDouble(B, 0);
        }

        /// <summary>
        /// Routine to swap bytes, converting from one endian representation to another
        /// </summary>
        /// <param name="X">The swapped representation</param>
        public static void SwapBytes(ref float X)
        {
            byte[] B = BitConverter.GetBytes(X);
            Array.Reverse(B);
            X = BitConverter.ToSingle(B, 0);
        }

        /// <summary>
        /// Swap two values
        /// </summary>
        /// <param name="X">The first value</param>
        /// <param name="Y">The second value</param>
        public static void Swap(ref int X, ref int Y)
        {
            int Temp = X;
            X = Y;
            Y = Temp;
        }

        /// <summary>
        /// Swap two values
        /// </summary>
        /// <param name="X">The first value</param>
        /// <param name="Y">The second value</param>
        public static void Swap(ref double X, ref double Y)
        {
            double Temp = X;
            X = Y;
            Y = Temp;
        }

        /// <summary>
        /// Swap two values
        /// </summary>
        /// <param name="X">The first value</param>
        /// <param name="Y">The second value</param>
        public static void Swap(ref float X, ref float Y)
        {
            float Temp = X;
            X = Y;
            Y = Temp;
        }

        #endregion

        #region Cloning Various collections

        /// <summary>
        /// Clone a list of strings
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Target"></param>
        /// <returns></returns>
        public static List<string> CloneList(List<string> Source)
        {
            List<string> Target = new List<string>(Source.Count);
            foreach (string Str in Source)
            {
                Target.Add(Str);
            }
            return Target;
        }

        /// <summary>
        /// Clone a dictionary (Deep copy)
        /// </summary>
        /// <param name="Source">The dictionary to copy</param>
        /// <param name="Target">The dictionary containing the copied entries when method returns</param>
        public static void CloneDictionary(Dictionary<string, string> Source, out Dictionary<string, string> Target)
        {
            Target = null;
            Target = new Dictionary<string, string>(Source.Count);
            foreach (KeyValuePair<string, string> Pair in Source)
            {
                Target.Add(Pair.Key, Pair.Value);
            }
        }

        /// <summary>
        /// Clone a dictionary (Deep copy)
        /// </summary>
        /// <param name="Source">The dictionary to copy</param>
        /// <param name="Target">The dictionary containing the copied entries when method returns</param>
        public static void CloneDictionary(Dictionary<int, string> Source, out Dictionary<int, string> Target)
        {
            Target = null;
            Target = new Dictionary<int, string>(Source.Count);
            foreach (KeyValuePair<int, string> Pair in Source)
            {
                Target.Add(Pair.Key, Pair.Value);
            }
        }
        #endregion

        #region Cleaning, trimming, and working with lines of text

        /// <summary>
        /// Trim all the strings in an array
        /// </summary>
        /// <param name="Pieces">The array of strings</param>
        public static void TrimAll(ref string[] Strings)
        {
            for (int i = 0; i < Strings.Length; i++)
            {
                Strings[i] = Strings[i].Trim();
            }
        }


        public static int SplitAndClean(StreamReader R, char Separator, out string[] Pieces)
        {
            string OneLine = R.ReadLine();
            return SplitAndClean(OneLine, Separator, out Pieces);
        }


        public static int SplitAndClean(string OneString, char Separator, out string[] Pieces)
        {
            Pieces = null;
            if (OneString == null) return -2;
            if (OneString.Length == 0) return -3;
            Pieces = OneString.Split(Separator);
            if (Pieces.Length == 0) return -4;
            bool AllPiecesBad = true;
            for (int i = 0; i < Pieces.Length; i++)
            {
                Pieces[i] = Pieces[i].Trim();
                if (Pieces[i].Length > 0) AllPiecesBad = false;
            }
            if (AllPiecesBad) return -5;
            return Pieces.Length;
        }

        /// <summary>
        /// Remove all the spaces in a string
        /// </summary>
        /// <param name="Str">The source string</param>
        /// <returns>The string with all spaces removed</returns>
        public static string RemoveSpaces(string Str)
        {
            StringBuilder OutStr = new StringBuilder(Str.Length);
            foreach (char OneChar in Str)
            {
                if (OneChar != ' ') OutStr.Append(OneChar);
            }
            return OutStr.ToString();
        }




        /// <summary>
        /// Remove all specified characters in a string
        /// </summary>
        /// <param name="Str">The source string</param>
        /// <param name="Character"> The character to remove</param>
        /// <returns>The string with all spaces removed</returns>
        public static string RemoveChar(string Str, char Character)
        {
            StringBuilder OutStr = new StringBuilder(Str.Length);
            foreach (char OneChar in Str)
            {
                if (OneChar != Character) OutStr.Append(OneChar);
            }
            return OutStr.ToString();
        }

        public static string Clean(string Str)
        {
            string OutStr = Str.Trim();
            OutStr = OutStr.ToUpper();
            return OutStr;
        }

        #endregion

        #region StreamReader Utilities

        /// <summary>
        /// Count the number of lines in a text file
        /// </summary>
        /// <param name="InFileName">Full path to the text file</param>
        /// <returns>-1 if file not open, -2 if other error, count of records otherwise</returns>
        public static int CountTextFileLines(string InFileName)
        {
            if (!File.Exists(InFileName)) return -1;
            StreamReader Rdr = null;
            int Count = 0;
            try
            {
                Rdr = new StreamReader(InFileName);
                while (!Rdr.EndOfStream)
                {
                    string OneLine = Rdr.ReadLine();
                    Count++;
                }
            }
            catch
            {
                Count = -2;
            }
            finally
            {
                if (Rdr != null)
                {
                    Rdr.Close();
                    Rdr.Dispose();
                    Rdr = null;
                }
            }
            return Count;
        }

        public static string ReadAndClean(StreamReader R)
        {
            string OneLine = R.ReadLine();
            return Clean(OneLine);
        }

        public static int ReadAndClean(StreamReader InStream, char Separator, out string[] Pieces, out string StringRead)
        {
            Pieces = null;
            StringRead = null;
            if (InStream.EndOfStream) return -1;
            StringRead = InStream.ReadLine();
            return SplitAndClean(StringRead, Separator, out Pieces);
        }

        public static void SkipFileLines(StreamReader InStream, int HowManyToSkip)
        {
            for (int i = 0; i < HowManyToSkip; i++)
                InStream.ReadLine();
        }

        public static void CloseStream(StreamReader InStream)
        {
            if (InStream == null) return;
            InStream.Close();
            InStream.Dispose();
            InStream = null;
        }

        #endregion

        #region StreamWriter Utilities
        public static void WriteSeparatedLine(StreamWriter Wtr, string LinePrefix, string[] Items, char Separator)
        {
            Wtr.Write(LinePrefix);
            for (int i = 0; i < Items.Length - 1; i++)
                Wtr.Write(Items[i] + Separator + " ");
            Wtr.WriteLine(Items[Items.Length - 1]);
        }

        public static void CloseStream(StreamWriter OutStream)
        {
            if (OutStream == null) return;
            OutStream.Flush();
            OutStream.Close();
            OutStream.Dispose();
            OutStream = null;
        }
        #endregion

        #region File System

        public static int CheckFileStatus(string FileName, string Operation)
        {
            const int OK = 0;
            const int NotFound = -1;
            const int InUse = -2;
            const int BadOperation = -10;

            Operation = Operation.ToUpper().Trim();
            switch (Operation)
            {
                case "READ":
                    if (!File.Exists(FileName)) return NotFound;
                    StreamReader Sr = null;
                    try
                    {
                        Sr = new StreamReader(FileName);
                    }
                    catch
                    {
                        return InUse;
                    }
                    if (Sr == null) return InUse;
                    return OK;
                default:
                    return BadOperation;
            }
        }

        public static bool FilePathOK(string FilePath)
        {
            //If it exists, it must be OK
            if (File.Exists(FilePath)) return true;

            //If it doesn't exist, try to open it and see what happens
            StreamReader R = null;
            bool Err = false;
            try
            {
                R = new StreamReader(FilePath);
            }
            catch
            {
                Err = true;
            }
            finally
            {
                if (R != null)
                {
                    R.Close();
                    R.Dispose();
                    R = null;
                }
            }

            return Err;
        }

        public static string GetTempFileName()
        {
            string NumStr = null;
            Random Ran = new Random();
            for (; ; )
            {
                int Number = Ran.Next(int.MaxValue);
                NumStr = Path.GetTempPath() + Number.ToString() + ".tmp";
                if (!File.Exists(NumStr)) return NumStr;
            }
        }

        public static void AddDirSlash(ref string DirPath)
        {
            if (DirPath.EndsWith("\\")) return;
            DirPath += "\\";
        }

        public static void DelDirSlash(ref string DirPath)
        {
            if (DirPath.EndsWith("\\"))
                DirPath = DirPath.Substring(0, DirPath.Length - 1);
        }

        public static void FileDelete(string FilePath)
        {
            if (File.Exists(FilePath)) File.Delete(FilePath);
        }

        public static void DirDelete(string DirPath)
        {
            if (Directory.Exists(DirPath))
                Directory.Delete(DirPath, true);
        }

        public static void DeleteAllFiles(string DirPath)
        {
            string[] Str = Directory.GetFiles(DirPath);
            foreach (string S in Str)
                File.Delete(S);
        }


        #endregion

        #region Precise Conversion Checks
        public static bool PreciseConversion(long FromLongValue, out double ToDoubleValue)
        {
            ToDoubleValue = FromLongValue;
            long NewLongValue = (long)ToDoubleValue;
            long Diff = (NewLongValue - FromLongValue);
            if (Diff == 0)
                return true;
            else
                return false;
        }

        public static bool PreciseConversion(uint FromLongValue, out double ToDoubleValue)
        {
            ToDoubleValue = FromLongValue;
            uint NewLongValue = (uint)ToDoubleValue;
            int Diff = (int)(NewLongValue - FromLongValue);
            if (Diff == 0)
                return true;
            else
                return false;
        }


        public static bool PreciseConversion(ulong FromLongValue, out double ToDoubleValue)
        {
            ToDoubleValue = FromLongValue;
            ulong NewLongValue = (ulong)ToDoubleValue;
            long Diff = (long)(NewLongValue - FromLongValue);
            if (Diff == 0)
                return true;
            else
                return false;

        }

        #endregion

        #region Misc
        /// <summary>
        /// Return a language independent cr/newline character
        /// </summary>
        /// <returns>The cr/newline character sequence as a string</returns>
        public static string NewLine()
        {
            return "/n";
        }

        /// <summary>
        /// Quote a string i.e., pass Junk and it returns "Junk"
        /// </summary>
        /// <param name="Str">What you want in quotes</param>
        /// <returns>The string in quotes</returns>
        public static string Quote(string Str)
        {
            QuoteStr(ref Str);
            return Str;
        }

        /// <summary>
        /// Quote a string, i.e., pass Junk and it returns "Junk"
        /// </summary>
        /// <param name="Str">The string you want in quotes</param>
        public static void QuoteStr(ref string Str)
        {
            string Quotes = ((char)34).ToString();
            Str = Quotes + Str + Quotes;
        }




        #endregion

        #region Textbox Keystroke Examiner for Number Entry (a class)
        //A class to check characters passed to text boxes.
        //Intended to be used by GUI programs
        //Does not do scientific notation.
        public class NumberChecker
        {
            public bool EditChar { get; set; }
            public bool NumberChar { get; set; }
            public bool MinusChar { get; set; }
            public bool ReturnChar { get; set; }
            public int KeyCode { get; set; }
            public bool DecimalChar { get; set; }
            public bool UnrecogChar { get; set; }

            public int CheckChar(string BoxText, KeyEventArgs e)
            {
                EditChar = false;
                NumberChar = false;
                MinusChar = false;
                ReturnChar = false;
                DecimalChar = false;
                UnrecogChar = true;

                KeyCode = (int)e.KeyCode;
                switch (KeyCode)
                {
                    case 13:  //Return
                        {
                            ReturnChar = true;
                            UnrecogChar = false;
                            break;
                        }
                    case 8:  //Backspace
                    case 37: //Arrow back
                    case 39: //Arrow forward
                    case 46: //Delete
                        {
                            EditChar = true;
                            UnrecogChar = false;
                            break;
                        }
                    case 96:  //Number pad 0
                    case 48:  //0
                    case 97:  //Number pad 1
                    case 49:  //1
                    case 98:  //Number pad 2
                    case 50:  //2
                    case 99:  //Number pad 3
                    case 51:  //3
                    case 100: //Number pad 4
                    case 52:  //4
                    case 101: //Number pad 5
                    case 53:  //5
                    case 102: //Number pad 6
                    case 54:  //6
                    case 103: //Number pad 7
                    case 55:  //7
                    case 104: //Number pad 8
                    case 56:  //8
                    case 105: //Number pad 9
                    case 57:  //9
                        NumberChar = true;
                        UnrecogChar = false;
                        break;
                    case 109: //Number pad minus
                    case 189: //minus
                        BoxText = BoxText.Trim();
                        if (BoxText == string.Empty)
                        {
                            MinusChar = true;
                            UnrecogChar = false;
                        }
                        else
                        {
                            MinusChar = false;
                            UnrecogChar = true;
                        }
                        break;
                    case 110: //Number pad decimal
                    case 190: //decimal
                        if (BoxText.Contains('.'))
                        {
                            DecimalChar = false;
                            UnrecogChar = true;
                        }
                        else
                        {
                            DecimalChar = true;
                            UnrecogChar = false;
                        }
                        break;
                    default:
                        UnrecogChar = true;
                        break;
                }

                return KeyCode;
            }
        }

        #endregion
    }



}
