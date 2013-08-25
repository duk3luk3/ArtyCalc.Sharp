using System;
using System.Collections.Generic;
using System.Text;

namespace GeometryLib.Math
{
    class Matrix
    {
        public double[][] M { get; private set; }


        public int[] Dimension { get; private set; }
        

        public Matrix(int[] dimension)
        {
            if (dimension.Length != 2)
                throw new InvalidOperationException("Matrix dimension must be 2");

            this.Dimension = dimension;

            M = new double[Dimension[0]][];
            for (int i = 0; i < Dimension[0]; i++)
                M[i] = new double[Dimension[1]];
        }

        public Matrix GetTransposed()
        {
            Matrix r = new Matrix(new int[] { Dimension[1], Dimension[0] });

            for (int i = 0; i < Dimension[0]; i++)
                for (int k = 0; k < Dimension[1]; i++)
                {
                    r.M[i][k] = M[k][i];
                }

            return r;
        }

        public static Matrix Zero(int[] dimension)
        {
            Matrix m = new Matrix(dimension);

            for (int i = 0; i < m.Dimension[0]; i++)
                for (int k = 0; k < m.Dimension[1]; i++)
                {
                    m.M[i][k] = 0;
                }

            return m;
        }

        /*public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{");

            for (int i = 0; i < Dimension[0]; i++)
            {
                sb.Append("{");
                for (int k = 0; k < Dimension[1]; k++)
                {
                    sb.Append(M[i][k]);
                    sb.Append(",");
                }
                sb.Append("},");
            }
            sb.Append("}");

            return sb.ToString();
        }*/

        public override string ToString()
        {
            string r = "{";
            for (int i = 0; i < Dimension[0]; i++)
            {
                r += "{";

                for (int k = 0; k < Dimension[1]; k++)
                {
                    r += M[i][k];
                    if (k < Dimension[1] - 1)
                        r += ", ";
                }

                r += "}";
                if (i < Dimension[0] - 1)
                    r += ", ";
                r += "\n";

            }
            r += "}";

            return r;
        }

        public void Gauss(double[] b, bool destroy, out double[] x)
        {
            if (b.Length != Dimension[1])
                throw new InvalidOperationException("parameter vector has wrong size");

            if (Dimension[0] != Dimension[1])
                throw new InvalidOperationException("Square matrix required for gauss algorithm");

            double[][] a;
            if (destroy)
            {
                a = M;
            }
            else
            {
                a = new double[Dimension[0]][];
                for (int i = 0; i < Dimension[0]; i++)
                    a[i] = new double[Dimension[1]];

                Array.Copy(M, a, Dimension[0] * Dimension[1]);
            }

            System.Console.WriteLine(this.ToString());

            for (int i = 0; i < Dimension[0]; i++)
            {
                //find pivot
                double max = 0;
                int p = -1;

                for (int j = i; j < Dimension[0]; j++) //j = [i to n-1]
                {
                    if (System.Math.Abs(a[j][i]) > max)
                    {
                        max = System.Math.Abs(a[j][i]);
                        p = j;
                    }
                }

                if (p == -1)
                    throw new InvalidOperationException("Matrix is not regular");

                if (p != i)
                {
                    //swap rows
                    double[] tmp = a[p];
                    a[p] = a[i];
                    a[i] = tmp;

                    //swap params
                    double s = b[p];
                    b[p] = b[i];
                    b[i] = s;
                }

                double pivot = a[i][i];

                //eliminate
                for (int j = i + 1; j < Dimension[0]; j++) // j = [i+1 to n-1]
                {
                    double factor = a[j][i] / pivot;
                    b[j] = b[j] - factor * b[i];
                    for (int k = i + 1; k < Dimension[0]; k++) // k = [i+1 to n-1]
                    {
                        a[j][k] = a[j][k] - factor * a[i][k];
                    }
                }
            }

            x = new double[b.Length];

            for (int i = Dimension[0] - 1; i >= 0; i--) // i = [n-1 downto 0]
            {
                double sum = 0;
                for (int j = i + 1; j < Dimension[0]; j++) // j = [i+1 to n-1]
                {
                    sum = sum + a[i][j] * x[j];
                }
                x[i] = (b[i] - sum) / a[i][i];
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Matrix))
                return false;

            Matrix a = this;
            Matrix b = obj as Matrix;

            if (a.Dimension[0] != b.Dimension[0] || a.Dimension[1] != b.Dimension[1])
                return false;

            for (int i = 0; i < a.Dimension[0]; i++)
                for (int j = 0; j < a.Dimension[1]; j++)
                {
                    if (a.M[i][j] != b.M[i][j])
                        return false;
                }

            return true;
        }

        public override int GetHashCode()
        {
            return Dimension.GetHashCode() + M.GetHashCode();
        }

        /*public static bool operator ==(Matrix a, Matrix b)
        {
            if (a.Dimension[0] != b.Dimension[0] || a.Dimension[1] != b.Dimension[1])
                return false;

            for(int i=0;i<a.Dimension[0];i++)
                for (int j = 0; j < a.Dimension[1]; j++)
                {
                    if (a.M[i, j] != b.M[i, j])
                        return false;
                }

            return true;
        }

        public static bool operator !=(Matrix a, Matrix b)
        {
            return !(a == b);
        }*/

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Dimension[1] != b.Dimension[0])
                throw new InvalidOperationException("Wrong form, cannot multiply");

            Matrix r = new Matrix(new int[] {a.Dimension[0],b.Dimension[1] });

            for(int i=0;i<r.Dimension[0];i++)
                for (int j = 0; j < r.Dimension[1]; j++)
                {
                    double s = 0;
                    for (int k = 0; k < a.Dimension[1]; k++)
                    {
                        s += a.M[i][k] * b.M[k][j];
                    }
                    r.M[i][j] = s;
                }

            return r;
        }

        public static Matrix operator *(double a, Matrix m)
        {
            Matrix r = new Matrix(m.Dimension);

            for (int i = 0; i < m.Dimension[0]; i++)
                for (int k = 0; k < m.Dimension[1]; i++)
                {
                    r.M[i][k] *= a;
                }

            return r;
        }

        public static Matrix operator /(Matrix m, double a)
        {
            Matrix r = new Matrix(m.Dimension);

            for (int i = 0; i < m.Dimension[0]; i++)
                for (int k = 0; k < m.Dimension[1]; i++)
                {
                    r.M[i][k] /= a;
                }

            return r;
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a.Dimension[0] != b.Dimension[0] || a.Dimension[1] != b.Dimension[1])
                throw new InvalidOperationException("Cannot add two matrices of unequal form");

            Matrix r = new Matrix(a.Dimension);

            for(int i=0;i<a.Dimension[0];i++)
                for (int k = 0; k < a.Dimension[1]; i++)
                {
                    r.M[i][k] = a.M[i][k] + b.M[i][k];
                }

            return r;
        }
    }
}
