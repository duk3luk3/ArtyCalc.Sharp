﻿using System;
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

        public double InternalValue
        {
            get { return value; }
            set { this.value = value; }
        }

        [XmlIgnore]
        public double RadiansValue
        {
            get { return GetRadiansRepresentation(); }
            set { SetRadiansRepresentation(value); }
        }

        abstract protected double GetRadiansRepresentation();

        abstract protected void SetRadiansRepresentation(double val);

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
            b.InternalValue = val;
            return b;
        }
    }

    [Serializable]
    public class DegreeAngle : BaseAngle
    {


        protected override double GetRadiansRepresentation()
        {
            return DegToRad(value);
        }


        protected override void SetRadiansRepresentation(double val)
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

        

        protected override double GetRadiansRepresentation()
        {
            return value;
        }

        

        protected override void SetRadiansRepresentation(double val)
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

        protected override double GetRadiansRepresentation()
        {
            return MilToRad(value);
        }


        protected override void SetRadiansRepresentation(double val)
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
                return 'd' + b.InternalValue.ToString("F2");
            }
            else if (b is MilAngle)
            {
                return 'm' + b.InternalValue.ToString("F0");
            }
            else if (b is RadAngle)
            {
                return 'r' + b.InternalValue.ToString();
            }
            else
            {
                return 'r' + b.RadiansValue.ToString();
            }
        }
    }
}
