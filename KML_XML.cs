using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using GIS;

namespace Carto {
    
    public abstract class KML
    {
        //Managing altitude mode
        public enum AltitudeModeType { CLAMPTOGROUND = 1, RELATIVETOGROUND = 2, ABSOLUTE = 3 }

        //Managing tesselation and extrusion
        public enum TesselateType { NO = 0, YES = 1 };
        public enum ExtrudeType { NO = 0, YES = 1 };
    }

    /// <summary>
    /// The base class for all KML Geometry types.  Its two important services
    /// are Setup which where the parent does basic setup for the children as
    /// requested by the children.  The second is CreateXML which does the lions
    /// share of generating the XML for the children.  This is possible since much
    /// of the XML is common to all children.
    /// </summary>
    public abstract class KMLGeometry : KML
    {
        public int ID = -1;
        public KML.ExtrudeType Extrude = KML.ExtrudeType.NO;
        public KML.TesselateType Tesselate = KML.TesselateType.NO;
        public KML.AltitudeModeType AltitudeMode = KML.AltitudeModeType.CLAMPTOGROUND;

        public void Setup(int ID = Const.INTUNDEFINED, 
                          KML.ExtrudeType Extrude = KML.ExtrudeType.NO, 
                          KML.TesselateType Tesselate = KML.TesselateType.NO,
                          KML.AltitudeModeType AltitudeMode = KML.AltitudeModeType.CLAMPTOGROUND)
        {
            this.ID = ID;
            this.Extrude = Extrude;
            this.Tesselate = Tesselate;
            this.AltitudeMode = AltitudeMode;
        }

        /// <summary>
        /// Create XML elements for the children depending on what the children have or need
        /// </summary>
        /// <param name="GeometryType">The recognized KML label for the geometry</param>
        /// <param name="Coordinates">Any simple coordinates that need to be written as part of the geometry</param>
        /// <returns>The KML formatted XElement</returns>
        public XElement CreateXML(string GeometryType, VectorObject Coordinates = null)
        {
            XElement P1 = null;

            if (ID != Const.INTUNDEFINED)
            {
                P1 = new XElement(GeometryType, new XAttribute("id", ID.ToString()));
            }
            else
                P1 = new XElement(GeometryType);


            if (Extrude == KML.ExtrudeType.YES)
            {
                XElement P2 = new XElement("extrude", (int)(Extrude));
                P1.Add(P2);
            }

            if (Tesselate == KML.TesselateType.YES)
            {
                XElement P2 = new XElement("tesselate", (int)(Tesselate));
                P1.Add(P2);
            }

            //Altitude mode management
            { 
                string Str = "UNKNOWN";
                switch (AltitudeMode)
                {
                    case AltitudeModeType.ABSOLUTE:
                        Str = "absolute";
                        break;
                    case AltitudeModeType.CLAMPTOGROUND:
                        Str = "clampToGround";
                        break;
                    case AltitudeModeType.RELATIVETOGROUND:
                        Str = "relativeToGround";
                        break;
                }
                XElement P2 = new XElement("altitudeMode", Str);
                P1.Add(P2);
            }

            if (Coordinates != null)
            {
                XElement P2 = new XElement("coordinates", Coordinates.ToString());
                P1.Add(P2);
            }




            return P1;
        }

        /// <summary>
        /// Convert the KML geometry to its XML equivalent.  This is the XML required for the specific
        /// KML element being writted to placemark / folder
        /// </summary>
        /// <returns></returns>
        public abstract XElement ToXML();   
    }

    /// <summary>
    /// The KML LineString is the basis for KML line objects
    /// </summary>
    public class KMLLineString : KMLGeometry
    {
        public VectorObject Coordinates = null;

        public KMLLineString (int ID, 
                              VectorObject Coordinates,
                              KML.ExtrudeType Extrude = KML.ExtrudeType.NO, 
                              KML.AltitudeModeType AltitudeMode = KML.AltitudeModeType.CLAMPTOGROUND, 
                              KML.TesselateType Tesselate = KML.TesselateType.NO)
        {
            base.Setup(ID, Extrude, Tesselate, AltitudeMode);
            this.Coordinates = new VectorObject(Coordinates.Count);
            this.Coordinates.CopyVerticesFrom(Coordinates);
            this.Coordinates.Assign(Figure.LINESTRING, PolyDirection.NOTAPPLICABLE, Closure.OPEN);
        }

        public override XElement ToXML() 
        {
            XElement P1 = CreateXML("LineString", Coordinates);
            return P1;
        }
    }

    /// <summary>
    /// The KML Point is a single vertex
    /// </summary>
    public class KMLPoint : KMLGeometry
    {
        public Coordinate Coord;

        public KMLPoint (int ID, KML.ExtrudeType Extrude, KML.AltitudeModeType AltitudeMode, Coordinate Coord)
        {
            this.Coord = new Coordinate(Coord);
            Setup(ID, Extrude,KML.TesselateType.NO, AltitudeMode);
        }

        /// <summary>
        /// Convert the KMLPoint to an XML element
        /// </summary>
        /// <returns></returns>
        public override XElement ToXML()
        {
            VectorObject C = new VectorObject(1);
            C.Assign(Figure.SINGLEPOINT,PolyDirection.NOTAPPLICABLE,Closure.NOTAPPLICABLE);
            C.Add(Coord);
            XElement P1 = CreateXML("Point", C);
            return P1;
        }

    }

    /// <summary>
    /// KML Rings form simple boundaries for polygons as well as inner and outer
    /// boundaries for more complex polygons with multiple parts or holes.
    /// </summary>
    public class KMLRing : KMLGeometry
    {
        public VectorObject Coordinates = null;

