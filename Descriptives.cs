using System;


namespace Carto
{
    public class Descriptives
    {
        //Standard formula for calculating percentage error
        static public double ErrPercent(double FirstNum, double SecondNum)
        {
            double Temp = (FirstNum - SecondNum) / FirstNum;
            return Temp * 100.0;
        }

        //Return the Maximum value
        static public double Maximum(double [] X)
        {
            double Maxv = X[0];
            for (int i = 1; i < X.Length; i++)
            {
                double  Val = X[i];
                if (Val.CompareTo(Maxv) > 0) Maxv = Val;
            }
            return Maxv;
        }

        //Return the minimum value 
        static public double Minimum(double [] X)
        {
            double Minv = X[0];
            for (int i = 1; i < X.Length; i++)
            {
                double Val = X[i];
                if (Val.CompareTo(Minv) < 0) Minv = Val;
            }
            return Minv;
        }

        //Return the limits of the range
        static public void Limits(double [] X, out double Min, out double Max)
        {
            Min = Minimum(X);
            Max = Maximum(X);
        }

        //Return the range itself (Max - Min)
        static public double Range (double [] X)
        {
            double Min = Minimum(X);
            double Max = Maximum(X);
            return Max - Min;
        }

        static public double Average(double[] X)
        {
            int Count = X.GetLength(0);
            double Sum = 0.0;
            foreach (double Number in X)
            {
                Sum += Number;
            }

            if (Count == 0)
            {
                throw new Exception("Numerator in function Average is zero.");
            }

            return Sum / X.GetLength(0);
        }

        static public double Correlation(double[] X, double[] Y)
        {
            double Covar = PopCovariance(X, Y);
            double VarX = PopCovariance(X, X);
            double VarY = PopCovariance(Y, Y);
            return Covar / Math.Sqrt(VarX * VarY);
        }

        static public double SampleCovariance(double[] X, double[] Y)
        {
            double XMean = Average(X);
            double YMean = Average(Y);
            double RunSum = 0.0;
            for (int i = 0; i < X.Length; i++)
            {
                double XDiff = X[i] - XMean;
                double YDiff = Y[i] - YMean;
                RunSum += (XDiff * YDiff);
            }
            return RunSum / (X.Length - 1);
        }


        static public double PopCovariance(double[] X, double[] Y)
        {
          double XMean  = Average(X);
          double YMean = Average(Y);
          double RunSum = 0.0;
          for (int i = 0; i < X.Length; i++) {
              double XDiff = X[i] - XMean;
              double YDiff = Y[i] - YMean;
              RunSum += (XDiff * YDiff);
          }
            return RunSum / X.Length;
        }

     
        static public double PopStdv(double[] X)
        {
            double Avg = Average(X);
            double Diff;
            double DiffSq;
            double SumDiffSq = 0.0;
            foreach (double Number in X)
            {
                Diff = Avg - Number;
                DiffSq = Diff * Diff;
                SumDiffSq += DiffSq;
            }
            double Quotient = SumDiffSq / X.GetLength(0);
            return Math.Sqrt(Quotient);
        }


        static public void SqDiff(double[] X, double[] SqDiff)
        {
            double Mean = Average(X);
            int ItemCount = X.Length;
            SqDiff = new double[ItemCount];

            for (int i = 0; i < ItemCount; i++)
            {
                double Diff = X[i] - Mean;
                SqDiff[i] = Diff * Diff;
            }
        }


        static public double Median(double[] X)
        {
            int SlotCount = X.GetLength(0);
            double[] XX;
            Sort(X, out XX);

            if (Utils.IsOdd(SlotCount))
            {
                return XX[SlotCount / 2];
            }
            else
            {
                int HiIdx = SlotCount / 2;
                int LoIdx = HiIdx - 1;
                return (XX[HiIdx] + X[LoIdx]) / 2.0;
            }
        }


        static public void Sort(double [] X, out double[] SortedX)
        {
            int Count = X.GetLength(0);
            SortedX = new double[Count];
            Array.Copy(X, SortedX, Count);
            Array.Sort(SortedX);
        }


        static public void Sort(float[] X, out float[] SortedX)
        {
            int Count = X.GetLength(0);
            SortedX = new float[Count];
            Array.Copy(X, SortedX, Count);
            Array.Sort(SortedX);
        }



        static public double Max(ref double[] X)
        {
            double Max = X[0];
            foreach (double Value in X)
            {
                if (Value > Max) Max = Value;
            }
            return Max;
        }


    }
}
