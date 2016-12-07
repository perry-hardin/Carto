using System;

namespace Carto { 
public static class MyMath
    {
        /// <summary>
        /// Convert degrees to radians
        /// </summary>
        /// <param name="Degrees">The degree value to convert</param>
        /// <returns>The radian equivalent to the degrees value</returns>
        public static double DegToRad(double Degrees)
        {
            double Answer = Degrees * Const.DEG2RAD;
            return Answer;
        }

        /// <summary>
        /// Convert radians to degrees
        /// </summary>
        /// <param name="Radians">The radian value to convert</param>
        /// <returns>The degree equivalent to the radian value </returns>
        public static double RadToDeg(double Radians)
        {
            double Answer = Radians * Const.RAD2DEG;
            return Answer;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Degrees"></param>
        /// <param name="Minutes"></param>
        /// <param name="Seconds"></param>
        /// <returns></returns>
        public static double DMSToDecimal(int Degrees, int Minutes, double Seconds)
        {
            bool Negative = false;
            if (Degrees < 0) Negative = true;
            double AbsDeg = Math.Abs(Degrees);
            double Answer = Degrees + Minutes / 60.0 + Seconds / 3600.0;
            if (Negative) Answer = 0.0 - Answer;
            return Answer;
        }


       

        public static void DecimalToDMS(double DecDegrees,
                                        out int Degrees,
                                        out int Minutes,
                                        out double Seconds)
        {
            bool Negative = false;
            if (DecDegrees < 0) Negative = true;
            DecDegrees = Math.Abs(DecDegrees);
            Degrees = (int)DecDegrees;
            double Remainder = DecDegrees - Degrees;
            double Min = Remainder * 60.0;
            Minutes = (int)Min;
            Remainder = Min - Minutes;
            Seconds = Remainder * 60.0;
            if (Negative) Degrees = 0 - Degrees;
        }


        public static double PlaneDistance(double X1, double Y1,
                                           double X2, double Y2) 
        {
            double DiffX = X1 - X2;
            double DiffY = Y1 - Y2;
            double DiffX2 = DiffX * DiffX;
            double DiffY2 = DiffY * DiffY;
            double Sum = DiffX2 + DiffY2;
            double Answer = Math.Sqrt(Sum);
            return Answer;
        }

        public static double PlaneDistance(XYPoint P1, XYPoint P2)
        {
            double Answer = PlaneDistance(P1.X, P1.Y, P2.X, P2.Y);
            return Answer;
        }

        public static double PlaneDistance(CivilUTM P1, CivilUTM P2)
        {
            //if (P1.ZoneLetter != P2.ZoneLetter) raise an exception
            //if (P1.ZoneNumber != P2.ZoneNumber) raise an Exception
            double Answer = PlaneDistance(P1.Easting, P1.Northing,
                                          P2.Easting, P2.Northing);
            return Answer;          
        }


        public static void LegalHemis(TypeHemiSphere HemiSphere, 
                                 out char Hemi1,
                                 out char Hemi2) {
                                     if (HemiSphere == TypeHemiSphere.LATITUDE)
                                     {
                                         Hemi1 = 'N';
                                         Hemi2 = 'S';
                                     }
                                     else
                                     {
                                         Hemi1 = 'E';
                                         Hemi2 = 'W';
                                     }
        
        }

#region Power functions
 public static double Iterative(double X, uint Exponent)
        {
            if (Exponent == 0) return 1.0;
            double Product = X;
            for (uint i = 1; i < Exponent; i++) Product *= X;
            return Product;
        }

        public static double Recursive(double X, int Exponent)
        {
            if (Exponent == 0)
                return 1.0;
            else if (Exponent == 1)
                return X;
            else
                return X * Recursive(X, Exponent - 1);
        }

        public static double Pow2(double X)
        {
            return X * X;
        }

        public static double Pow3(double X)
        {
            return X * X * X;
        }

        public static double Pow4(double X)
        {
            return X * X * X * X;
        }

        public static double Pow5(double X)
        {
            return X * X * X * X * X;
        }

        public static double Pow6(double X)
        {
            return X * X * X * X * X * X;
        }

        public static double Pow7(double X)
        {
            return X * X * X * X * X * X * X;
        }

        public static double Pow8(double X)
        {
            return X * X * X * X * X * X * X * X;
        }

        public static double Pow9(double X)
        {
            return X * X * X * X * X * X * X * X * X;
        }

        public static double Pow10(double X)
        {
            return X * X * X * X * X * X * X * X * X * X;
        }

        public static double Pow25(double X)
        {
            return X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X;
        }

        public static double Pow(double X, uint Exponent)
        {
            switch (Exponent)
            {
                case 00: return 1;
                case 01: return X;
                case 02: return X * X;
                case 03: return X * X * X;
                case 04: return X * X * X * X;
                case 05: return X * X * X * X * X;
                case 06: return X * X * X * X * X * X;
                case 07: return X * X * X * X * X * X * X;
                case 08: return X * X * X * X * X * X * X * X;
                case 09: return X * X * X * X * X * X * X * X * X;
                case 10: return X * X * X * X * X * X * X * X * X * X;
                case 11: return X * X * X * X * X * X * X * X * X * X * X;
                case 12: return X * X * X * X * X * X * X * X * X * X * X * X;
                case 13: return X * X * X * X * X * X * X * X * X * X * X * X * X;
                case 14: return X * X * X * X * X * X * X * X * X * X * X * X * X * X;
                case 15: return X * X * X * X * X * X * X * X * X * X * X * X * X * X * X;
                case 16: return X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X;
                case 17: return X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X;
                case 18: return X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X;
                case 19: return X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X;
                case 20: return X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X;
                case 21: return X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X;
                case 22: return X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X;
                case 23: return X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X;
                case 24: return X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X;
                case 25: return X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X * X;
                default: return Math.Pow(X, Exponent);
            }
        }
#endregion


    }
}
