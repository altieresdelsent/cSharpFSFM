using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearAlgebra
{
    public struct LinearFunc
    {
        private double x1,
         y1,
         x2,
        y2,
        xMax,
        yMax,
        xMin,
        yMin,
        slope,
        slopeInv,
        bias,
        biasInv;

        public double BiasInv
        {
            get { return biasInv; }
        }

        public double Bias
        {
            get { return bias; }
        }

        public double SlopeInv
        {
            get { return slopeInv; }
        }

        public double Slope
        {
            get { return slope; }
        }

        public double YMin
        {
            get { return yMin; }
        }

        public double XMin
        {
            get { return xMin; }
        }

        public double YMax
        {
            get { return yMax; }
        }

        public double XMax
        {
            get { return xMax; }
        }

        public double Y2
        {
            get { return y2; }
        }

        public double X2
        {
            get { return x2; }
        }

        public double Y1
        {
            get { return y1; }
        }

        public double X1
        {
            get { return x1; }
        }
        private bool isXFixed,
        isYFixed;

        public bool IsXFixed
        {
            get { return isXFixed; }
        }

        public bool IsYFixed
        {
            get { return isYFixed; }
        }

        public LinearFunc(double x1, double y1, double x2, double y2)
        {
            double changeX = x1 - x2;
            double changeY = y1 - y2;

            this.xMax = Math.Max(x1, x2);
            this.yMax = Math.Max(y1, y2);
            this.xMin = Math.Min(x1, x2);
            this.yMin = Math.Min(y1, y2);

            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;

            if (changeX != 0.0d && changeY != 0)
            {
                this.slope = changeY / changeX;
                this.slopeInv = changeX / changeY;
                this.bias = this.y1 - this.slope * x1;
                this.biasInv = this.x1 - this.slopeInv * y1;
                this.isXFixed = false;
                this.isYFixed = false;
            }
            else if (changeX != 0.0d)
            {
                this.slope = 0.0d;
                this.slopeInv = double.MaxValue;
                this.bias = this.yMax;
                this.biasInv = double.MaxValue;
                this.isXFixed = false;
                this.isYFixed = true;
            }
            else if (changeY != 0.0d)
            {
                this.slope = double.MaxValue;
                this.slopeInv = 0.0d;
                this.bias = double.MaxValue;
                this.biasInv = xMax;
                this.isXFixed = true;
                this.isYFixed = false;
            }
            else
            {
                this.slope = 0.0d;
                this.slopeInv = 0.0d;
                this.bias = 0.0d;
                this.biasInv = 0.0d;
                this.isXFixed = true;
                this.isYFixed = true;
            }
        }
        public LinearFunc(double slope, double bias, double biasInv)
        {
            if (slope == 0.0d)
            {
                this.slope = 0.0d;
                this.yMax = double.MaxValue;
                this.yMin = double.MinValue;
                this.biasInv = biasInv;
                this.xMax = biasInv;
                this.xMin = biasInv;
                this.x1 = biasInv;
                this.x2 = biasInv;
                this.y1 = double.MaxValue;
                this.y2 = double.MinValue;
                this.bias = double.PositiveInfinity;
                this.slopeInv = double.PositiveInfinity;
                this.isXFixed = false;
                this.isYFixed = true;
            }
            else if (double.IsPositiveInfinity(slope) || slope == double.MaxValue)
            {
                this.slope = double.MaxValue;
                this.xMax = double.MaxValue;
                this.xMin = double.MinValue;
                this.bias = bias;
                this.yMax = bias;
                this.yMin = bias;
                this.y1 = bias;
                this.y2 = bias;
                this.x1 = double.MaxValue;
                this.x2 = double.MinValue;
                this.biasInv = double.PositiveInfinity;
                this.slopeInv = 0.0d;
                this.isXFixed = true;
                this.isYFixed = false;
            }
            else
            {
                this.slope = slope;
                this.slopeInv = 1.0d / slope;
                this.bias = bias;
                this.biasInv = biasInv;
                if (this.slope > 0.0d)
                {
                    if (this.slope > 1.0d)
                    {
                        this.yMax = double.MaxValue - Math.Abs(biasInv);
                        this.xMax = this.yMax * this.slopeInv + this.biasInv;

                        this.yMin = double.MinValue + Math.Abs(this.biasInv);
                        this.xMin = this.yMin * this.slopeInv + this.biasInv;
                    }
                    else if (slope < 1.0d)
                    {
                        this.xMax = double.MaxValue - Math.Abs(this.bias);
                        this.yMax = this.xMax * this.slope + this.bias;

                        this.xMin = double.MinValue + Math.Abs(this.bias);
                        this.yMin = this.xMin * this.slope + this.bias;

                    }
                    else
                    {
                        this.xMax = double.MaxValue - Math.Abs(this.bias);
                        this.yMax = double.MaxValue - Math.Abs(this.bias);

                        this.xMin = double.MinValue + Math.Abs(this.bias);
                        this.yMin = double.MinValue + Math.Abs(this.bias);
                    }
                    this.x1 = this.xMax;
                    this.y1 = this.yMax;
                    this.x2 = this.xMin;
                    this.y2 = this.yMin;
                }
                else
                {
                    if (this.slope < -1.0d)
                    {
                        this.yMax = double.MaxValue - Math.Abs(biasInv);
                        this.xMin = this.yMax * this.slopeInv + this.biasInv;

                        this.yMin = double.MinValue + Math.Abs(this.biasInv);
                        this.xMax = this.yMin * this.slopeInv + this.biasInv;
                    }
                    else if (slope > -1.0d)
                    {
                        this.xMax = double.MaxValue - Math.Abs(this.bias);
                        this.yMin = this.xMax * this.slope + this.bias;

                        this.xMin = double.MinValue + Math.Abs(this.bias);
                        this.yMax = this.xMin * this.slope + this.bias;

                    }
                    else
                    {
                        this.xMax = double.MaxValue - Math.Abs(this.bias);
                        this.yMax = double.MaxValue - Math.Abs(this.bias);

                        this.xMin = double.MinValue + Math.Abs(this.bias);
                        this.yMin = double.MinValue + Math.Abs(this.bias);
                    }
                    this.x1 = this.xMax;
                    this.y1 = this.yMin;
                    this.x2 = this.xMin;
                    this.y2 = this.yMax;
                }
                this.isXFixed = false;
                this.isYFixed = false;
            }
        }
        public LinearFunc(double x1, double y1, double radianAngle, bool mark)
        {
            this.slope = Math.Tan(radianAngle);
            this.x1 = x1;
            this.y1 = y1;
            var pi = Math.PI;
            var quadrant = (radianAngle > 0.0d && radianAngle < pi / 2) ? 1.0d : 0.0d;
            quadrant = (radianAngle > pi / 2 && radianAngle < pi) ? 2.0d : 0.0d;
            quadrant = (radianAngle > pi && radianAngle < pi / 2 + pi) ? 3.0d : 0.0d;
            quadrant = (radianAngle > pi / 2 + pi && radianAngle < pi) ? 4.0d : 0.0d;
            var xGrows = false;
            var yGrows = false;
            if (quadrant == 1.0d)
            {
                xGrows = true;
                yGrows = true;
            }
            else if (quadrant == 2.0d)
            {
                xGrows = false;
                yGrows = true;
            }
            else if (quadrant == 3.0d)
            {
                xGrows = false;
                yGrows = false;
            }
            else if (quadrant == 4.0d)
            {
                xGrows = true;
                yGrows = false;
            }
            else
            {
                if (radianAngle == 0.0d)
                {
                    xGrows = true;
                    this.slope = 0.0d;
                }
                else if (radianAngle == pi / 2.0d)
                {
                    yGrows = true;
                    this.slope = double.MaxValue;
                }
                else if (radianAngle == (pi / 2.0d + pi))
                {
                    xGrows = false;
                    this.slope = 0.0d;
                }
                else if (radianAngle == pi * 2.0d)
                {
                    yGrows = false;
                    this.slope = double.MaxValue;
                }
            }
            if (this.slope == 0.0d)
            {
                this.slope = 0.0d;
                this.bias = double.MaxValue;
                this.slopeInv = double.MaxValue;

                this.yMax = this.y1;
                this.yMin = this.y1;
                this.biasInv = this.y1;
                this.xMax = (xGrows) ? double.MaxValue : x1;
                this.xMin = (!xGrows) ? double.MinValue : x1;

                this.isXFixed = false;
                this.isYFixed = true;

                this.x1 = xGrows ? xMax : xMin;
                this.x2 = !xGrows ? xMax : xMin;

                this.y1 = yGrows ? yMax : yMin;
                this.y2 = !yGrows ? yMax : yMin;
            }
            else if (this.slope == double.MaxValue)
            {
                this.slope = double.MaxValue;
                this.slopeInv = 0.0d;
                this.biasInv = double.MaxValue;
                this.xMax = this.x1;
                this.xMin = this.x1;
                this.bias = this.x1;
                this.yMax = (yGrows) ? double.MaxValue : this.y1;
                this.yMin = (!yGrows) ? double.MinValue : this.y1;
                this.isXFixed = true;
                this.isYFixed = false;

                this.x1 = xGrows ? this.xMax : this.xMin;
                this.x2 = !xGrows ? this.xMax : this.xMin;

                this.y1 = yGrows ? this.yMax : this.yMin;
                this.y2 = !yGrows ? this.yMax : this.yMin;
            }
            else
            {
                this.slopeInv = 1.0d / this.slope;

                this.bias = this.y1 - this.slope * this.x1;
                this.biasInv = this.x1 - this.slopeInv * this.y1;

                this.isXFixed = false;
                this.isYFixed = false;
                if (this.slope > 1.0d)
                {
                    this.yMax = (yGrows) ? double.MaxValue - Math.Abs(this.biasInv) : this.y1;
                    this.xMax = (xGrows) ? this.yMax * this.slopeInv + this.biasInv : this.x1;

                    this.yMin = (!yGrows) ? double.MinValue + Math.Abs(this.biasInv) : this.y1;
                    this.xMin = (!xGrows) ? this.yMin * this.slopeInv + this.biasInv : this.x1;

                    this.x1 = xGrows ? this.xMax : this.xMin;
                    this.x2 = !xGrows ? this.xMax : this.xMin;

                    this.y1 = yGrows ? this.yMax : this.yMin;
                    this.y2 = !yGrows ? this.yMax : this.yMin;
                }
                else if (slope < 1.0d)
                {
                    this.xMax = (xGrows) ? double.MaxValue - Math.Abs(this.bias) : this.x1;
                    this.yMax = (yGrows) ? this.xMax * this.slope + this.bias : this.y1;

                    this.xMin = (!xGrows) ? double.MinValue + Math.Abs(this.bias) : this.x1;
                    this.yMin = (!yGrows) ? this.xMin * this.slope + this.bias : this.y1;

                    this.x1 = xGrows ? this.xMax : this.xMin;
                    this.x2 = !xGrows ? this.xMax : this.xMin;

                    this.y1 = yGrows ? this.yMax : this.yMin;
                    this.y2 = !yGrows ? this.yMax : this.yMin;

                }
                else
                {
                    this.xMax = (xGrows) ? double.MaxValue - Math.Abs(this.bias) : this.x1;
                    this.yMax = (yGrows) ? double.MaxValue - Math.Abs(this.bias) : this.y1;

                    this.xMin = (!xGrows) ? double.MinValue + Math.Abs(this.bias) : this.x1;
                    this.yMin = (!yGrows) ? double.MinValue + Math.Abs(this.bias) : this.y1;

                    this.x1 = xGrows ? this.xMax : this.xMin;
                    this.x2 = !xGrows ? this.xMax : this.xMin;

                    this.y1 = yGrows ? this.yMax : this.yMin;
                    this.y2 = !yGrows ? this.yMax : this.yMin;
                }
            }
        }
        public static Point? getEncounterPoint(LinearFunc first, LinearFunc second)
        {
            double x;
            double y;

            if (!first.isXFixed && !first.isYFixed && !second.isXFixed && !second.isYFixed)
            {
                //if enter here, means the no slope are zeros, and we can solve using normal equation solution
                //check if both lines are the same line and the range touch of one touch the another
                if (first.slope == second.slope && first.bias == second.bias && ((first.xMax <= second.xMax && first.xMax >= second.xMin) || (first.xMin <= second.xMax && first.xMin >= second.xMin)))
                {
                    return null;
                }
                //check if both line are the same line
                else if (second.slope == first.slope && first.bias == second.bias)
                {
                    return null;
                }
                //check if both lines are parallel
                else if (second.slope == first.slope)
                {
                    return null;
                }
                // finally solve the equation system
                // first.slope*x + first.bias = y
                // second.slope*x + second.bias = y
                // by the magic of high school math it can be transforme in
                // first.slope*x + first.bias = second.slope*x + second.bias
                // and
                // first.slope*x  = second.slope*x + second.bias  - first.bias
                // and
                // first.slope*x - second.slope*x  = second.bias  - first.bias
                // and
                // x * (first.slope - second.slope) = second.bias  - first.bias
                // and FINALLY
                // x = (second.bias - first.bias)/(first.slope-second.slope)
                // after solve the "x" unknown, just put x in any of the equations
                // like
                // y = x*first.slope + first.bias
                // and we have x and y
                else
                {
                    x = (second.bias - first.bias) / (first.slope - second.slope);
                    y = x * first.slope + first.bias;

                }
            }
            //Test if both linear func are points
            else if (first.isXFixed && first.isYFixed && second.isXFixed && second.isYFixed)
            {
                //if they are the same points, they are visible one to other and no one is
                //blocking the vision
                if (first.xMax == second.xMax && first.yMax == second.yMax)
                {
                    x = first.xMax;
                    y = first.yMax;
                    return new Point(x,y);
                }
                else
                    //otherwise they have no solution, but still dont block the vision
                    return null;
            }
            //if first is a point, check is that point is part of the line of the second
            else if (first.isXFixed && first.isYFixed)
            {
                y = first.xMax * second.slope + second.bias;
                x = first.xMax;
                //if it's not, it will return that it's not blocking the vision, if it is, will
                //load y and x to later check if they are in the range.
                if (y != first.yMax)
                    return null;
            }
            //the same check but now with second
            else if (second.isXFixed && second.isYFixed)
            {
                y = second.xMax * first.slope + first.bias;
                x = second.xMax;
                if (y != second.yMax)
                    return null;
            }
            // if first and second are perpendicular it's easy to calc the encounter point
            else if (first.isXFixed && second.isYFixed)
            {
                x = first.xMax;
                y = second.yMax;
            }
            // if first and second are perpendicular it's easy to calc the encounter point( again, hehe)
            else if (first.isYFixed && second.isXFixed)
            {
                x = second.xMax;
                y = first.yMax;
            }
            //if first and second are parallel horizontaly
            else if (first.isXFixed && second.isXFixed)
            {
                //check if they are the same line, and also check if the range of the two lines crosses one another
                //if yes, they are blocking the vision
                if (first.xMax == second.xMax && ((first.yMax < second.yMax && first.yMax > second.yMin) || (first.yMin < second.yMax && first.yMin > second.yMin) || (second.yMin < first.yMax && second.yMin > first.yMin)))
                    return null;
                //otherwise still check if they are the same line, but they are not blocking the vision and systems have many solutions
                else if (first.xMax == second.xMax)
                    return null;
                else
                    // if they are not the same line, there is no solution
                    return null;
            }
            //if first and second are parallel vertically, do the same check before
            else if (first.isYFixed && second.isYFixed)
            {
                if (first.yMax == second.yMax && ((first.xMax < second.xMax && first.xMax > second.xMin) || (first.xMin < second.xMax && first.xMin > second.xMin) || (second.xMin < first.xMax && second.xMin > first.xMin)))
                    return null;
                else if (first.yMax == second.yMax)
                    return null;
                else
                    return null;
            }
            //if it is here, the two lines are not fixed, so check
            //if one is fixed, if it is, keep the variable who is fixed, fixed
            // and run the linear func to calc the y point
            else if (first.isXFixed)
            {
                x = first.xMax;
                y = first.xMax * second.slope + second.bias;
            }
            // the same thing as before
            else if (first.isYFixed)
            {
                x = first.yMax * second.slopeInv + second.biasInv;
                y = first.yMax;
            }
            // the same thing as before ( again)
            else if (second.isXFixed)
            {
                x = second.xMax;
                y = second.xMax * first.slope + first.bias;
            }
            // the same thing as before ( again II)
            else if (second.isYFixed)
            {
                x = second.yMax * first.slopeInv + first.biasInv;
                y = second.yMax;
            }
            else
            {
                return null;
                //warn("if falls here, it will create a black hole and destroy all humankind")
            }
            return new Point(x, y);
        }
        public static bool IsTheretAEncounterAtTheRange(LinearFunc first, LinearFunc second)
        {
            double x = 0.0d;
            double y = 0.0d;

            if (!first.isXFixed && !first.isYFixed && !second.isXFixed && !second.isYFixed)
            {
                //if enter here, means the no slope are zeros, and we can solve using normal equation solution
                //check if both lines are the same line and the range touch of one touch the another
                if (first.slope == second.slope && first.bias == second.bias && ((first.xMax <= second.xMax && first.xMax >= second.xMin) || (first.xMin <= second.xMax && first.xMin >= second.xMin)))
                {
                    return true;
                }
                //check if both line are the same line
                else if (second.slope == first.slope && first.bias == second.bias)
                {
                    return false;
                }
                //check if both lines are parallel
                else if (second.slope == first.slope)
                {
                    return false;
                }
                // finally solve the equation system
                // first.slope*x + first.bias = y
                // second.slope*x + second.bias = y
                // by the magic of high school math it can be transforme in
                // first.slope*x + first.bias = second.slope*x + second.bias
                // and
                // first.slope*x  = second.slope*x + second.bias  - first.bias
                // and
                // first.slope*x - second.slope*x  = second.bias  - first.bias
                // and
                // x * (first.slope - second.slope) = second.bias  - first.bias
                // and FINALLY
                // x = (second.bias - first.bias)/(first.slope-second.slope)
                // after solve the "x" unknown, just put x in any of the equations
                // like
                // y = x*first.slope + first.bias
                // and we have x and y
                else
                {
                    x = (second.bias - first.bias) / (first.slope - second.slope);
                    y = x * first.slope + first.bias;

                }
            }
            //Test if both linear func are points
            else if (first.isXFixed && first.isYFixed && second.isXFixed && second.isYFixed)
            {
                //if they are the same points, they are visible one to other and no one is
                //blocking the vision
                if (first.xMax == second.xMax && first.yMax == second.yMax)
                {
                    x = first.xMax;
                    y = first.yMax;
                    return false;
                }
                else
                    //otherwise they have no solution, but still dont block the vision
                    return false;
            }
            //if first is a point, check is that point is part of the line of the second
            else if (first.isXFixed && first.isYFixed)
            {
                y = first.xMax * second.slope + second.bias;
                x = first.xMax;
                //if it's not, it will return that it's not blocking the vision, if it is, will
                //load y and x to later check if they are in the range.
                if (y != first.yMax)
                    return false;
            }
            //the same check but now with second
            else if (second.isXFixed && second.isYFixed)
            {
                y = second.xMax * first.slope + first.bias;
                x = second.xMax;
                if (y != second.yMax)
                    return false;
            }
            // if first and second are perpendicular it's easy to calc the encounter point
            else if (first.isXFixed && second.isYFixed)
            {
                x = first.xMax;
                y = second.yMax;
            }
            // if first and second are perpendicular it's easy to calc the encounter point( again, hehe)
            else if (first.isYFixed && second.isXFixed)
            {
                x = second.xMax;
                y = first.yMax;
            }
            //if first and second are parallel horizontaly
            else if (first.isXFixed && second.isXFixed)
            {
                //check if they are the same line, and also check if the range of the two lines crosses one another
                //if yes, they are blocking the vision
                if (first.xMax == second.xMax && ((first.yMax < second.yMax && first.yMax > second.yMin) || (first.yMin < second.yMax && first.yMin > second.yMin) || (second.yMin < first.yMax && second.yMin > first.yMin)))
                    return true;
                //otherwise still check if they are the same line, but they are not blocking the vision and systems have many solutions
                else if (first.xMax == second.xMax)
                    return false;
                else
                    // if they are not the same line, there is no solution
                    return false;
            }
            //if first and second are parallel vertically, do the same check before
            else if (first.isYFixed && second.isYFixed)
            {
                if (first.yMax == second.yMax && ((first.xMax < second.xMax && first.xMax > second.xMin) || (first.xMin < second.xMax && first.xMin > second.xMin) || (second.xMin < first.xMax && second.xMin > first.xMin)))
                    return true;
                else if (first.yMax == second.yMax)
                    return false;
                else
                    return false;
            }
            //if it is here, the two lines are not fixed, so check
            //if one is fixed, if it is, keep the variable who is fixed, fixed
            // and run the linear func to calc the y point
            else if (first.isXFixed)
            {
                x = first.xMax;
                y = first.xMax * second.slope + second.bias;
            }
            // the same thing as before
            else if (first.isYFixed)
            {
                x = first.yMax * second.slopeInv + second.biasInv;
                y = first.yMax;
            }
            // the same thing as before ( again)
            else if (second.isXFixed)
            {
                x = second.xMax;
                y = second.xMax * first.slope + first.bias;
            }
            // the same thing as before ( again II)
            else if (second.isYFixed)
            {
                x = second.yMax * first.slopeInv + first.biasInv;
                y = second.yMax;
            }
            else
            {
                return false;
                //warn("if falls here, it will create a black hole and destroy all humankind")
            }

            // if it's here, means the both lines are not parallel, neither the same line
            // so checks if the only solution to the system are inside of the range of both lines
            // if it is, it means that the lines are blocking the vision one from another
            if (x <= first.xMax && x >= first.xMin && y <= first.yMax && y >= first.yMin && x <= second.xMax && x >= second.xMin && y <= second.yMax && y >= second.yMin)
            {
                //the only exception is if the encounter point is exactly the start or the end of any of the lines
                // it could consider that as blocking the vision, but the way it's now, it's easier to check if
                // two obstacles are crossing one another and also the visibility problem, so I solve two problems with one algorithm
                if ((x == first.x1 && y == first.y1) || (x == first.x2 && y == first.y2) || (x == second.x1 && y == second.y1) || (x == second.x2 && y == second.y2))
                    return false;
                else
                    return true;
            }
            // otherwise the system has one solution and line intersection are out of the range
            else
            {
                return false;
            }
        }
    }
}
