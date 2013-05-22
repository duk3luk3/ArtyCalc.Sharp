using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows;

namespace ArtyCalc.Model
{
    [ValueConversion(typeof(string), typeof(BaseAngle))]
    public class GridToStringConverter : BaseConverter, IValueConverter
    {
        public GridToStringConverter()
            : base()
        {
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            if (!(value is string))
                return DependencyProperty.UnsetValue;

            string gridStr = (string)value;

            if ((gridStr.Length % 2) != 0)
            {
                return DependencyProperty.UnsetValue;
            }

            return value;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class Coordinate : INotifyPropertyChanged
    {
        private int formatlen = 4;

        private double gridX;
        private double gridY;

        private double altitude;

        public double Altitude
        {
            get { return altitude; }
            set
            {
                altitude = value;
                OnPropertyChanged("Altitude");
            }
        }

        public string Grid
        {
            get
            {

                string fmt = "";

                double div = 0;

                switch (formatlen)
                {
                    case 1:
                        fmt = "1:0";
                        div = 10000;
                        break;
                    case 2:
                        fmt = "2:00";
                        div = 1000;
                        break;
                    case 3:
                        fmt = "3:000";
                        div = 100;
                        break;
                    case 4:
                        fmt = "4:0000";
                        div = 10;
                        break;
                    default:
                        fmt = "5:00000";
                        div = 1;
                        break;
                }
                                
                return String.Format("{0,"+fmt+"}{1,"+fmt+"}", (int)(gridX/div), (int)(gridY/div)); 
            }
            set {
                setGrid(value);
                
                OnPropertyChanged("Grid");
            }
        }

        public static double range(Coordinate c0, Coordinate c1)
        {
            var dx = c0.gridX - c1.gridX;
            var dy = c0.gridY - c1.gridY;

            var range2 = dx * dx + dy * dy;

            return Math.Sqrt(range2);
        }

        public static double azimuth(Coordinate c0, Coordinate c1)
        {
            

            var range = Coordinate.range(c0, c1);
            var az = Math.Acos((c1.gridY - c0.gridY) / range);
            if (c0.gridX > c1.gridX)
            {
                az = 2 * Math.PI - az;
            }

            return az;
        }

        public static Coordinate Add(Coordinate c0, Coordinate c1)
        {
            return new Coordinate(c0.gridX + c1.gridX, c0.gridY + c1.gridY, c0.altitude + c1.altitude);
        }

        public Coordinate Shift(double angle, double add, double right, double up)
        {
            double north = Math.Cos(angle) * add + Math.Sin(angle) * right;
            double east = Math.Sin(angle) * add + Math.Cos(angle) * right;

            return new Coordinate(this.gridX + east, this.gridY + north, this.altitude + up);
        }

        private void setGrid(string gridStr)
        {
            if (gridStr == "")
                return;

            if ((gridStr.Length % 2) != 0)
            {
                throw new ArgumentException("Illegal grid string");
            }

            int halfLength = gridStr.Length / 2;
            formatlen = halfLength;

            string str0 = gridStr.Substring(0, halfLength);
            string str1 = gridStr.Substring(halfLength);

            double sig = 10000;
            double x = 0, y = 0;

            for (int i = 0; i < halfLength; i++)
            {
                x += int.Parse(str0[i].ToString()) * sig;
                y += int.Parse(str1[i].ToString()) * sig;
                sig /= 10;
            }

            gridX = x;
            gridY = y;

        }

        public Coordinate(string grid, float alt)
        {
            Grid = grid;
            this.altitude = alt;
        }

        public Coordinate(double x, double y, double alt)
        {
            this.altitude = alt;
            this.gridX = x;
            this.gridY = y;
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static Coordinate Zero { get { return new Coordinate(0, 0, 0); } }
    }
}
