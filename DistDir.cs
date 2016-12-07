using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace Carto
{

    public static class Distance
    {

        public static double Planar(double[] P1, double[] P2, bool Squared = false)
        {
            if (P1.Length != P2.Length)
                throw new Exception("Distance.Planar reports that length of P1 and P2 are not the same");
            double Sum = 0.0;
            for (int i = 0; i < P1.Length; i++)
            {
                double Diff = P1[i] - P2[i];
                Sum = Sum + Diff * Diff;
            }
            if (Squared)
                return Sum;
            else
                return Math.Sqrt(Sum);
        }

        public static double Planar(double X1, double Y1, double X2, double Y2)
        {
            double XDiff = X1 - X2;
            double YDiff = Y1 - Y2;
            XDiff = XDiff * XDiff;
            YDiff = YDiff * YDiff;
            return Math.Sqrt(XDiff + YDiff);
        }

        public static double Planar(Carto.CivilUTM P1, Carto.CivilUTM P2)
        {
            if (P1.ZoneNumber != P2.ZoneNumber)
                throw new Exception("Distance.Planar reports that points one and two must have same grid zone number.");
            double D = Planar(P1.Easting, P1.Northing, P2.Easting, P2.Northing);
            return D;
        }


        public static Carto.XYPoint Project(double X1, double Y1, double Angle, double Distance)
        {
            Carto.XYPoint Answer = new Carto.XYPoint();
            Angle = Angle * Carto.Const.DEG2RAD;
            double DeltaX = Math.Cos(Angle) * Distance;
            double DeltaY = Math.Sin(Angle) * Distance;
            Answer.X = X1 + DeltaX;
            Answer.Y = Y1 + DeltaY;
            return Answer;
        }

        /// <summary>
        /// Go from a pair of latitude and longitude points to distance. Vincenty's Formulae. Source: http://www.movable-type.co.uk/scripts/latlong-vincenty.html
        /// </summary>
        /// <param name="Lat1"></param>
        /// <param name="Lon1"></param>
        /// <param name="Lat2"></param>
        /// <param name="Lon2"></param>
        /// <returns>Meters</returns>
        public static double Ellipsoidal(double Lat1, double Lon1, double Lat2, double Lon2, double a, double b, double f)
        {
            const double accuracy = 1e-12;
            const double DegToRad = Math.PI / 180.0;
            double L = (Lon2 - Lon1) * DegToRad;
            double U1 = Math.Atan((1 - f) * Math.Tan(Lat1 * DegToRad));
            double U2 = Math.Atan((1 - f) * Math.Tan(Lat2 * DegToRad));
            double sinU1 = Math.Sin(U1);
            double sinU2 = Math.Sin(U2);
            double cosU1 = Math.Cos(U1);
            double cosU2 = Math.Cos(U2);
            double lambda = L;
            double lambdaP = lambda;
            double sinLambda, cosLambda, sinSigma, cosSigma, sigma, sinAlpha, cosSqAlpha, cos2SigmaM, C;
            do
            {
                sinLambda = Math.Sin(lambda);
                cosLambda = Math.Cos(lambda);
                sinSigma = Math.Sqrt((cosU2 * sinLambda) * (cosU2 * sinLambda) + (cosU1 * sinU2 - sinU1 * cosU2 * cosLambda) * (cosU1 * sinU2 - sinU1 * cosU2 * cosLambda));
                if (sinSigma == 0) return 0;    //co-incident points
                cosSigma = sinU1 * sinU2 + cosU1 * cosU2 * cosLambda;
                sigma = Math.Atan2(sinSigma, cosSigma);
                sinAlpha = cosU1 * cosU2 * sinLambda / sinSigma;
                cosSqAlpha = 1 - sinAlpha * sinAlpha;
                cos2SigmaM = cosSigma - 2 * sinU1 * sinU2 / cosSqAlpha;
                if (double.IsNaN(cos2SigmaM)) cos2SigmaM = 0;   //equatorial line
                C = (f / 16.0) * cosSqAlpha * (4 + f * (4 - 3 * cosSqAlpha));
                lambdaP = lambda;
                lambda = L + (1 - C) * f * sinAlpha * (sigma + C * sinSigma * (cos2SigmaM + C * cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM)));
            }
            while (Math.Abs(lambda - lambdaP) > accuracy);

            double uSq = cosSqAlpha * (a * a - b * b) / (b * b);
            double A = 1 + uSq / 16384 * (4096 + uSq * (-768 + uSq * (320 - 175 * uSq)));
            double B = uSq / 1024 * (256 + uSq * (-128 + uSq * (74 - 47 * uSq)));
            double deltaSigma = B * sinSigma * (cos2SigmaM + (B / 4) * (cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM) - (B / 6) * cos2SigmaM * (-3 + 4 * sinSigma * sinSigma) * (-3 + 4 * cos2SigmaM * cos2SigmaM)));
            double s = b * A * (sigma - deltaSigma);

            return s;
        }
    }


    public static class Direction
    {
        public static double Planar(double X1, double Y1, double X2, double Y2)
        {
            double xDiff = X2 - X1;
            double yDiff = Y2 - Y1;
            double Angle = Math.Atan(yDiff / xDiff) * Carto.Const.RAD2DEG;
            if ((xDiff > 0) && (yDiff > 0))
            {
                Angle = 90.0 - Angle;
            }
            else if ((xDiff < 0) && (yDiff < 0))
            {
                Angle = 270.0 - Angle;
            }
            else if ((xDiff > 0) && (yDiff < 0))
            {
                Angle = 90.0 - Angle;
            }
            else if ((xDiff < 0) && (yDiff > 0))
            {
                Angle = 270.0 - Angle;
            }
            else if ((xDiff > 0) && (yDiff == 0))
            {
                Angle = 90.0;
            }
            else if ((xDiff < 0) && (yDiff == 0))
            {
                Angle = 270.0;
            }
            else if ((xDiff == 0) && (yDiff > 0))
            {
                Angle = 0.0;
            }
            else if ((xDiff == 0) && (yDiff < 0))
            {
                Angle = 180.0;
            }
            return Angle;
        }


    }


}
