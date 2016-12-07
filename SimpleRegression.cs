using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carto
{
    public static class SimpleRegression
    {
        //A simple function to do a linear regression.  Example with a little data set follows.

        static public void Calculate(List<double> X, List<double> Y, out double Intercept, out double Slope, out double R2)
        {
            double[] XX = new double[X.Count];
            double[] YY = new double[Y.Count];
            for (int i = 0; i < X.Count; i++)
            {
                XX[i] = X[i];
                YY[i] = Y[i];
            }
            Calculate(XX, YY, out Intercept, out Slope, out R2);
        }


        //double Intercept, Slope, R2;
        //double[] X = new double[] { 14, 17, 18, 5, -3, 25 };
        //double[] Y = new double[] { 62.1, 69.5, 73.6, 37.2, 31.1, 88.0 };
        //Correlation.Regression(X, Y, out Intercept, out Slope, out R2);
        //Results are 32.94314, 2.15581, and 0.971153 for Intercept, Slope, and R2 respectively
        static public void Calculate(double[] X, double[] Y, out double Intercept, out double Slope, out double R2)
        {
            //Check to see if the array is big enough to do a regression problem
            int NumDataPairs = X.GetLength(0);
            if (NumDataPairs < 2)
            {
                Slope = -999.99;
                Intercept = -999.99;
                R2 = -999.99;
                return;
            }

            double SumX = 0.0;
            double SumY = 0.0;
            double SumXY = 0.0;
            double SumXSq = 0.0;
            double SumYSq = 0.0;
            for (int i = 0; i < NumDataPairs; i++)
            {
                double XVal = X[i];
                double YVal = Y[i];
                SumX = SumX + XVal;
                SumY = SumY + YVal;
                SumXY = SumXY + XVal * YVal;
                SumXSq = SumXSq + XVal * XVal;
                SumYSq = SumYSq + YVal * YVal;
            }

            //Calc slope and intercept
            double Denom = SumXSq - SumX * SumX / NumDataPairs;
            if (Denom == 0.0)
            {
                Slope = -999.99;
                Intercept = -999.99;
                R2 = -999.99;
                return;
            }
            Slope = (SumXY - SumX * SumY / NumDataPairs) / Denom;
            Intercept = (SumY - Slope * SumX) / NumDataPairs;

            //Calc correlation coeff
            Denom = SumYSq - SumY * SumY / NumDataPairs;
            if (Denom == 0.0)
            {
                R2 = 1.0;

            }
            else
            {
                R2 = Slope * (SumXY - SumX * SumY / NumDataPairs) / Denom;
            }
        }




    }
}