        public KMLRing(int ID, Figure OuterOrInnerBoundary, VectorObject Points, 
                       KML.ExtrudeType Extrude = KML.ExtrudeType.NO, 
                       KML.AltitudeModeType AltitudeMode = KML.AltitudeModeType.CLAMPTOGROUND, 
                       KML.TesselateType Tesselate = KML.TesselateType.NO)
        {
            Coordinates = new VectorObject(Points.Count);
            Coordinates.CopyVerticesFrom(Points);
            Coordinates.Assign(OuterOrInnerBoundary, PolyDirection.CLOCKWISE, Closure.CLOSED);
            base.Setup(ID, Extrude, Tesselate, AltitudeMode);
        }

        /// <summary>
        /// Convert the Ring to its XML equivalent
        /// </summary>
        /// <returns></returns>
        public override XElement ToXML()
        {
            XElement P1 = CreateXML("LinearRing", Coordinates);
            return P1;
        }
    }

    /// <summary>
    /// A KML polygon is made of outer and optional inner rings
    /// </summary>
    public class KMLPolygon : KMLGeometry
    {
        public List<KMLRing> Rings;
        
        ~KMLPolygon()
        {
            Rings = null;
        }

        public KMLPolygon(int ID, KML.ExtrudeType Extrude, KML.AltitudeModeType AltitudeMode, 
                          KML.TesselateType Tesselate, List <VectorObject> Rings)
        {

            this.Rings = new List<KMLRing>(Rings.Count);
            
            //A single outer boundary
            KMLRing Boundary = new KMLRing(Const.INTUNDEFINED, Figure.OUTERRING, Rings[0]);
            this.Rings.Add(Boundary);

            //Could be zero or many inner boundaries (aka holes)
            for (int i = 1; i < Rings.Count; i++)
            {
                Boundary = new KMLRing(Const.INTUNDEFINED, Figure.INNERRING, Rings[i]);
                this.Rings.Add(Boundary);
            }

            Setup(ID, Extrude, Tesselate, AltitudeMode);
        }


        public override XElement ToXML()
        {
            //Basic items
            XElement P1 = CreateXML("Polygon");

            //Manage the outer boundary and its single ring
            P1.Add(new XElement("outerBoundaryIs", Rings[0].ToXML()));

            //Create the elements for the inner boundaries
            for (int i = 1; i < Rings.Count; i++)
                P1.Add(new XElement("innerBoundaryIs", Rings[i].ToXML()));
            return P1;
        }
    }

    /// <summary>
    /// A KML multiple geometry object
    /// </summary>
    public class KMLMultiGeometry : KMLGeometry
    {
        public List<KMLGeometry> Geometries = null;

        public KMLMultiGeometry()
        {
            Geometries = new List<KMLGeometry>();
            Setup();
        }

        public void AddGeometry(KMLGeometry Geo) 
        {
            Geometries.Add(Geo);
        }

        public override XElement ToXML()
        {
            XElement P1 = CreateXML("MultiGeometry");
            for (int i = 0; i < Geometries.Count; i++)
                P1.Add(Geometries[i].ToXML());
            return P1;
        }

        //public KMLPlaceMark PlaceMark(int ID, string Name, string Description)
        //{
        //    KMLPlaceMark Mark = new KMLPlaceMark();
        //    Mark.ID = ID;
        //    Mark.Name = Name;
        //    Mark.Description = Description;
        //    Mark.Geometry = this;
        //    return Mark;
        //}

    }

    /// <summary>
    /// A KML Document is a holder of placemarks (or folders) in their
    /// XML (KML) format.  When you want to write a placemark
    /// or set of placemarks to disk, you write a Document
    /// </summary>
    public class KMLDocument
    {
        public XElement XML;

        public KMLDocument()
        {
            XML = new XElement("Folder");
        }

        public void Add(KMLPlaceMark Mark)
        {
            XML.Add(Mark.ToXML());
        }

        /// <summary>
        /// Write the whole KML tree to the specific base path
        /// </summary>
        /// <param name="BasePath">The path to write to, no extension</param>
        /// <param name="CreateXML">Create an XML extension too, for ease in viewing</param>
        public void Write(string BasePath, bool CreateXML = true)
        {
            XmlWriterSettings Settings = new XmlWriterSettings();
            Settings.Indent = true;
            XmlWriter W = XmlWriter.Create(BasePath + ".KML", Settings);
            W.WriteRaw("<kml xmlns=\"http://www.opengis.net/kml/2.2\">");
            XML.WriteTo(W);
            W.WriteRaw("</kml>");
            W.Flush();
            W.Close();
            W = null;

            if (CreateXML)
            {
                W = XmlWriter.Create(BasePath + ".XML", Settings);
                W.WriteRaw("<kml xmlns=\"http://www.opengis.net/kml/2.2\">");
                XML.WriteTo(W);
                W.WriteRaw("</kml>");
                W.Flush();
                W.Close();
                W = null;
            }
        }
    }

    /// <summary>
    /// A placemark is a holder of geometry representing a spatial
    /// feature.  To a large extent, there is (I think) a one-to-one
    /// correspondence between placemark and shapefile feature
    /// </summary>
    public class KMLPlaceMark
    {
        public int ID;
        public KMLGeometry Geometry;
        public string Name;
        public string Description;

        public XElement ToXML()
        {
            XElement P1 = new XElement("Placemark", new XAttribute("id", ID.ToString()),
                new XElement("name", Name),
                new XElement("description", Description));
            P1.Add(Geometry.ToXML());
            return P1;
        }

        


    }
}
