using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryLib.Math.Primitives
{
    public class Vector3
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
