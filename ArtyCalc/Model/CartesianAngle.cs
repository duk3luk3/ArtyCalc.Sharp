using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;

namespace ArtyCalc.Model
{
    public abstract class BaseAngle
    {
        protected double value;

        abstract public double GetInternalRepresentation();
        abstract public double GetRadiansRepresentation();
        abstract public void SetInternalRepresentation(double val);
        abstract public void SetRadiansRepresentation(double val);

        protected static double DegToRad(double deg)
        {
            return deg * Math.PI / 180.0;
        }

        protected static double RadToDeg(double rad)
        {
            return rad * 180.0 / Math.PI;
        }

        protected static double MilToRad(double mils)
        {
            return mils * Math.PI / 3200.0;
        }

        protected static double RadToMil(double rads)
        {
            return rads * 3200.0 / Math.PI;
        }

        public static T Create<T>(double val) where T: BaseAngle, new()
        {
            T b = new T();
            b.SetInternalRepresentation(val);
            return b;
        }
    }

    public class DegreeAngle : BaseAngle
    {
        public override double GetInternalRepresentation()
        {
            return value;
        }

        public override double GetRadiansRepresentation()
        {
            return DegToRad(value);
        }

        public override void SetInternalRepresentation(double val)
        {
            this.value = val;
        }

        public override void SetRadiansRepresentation(double val)
        {
            this.value = RadToDeg(val);
        }
    }

    public class RadAngle : BaseAngle
    {

        public override double GetInternalRepresentation()
        {
            return value;
        }

        public override double GetRadiansRepresentation()
        {
            return value;
        }

        public override void SetInternalRepresentation(double val)
        {
            this.value = val;
        }

        public override void SetRadiansRepresentation(double val)
        {
            this.value = val;
        }
    }

    public class MilAngle : BaseAngle
    {

        public override double GetInternalRepresentation()
        {
            return value;
        }

        public override double GetRadiansRepresentation()
        {
            return MilToRad(value);
        }

        public override void SetInternalRepresentation(double val)
        {
            this.value = val;
        }

        public override void SetRadiansRepresentation(double val)
        {
            this.value = RadToMil(val);
        }
    }

    

    [ValueConversion(typeof(string), typeof(BaseAngle))]
    public class AngleToStringConverter : BaseConverter, IValueConverter
    {

        public AngleToStringConverter()
            : base()
        {
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            string s = value as string;

            if (s == "")
                return null;

            BaseAngle val = null;

            char d = s[0];

            //is d a digit?
            if (d > 47 && d < 58)
            {
                val = BaseAngle.Create<MilAngle>(double.Parse(s));
            }
            else
            {
                double v;
                if (double.TryParse(s.Substring(1), out v))
                {

                    switch (d)
                    {
                        case 'd':
                            val = BaseAngle.Create<DegreeAngle>(v);
                            break;
                        case 'm':
                            val = BaseAngle.Create<MilAngle>(v);
                            break;
                        case 'r':
                            val = BaseAngle.Create<RadAngle>(v);
                            break;
                        default:
                            throw new InvalidOperationException("Invalid angle string");
                    }
                }
                else
                {
                    return DependencyProperty.UnsetValue;
                }
            }

            return val;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            BaseAngle b = value as BaseAngle;

            if (b is DegreeAngle)
            {
                return 'd' + b.GetInternalRepresentation().ToString();
            }
            else if (b is MilAngle)
            {
                return 'm' + b.GetInternalRepresentation().ToString();
            }
            else if (b is RadAngle)
            {
                return 'r' + b.GetInternalRepresentation().ToString();
            }
            else
            {
                return 'r' + b.GetRadiansRepresentation().ToString();
            }
        }
    }
}
