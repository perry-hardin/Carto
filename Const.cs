using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Carto
{
    /// <summary>
    /// A set of public constants for cartography,
    /// remote sensing, GIS, and GPS
    /// Author: Jake
    /// </summary>
    public static class Const
    {

        //Need to capitalize these constants
        #region Character constants
        public const char CharCR = (char)13;
        public const char CharLF = (char)10;
        public const int ValCR = 13;
        public const int ValLF = 10;
        public const string StrCRLF = "\r\n";
        public const char CharSpace = ' ';
        public const string StrSpace = " ";
        public const int ValSpace = ((int)CharSpace);
        public const char CharPeriod = '.';
        public const char CharFullStop = CharPeriod;
        public const char CharQuestionMark = '?';
        public const char CharExclamationPoint = '!';
        public const char CharQuotes = '\"';
        public const char CharApostrophe = '\'';
        public const char CharNull = (char)0;
        public const char CharComma = ',';
        public const char CharDash = '-';
        public const char CharTab = (char)9;
        public const char CharAccent = (char)96;
        public const char CharOpenCurlyBracket = '{';
        public const char CharCloseCurlyBracket = '}';
        public const char CharOpenParen = '(';
        public const char CharCloseParen = ')';
        public const char CharOpenSquareBracket = '[';
        public const char CharCloseSquareBracket = ']';
        #endregion

      //  #region Sentence ending constants and constant arrays
      //  public static char[] SentenceStop = new char[] { Research.Constant.CharPeriod,
      //  Research.Constant.CharQuestionMark, Research.Constant.CharExclamationPoint };
      //  public static char[] SentenceEndChar = new char[] { Research.Constant.CharPeriod,
      //  Research.Constant.CharQuestionMark, Research.Constant.CharExclamationPoint,
      //  Research.Constant.CharQuotes, Research.Constant.CharApostrophe, Research.Constant.CharNull };

      //  //Don't use this, use the SentenceEnd version which is sorted and has no duplicates
      //  private static string[] UnsortedSentenceEnd = new string[] {
      //  ".", "?", "!", "?!","!?","!!", "??",".\"", "?\"","!\"","?!\"", "!?\"","!!\"", "??\"", ".\'","?\'","!\'", "?!\'","!?\'","!!\'","??\'",
      //  ".\'", "?\'","!\'","?!\'", "!?\'","!!\'", "??\'", ".\'","?\'","!\'", "?!\'","!?\'","!!\'","??\'", ".", "?", "!", "?!","!?","!!", "??",
      //  ".\'", "?\'","!\'","?!\'", "!?\'","!!\'", "??\'", ".\'","?\'","!\'", "?!\'","!?\'","!!\'","??\'", ".\'", "?\'","!\'","?!\'", "!?\'",
      //  "!!\'", "??\'", ".\'","?\'","!\'", "?!\'","!?\'","!!\'","??\'"};

      //  #region Sentence splitting characters (no strings)
      //  //Don't use this, use the SplitDelimiter version which is sorted and has no duplicates
      //  public static char[] UnsortedSplitDelimiters = new char[] {
      // '{', '}', '(', ')', '[', ']', '>', '<','-', '_', '=', '+',
      // '|', '\\', ':', ';', ' ', '\'', ',', '.', '/', '?', '~', '!',
      // '@', '#', '$', '%', '^', '&', '*', ' ', '\r', '\n', '\t'};


      //  public static char[] SplitDelimiter = new char[] {
      //(char)9,(char)10,(char)13,(char)32,(char)33,(char)35,(char)36,(char)37,(char)38,(char)39,
      //(char)40,(char)41,(char)42,(char)43,(char)44,(char)45,(char)46,(char)47,(char)58,(char)59,
      //(char)60,(char)61,(char)62,(char)63,(char)64,(char)91,(char)92,(char)93,(char)94,(char)95,
      //(char)123,(char)124,(char)125,(char)126};
      //  #endregion



      //  //Use this one, the sorted one
      //  public static string[] SentenceEnd = new string[] { "!", "!\'", "!!", "!!\'", "!!\"", "!\"", "!?", "!?\'", "!?\"", ".",
      // ".\'", ".\"", "?", "?\'", "?!", "?!\'", "?!\"", "?\"", "??", "??\'", "??\"" };

      //  /// <summary>
      //  /// This is for programmer use if s/he needs to add another something to SentenceEnd 
      //  /// array and needs to create the sorted version
      //  /// </summary>
      //  /// <param name="Unsorted"></param>
      //  private static void SortSentenceEnds(string[] Unsorted)
      //  {
      //      //Add each ending to a list, making sure it doesn't already exist
      //      List<string> L = new List<string>();
      //      foreach (string S in UnsortedSentenceEnd)
      //          if (!L.Contains(S)) L.Add(S);

      //      //Sort it and print it to console character by character
      //      //adding necessary control chars for cut-and-paste into
      //      //C# code file.
      //      L.Sort();
      //      foreach (string S in L)
      //      {
      //          Console.Write('\"');
      //          foreach (char c in S)
      //          {

      //              if (c == '\'')
      //                  Console.Write('\\'.ToString() + '\''.ToString());
      //              else if (c == '\"')
      //                  Console.Write('\\'.ToString() + '\"'.ToString());
      //              else
      //                  Console.Write(c);
      //          }
      //          Console.Write('\"' + ", ");
      //      }
      //  }

      //  #endregion

        #region Error condition indicators
        public const string STRUNKNOWN = "UNKNOWN";
        public const string STRNULL = null;
        public const double DBLUNSPECIFIED = double.MaxValue;
        public const int INTUNSPECIFIED = int.MaxValue;
        public const double DBLUNDEFINED = double.MaxValue;
        public const int INTUNDEFINED = int.MaxValue;
        public const long LONGUNDEFINED = long.MaxValue;
        public const string STRNODATA = "NODATA";
        public const string STRUNSPEC = STRUNKNOWN;
        public const int INTUNSPEC = int.MaxValue;
        public const double DBLUNSPEC = double.MaxValue;
        #endregion

        #region File extensions
        public const string RDC = ".rdc";
        public const string RST = ".rst";
        #endregion

        #region Geometry and trigonometry
        public const int LONGITUDE = 0;
        public const int LATITUDE = 1;
        public const int ALTITUDE = 2;
        public const int X = 0;
        public const int Y = 1;
        public const int Z = 2;
        public const int M = 3;
        public const double MAXLATITUDE = +90.0;
        public const double MINLATITUDE = -90.0;
        public const double MAXLONGITUDE = +180.0;
        public const double MINLONGITUDE = -180.0;
        public const double MINUTMLATITUDE = -80.0;
        public const double MAXUTMLATITUDE = +84.0;
        public const double PROVOLATITUTDE = 40.2338889;
        public const double PROVOLONGITUDE = -111.657777;
        #endregion

        #region Prefixes
        //public constants for prefixes
        public const double TERA = 1.0E+12;
        public const double GIGA = 1.0E+9;
        public const double MEGA = 1.0E+6;
        public const double KILO = 1.0E+3;
        public const double HECTO = 1.0E+2;
        public const double DEKA = 1.0E+1;
        public const double DECI = 1.0E-1;
        public const double CENTI = 1.0E-2;
        public const double MILLI = 1.0E-3;
        public const double MICRO = 1.0E-6;
        public const double NANO = 1.0E-9;
        public const double PICO = 1.0E-12;
        #endregion Prefixes

        #region Ellipsoids 
        public const double WGS84_EQUATORIAL = 6378137.0;
        public const double WGS84_POLAR = 6356752.3142;
        public const double WGS84_A = WGS84_EQUATORIAL;
        public const double WGS84_B = WGS84_POLAR;
        public const double WGS84_FLAT = 1.0 - WGS84_B / WGS84_A;
        public const double WGS84_INVFLAT = 1.0 / WGS84_FLAT;
        public const double WGS84_ESQ = (2.0 * WGS84_FLAT - WGS84_FLAT * WGS84_FLAT);

        public const double EARTHCIRCUMFERENCE = ((WGS84_A + WGS84_B) / 2.0) * 2.0 * Math.PI;

        public const double NAD27_EQUATORIAL = 6378206.40;
        public const double NAD27_POLAR = 6356583.80;
        public const double NAD27_A = NAD27_EQUATORIAL;
        public const double NAD27_B = NAD27_POLAR;
        public const double NAD27_FLAT = 1.0 - NAD27_B / NAD27_A;
        public const double NAD27_INVFLAT = 1.0 / NAD27_FLAT;
        public const double NAD27_ESQ = (2.0 * NAD27_FLAT - NAD27_FLAT * NAD27_FLAT);

        public const double INTL_EQUATORIAL = 6378388.00;
        public const double INTL_POLAR = 6356911.95;
        public const double INTL_A = INTL_EQUATORIAL;
        public const double INTL_B = INTL_POLAR;
        public const double INTL_FLAT = 1.0 - INTL_B / INTL_A;
        public const double INTL_INVFLAT = 1.0 / INTL_FLAT;
        public const double INTL_ESQ = (2.0 * INTL_FLAT - INTL_FLAT * INTL_FLAT);
        #endregion 

        #region Degrees to radians
        public const double DEG2RAD = Math.PI / 180.0;
        public const double RAD2DEG = 1.0 / DEG2RAD;
        #endregion

        #region Multipliers for length conversion

        //public constants for length conversion
        public const double ANGSTROM2NANOMETER = 0.1;
        public const double NANOMETER2ANGSTROM = 1.0 / ANGSTROM2NANOMETER;

        public const double ANGSTROM2METER = 1.0E-10;
        public const double METER2ANGSTROM = 1.0 / ANGSTROM2METER;

        public const double FATHOM2METER = 1.828804;
        public const double METER2FATHOM = 1.0 / FATHOM2METER;

        public const double FOOT2METER = 0.3048;
        public const double METER2FOOT = 1.0 / FOOT2METER;

        public const double SURVEYFOOT2METER = 0.3048006;
        public const double METER2SURVEYFOOT = 1.0 / SURVEYFOOT2METER;

        public const double INCH2CENTIMETER = 2.54;
        public const double CENTIMETER2INCH = 1.0 / INCH2CENTIMETER;

        public const double INCH2MILLIMETER = 25.4;
        public const double MILLIMETER2INCH = 1.0 / INCH2MILLIMETER;

        public const double MICROINCH2MICROMETER = 0.0254;
        public const double MICROMETER2MICROINCH = 1.0 / MICROINCH2MICROMETER;

        public const double MIL2MILLIMETER = 0.0254;
        public const double MILLIMETER2MIL = 1.0 / MIL2MILLIMETER;

        public const double MIL2MICROMETER = 25.4;
        public const double MICROMETER2MIL = 1.0 / MIL2MICROMETER;

        public const double YARD2METER = 0.9144;
        public const double METER2YARD = 1.0 / YARD2METER;

        public const double MILE2KILOMETER = 1.609344;
        public const double KILOMETER2MILE = 1.0 / MILE2KILOMETER;

        public const double NAUTICALMILE2KILOMETER = 1.852;
        public const double KILOMETER2NAUTICALMILE = 1.0 / NAUTICALMILE2KILOMETER;

        public const double NAUTICALMILE2MILE = 1.1507794;
        public const double MILE2NAUTICALMILE = 1.0 / NAUTICALMILE2MILE;

        public const double POINT2MILLIMETER = 0.35146;
        public const double MILLIMETER2POINT = 1.0 / POINT2MILLIMETER;

        public const double PICA2MILLIMETER = 4.2175;
        public const double MILLIMETER2PICA = 1.0 / PICA2MILLIMETER;

        #endregion Multipliers for length conversion

        #region Multipliers for area conversion
        //public constants for area conversion

        public const double ACRE2SQUAREMETER = 4046.873;
        public const double SQUAREMETER2ACRE = 1.0 / ACRE2SQUAREMETER;

        public const double ACRE2HECTARE = 0.4046873;
        public const double HECTARE2ACRE = 1.0 / ACRE2HECTARE;

        public const double CIRCULARMIL2SQUAREMILLIMETER = 0.000506708;
        public const double SQUAREMILLIMETER2CIRCULARMIL = 1.0 / CIRCULARMIL2SQUAREMILLIMETER;

        public const double SQUAREINCH2SQUARECENTIMETER = 6.4516;
        public const double SQUARECENTIMETER2SQUAREINCH = 1.0 / SQUAREINCH2SQUARECENTIMETER;

        public const double SQUAREINCH2SQUAREMILLIMETER = 645.16;
        public const double SQUAREMILLIMETER2SQUAREINCH = 1.0 / SQUAREINCH2SQUAREMILLIMETER;

        public const double SQUAREFOOT2SQUAREMETER = 0.09290304;
        public const double SQUAREMETER2SQUAREFOOT = 1.0 / SQUAREFOOT2SQUAREMETER;

        public const double SQUAREYARD2SQUAREMETER = 0.83612736;
        public const double SQUAREMETER2SQUAREYARD = 1.0 / SQUAREYARD2SQUAREMETER;

        public const double SQUAREMILE2SQUAREKILOMETER = 2.589988;
        public const double SQUAREKILOMETER2SQUAREMILE = 1.0 / SQUAREMILE2SQUAREKILOMETER;

        public const double SQUAREINCH2SQUAREMILE = 2.49097669E-10;

        public const double SQUAREINCH2SQUAREFOOT = .0069444;
        public const double SQUAREFOOT2SQUAREINCH = 1.0 / SQUAREINCH2SQUAREFOOT;

        public const double ACRE2SQUAREFOOT = 43560;
        public const double SQUAREFOOT2ACRE = 1.0 / ACRE2SQUAREFOOT;

        public const double ACRE2SQUAREINCH = 6272640;
        public const double SQUAREINCH2ACRE = 1.0 / ACRE2SQUAREINCH;

        public const double SQUAREMILE2ACRE = 640;
        public const double ACRE2SQUAREMILE = 1.0 / SQUAREMILE2ACRE;

        #endregion Multipliers for area conversion

        #region Multipliers for volume conversion
        //public constants for volume conversion

        public const double ACREFOOT2CUBICMETER = 1233.489;
        public const double CUBICMETER2ACREFOOT = 1.0 / ACREFOOT2CUBICMETER;

        public const double BARREL2CUBICMETER = 0.1589873;
        public const double CUBICMETER2BARREL = 1.0 / BARREL2CUBICMETER;

        public const double BARREL2LITER = 158.9873;
        public const double LITER2BARREL = 1.0 / BARREL2LITER;

        public const double BARREL2GALLON = 42.0;
        public const double GALLON2BARREL = 1.0 / BARREL2GALLON;

        public const double CUBICYARD2CUBICMETER = 0.764555;
        public const double CUBICMETER2CUBICYARD = 1.0 / CUBICYARD2CUBICMETER;

        public const double CUBICFOOT2CUBICMETER = 0.02831685;
        public const double CUBICMETER2CUBICFOOT = 1.0 / CUBICFOOT2CUBICMETER;

        public const double CUBICFOOT2LITER = 28.31685;
        public const double LITER2CUBICFOOT = 1.0 / CUBICFOOT2LITER;

        public const double BOARDFOOT2CUBICMETER = 0.002359737;
        public const double CUBICMETER2BOARDFOOT = 1.0 / BOARDFOOT2CUBICMETER;

        public const double REGISTERTON2CUBICMETER = 2.831685;
        public const double CUBICMETER2REGISTERTON = 1.0 / REGISTERTON2CUBICMETER;

        public const double BUSHEL2CUBICMETER = 0.03523907;
        public const double CUBICMETER2BUSHEL = 1.0 / BUSHEL2CUBICMETER;

        public const double GALLON2LITER = 3.785412;
        public const double LITER2GALLON = 1.0 / GALLON2LITER;

        public const double QUART2LITER = 0.9463529;
        public const double LITER2QUART = 1.0 / QUART2LITER;

        public const double PINT2LITER = 0.4731765;
        public const double LITER2PINT = 1.0 / PINT2LITER;

        public const double FLUIDOUNCE2MILLILETER = 29.57353;
        public const double MILLILETER2FLUIDOUNCE = 1.0 / FLUIDOUNCE2MILLILETER;

        public const double CUBICINCH2CUBICCENTIMETER = 16.387064;
        public const double CUBICCENTIMETER2CUBICINCH = 1.0 / CUBICINCH2CUBICCENTIMETER;

        #endregion Multipliers for volume conversion

        #region Multipliers for velocity conversion
        //public constants for velocity conversions

        public const double FOOTPERSECOND2METERPERSECOND = 0.3048;
        public const double METERPERSECOND2FOOTPERSECOND = 1.0 / FOOTPERSECOND2METERPERSECOND;

        public const double MILEPERHOUR2KILOMETERPERHOUR = 1.609344;
        public const double KILOMETERPERHOUR2MILEPERHOUR = 1.0 / MILEPERHOUR2KILOMETERPERHOUR;

        public const double KNOT2KILOMETERPERHOUR = 1.852;
        public const double KILOMETERPERHOUR2KNOT = 1.0 / KNOT2KILOMETERPERHOUR;

        public const double KNOT2MILEPERHOUR = 1.507794;
        public const double MILEPERHOUR2KNOT = 1.0 / KNOT2MILEPERHOUR;

        #endregion Multipliers for velocity conversion


    } //End of the public constant class
}
