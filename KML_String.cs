using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GIS
{

    public abstract class KMLComponent
    {
        protected readonly string Quotes = "\"";
        protected readonly string Quote = "\'";
        protected readonly string Space = " ";
        protected readonly string LF = ((char)10).ToString();
        protected readonly string CR = ((char)13).ToString();
        protected readonly string CRLF;

        public KMLComponent()
        {
            CRLF = CR + LF;
        }

        public abstract string Compose();
    }

    //public class KMLSection
    //{
    //    public string Tag = null;

    //    public KMLSection(string Tag)
    //    {
    //        this.Tag = Tag;
    //    }
    //}

    public class KMLEndTag : KMLComponent
    {
        public override string Compose()
        {
            return "</kml>";
        }
    }

    public class KMLHeader : KMLComponent
    {
        public override string Compose()
        {
            StringBuilder S = new StringBuilder();
            S.Append("<?xml version= \"1.0\" encoding=\"UTF-8\"?>");
            S.Append("<kml xmlns=");
            S.Append(Quotes + @"http://www.opengis.net/kml/2.2" + Quotes);
            S.Append(" xmlns:gx=");
            S.Append(Quotes + @"http://www.google.com/kml/ext/2.2" + Quotes);
            S.Append(" xmlns:kml=");
            S.Append(Quotes + @"http://www.opengis.net/kml/2.2" + Quotes);
            S.Append(" xmlns:atom=");
            S.Append(Quotes + @"http://www.w3.org/2005/Atom" + Quotes + ">");
            string Str = S.ToString();
            return Str;

        }


    }

    public class KMLListStyle : KMLComponent
    {
        //public const string EndString = "</ListStyle>";

        public override string Compose()
        {
            StringBuilder S = new StringBuilder();
            S.Append("<ListStyle>");
            S.Append("</ListStyle>");
            string Str = S.ToString();
            return Str;
        }
    }

    public class KMLIcon : KMLComponent
    {
        public string href;
        public KMLIcon(string href)
        {
            this.href = href;
        }

        public override string Compose()
        {
            StringBuilder S = new StringBuilder();
            S.Append("<Icon>");
            S.Append("<href>");
            S.Append(href);
            S.Append("</href>");
            S.Append("</Icon>");
            string Str = S.ToString();
            return Str;
        }

        //public void Write(StreamWriter Wtr) 
        //{
        //    StringBuilder S = new StringBuilder();
        //    S.Append("<Icon>");
        //    S.Append("<href>");
        //    S.Append(href);
        //    S.Append("</href>");
        //    S.Append("</Icon>");
        //    string Str = S.ToString();
        //    Wtr.WriteLine(Str);
        //}
    }

    public class KMLIconStyle : KMLComponent
    {
        public string Color;
        public KMLIcon Icon;
        public string Scale;
        //public const string EndString = "</IconStyle>";

        public KMLIconStyle(string Color, double Scale, KMLIcon Icon)
        {
            this.Color = Color;
            this.Scale = Scale.ToString();
            this.Icon = Icon;
        }

        public override string Compose()
        {
            StringBuilder S = new StringBuilder();
            S.Append("<IconStyle>");
            S.Append("<color>");
            S.Append(Color);
            S.Append("</color>");
            S.Append(Icon.Compose());
            S.Append("</IconStyle>");
            string Str = S.ToString();
            return Str;
        }
    }

    public class KMLStyle : KMLComponent
    {
        string StyleID;
        KMLIconStyle IconStyle;
        KMLListStyle ListStyle;

        public KMLStyle(string StyleID, KMLIconStyle IconStyle, KMLListStyle ListStyle)
        {
            this.StyleID = StyleID;
            this.IconStyle = IconStyle;
            this.ListStyle = ListStyle;
        }

        public override string Compose()
        {
            StringBuilder S = new StringBuilder();
            S.Append("<Style id=");
            S.Append(Quotes);
            S.Append(StyleID);
            S.Append(Quotes);
            S.Append(">");
            S.Append(IconStyle.Compose());
            S.Append(ListStyle.Compose());
            S.Append("</Style>");
            string Str = S.ToString();
            return Str;
        }

    }

    public class KMLPair : KMLComponent
    {
        string Key;
        string StyleURL;

        public KMLPair(string Key, string StyleURL)
        {
            this.Key = Key;
            this.StyleURL = StyleURL;
        }

        public override string Compose()
        {
            StringBuilder S = new StringBuilder();
            S.Append("<Pair>");
            S.Append("<key>");
            S.Append(Key);
            S.Append("</key>");

            S.Append("<styleUrl>");
            S.Append(StyleURL);
            S.Append("</styleUrl>");
            S.Append("</Pair>");
            string Str = S.ToString();
            return Str;
        }
    }

    public class KMLStyleMap : KMLComponent
    {
        private List<KMLPair> Pairs;
        private string ID;

        public KMLStyleMap(string ID)
        {
            this.ID = ID;
            this.Pairs = new List<KMLPair>(2);
        }

        public void AddPair(KMLPair NewPair)
        {
            Pairs.Add(NewPair);
        }

        public override string Compose()
        {
            StringBuilder S = new StringBuilder();
            S.Append("<StyleMap id=");
            S.Append(Quotes);
            S.Append(ID);
            S.Append(Quotes);
            S.Append(">");
            for (int i = 0; i < Pairs.Count; i++)
            {
                S.Append(Pairs[i].Compose());
            }
            S.Append("</StyleMap>");
            string Str = S.ToString();
            return Str;
        }
    }

    public class KMLDescription : KMLComponent
    {
        private string[] Items;

        public KMLDescription(params string[] DescItems)
        {
            Items = new string[DescItems.Length];
            Array.Copy(DescItems, Items, DescItems.Length);
        }

        public override string Compose()
        {
            StringBuilder S = new StringBuilder();
            S.Append("<description>");
            for (int i = 0; i < Items.Length; i++)
            {
                S.Append(Items[i]);
                if (i < Items.Length - 1)
                    S.Append(CRLF);
            }
            S.Append("</description>");
            string Str = S.ToString();
            return Str;
        }
    }

    public class KMLPlaceMark : KMLComponent
    {
        string Name;
        string Open;
        string StyleURL;
        KMLDescription Description;
        KMLLookAt LookAt;
        KMLPoint Point;

        public KMLPlaceMark(string Name, int Open, string StyleURL,
                            KMLDescription Description, KMLLookAt LookAt,
                            KMLPoint Point)
        {
            this.Name = Name;
            this.Open = Open.ToString();
            this.StyleURL = StyleURL;
            this.Description = Description;
            this.LookAt = LookAt;
            this.Point = Point;
        }

        public override string Compose()
        {
            StringBuilder S = new StringBuilder();
            S.Append("<Placemark>");
            S.Append("<name>" + Name + "</name>");
            S.Append(Description.Compose());
            S.Append(LookAt.Compose());
            S.Append("<styleUrl> " + StyleURL + "</styleUrl>");
            S.Append(Point.Compose());
            S.Append("</Placemark>");
            string Str = S.ToString();
            return Str;
        }
    }

    public class KMLPoint : KMLComponent
    {
        string DrawOrder;
        string Latitude;
        string Longitude;
        string Altitude;

        public KMLPoint(int DrawOrder, double Latitude, double Longitude, double Altitude)
        {
            this.DrawOrder = DrawOrder.ToString();
            this.Latitude = Latitude.ToString();
            this.Longitude = Longitude.ToString();
            this.Altitude = Altitude.ToString();
        }

        public override string Compose()
        {
            StringBuilder S = new StringBuilder();
            S.Append("<Point>");
            S.Append("<gx:drawOrder>");
            S.Append(DrawOrder);
            S.Append("</gx:drawOrder>");
            S.Append("<coordinates>");
            S.Append(Longitude + "," + Latitude + "," + Altitude);
            S.Append("</coordinates>");
            S.Append("</Point>");
            string Str = S.ToString();
            return Str;
        }
    }

    public class KMLLookAt : KMLComponent
    {
        string Longitude;
        string Latitude;
        string Altitude;
        string Heading;
        string Tilt;
        string Range;
        string AltitudeMode;

        public KMLLookAt(double Latitude, double Longitude, double Altitude,
                         double Heading, double Tilt, double Range,
                         string AltitudeMode)
        {
            this.Latitude = Latitude.ToString();
            this.Longitude = Longitude.ToString();
            this.Altitude = Altitude.ToString();
            this.Heading = Heading.ToString();
            this.Tilt = Tilt.ToString();
            this.Range = Range.ToString();
            this.AltitudeMode = AltitudeMode;
        }

        public override string Compose()
        {
            StringBuilder S = new StringBuilder();
            S.Append("<LookAt>");
            S.Append("<longitude>" + this.Longitude + "</longitude>");
            S.Append("<latitude>" + this.Latitude + "</latitude>");
            S.Append("<altitude>" + this.Altitude + "</altitude>");
            S.Append("<heading>" + this.Heading + "</heading>");
            S.Append("<tilt>" + this.Tilt + "</tilt>");
            S.Append("<range>" + this.Range + "</range>");
            S.Append("<gx:altitudeMode>" + this.AltitudeMode + "</gx:altitudeMode>");
            S.Append("</LookAt>");
            string Str = S.ToString();
            return Str;
        }
    }

    public class KMLFolder : KMLComponent
    {
        string Name;
        string Open;
        List<KMLPlaceMark> Marks;

        public KMLFolder(string Name, int Open)
        {
            this.Name = Name;
            this.Open = Open.ToString();
            Marks = new List<KMLPlaceMark>();
        }

        public override string Compose()
        {
            StringBuilder S = new StringBuilder();
            S.Append("<Folder>");
            S.Append("<name>" + Name + "</name>");
            S.Append("<open>" + Open + "</open>");
            for (int i = 0; i < Marks.Count; i++)
            {
                S.Append(Marks[i].Compose());
            }
            S.Append("</Folder>");
            string Str = S.ToString();
            return Str;
        }


        public void AddPlacemark(string Name, int Open, string StyleURL,
                                 double LookLatitude, double LookLongitude, double LookAltitude, double LookHeading, double LookTilt, double LookRange, string LookAltitudeMode,
                                 int PointDrawOrder, double PointLatitude, double PointLongitude, double PointAltitude,
                                 params string[] Description)
        {
            KMLDescription D = new KMLDescription(Description);
            KMLPoint P = new KMLPoint(PointDrawOrder, PointLatitude, PointLongitude, PointAltitude);
            KMLLookAt L = new KMLLookAt(LookLatitude, LookLongitude, LookAltitude, LookHeading, LookTilt, LookRange, LookAltitudeMode);
            KMLPlaceMark NewMark = new KMLPlaceMark(Name, Open, StyleURL, D, L, P);
            AddPlacemark(NewMark);
        }



        public void AddPlacemark(KMLPlaceMark NewMark)
        {
            Marks.Add(NewMark);
        }
    }

    public class KMLDocument : KMLComponent
    {
        KMLStyleMap StyleMap;
        List<KMLStyle> Styles;
        List<KMLFolder> Folders;
        KMLHeader Header;
        KMLEndTag EndTag;
        string Name;


        public KMLDocument(string Name)
        {
            this.Name = Name;
            this.StyleMap = null;
            Styles = new List<KMLStyle>();
            Folders = new List<KMLFolder>();
            Header = new KMLHeader();
            EndTag = new KMLEndTag();
        }

        public void AttachStyleMap(KMLStyleMap StyleMap)
        {
            this.StyleMap = StyleMap;
        }

        
        public void AddFolder(KMLFolder NewFolder)
        {
            Folders.Add(NewFolder);
        }

        public void AddStyle(KMLStyle NewStyle)
        {
            Styles.Add(NewStyle);
        }


        public void AddStyle(string StyleID, string IconColor, double IconScale, string IconHref)
        {
            KMLStyle Style = new KMLStyle(StyleID,
                                          new KMLIconStyle(IconColor, IconScale, new KMLIcon(IconHref)),
                                          new KMLListStyle());
            AddStyle(Style);
        }

        public override string Compose()
        {
            StringBuilder S = new StringBuilder();
            S.Append(Header.Compose());
            S.Append("<Document>");
            S.Append("<name>" + Name + "</name>");
            for (int i = 0; i < Styles.Count; i++) S.Append(Styles[i].Compose());
            S.Append(StyleMap.Compose());
            for (int i = 0; i < Folders.Count; i++) S.Append(Folders[i].Compose());
            S.Append("</Document>");
            S.Append(EndTag.Compose());
            string Str = S.ToString();
            return Str;
        }

        public void Write(string DirPath)
        {
            if (!DirPath.EndsWith("\\")) DirPath += "\\";

            string FileName = DirPath + Name.ToUpper() + ".KML";
            StreamWriter W = new StreamWriter(FileName, false, Encoding.ASCII);
            string S = Compose();
            bool StopActive = false;
            foreach (char C in S)
            {
                if (C == '/') StopActive = true;
                if ((C == '>') && (StopActive))
                {
                    W.Write(C);
                    W.WriteLine();
                    StopActive = false;
                }
                else
                {
                    W.Write(C);
                }
            }
            W.Flush();
            W.Close();
            W = null;
        }





    }


}
