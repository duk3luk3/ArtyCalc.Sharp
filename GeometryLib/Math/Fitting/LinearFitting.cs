using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryLib.Math.Primitives;

namespace GeometryLib.Math.Fitting
{
    public class LinearFitting
    {
        /// <summary>
        /// Least Squares Fit of 3D points to a plane, according to http://www.geometrictools.com/Documentation/LeastSquaresFitting.pdf
        /// </summary>
        /// <param name="points">The 3d points to fit</param>
        /// <param name="error">the residual error</param>
        /// <returns>The best-fit plane</returns>
        public static Plane3 FitPlane(IEnumerable<Vector3> points, out double error)
        {
            Matrix m = new Matrix(new int[]{3,3});
            double[] b = new double[3];

            double m00 = 0;
            double m01 = 0;
            double m02 = 0;

            double m10 = 0;
            double m11 = 0;
            double m12 = 0;

            double m20 = 0;
            double m21 = 0;
            double m22 = 0;

            foreach (var p in points)
            {
                m00 += p.X * p.X;
                m01 += p.X * p.Y;
                m02 += p.X;
              
                m11 += p.Y * p.Y;
                m12 += p.Y;

                m22++;

                b[0] += p.X * p.Z;
                b[1] += p.Y * p.Z;
                b[2] += p.Z;
            }

            m10 = m01;
            m20 = m02;
            m21 = m12;

            m.M[0][0] = m00;
            m.M[0][1] = m01;
            m.M[0][2] = m02;
            
            m.M[1][0] = m10;
            m.M[1][1] = m11;
            m.M[1][2] = m12;
            
            m.M[1][0] = m20;
            m.M[1][1] = m21;
            m.M[1][2] = m22;

            double[] x;

            m.Gauss(b, true, out x);

            var r = new Plane3(x[0], x[1], x[2]);

            double e = 0;

            foreach (var p in points)
            {
                e += (r.A * p.X + r.B * p.Y + r.C - p.Z) * (r.A * p.X + r.B * p.Y + r.C - p.Z);
            }

            error = e;
            return r;
        }
    }
}
