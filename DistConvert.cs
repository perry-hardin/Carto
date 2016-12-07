using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Carto
{
    //Class to do distance conversions
    public class DistConvert
    {
        //Supported ConvertUnits in various forms
        
        private string[] UNITSTR = new string[] { "Millimeters", "Centimeters", "Inches", "Feet", "Yards", "Meters", "Kilometers", "Miles" };
        private string[] UNITSTRU = new string[] { "MILLIMETERS", "CENTIMETERS", "INCHES", "FEET", "YARDS", "METERS", "KILOMETERS", "MILES" };

        //Factors to convert to meters in same order as supported unit lists above
        private double[] TOMETERS = new double[] {double.NegativeInfinity, 0.001,0.01,0.0254,0.3048,0.9144,1.0,1000.0,1609.344};
       
        //Constructor 
        public DistConvert(){ }

        //Cleanup routine
        public void Cleanup()
        {
            if (UNITSTR != null) UNITSTR = null;
            if (UNITSTRU != null) UNITSTRU = null;
            if (TOMETERS != null) TOMETERS = null;
        }

        //Base function to do conversion given a number to convert, the input units, the output units
        public double Convert(double InNumber, ConvertUnits InUnits, ConvertUnits OutUnits) 
        {
            double Temp = InNumber * TOMETERS[(int) InUnits];  //Convert input ConvertUnits to meters
            return Temp / TOMETERS[(int)OutUnits];  //Convert meters to output units
        }

        //Alternative form to do conversion, takes strings.  Returns negative infinity if bad input,
        //  probably an unsupported unit type for either In or Out ConvertUnits.
        public double Convert(double Number, string InUnitStr, string OutUnitStr)
        {
            ConvertUnits InUnit; 
            StringToUnits(ref InUnitStr, out InUnit);
            if (InUnit == ConvertUnits.UNKNOWN) return double.NegativeInfinity;

            ConvertUnits OutUnit; 
            StringToUnits(ref OutUnitStr, out OutUnit);
            if (OutUnit == ConvertUnits.UNKNOWN) return double.NegativeInfinity;

            return Convert(Number, InUnit, OutUnit);
        }

        //Ancillary public function, returns boolean whether unit requested is supported
        public bool UnitsAreSupported(ref string UnitsStr)
        {
            ConvertUnits StdUnitDesignation;
            return StringToUnits(ref UnitsStr, out StdUnitDesignation );
        }

        //Ancillary public function.  Return all ConvertUnits supported in mixed case form
        public string[] SupportedUnits()
        {
            string[] Temp = new string[UNITSTR.Length];
            Array.Copy(UNITSTR, Temp, UNITSTR.Length);
            return Temp;
        }

        //Returns true if unit is supported, false if not
        //in out variable Unit, returns the standardize enumeration form of the unit input
        public bool StringToUnits(ref string UnitStr, out ConvertUnits Unit) 
        {
            string TheUnits = UnitStr.ToUpper().Trim();
            switch (TheUnits)
            {
                case "MM":
                case "MMS":
                case "MILLIMETER":
                case "MILLIMETERS":
                    Unit = ConvertUnits.MM;
                    break;
                case "CM":
                case "CMS":
                case "CENTIMETER":
                case "CENTIMETERS":
                    Unit = ConvertUnits.CM;
                    break;
                case "IN":
                case "INS":
                case "INCH":
                case "INCHES":
                    Unit = ConvertUnits.IN;
                    break;
                case "FT":
                case "FOOT":
                case "FEET":
                    Unit = ConvertUnits.FT;
                    break;
                case "YD":
                case "YDS":
                case "YARD":
                case "YARDS":
                    Unit = ConvertUnits.YD;
                    break;
                case "M":
                case "METER":
                case "METERS":
                case "MTRS":
                    Unit = ConvertUnits.M;
                    break;
                case "KM":
                case "KMS":
                case "KILOMETER":
                case "KILOMETERS":
                    Unit = ConvertUnits.KM;
                    break;
                case "MILE":
                case "MILES":
                    Unit = ConvertUnits.MI;
                    break;
                default:
                    Unit = ConvertUnits.UNKNOWN;
                    return false; //String was not understood as a length unit
            }
            return true;
        }
   
    }
}
