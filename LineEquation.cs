using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Carto
{
    /// <summary>
    ///Encapsulates basic functionality of a line equation in y = mx + b format
    ///Programmer:  Perry J. Hardin
    ///Last modified: February 4, 2013
    ///Version 3.01
    ///Usage:  As explained in class.  A simple class that contains a lot of concepts for learning purposes.  Some things are presented in a way that I would not choose for real-world coding, but help students by giving them a template of  a simple dynamic class
    ///Known bugs:  None
    ///Known limitations:  It may not stop you from doing strange things like setting the intercept to NAN or Infinity. 
    /// </summary>
    public class LineEquation
    {
        #region Data members and data accessor properties  ******************************
        /// <summary>
        /// To hold slope. A private data member.
        /// </summary>
        private double mSlope;

        /// <summary>
        /// Fully developed property with some quick (but useful) error checking
        /// </summary>
        /// <exception cref="(SET) System.ArgumentOutOfRangeException">Thrown when the slope is set to -/+ infinity or NaN</exception>
        public double Slope
        {
            get
            {
                return mSlope;
            }
            set //Slope must be a real number, not undefined
            {  
                if (double.IsNegativeInfinity(value)) throw new ArgumentOutOfRangeException("Line equation slope cannot be negative infinity");
                if (double.IsPositiveInfinity(value)) throw new ArgumentOutOfRangeException("Line equation slope cannot be positive infinity.");
                if (double.IsNaN(value)) throw new ArgumentOutOfRangeException("Line equation slope cannot be undefined."); 
                mSlope = value;
            }
        }

        /// <summary>
        /// The Y-Intercept of the linequation. Quick property, no error checking on input.
        /// </summary>
        public double Intercept { get; set; }  // You can have qualifiers on the get and set if you want 
        #endregion

        #region Private helper methods  ******************************
        /// <summary>
        /// Check to see if the slope is valid or not.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        private static bool SlopeOK(double x1, double y1, double x2, double y2)
        {
            if (x2 == x1) 
                return false;
            else
                return true;
        }
        #endregion

        #region Constructors and setup ******************************
        //Default constructor.
        /// <summary>
        /// Default constructor. Create a line equation where the slope is 1 and the intercept is zero
        /// </summary>
        public LineEquation()
        {
            Setup(1.0, 0.0);
        }

        //Constructor that accepts a slope and intercept
        /// <summary>
        /// Create a line equation by providing the slope and intercept
        /// </summary>
        /// <param name="Slope"></param>
        /// <param name="Intercept"></param>
        public LineEquation(double Slope, double Intercept) 
        {
            Setup(Slope, Intercept);
        }

        //Constructor that takes two x,y pairs
        /// <summary>
        /// Create a line equation by providing two x,y pairs
        /// </summary>
        /// <param name="X1"></param>
        /// <param name="Y1"></param>
        /// <param name="X2"></param>
        /// <param name="Y2"></param>
        public LineEquation(double X1, double Y1, double X2, double Y2)
        {
            Setup(X1, Y1, X2, Y2);  //Call setup
        }

        /// <summary>
        /// A setup routine that takes two x,y pairs
        /// </summary>
        /// <param name="X1"></param>
        /// <param name="Y1"></param>
        /// <param name="X2"></param>
        /// <param name="Y2"></param>
        public void Setup(double X1, double Y1, double X2, double Y2) 
        {
            if (!SlopeOK(X1, Y1, X2, Y2)) throw new ArgumentException("Line equation slope cannot be infinity");
            
            double NewSlope, NewIntercept;
            CalcParameters(X1, Y1, X2, Y2, out NewSlope, out NewIntercept);
            Slope = NewSlope;
            Intercept = NewIntercept;
        }
        
        //Setup routine taking a slope and intercept
        /// <summary>
        /// A setup routine that takes a slope and an intercept
        /// </summary>
        /// <param name="Slope"></param>
        /// <param name="Intercept"></param>
        public void Setup(double Slope, double Intercept)
        {
            this.Slope = Slope;
            this.Intercept = Intercept;
        }

        #endregion

        #region Destructors *********************************
        //Note that this class, because it does not allocate anything
        //using the new keyword does not really need a cleanup or 
        //destructor methods.  I am adding them just so you can have
        //an example of how they might work.

        /// <summary>
        /// Cleanup
        /// </summary>
        /// <param name="Equation">The line equation to clean up</param>
        public static void Cleanup(LineEquation Equation)
        {
            if (Equation == null) return;
            Equation.Slope = double.NaN;
            Equation.Intercept = double.NaN;
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        public void Cleanup()
        {
            Slope = double.NaN;
            Intercept = double.NaN;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~LineEquation()
        {
            Cleanup(this);
        }


        #endregion

        #region Public dynamic services  ******************************
        //When given an X, calculate a Y
        /// <summary>
        /// Calculate an Y from a given X. (X -> Y transformation)
        /// </summary>
        /// <param name="InX"></param>
        /// <returns></returns>
        public double Fwd(double InX)
        {
            return Slope * InX + Intercept;
        }

        //When given a Y, calculate an X
        /// <summary>
        /// Calculate a X from a given Y. (Y -> X transformation)
        /// </summary>
        /// <param name="InY"></param>
        /// <returns></returns>
        public double Inv(double InY)
        {
            return (InY - Intercept) / Slope;
        }

        //Return both the slope and intercept of the equation
        /// <summary>
        /// Get both the slope and intercept of the equation
        /// </summary>
        /// <param name="OutSlope"></param>
        /// <param name="OutIntercept"></param>
        public void GetParameters(out double OutSlope, out double OutIntercept)
        {
            OutSlope = Slope;
            OutIntercept = Intercept;
        }
        #endregion

        #region Public static services  ******************************
        
        //When given two pairs of X,Y, return the slope and intercept for the line
        /// <summary>
        /// Calculate the parameters of a line equation for a given pair of x,y coordinates
        /// </summary>
        /// <param name="X1"></param>
        /// <param name="Y1"></param>
        /// <param name="X2"></param>
        /// <param name="Y2"></param>
        /// <param name="OutSlope"></param>
        /// <param name="OutIntercept"></param>
        public static void CalcParameters(double X1, double Y1, double X2, double Y2,
                                          out double OutSlope, out double OutIntercept)
        {
            bool OK = SlopeOK(X1, Y1, X2, Y2);
            if (!OK) throw new Exception("Slope of line is undefined because X1 and X2 have the same value.");
            OutSlope = (Y2 - Y1) / (X2 - X1);
            OutIntercept = Y1 - OutSlope * X1;
        }
        #endregion
    }



    public class LabeledEquation : LineEquation
    {
        private string mLabel;

        public string Label
        {
            get
            {
                return mLabel;
            }

            set
            {
                value = value.Trim();
                if (value == string.Empty)
                {
                    throw new Exception("Label in LabeledEquation cannot be empty.");
                }
                else
                    mLabel = value;
            }

        }


        public LabeledEquation()
            : base()
        {
            Label = "None";
        }

        public LabeledEquation(double Slope, double Intercept, string Label)
            : base(Slope, Intercept)
        {
            this.Label = Label;
        }

        public LabeledEquation(double X1, double Y1, double X2, double Y2, string Label)
            : base(X1, Y1, X2, Y2)
        {
            this.Label = Label;
        }

        public void Setup(double Slope, double Intercept, string Label)
        {
            base.Setup(Slope, Intercept);
            Label = mLabel;
        }

        public void Setup(double X1, double Y1, double X2, double Y2, string Label)
        {
            base.Setup(X1, Y1, X2, Y2);
            Label = mLabel;
        }

    }

}
