using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearAlgebra
{
    public struct Point
    {
        public double X{get; private set;}
        public double Y { get; private set; }
        public Point(double x,double y) : this()
        {
            this.X = x;
            this.Y = y;
        }
    }
}
