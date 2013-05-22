using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryLib.Math.Primitives
{
    /// <summary>
    /// Represents a 3-dimensional plane, using the plane equation z = Ax + By + C
    /// </summary>
    public class Plane3
    {
        public double A { get; private set; }
        public double B { get; private set; }
        public double C { get; private set; }

        public Plane3(double a, double b, double c)
        {
            A = a;
            B = b;
            C = c;
        }
    }
}
