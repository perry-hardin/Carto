using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Carto
{
    public enum CSVHeaderFormat { UNKNOWN, COUNTONLY, HEADERLINEONLY, COUNTANDHEADERLINE, NOPREDATALINES };
    public enum GroundUnits { UNKNOWN, MILES, FEET, KILOMETERS, METERS };
    public enum MapUnits { UNKNOWN, INCHES, CENTIMETERS, MILLIMETERS };
    public enum ConvertUnits { UNKNOWN, MM, CM, IN, FT, YD, M, KM, MI };
    public enum HemiSphere { UNKNOWN, NORTH, SOUTH, EAST, WEST };
    public enum TypeHemiSphere { UNKNOWN, LATITUDE, LONGITUDE };

    //Byte order for data files
    public enum ByteOrder {UNKNOWN = 255, PC = 0, DEC = 1, LSB_FIRST = 0, MSB_FIRST = 1};

    //Data Types
    public enum ValueType { UNKNOWN = 255, UBYTE8 = 1, SINT16 = 2, SINT32 = 3, FLOAT32 = 4, FLOAT64 = 5, UINT16 = 12, UINT32 = 13, SINT64 = 14, UINT64 = 15 };

    //Array base for my array classes
    public enum ArrayBase { ZERO = 0, ONE = 1 };

    //Data interleave for files on disk of imagery
    public enum BandInterleave {UNKNOWN, BS, BIL, BIP };

    //These deal with ESRI shape file reading
    public enum ShapeFileFeature { UNKNOWN = 255, NOTAPPLICABLE = 254, NULL = 0, POINT = 1, POLYLINE = 3, POLYGON = 5, MULTIPOINT = 8 };
    public enum Figure { UNKNOWN = 255, NOTAPPLICABLE = 254, LINESTRING = 0, OUTERRING = 1, INNERRING = 2, MULTIPOINT = 3, SINGLEPOINT = 4 }
    public enum Closure { UNKNOWN = 255, NOTAPPLICABLE = 254, OPEN = 0, CLOSED = 1 }
    public enum CoordSpace { UNKNOWN = 255, NOTAPPLICABLE = 254, PLANAR = 0, ELLIPSOIDAL = 2 };
    public enum PolyDirection { UNKNOWN = 255, NOTAPPLICABLE = 254, CLOCKWISE = 0, COUNTERCLOCKWISE = 1 };

    //Datum
    public enum HorizontalDatum { UNKNOWN, WGS84, NAD27, NAD83, INTL };
    public enum VerticalDatum { UNKNOWN, NAVD88, NGVD29, LMSL };
    public enum LengthUnits { UNKNOWN, METERS, FEET };
    public enum Projection { UNKNOWN, TM, POLYCONIC, UTM };
    public enum TopoSeries { UNKNOWN, SERIES_75, SERIES_10, SERIES_AK };

    //public static class MyEnum
    //{
    //    //public enum CSVHeaderFormat { UNKNOWN, COUNTONLY, HEADERLINEONLY, COUNTANDHEADERLINE, NOPREDATALINES };
    //    //public enum GroundUnits { UNKNOWN, MILES, FEET, KILOMETERS, METERS };
    //    //public enum MapUnits { UNKNOWN, INCHES, CENTIMETERS, MILLIMETERS };
    //    //public enum HemiSphere { UNKNOWN, NORTH, SOUTH, EAST, WEST };
    //    //public enum TYPEHEMIPHERE { UNKNOWN, LATITUDE, LONGITUDE };


    //    ////Byte order for data files
    //    //public enum ByteOrder { PC, DEC };

    //    ////Array base for my array classes
    //    //public enum ArrayBase { ZERO = 0, ONE = 1 };

    //    ////Data interleave for files on disk of imagery
    //    //public enum BandInterleave { BS, BIL, BIP };

    //    ////These deal with ESRI shape file reading
    //    //public enum ShapeFileFeature { UNKNOWN = 255, NOTAPPLICABLE = 254, NULL = 0, POINT = 1, POLYLINE = 3, POLYGON = 5, MULTIPOINT = 8 };
    //    //public enum Figure { UNKNOWN = 255, NOTAPPLICABLE = 254, LINESTRING = 0, OUTERRING = 1, INNERRING = 2, MULTIPOINT = 3, SINGLEPOINT = 4 }
    //    //public enum Closure { UNKNOWN = 255, NOTAPPLICABLE = 254, OPEN = 0, CLOSED = 1 }
    //    //public enum CoordSpace { UNKNOWN = 255, NOTAPPLICABLE = 254, PLANAR = 0, ELLIPSOIDAL = 2 };
    //    //public enum PolyDirection { UNKNOWN = 255, NOTAPPLICABLE = 254, CLOCKWISE = 0, COUNTERCLOCKWISE = 1 };

    //    ////Datum
    //    //public enum HorizontalDatum { UNKNOWN, WGS84, NAD27, NAD83, INTL };
    //    //public enum VerticalDatum { UNKNOWN, NAVD88, NGVD29, LMSL };
    //    //public enum LengthUnits { UNKNOWN, METERS, FEET };
    //    //public enum Projection { UNKNOWN, TM, POLYCONIC, UTM };
    //    //public enum TopoSeries { UNKNOWN, SERIES_75, SERIES_10, SERIES_AK };


    //    public static string EnumStr(HorizontalDatum H)
    //    {
    //        int Val = (int)H;
    //        string Str = Enum.GetName(typeof(HorizontalDatum), Val);
    //        return Str;
    //    }

    //    public static string EnumStr(VerticalDatum V)
    //    {
    //        int Val = (int)V;
    //        string Str = Enum.GetName(typeof(VerticalDatum), Val);
    //        return Str;
    //    }

    //    public static string EnumStr(LengthUnits U)
    //    {
    //        int Val = (int)U;
    //        string Str = Enum.GetName(typeof(LengthUnits), Val);
    //        return Str;
    //    }

       

    //    public static string EnumStr(Projection P)
    //    {
    //        int Val = (int)P;
    //        string Str = Enum.GetName(typeof(Projection), Val);
    //        return Str;
    //    }

    //}
}
