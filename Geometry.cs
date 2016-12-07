using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Carto     
{
    /// <summary>
    /// The class which holds the x,y,z or longititude,
    /// latitude, altitude information for a single vertex
    /// The class is expandable to hold any number of dimensions
    /// although that expands the complexity of the class a bit
    /// </summary>
    public class Coordinate
    {
        public double[] XYZM;

        public double Latitude
        {
            get
            {
                return XYZM[Const.LATITUDE];
            }
            set
            {
                XYZM[Const.LATITUDE] = value;
            }
        }

        public double Longitude
        {
            get
            {
                return XYZM[Const.LONGITUDE];
            }
            set
            {
                XYZM[Const.LONGITUDE] = value;
            }
        }

        public double Altitude
        {
            get
            {
                return XYZM[Const.ALTITUDE];
            }
            set
            {
                XYZM[Const.ALTITUDE] = value;
            }
        }

        public double M
        {
            get
            {
                return XYZM[Const.M];
            }
            set
            {
                XYZM[Const.M] = value;
            }
        }

        public int NumDimensions
        {
            get { return XYZM.Length; }
        }

        public int MaxDimension
        {
            get { return XYZM.Length - 1; }
        }

        public double this[int Index]  
        {
            get
            {
                if ((Index < 0) || (Index > MaxDimension)) throw new Exception("Zero-based index to coordinate parts (e.g. X, Y, Z) is out of range.");
                return XYZM[Index];
            }
            set
            {
                if ((Index < 0) || (Index > MaxDimension)) throw new Exception("Zero-based Index to coordinate parts (e.g. X, Y, Z) is out of range.");
                XYZM[Index] = value;
            }
        }

        public static bool AreSame(Coordinate C1, Coordinate C2)
        {
            if (C1.MaxDimension != C2.MaxDimension) return false;
            for (int i = 0; i <= C1.MaxDimension; i++)
            {
                if (C2.XYZM[i] != C1.XYZM[i]) return false;
            }
            return true;
        }

        ~Coordinate()
        {
            XYZM = null;
        }

        public Coordinate(int NumDimensions)
        {
            XYZM = new double[NumDimensions];
        }

        public Coordinate (double[] XYZM) : this(XYZM.Length)
        {
            Array.Copy(XYZM, this.XYZM, XYZM.Length);
        }

        public Coordinate (Coordinate Srce) : this(Srce.XYZM) {}
       
        public override string ToString()
        {
            if (NumDimensions == 2)
                return this[Const.LONGITUDE].ToString() + "," + this[Const.LATITUDE].ToString();
            else if (NumDimensions == 3)
                return this[Const.LONGITUDE].ToString() + "," + this[Const.LATITUDE].ToString() + "," + this[Const.ALTITUDE].ToString();
            else
                throw new Exception("Cannot convert to string a coordinate with more than three dimensions.  You need to write it.");
        }

        public void Read(BinReader Rdr)
        {
            for (int i = 0; i <= MaxDimension; i++)
                XYZM[i] = Rdr.ReadDouble();
        }

        public static double Distance(Coordinate C1, Coordinate C2, CoordSpace TheCoordSpace, int NumDimensions = 2)
        {
            if ((TheCoordSpace == CoordSpace.ELLIPSOIDAL) && (NumDimensions != 2))
                throw new Exception("Geometry.Distance reports that distance on the ellipsoid can only be calculated in two dimensions");

            //TODO Change format of exceptions like the one above 

            if (TheCoordSpace == CoordSpace.PLANAR)
            {
                double SumDiffSq = 0.0;
                for (int i = 0; i < NumDimensions; i++)
                {
                    double Diff = C1.XYZM[i] - C2.XYZM[2];
                    SumDiffSq = SumDiffSq + Diff * Diff;
                }
                return Math.Sqrt(SumDiffSq);
            }

            if (TheCoordSpace == CoordSpace.ELLIPSOIDAL)
            {
                //TODO Finish this
            }

            return Const.DBLUNDEFINED;
        }  
    }

    /// <summary>
    /// An easier to use two-dimensional coordinate
    /// </summary>
    public class XYPoint : Coordinate
    {
        public XYPoint() : base(2) { }

        public double X
        {
            set { base[Const.X] = value; }
            get { return base[Const.X]; }
        }

        public double Y
        {
            set { base[Const.Y] = value; }
            get { return base[Const.Y]; }
        }
    }

    /// <summary>
    /// An easier to use UTM easting and northing record
    /// </summary>
    public class CivilUTM : XYPoint
    {
        public byte ZoneNumber { get; set; }
        public char ZoneLetter { get; set; }
        public char HemiSphere { get; set; }
        
        public CivilUTM() : base() { }

        public double Easting
        {
            set { base[Const.X] = value; }
            get { return base[Const.X]; }
        }

        public double Northing
        {
            set { base[Const.Y] = value; }
            get { return base[Const.Y]; }
        }
    }

    /// <summary>
    /// A box is just coordinates that demarcate the min and max limits in X and Y
    /// </summary>
    public class Box
    {
        public Coordinate Mins = null;
        public Coordinate Maxs = null;

        public Box(int NumDimensions)
        {
            Mins = new Coordinate(NumDimensions);
            Maxs = new Coordinate(NumDimensions);
        }
    }

    /// <summary>
    /// A vector object stores a series of coordinates, which
    /// can have different meanings depending on their 
    /// arrangement.
    /// </summary>
    public class VectorObject : List<Coordinate>
    {
        /// <summary>
        /// Three things, which when combined differently create the 
        /// distinction between vector objects in KML and ShapeFile form
        /// </summary>
        public Closure Closure { get; private set; }
        public PolyDirection PolyDirection { get; private set;  }
        public Figure VectorType { get; private set; }

       /// <summary>
       /// Constructor for generic vector object
       /// </summary>
       /// <param name="NumPoints">The number of 'shape vertices' in the object</param>
        public VectorObject(int NumPoints)
            : base(NumPoints)
        {
            Closure = Closure.UNKNOWN;
            PolyDirection = PolyDirection.UNKNOWN;
            VectorType = Figure.UNKNOWN;
        }

        /// <summary>
        /// After creation, we need to set information about the type of vector object
        /// </summary>
        /// <param name="VectorType">The type of vector</param>
        /// <param name="PolyDirection">Which traverse direction (if any) is required for object?</param>
        /// <param name="Closure">"Is the object supposed to be closed or unclosed?</param>
        public void Assign(Figure VectorType, PolyDirection PolyDirection, Closure Closure)
        {
            this.VectorType = VectorType;
            this.PolyDirection = PolyDirection;
            this.Closure = Closure;
            EnforceClosure();
            EnforcePolyDirection();
        }

        /// <summary>
        /// By comparing the first and last vertices, checks to see if the object is closed.
        /// </summary>
        public bool IsClosed
        {
            get { return Coordinate.AreSame(this[0], this[this.Count - 1]); }
        }

        /// <summary>
        /// Checks for closure, and if not closed, adds the first vertex to the end of the
        /// vertex list.
        /// </summary>
        public void EnforceClosure()
        {
            if (Closure != Closure.CLOSED) return;
            if (IsClosed) return;
            this.Add(this[0]);
        }

        /// <summary>
        /// Compares current polygon traverse direction with what it should be.  If 
        /// different, reverses the vertex list so it is correct.
        /// </summary>
        public void EnforcePolyDirection()
        {
            if (!PolyDirection.ToString().Contains("WISE")) return;
            if (!IsClosed) return;
            PolyDirection Current = CalcPolyDirection();
           if (Current != PolyDirection) this.Reverse();
        }

        /// <summary>
        /// Calculates the area of a polygon
        /// </summary>
        /// <returns>The area in the input units</returns>
        public double CalcArea()
        {
            if (Closure != Closure.CLOSED) return Const.DBLUNDEFINED; 
            int Vertices = this.Count;
            double Sum = 0.0;
            for (int i = 0; i < Vertices - 1; i++)
            {
                double Diff_X = this[i + 1][Const.X] - this[i][Const.X];
                double Diff_Y = this[i][Const.Y] + this[i + 1][Const.Y];
                Sum += Diff_X * Diff_Y;
            }
            return Sum;
        }

        /// <summary>
        /// By looking at the vertices determines the traverse direction
        /// of the polygon
        /// </summary>
        /// <returns></returns>
        public PolyDirection CalcPolyDirection()
        {
            if (Closure != Closure.CLOSED) 
                return PolyDirection.NOTAPPLICABLE;
            double RingArea = CalcArea();
            if (RingArea < 0.0) return PolyDirection.COUNTERCLOCKWISE;
            if (RingArea > 0.0) return PolyDirection.CLOCKWISE;
            return PolyDirection.UNKNOWN;
        }

        /// <summary>
        /// Makes a deep copy of the vertices.  Copies nothing else 
        /// </summary>
        /// <param name="Vector">Source vector from which to copy the vertices</param>
        public void CopyVerticesFrom(VectorObject Vector)
        {
            foreach (Coordinate C in Vector) Add(C);
        }

        /// <summary>
        /// Creates a string from the vertices in the vector
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder Str = new StringBuilder();
            for (int i = 0; i < Count; i++)
            {
                Str.Append(this[i].ToString());
                Str.Append(" ");
            }
            return Str.ToString();
        }
    }
}







