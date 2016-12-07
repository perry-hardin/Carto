using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Carto
{

    public class Ceptometer
    {
        private static double DegToRad(double X)
        {
            return X * Const.DEG2RAD;
        }


        //***** Decagon routines **********************************************
        public static double Equation13(double a, double fb, double Zenith, double Chi, double LAI)
        {
            double K = CalcK(Zenith, Chi);
            double A = 0.283 + 0.785 * a - 0.159 * a * a;
            double T1 = A * (1.0 - 0.47 * fb) * LAI;
            double T2 = (1.0 - 1.0 / (2.0 * K)) * fb - 1.0;
            double Tau = Math.Exp(T1 / T2);
            return Tau;
        }

        private static double CalcK(double SolarZenith, double Chi)
        {
            SolarZenith = DegToRad(SolarZenith);
            double K;

            if (Chi == 1.0)
            {
                K = 1.0 / (2.0 * Math.Cos(SolarZenith));
                return K;
            }

            double Top = Chi * Chi + Math.Tan(SolarZenith) * Math.Tan(SolarZenith);
            Top = Math.Sqrt(Top);
            double Bottom = Chi + 1.182;
            Bottom = Math.Pow(Bottom, -0.733);
            Bottom = Bottom * 1.744;
            Bottom = Chi + Bottom;
            K = Top / Bottom;
            return K;
        }

        public static double CalcTau(double AbovePar, double BelowPar)
        {
            return BelowPar / AbovePar;
        }

        //See Decagon book page 48, below Equation 13
        private static double CalcA(double a)
        {
            return 0.283 + 0.785 * a - 0.159 * a * a;
        }

        //See Decagon application note entitled "Beam fraction calculation in the LP80."
        public static double CalcFb(double AbovePar, double SolarZenith)
        {
            SolarZenith = DegToRad(SolarZenith);
            double R = AbovePar / (2550.0 * Math.Cos(SolarZenith));
            if (R > 0.82) R = 0.82;
            if (R < 0.2) R = 0.2;
            double T = 1.395 + R * (-14.43 + R * (48.57 + R * (-59.024 + R * 24.835)));
            return T;
        }

        //The inversion of equation 13.  See AccuPar book page 49
        public static double CalcLAI(double fb,
                                   double BelowPar,
                                   double AbovePar,
                                   double SolarZenith,
                                   double Chi,
                                   double a)
        {
            double tau = CalcTau(AbovePar, BelowPar);
            double K = CalcK(SolarZenith, Chi);
            double A = CalcA(a);

            double t = 1 / (2 * K);
            t = 1.0 - t;
            t = t * fb;
            t = t - 1.0;
            t = t * Math.Log(tau);

            double b = 0.47 * fb;
            b = 1.0 - b;
            b = A * b;

            return t / b;

        }



        //***** Norman and Campbell Chapter 14 programs ***********************
    //    public static void Inversion(double[] ZenithAngle,  //Zenith angles of measurements in degrees
    //                                 double[] Transmission, //Transmission at those zenith angles
    //                                 double Constraint,     //Constraint value, close to zero
    //                                 int NumLeafAngles,     //Number of leaf angles
    //                                 out double[] LAI,      //LAI at set leaf angles
    //                                 out double[] LeafAngle, //The leaf angles associated with the LAI
    //                                 out double SumLAI)      //Total LAI
    //    {
    //        double Temp;
    //        double GA = Constraint;
    //        int NA = NumLeafAngles;
    //        int NZ = ZenithAngle.GetLength(0);
    //        MathVector Z = new MathVector(1, NZ);
    //        MathVector T = new MathVector(1, NZ);
    //        MathVector X = new MathVector(1, NA);
    //        MathMatrix G = new MathMatrix(1, NZ, NA);
    //        MathMatrix A = new MathMatrix(1, NA, NA);
    //        MathMatrix W = new MathMatrix(1, NA, NA);
    //        MathVector B = new MathVector(1, NA);
    //        MathVector C = new MathVector(1, NA);
    //        Z.FromArray(ZenithAngle);
    //        T.FromArray(Transmission);
    //        for (int i = 1; i <= NZ; i++)
    //        {
    //            Z.Transform(i, TrigFunction.DegToRad);
    //            T.Transform(i, MathFunction.NegLog);
    //        }

    //        //Setup kernel matrix
    //        for (int i = 1; i <= NA; i++)
    //        {
    //            double LI = (i - 0.5) * Math.PI / (2.0 * NA);
    //            for (int j = 1; j <= NZ; j++)
    //            {
    //                double KB;
    //                double ZA = Z.Get(j);
    //                if (LI < (Math.PI / 2.0 - ZA + 0.01))
    //                    KB = Math.Cos(LI);
    //                else
    //                {
    //                    Temp = Math.Tan(LI) * Math.Tan(ZA);
    //                    Temp = Temp * Temp;
    //                    double TB = Math.Sqrt(Temp - 1.0);
    //                    double BB = Math.Atan(TB);
    //                    KB = Math.Cos(LI) * (1.0 + 2.0 * (TB - BB) / Math.PI);
    //                }
    //                G.Set(j, i, KB);
    //            }
    //        }

    //        //GT * G and GT * D
    //        C.Zero();
    //        A.Zero();
    //        for (int i = 1; i <= NA; i++)
    //            for (int j = 1; j <= NA; j++)
    //            {
    //                for (int k = 1; k <= NZ; k++) A.Add(i, j, G.Get(k, i) * G.Get(k, j));
    //                A.Reflect(i, j);
    //            }            
            
    //        for (int i = 1; i <= NA; i++)
    //            for (int k = 1; k <= NZ; k++) 
    //                C.Add(i, G.Get(k, i) * T.Get(k)); 
            
    //        //Add the constraint matrix
    //        for (int i = 1; i <= NA; i++)
    //        {
    //            B.CopyFrom(C, i);
    //            for (int j = 1; j <= NA; j++) W.CopyFrom(A, i, j);
    //        }

    //        for (int i = 2; i <= NA - 1; i++)
    //        {
    //            W.Add(i, i, 2.0 * GA); 
    //            W.Subtract(i, i - 1, GA);               
    //            W.Subtract(i, i + 1, GA);
    //        }

    //        W.Add(1, 1, GA);
    //        W.Subtract(1, 2, GA);
    //        W.Subtract(NA, NA - 1, GA);
    //        W.Add(NA, NA, GA);
           
    //        //Solve for the LAIs
    //        for (int j = 1; j <= NA - 1; j++)
    //        {
    //            double Pivot = W.Get(j, j);
    //            for (int i = j + 1; i <= NA; i++)
    //            {
    //                double Mult = W.Get(i, j) / Pivot;
    //                for (int k = j + 1; k <= NA; k++)
    //                {
    //                    W.Subtract(i, k, Mult * W.Get(j, k));
    //                    B.Subtract(i, Mult * B.Get(j));
    //                }
    //            }
    //        }
    //        X.Set(NA, B.Get(NA) / W.Get(NA, NA));
    //        for (int i = NA - 1; i >= 1; i--)
    //        {
    //            double Top = B.Get(i);
    //            for (int k = i + 1; k <= NA; k++) Top = Top - W.Get(i, k) * X.Get(k);
    //            X.Set(i, Top / W.Get(i, i));
    //        }

    //        LeafAngle = new double[NA];
    //        SumLAI = X.Sum();
    //        for (int i = 1; i <= NA; i++) LeafAngle[i - 1] = (i - 0.5) * 90.0 / NA;
    //        LAI = X.ToArray();
    //    }
    }
}
