using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;
using System.Globalization;
using System.Xml.Serialization;

namespace ArtyCalc.Model
{
    [Serializable]
    [XmlInclude(typeof(MilAngle))]
    [XmlInclude(typeof(RadAngle))]
    [XmlInclude(typeof(DegreeAngle))]
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

    [Serializable]
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

        public static BaseAngle TryParse(string s)
        {
            if (s[0] == 'd')
            {
                var b = new DegreeAngle();
                b.value = double.Parse(s.Substring(1), NumberFormatInfo.InvariantInfo);
                return b;
            }

            return null;
        }

        public override string ToString()
        {
            return "d" + value;
        }
    }

    [Serializable]
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

        public static BaseAngle TryParse(string s)
        {
            if (s[0] == 'r')
            {
                var b = new RadAngle();
                b.value = double.Parse(s.Substring(1),NumberFormatInfo.InvariantInfo);
                return b;
            }

            return null;
        }

        public override string ToString()
        {
            return "r" + value;
        }
    }

    [Serializable]
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

        public static BaseAngle TryParse(string s)
        {
            if (s[0] == 'm')
            {
                var b = new MilAngle();
                b.value = double.Parse(s.Substring(1),NumberFormatInfo.InvariantInfo);
                return b;
            }

            return null;
        }

        public override string ToString()
        {
            return "m" + value;
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
                return DependencyProperty.UnsetValue;

            string s = value as string;

            if (s == "")
                return DependencyProperty.UnsetValue;

            BaseAngle val = null;

            val = DegreeAngle.TryParse(s);
            if (val == null)
                val = RadAngle.TryParse(s);
            if (val == null)
                val = MilAngle.TryParse(s);
            if (val == null)
            {
                double d;
                if (double.TryParse(s,NumberStyles.Float,NumberFormatInfo.InvariantInfo, out d))
                    val = BaseAngle.Create<MilAngle>(double.Parse(s,NumberFormatInfo.InvariantInfo));
            }

            if (val == null)
                return DependencyProperty.UnsetValue;
            else
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
