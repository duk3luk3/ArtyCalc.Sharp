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

    public class Coordinate
    {
        private double gridX;
        private double gridY;

        private float altitude;

        public float Altitude
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
            get { return String.Format("{0,5:00000}{1,5:00000}", (int)gridX, (int)gridY); }
            set {
                setGrid(value);
                OnPropertyChanged("Grid");
            }
        }

        private void setGrid(string gridStr)
        {
            if ((gridStr.Length % 2) != 0)
            {
                throw new ArgumentException("Illegal grid string");
            }

            int halfLength = gridStr.Length / 2;

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

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
