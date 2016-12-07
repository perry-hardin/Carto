using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Carto
{

    #region HDMS **************************************************************
    /// <summary>
    /// A class to hold a HemiSphere, degrees, minutes, seconds  (Homework)
    /// </summary>
    public class HDMS
    {
        public HemiSphere H;
        public byte D;
        public byte M;
        public double S;

        public HDMS()
        {
            SetHDMS(HemiSphere.NORTH, 0, 0, 0.0);
        }
        
        public HDMS(HemiSphere Hemi, int Degrees, int Minutes, double Seconds)
        {
            SetHDMS(Hemi, Degrees, Minutes, Seconds);
        }

        public HDMS(HDMS Source)
        {
            SetHDMS(Source.H, Source.D, Source.M, Source.S);
        }

        public HDMS Value
        {
            set
            {
                bool Result = HDMS.IsGood(value);
                if (Result == false) throw new Exception("Value entered for HDMS was not legal. Check HemiSphere, Degrees, Minutes, and Seconds fields");
                this.H = value.H;
                this.D = value.D;
                this.M = value.M;
                this.S = value.S;

            }
            get
            {
                return this;
            }

        }

        public void SetHDMS(HemiSphere Hemi, int Degrees, int Minutes, double Seconds)
        {
            bool Result = HDMS.IsGood(Hemi, Degrees, Minutes, Seconds);
            if (Result == false) throw new Exception("One or more parameters (Hemi, Degrees, Minutes, Seconds) were not legal.");
            H = Hemi;
            D = (byte) Degrees;
            M = (byte) Minutes;
            S = Seconds;
        }
        
        public void GetHDMS(out HemiSphere Hemi, out int Degrees, out int Minutes, out double Seconds)
        {
            Hemi = H;
            Degrees = D;
            Minutes = M;
            Seconds = S;
        }

        public HDMS SetFrom(HDMS Source)
        {
            HDMS Target = new HDMS(Source.H, Source.D, Source.M, Source.S);
            return Target;
        }


        public static bool IsGood(HemiSphere H, int D, int M, double S)
        {
            if (H == HemiSphere.UNKNOWN) return false;

            //Check minutes and seconds first
            if (M >= 60) return false;
            if (M < 0) return false;
            if (S >= 60.0) return false;
            if (S < 0.0) return false;

            //Both lat and lon need to be positive
            if (D < 0) return false;

            //Now check positive range for value of degrees
            if (((H == HemiSphere.NORTH) || (H == HemiSphere.SOUTH)) && (D > 90.0)) return false;
            if (((H == HemiSphere.EAST) || (H == HemiSphere.WEST)) && (D > 180.0)) return false;

            //Check total value
            double Total = D + M / 60.0 + S / 3600.0;
            if (((H == HemiSphere.NORTH) || (H == HemiSphere.SOUTH)) && (Total > 90.0)) return false;
            if (((H == HemiSphere.EAST) || (H == HemiSphere.WEST)) && (Total > 180.0)) return false;

            //If no return so far, value is OK
            return true;

        }

        public static bool IsGood(HDMS Source) 
        {
            bool Result = HDMS.IsGood(Source.H, Source.D, Source.M, Source.S);
            return Result;
        }


    }
    #endregion

    #region GeoDMS ************************************************************
    /// <summary>
    /// A class to hold latitude and longtiude in HDMS format
    /// </summary>
    public class GeoDMS
    {
        private HDMS mLatitude;
        private HDMS mLongitude;

        public HDMS Latitude
        {
            get
            {
                return mLatitude;
            }
            set
            {
                mLatitude = value;
            }
        }

        public HDMS Longitude
        {
            get
            {
                return mLongitude;
            }
            set
            {
                mLongitude = value;
            }
        }

        public void Setup(HemiSphere LatHemi, int LatDegrees, int LatMinutes, double LatSeconds,
                         HemiSphere LonHemi, int LonDegrees, int LonMinutes, double LonSeconds)
        {
            Latitude = new HDMS(LatHemi, LatDegrees, LatMinutes, LatSeconds);
            Longitude = new HDMS(LonHemi, LonDegrees, LonMinutes, LonSeconds);
        }

        public void Setup(HDMS Latitude, HDMS Longitude)
        {
            Latitude = new HDMS(Latitude.H, Latitude.D, Latitude.M, Latitude.S);
            Longitude = new HDMS(Longitude.H, Longitude.D, Longitude.M, Longitude.S);
        }

        public GeoDMS()
        {
            Latitude = new HDMS();
            Longitude = new HDMS();
        }

        public GeoDMS(HemiSphere LatHemi, int LatDegrees, int LatMinutes, double LatSeconds,
                      HemiSphere LonHemi,int LonDegrees, int LonMinutes, double LonSeconds)
        {
            Setup(LatHemi, LatDegrees, LatMinutes, LatSeconds,
                  LonHemi, LonDegrees, LonMinutes, LonSeconds);
        }

        public GeoDMS(HDMS Latitude, HDMS Longitude)
        {
            Setup(Latitude, Longitude);
        }


    }
    #endregion

    #region GeoDecimal and GeoPoint (synonym) ********************************
    /// <summary>
    /// A class to hold latitude and longitude as decimal values.
    /// </summary>
    public class GeoDecimal
    {
        public double mLatitude;  //North is +, South is -
        public double mLongitude; //East is +, West is -

        /// <summary>
        /// Latitude property. North is +, South is -. Range is -90 to +90 
        /// </summary>
        public double Latitude
        {
            get 
            {
                return mLatitude;
            }
            set
            {
                if (value > 90.0) throw new Exception("Latitude property value cannot be greater than +90.0");
                if (value < -90.0) throw new Exception("Latitude property value cannot be less than -90.0");
                mLatitude = value;
            }
        }

        /// <summary>
        /// Longitude property.  East is +, West is -.  Range is -180 to +180. 
        /// </summary>
        public double Longitude
        {
            get
            {
                return mLongitude;
            }
            set
            {
                if (value > 180.0) throw new Exception("Longitude property value cannot be greater than +180.0");
                if (value < -180.0) throw new Exception("Longitude property value cannot be less than -180.0");
                mLongitude = value;
            }
        }

        public GeoDecimal()
        {
            Latitude = 0.0;
            Longitude = 0.0;
        }

        public GeoDecimal(double Latitude, double Longitude)
        {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
        }

        public void Set(double Latitude, double Longitude)
        {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
        }

        public void Get(out double Latitude, out double Longitude)
        {
            Latitude = this.Latitude;
            Longitude = this.Longitude;
        }
    }

    /// <summary>
    /// GeoPoint is the same as Geodecimal.  This is a synonym.
    /// </summary>
    public class GeoPoint : GeoDecimal
    {
        public GeoPoint() : base() { }
        public GeoPoint(double InLatitude, double InLongitude) : base(InLatitude, InLongitude) { }
    }
    #endregion

    #region Civilian UTM structure ********************************************
    //See Geometry Class
    ///// <summary>
    ///// A civilian UTM structure
    ///// </summary>
    ///// <summary>
    ///// The Civilian UTM class manages four data fields
    ///// as properties.
    ///// 1.  The grid letter, an alphabetic character from a 
    ///// predefined set.
    ///// 2.  The grid number, an integer from 1 to 60,
    ///// 3.  The easting in meters, ranging between 0 and 900000.0
    ///// 4.  The northing in meters, ranging between 0 and 10000000.0 
    ///// </summary>
    //public class CivilUTM
    //{
    //    #region Constants

    //    public const string FileHeaderLine = "GRIDNUMBER,GRIDLETTER,EASTING,NORTHING";
    //    private const string Item0 = "GRIDNUMBER";
    //    private const string Item1 = "GRIDLETTER";
    //    private const string Item2 = "EASTING";
    //    private const string Item3 = "NORTHING";

    //    #endregion

    //    #region Data members

    //    private char mGridLetter;  //See Snyder document
    //    private byte mGridNumber;  //1 to 60
    //    private double mEasting;   //0 to 900000
    //    private double mNorthing;  //0 to 10000000

    //    #endregion

    //    #region Constructors, destructors and setup

    //    /// <summary>
    //    /// Default constructor
    //    /// </summary>
    //    public CivilUTM()
    //    {
    //        mGridLetter = '?';
    //        mGridNumber = byte.MaxValue;
    //        mEasting = double.MaxValue;
    //        mNorthing = double.MaxValue;
    //    }

    //    /// <summary>
    //    /// A typical constructor that initializes all fields
    //    /// </summary>
    //    /// <param name="GridNumber">The UTM grid zone number</param>
    //    /// <param name="GridLetter">The UTM grid zone letter</param>
    //    /// <param name="Easting">The UTM grid easting</param>
    //    /// <param name="Northing">the UTM grid northing</param>
    //    public CivilUTM(byte GridNumber, char GridLetter, double Easting, double Northing)
    //    {
    //        Set(GridNumber, GridLetter, Easting, Northing);
    //    }

    //    /// <summary>
    //    /// Constructor that reads properties from a source UTM object
    //    /// </summary>
    //    /// <param name="Source">The object from which to take the properties</param>
    //    public CivilUTM(CivilUTM Source)
    //    {
    //        Set(Source.GridNumber, Source.GridLetter, Source.Easting, Source.Northing);
    //    }

    //    /// <summary>
    //    /// A method to set parameters after object has been constructed
    //    /// </summary>
    //    /// <param name="GridNumber">The UTM grid zone number</param>
    //    /// <param name="GridLetter">The UTM grid zone letter</param>
    //    /// <param name="Easting">The UTM grid easting</param>
    //    /// <param name="Northing">the UTM grid northing</param>
    //    public void Set(byte GridNumber, char GridLetter, double Easting, double Northing)
    //    {
    //        this.GridLetter = GridLetter;
    //        this.GridNumber = GridNumber;
    //        this.Easting = Easting;
    //        this.Northing = Northing;
    //    }

    //    #endregion

    //    #region Properties

    //    /// <summary>
    //    /// The UTM zone grid letter
    //    /// </summary>
    //    public char GridLetter
    //    {
    //        get
    //        {
    //            return mGridLetter;
    //        }
    //        set
    //        {
    //            string L = value.ToString();
    //            L = L.ToUpper();
    //            switch (L)
    //            {
    //                case "X":
    //                case "W":
    //                case "V":
    //                case "U":
    //                case "T":
    //                case "S":
    //                case "R":
    //                case "Q":
    //                case "P":
    //                case "N":
    //                case "M":
    //                case "L":
    //                case "K":
    //                case "J":
    //                case "H":
    //                case "G":
    //                case "F":
    //                case "E":
    //                case "D":
    //                case "C":
    //                    mGridLetter = L[0];
    //                    break;
    //                default:
    //                    throw new Exception("UTM Grid letter " + L +
    //                    " is not a member of the recognized set.");
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// The grid number between 1 and 60
    //    /// </summary>
    //    public byte GridNumber
    //    {
    //        set
    //        {
    //            if ((value > 0) && (value < 61))
    //                mGridNumber = value;
    //            else
    //                throw new Exception("UTM Grid number " + value.ToString() +
    //                   " is not within legal range of 1 to 60 inclusive.");

    //        }
    //        get
    //        {
    //            return mGridNumber;
    //        }
    //    }

    //    /// <summary>
    //    /// The UTM Easting
    //    /// </summary>
    //    public double Easting
    //    {
    //        set
    //        {
    //            if ((value > 0.0) && (value <= 900000.0))
    //                mEasting = value;
    //            else
    //                throw new Exception("UTM Easting of " + value.ToString() +
    //                   " is not within legal range between 0 and 900000.0");

    //        }
    //        get
    //        {
    //            return mEasting;
    //        }
    //    }

    //    /// <summary>
    //    /// The UTM Northing
    //    /// </summary>
    //    public double Northing
    //    {
    //        set
    //        {
    //            if ((value > 0.0) && (value <= 10000000.0))
    //                mNorthing = value;
    //            else
    //                throw new Exception("UTM Northing of " + value.ToString() +
    //                   " is not within legal range between 0 and 10000000.0");
    //        }
    //        get
    //        {
    //            return mNorthing;
    //        }
    //    }

    //    #endregion

    //    #region Copy and clone

    //    /// <summary>
    //    /// Clone a civilian UTM object (Memberwise)
    //    /// </summary>
    //    /// <param name="Source">The source object</param>
    //    /// <param name="Target">The target object to be overwritten</param>
    //    public static void Clone(CivilUTM Source, out CivilUTM Target)
    //    {
    //        if (Source == null)
    //        {
    //            Target = null;

    //        }
    //        else
    //        {
    //            Target = null;
    //            Target = new CivilUTM(Source.GridNumber, Source.GridLetter, Source.Easting, Source.Northing);
    //        }
    //    }

    //    /// <summary>
    //    /// Clone a civilian UTM object (Memberwise)
    //    /// </summary>
    //    /// <param name="Source">The source object to clone</param>
    //    /// <returns>The clone of the source object</returns>
    //    public static CivilUTM Clone(CivilUTM Source)
    //    {
    //        CivilUTM Target = null;
    //        Clone(Source, out Target);
    //        return Target;
    //    }

    //    /// <summary>
    //    ///Copy properties from another civilian UTM object 
    //    /// </summary>
    //    /// <param name="Source">The object from which to take the properties</param>
    //    public void CopyFrom(CivilUTM Source)
    //    {
    //        if (Source == null)
    //            throw new Exception("Cannot copy properties from a null civilian UTM object.  Have you constructed it and set its properties?");

    //        Set(Source.GridNumber, Source.GridLetter, Source.Easting, Source.Northing);
    //    }

    //    #endregion

    //    #region Array initialization

    //    /// <summary>
    //    /// Initialize an existing array of UTM objects
    //    /// </summary>
    //    /// <param name="X">The array of UTM objects to initialize</param>
    //    public static void Initialize(ref CivilUTM[] X)
    //    {
    //        if (X == null)
    //            throw new Exception("Cannot initialize a null array of civilian UTM objects.  Have you dimensioned the array?");

    //        //Construct them
    //        for (int i = 0; i < X.Length; i++)
    //        {
    //            X[i] = new CivilUTM();
    //        }
    //    }

    //    /// <summary>
    //    /// Create (and initialize) a new array of UTM objects
    //    /// </summary>
    //    /// <param name="Count">The number of "slots" in the array</param>
    //    /// <returns></returns>
    //    public static CivilUTM[] CreateArray(int Count)
    //    {
    //        CivilUTM[] X = new CivilUTM[Count];
    //        Initialize(ref X);
    //        return X;
    //    }
    //    #endregion

    //    #region Text (CSV) file elementary operations

    //    /// <summary>
    //    /// Write a column label header line for the CSV output file.  
    //    /// Labels will be GRIDLETTER,GRIDNUMBER,EASTING,NORTHING 
    //    /// </summary>
    //    /// <param name="OutFile">The outfile to write to</param>
    //    public static void WriteHeaderLine(ref StreamWriter OutFile)
    //    {
    //        try
    //        {
    //            OutFile.WriteLine(FileHeaderLine);
    //        }
    //        catch
    //        {
    //            throw new Exception("Cannot write header line to output file.  Has the file been opened for reading?");
    //        }
    //    }

    //    /// <summary>
    //    /// Read the CSV column label header line. 
    //    /// Should be GRIDNUMBER,GRIDLETTER,EASTING,NORTHING. 
    //    /// Throws exceptions if line does match that string.
    //    /// </summary>
    //    /// <param name="InFile">The infile from which to read the header</param>
    //    public static void ReadHeaderLine(ref StreamReader InFile)
    //    {
    //        string InLine = null;
    //        try
    //        {
    //            InLine = InFile.ReadLine();
    //        }
    //        catch
    //        {
    //            throw new Exception("Could not read header line from input text file.  Does the file exist?  Has it been opened?");
    //        }

    //        InLine = InLine.ToUpper();
    //        string[] Piece = InLine.Split(',');
    //        if (Piece.Length < 4)
    //            throw new Exception("First four items in the input file header line must literally read " + FileHeaderLine);
    //        for (int i = 0; i < Piece.Length; i++)
    //            Piece[i] = Piece[i].Trim();
    //        if (Piece[0] != Item0) throw new Exception("First item in the input file header line must literally read " + Item0);
    //        if (Piece[1] != Item1) throw new Exception("Second item in the input file header line must literally read " + Item1);
    //        if (Piece[2] != Item2) throw new Exception("Third item in the input file header line must literally read " + Item2);
    //        if (Piece[3] != Item3) throw new Exception("Fourth item in the input file header line must literally read " + Item3);
    //    }

    //    /// <summary>
    //    /// Read a civilian UTM record from a CSV file. 
    //    /// Format example is 12,T,4567.3,45678.9
    //    /// </summary>
    //    /// <param name="InFile">The input file in a permitted format</param>
    //    /// <param name="LineNumber">The file line number attempting to be read and parsed</param>
    //    /// <param name="LineNumber">The data line that was parsed for that line (refered to by line number)</param>
    //    public void Read(ref StreamReader InFile, ref int LineNumber, ref string DataLine)
    //    {
    //        //Try to read a line of data from the open file
    //        LineNumber += 1;
    //        DataLine = null;
    //        try
    //        {
    //            DataLine = InFile.ReadLine();
    //        }
    //        catch
    //        {
    //            throw new Exception("Could not read data on line " + LineNumber.ToString() + " of the input data file.");
    //        }

    //        //Clean it up and do an initial check to see if there are enough fields
    //        if (DataLine == null)
    //            throw new Exception("Expected data on input data file line " + LineNumber.ToString() + " , but found none.  Blank line? Truncated file? Incorrect count?");

    //        DataLine = DataLine.Trim();
    //        if (string.IsNullOrEmpty(DataLine))
    //            throw new Exception("Expected data on input data file line " + LineNumber.ToString() + " , but found none.  Blank line? Truncated file? Incorrect count?");

    //        DataLine = DataLine.ToUpper();
    //        string[] Piece = DataLine.Split(',');
    //        if (Piece.Length < 4)
    //            throw new Exception("For specified format, there needs to be four correctly composed UTM data fields in data file line " + LineNumber.ToString());
    //        for (int i = 0; i < Piece.Length; i++)
    //            Piece[i] = Piece[i].Trim();


    //        //Try to do the conversion and assign the properties
    //        //Note that the properties themselves will also catch errors in the range of input values
    //        //but no line number will be reported as part of the exception
    //        try
    //        {
    //            GridNumber = byte.Parse(Piece[0]);
    //            GridLetter = Piece[1][0];
    //            Easting = double.Parse(Piece[2]);
    //            Northing = double.Parse(Piece[3]);
    //        }
    //        catch
    //        {
    //            throw new Exception("Could not parse data file line " + LineNumber.ToString() +
    //                                " into the four required civilian UTM fields. \n" +
    //                                "Correct format example is 12,T,4567.8, 56789.0 \n" +
    //                                "The line found was " + DataLine);
    //        }

    //    }

    //    /// <summary>
    //    /// Write the fields (properties) to the current position in the text file
    //    /// </summary>
    //    /// <param name="?"></param>
    //    public void Write(ref StreamWriter Wrtr)
    //    {
    //        if (Wrtr == null)
    //            throw new Exception("Data file for civilian UTM coordinates is not open.");
    //        try
    //        {
    //            string L = GridNumber.ToString() + "," + GridLetter.ToString() + "," + Easting.ToString() + "," + Northing.ToString();
    //            Wrtr.WriteLine(L);
    //        }
    //        catch
    //        {
    //            throw new Exception("Could not write UTM coordinate data to civilian UTM data file.");
    //        }
    //    }


    //    #endregion

    //    #region Whole array CSV file reader and writer

    //    /// <summary>
    //    /// Read a comma delimited format text file of Civilian UTM coordinates
    //    /// </summary>
    //    /// <param name="InFileName">Full path to the CSV file</param>
    //    /// <param name="Records">The array of civilian UTM records to be returned</param>
    //    /// <param name="HeaderFormat">The format of the header line(s) in the file</param>
    //    public static void ReadFile(string InFileName, out CivilUTM[] Records, CSVHeaderFormat HeaderFormat)
    //    {
    //        Records = null;
    //        StreamReader Rdr = null;
    //        string CountStr = string.Empty;
    //        int Count = 0;
    //        int LineNumber = 0;
    //        bool ParseResult = false;


    //        //Make sure the file is there before we try to do something with it
    //        if (!File.Exists(InFileName))
    //            throw new Exception("File " + InFileName + " was not found.");

    //        //Manage the header information according to the format
    //        switch (HeaderFormat)
    //        {
    //            case CSVHeaderFormat.COUNTANDHEADERLINE:
    //                Rdr = new StreamReader(InFileName);
    //                CountStr = Rdr.ReadLine();
    //                CountStr = CountStr.Trim();
    //                ParseResult = int.TryParse(CountStr, out Count);
    //                if (ParseResult == false)
    //                    throw new Exception("Could not find a recognizable record count (an integer) on first line of civilian UTM input data file.");
    //                CivilUTM.ReadHeaderLine(ref Rdr);
    //                LineNumber = 2;
    //                break;
    //            case CSVHeaderFormat.COUNTONLY:
    //                Rdr = new StreamReader(InFileName);
    //                CountStr = Rdr.ReadLine();
    //                CountStr = CountStr.Trim();
    //                ParseResult = int.TryParse(CountStr, out Count);
    //                if (ParseResult == false)
    //                    throw new Exception("Could not find a recognizable record count (an integer) on first line of civilian UTM input data file.");
    //                LineNumber = 1;
    //                break;
    //            case CSVHeaderFormat.HEADERLINEONLY:
    //                Count = Utils.CountTextFileLines(InFileName);
    //                Count -= 1;
    //                Rdr = new StreamReader(InFileName);
    //                CivilUTM.ReadHeaderLine(ref Rdr);
    //                LineNumber = 1;
    //                break;
    //            case CSVHeaderFormat.NOPREDATALINES:
    //                Count = Utils.CountTextFileLines(InFileName);
    //                Rdr = new StreamReader(InFileName);
    //                LineNumber = 0;
    //                break;
    //            default:
    //                throw new Exception("Cannot read an input file with unknown header type.");
    //        }

    //        //Try to read the data lines and close the file
    //        string DataLine = null;
    //        try
    //        {
    //            //Read the data lines
    //            Records = null;
    //            Records = new CivilUTM[Count];
    //            CivilUTM.Initialize(ref Records);
    //            for (int i = 0; i < Count; i++)
    //            {
    //                Records[i].Read(ref Rdr, ref LineNumber, ref DataLine);
    //            }

    //        }
    //        catch
    //        {
    //            throw new Exception("Could not parse data file line " + LineNumber.ToString() +
    //                                " into the four required civilian UTM fields. \n" +
    //                                "Correct format example is 12,T,4567.8, 56789.0 \n" +
    //                                "The line found was " + DataLine +
    //                                "\nThe input file name was " + InFileName);
    //        }
    //        finally
    //        {
    //            //Clean up the stream reader
    //            if (Rdr != null)
    //            {
    //                Rdr.Close();
    //                Rdr.Dispose();
    //                Rdr = null;
    //            }

    //        }
    //    }

    //    /// <summary>
    //    /// Write a comma delimited format text file of Civilian UTM coordinates
    //    /// </summary>
    //    /// <param name="OutFileName">Full path to the CSV file</param>
    //    /// <param name="Records">The array of civilian UTM records to be written</param>
    //    /// <param name="HeaderFormat">The format of the header line(s) in the file</param>
    //    public static void WriteFile(string OutFileName, CivilUTM[] Records, CSVHeaderFormat HeaderFormat)
    //    {
    //        bool PathOK = Utils.FilePathOK(OutFileName);
    //        if (!PathOK)
    //            throw new Exception("Outfile path " + OutFileName + "appears to be an incorrectly formed or could not be located / opened.");

    //        //Manage the header information according to the format
    //        StreamWriter Wtr = null;
    //        Wtr = new StreamWriter(OutFileName);
    //        switch (HeaderFormat)
    //        {
    //            case CSVHeaderFormat.COUNTANDHEADERLINE:
    //                Wtr.WriteLine(Records.Length.ToString());
    //                CivilUTM.WriteHeaderLine(ref Wtr);
    //                break;
    //            case CSVHeaderFormat.COUNTONLY:
    //                Wtr.WriteLine(Records.Length.ToString());
    //                break;
    //            case CSVHeaderFormat.HEADERLINEONLY:
    //                CivilUTM.WriteHeaderLine(ref Wtr);
    //                break;
    //            case CSVHeaderFormat.NOPREDATALINES:
    //                break;
    //            default:
    //                throw new Exception("Cannot write an output file with unspecified header type.");
    //        }

    //        //Try to write the data lines and close the file
    //        try
    //        {
    //            //Write the data lines
    //            foreach (CivilUTM Rec in Records)
    //                Rec.Write(ref Wtr);
    //        }
    //        catch
    //        {
    //            throw new Exception("Unanticipated error while writing civilian UTM data file. Disk full?  Network error?  File path was: " + OutFileName);
    //        }
    //        finally
    //        {
    //            //Clean up the stream writer
    //            if (Wtr != null)
    //            {
    //                Wtr.Flush();
    //                Wtr.Close();
    //                Wtr.Dispose();
    //                Wtr = null;
    //            }

    //        }
    //    }

    //    #endregion
    //}


    #endregion

    #region Generic two dimensional point *************************************
    //See Geometry
    //public class XYPoint
    //{
    //    public double X;
    //    public double Y;
    //}
    #endregion

}
