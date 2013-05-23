using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows;
using Geometry.Interpolation;
using System.ComponentModel;
using System.Windows.Data;
using System.Globalization;

namespace ArtyCalc.Model
{
    [ValueConversion(typeof(string), typeof(int))]
    public class TimeToStringConverter : BaseConverter, IValueConverter
    {
        public TimeToStringConverter()
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

            double p;
            if (Double.TryParse(s, NumberStyles.Float,NumberFormatInfo.InvariantInfo, out p))
            {
                return (int)(p * 1000);
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }

            
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is int))
            {
                return DependencyProperty.UnsetValue;
            }

            int v = (int)value;

            int secs = v / 1000;
            int ms = v % 1000;

            return "" + secs + "." + ms;
        }
    }

    public class FireSolution : INotifyPropertyChanged
    {
        private int charge;
        private double deflection;
        private double elevation;
        private int time;

        public int Charge
        {
            get { return charge; }
            set
            {
                charge = value;
                OnPropertyChanged("Charge");
            }
        }

        public double Deflection
        {
            get { return deflection; }
            set
            {
                deflection = value;
                OnPropertyChanged("Deflection");
            }
        }

        public double Elevation
        {
            get { return elevation; }
            set
            {
                elevation = value;
                OnPropertyChanged("Elevation");
            }
        }

        public int Time
        {
            get { return time; }
            set
            {
                time = value;
                OnPropertyChanged("Time");
            }
        }

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class BallisticModel
    {
        public static Tuple<double, double, double> RangeAzimuthUp(Battery battery, MissionSpec mission)
        {
            double range = 0;
            double azimuth = 0;
            double up = 0;

            range = Coordinate.range(battery.Coords, mission.AdjustedCoords);
            azimuth = Coordinate.azimuth(battery.Coords, mission.AdjustedCoords);

            up = mission.AdjustedCoords.Altitude - battery.Coords.Altitude;

            return new Tuple<double, double, double>(range, azimuth, up);
        }

        public static List<FireSolution> CalcFire(Battery battery, MissionSpec mission)
        {
            var res = new List<FireSolution>();

            var munition = mission.Ammunition;

            if (munition == null)
            {
                return res;
            }

            var rangedata = RangeAzimuthUp(battery, mission);

            System.Console.WriteLine(rangedata);

            var range = rangedata.Item1;
            var azimuth = rangedata.Item2;
            var up = rangedata.Item3;

            var deflection = (Math.PI + (azimuth - battery.Dir.GetRadiansRepresentation()));
            if (deflection < 0)
                deflection = deflection + (2 * Math.PI);

            if (deflection > 2 * Math.PI)
                deflection = deflection - (2 * Math.PI);

            MilAngle m = new MilAngle();
            m.SetRadiansRepresentation(deflection);


            var rangeArr = new float[] { (float)rangedata.Item1 };

            foreach (var rt in munition.Rangetables)
            {
                var min = rt.Table.Min(rw => rw.Range);
                var max = rt.Table.Max(rw => rw.Range);

                if (min > rangedata.Item1 || max < rangedata.Item1)
                {
                    res.Add(
                        new FireSolution()
                        {
                            Charge = rt.Charge,
                            Elevation = -1
                        }
                        );
                    continue;
                }

                var elev = rt.ElevSpline.Eval(rangeArr);
                var time = rt.TimeSpline.Eval(rangeArr);
                //TODO: adjust for height diff
                var elevAdjust = rt.ElevAdjustSpline.Eval(rangeArr);
                var timeAdjust = rt.TimeAdjustSpline.Eval(rangeArr);
                
                //adjust is per -100 dAlt
                var elevAdjustVal = elevAdjust[0] * up / -100.0;
                var timeAdjustVal = timeAdjust[0] * up / -100.0;

                

                res.Add(new FireSolution()
                {
                    Charge = rt.Charge,
                    Deflection = m.GetInternalRepresentation(),
                    Elevation = elev[0] + elevAdjustVal,
                    Time = (int)(time[0] + timeAdjustVal)
                }
                );
            }

            return res;
        }
    }

    public class Weapon
    {
#if DEBUG
        static string fi = @"C:\Users\luke\Documents\Visual Studio 2010\Projects\ArtyCalc\ArtyCalc\bin\Debug\";
#else
        const string fi = "";
#endif


        public static ObservableCollection<Weapon> DefinedWeapons = new ObservableCollection<Weapon>(new Weapon[] {
            new Weapon() {designation = "Tampella 120mm Mortar", Munitions = new ObservableCollection<Ammunition>(new Ammunition[] {
                new Ammunition() { Designation = "HE", Lot="DM61 (Proximity) / DM11A5 (Quick)", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromFile(0,(fi  +  "Rangetables\\tampella\\tampella_120mm_HE_Charge0")),
                        Rangetable.FromFile(1,(fi  +  "Rangetables\\tampella\\tampella_120mm_HE_Charge1")),
                        Rangetable.FromFile(2,(fi  +  "Rangetables\\tampella\\tampella_120mm_HE_Charge2")),
                        Rangetable.FromFile(3,(fi  +  "Rangetables\\tampella\\tampella_120mm_HE_Charge3")),
                        Rangetable.FromFile(4,(fi  +  "Rangetables\\tampella\\tampella_120mm_HE_Charge4"))
                    }
                    ),
                    Fuzes = new ObservableCollection<Fuze>(new Fuze[] {
                        new Fuze() { Designation = "Proximity Burst", Short="Proximity", HasTimeFuze=false },
                        new Fuze() { Designation = "Impact Burst", Short="Quick", HasTimeFuze=false }
                    }
                    )
                },
                new Ammunition() { Designation = "HC Smoke", Lot="DM35", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromFile(0,(fi  +  "Rangetables\\tampella\\tampella_120mm_WP_Charge0")),
                        Rangetable.FromFile(1,(fi  +  "Rangetables\\tampella\\tampella_120mm_WP_Charge1")),
                        Rangetable.FromFile(2,(fi  +  "Rangetables\\tampella\\tampella_120mm_WP_Charge2")),
                        Rangetable.FromFile(3,(fi  +  "Rangetables\\tampella\\tampella_120mm_WP_Charge3")),
                        Rangetable.FromFile(4,(fi  +  "Rangetables\\tampella\\tampella_120mm_WP_Charge4"))
                    }
                    ),
                    Fuzes = new ObservableCollection<Fuze>(new Fuze[] {
                        new Fuze() { Designation = "Time Fuze", Short="VT", HasTimeFuze=true}
                    }
                    )
                },
                new Ammunition() { Designation = "Illum", Lot="DM26", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromFile(0,(fi  +  "Rangetables\\tampella\\tampella_120mm_Illum_Charge0")),
                        Rangetable.FromFile(1,(fi  +  "Rangetables\\tampella\\tampella_120mm_Illum_Charge1")),
                        Rangetable.FromFile(2,(fi  +  "Rangetables\\tampella\\tampella_120mm_Illum_Charge2")),
                        Rangetable.FromFile(3,(fi  +  "Rangetables\\tampella\\tampella_120mm_Illum_Charge3")),
                        Rangetable.FromFile(4,(fi  +  "Rangetables\\tampella\\tampella_120mm_Illum_Charge4"))
                    }
                    ),
                    Fuzes = new ObservableCollection<Fuze>(new Fuze[] {
                        new Fuze() { Designation = "Time Fuze", Short="VT", HasTimeFuze=true}
                    }
                    )
                }
            })
            },
            new Weapon() {designation = "M252 81mm Mortar", Munitions = new ObservableCollection<Ammunition>(new Ammunition[] {
                new Ammunition() { Designation = "HE", Lot="M821A2", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromFile(0,(fi  +  "Rangetables\\M252\\m252_81mm_HE_Charge0")),
                        Rangetable.FromFile(1,(fi  +  "Rangetables\\M252\\m252_81mm_HE_Charge1")),
                        Rangetable.FromFile(2,(fi  +  "Rangetables\\M252\\m252_81mm_HE_Charge2")),
                        Rangetable.FromFile(3,(fi  +  "Rangetables\\M252\\m252_81mm_HE_Charge3")),
                        Rangetable.FromFile(4,(fi  +  "Rangetables\\M252\\m252_81mm_HE_Charge4"))
                    }
                    ),
                    Fuzes = new ObservableCollection<Fuze>(new Fuze[] {
                        new Fuze() { Designation = "Impact Burst", Short="Quick", HasTimeFuze=false },
                        new Fuze() { Designation = "Near-Surface Burst", Short="Near-Surface", HasTimeFuze=false },
                        new Fuze() { Designation = "Proximity Burst", Short="Proximity", HasTimeFuze=false },
                        new Fuze() { Designation = "Delay", Short="Delay", HasTimeFuze=false }
                    }
                    )
                },
                new Ammunition() { Designation = "WP", Lot="M375A3", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromFile(0,(fi  +  "Rangetables\\M252\\m252_81mm_WP_Charge0")),
                        Rangetable.FromFile(1,(fi  +  "Rangetables\\M252\\m252_81mm_WP_Charge1")),
                        Rangetable.FromFile(2,(fi  +  "Rangetables\\M252\\m252_81mm_WP_Charge2")),
                        Rangetable.FromFile(3,(fi  +  "Rangetables\\M252\\m252_81mm_WP_Charge3")),
                        Rangetable.FromFile(4,(fi  +  "Rangetables\\M252\\m252_81mm_WP_Charge4"))
                    }
                    ),
                    Fuzes = new ObservableCollection<Fuze>(new Fuze[] {
                        new Fuze() { Designation = "Point Detonate", Short="Quick", HasTimeFuze=false }
                    }
                    )
                },
                new Ammunition() { Designation = "Illum", Lot="M853A1", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromFile(1,(fi  +  "Rangetables\\M252\\m252_81mm_Illum_Charge1")),
                        Rangetable.FromFile(2,(fi  +  "Rangetables\\M252\\m252_81mm_Illum_Charge2")),
                        Rangetable.FromFile(3,(fi  +  "Rangetables\\M252\\m252_81mm_Illum_Charge3")),
                        Rangetable.FromFile(4,(fi  +  "Rangetables\\M252\\m252_81mm_Illum_Charge4"))
                    }
                    ),
                    Fuzes = new ObservableCollection<Fuze>(new Fuze[] {
                        new Fuze() { Designation = "Time Fuze", Short="VT", HasTimeFuze=true}
                    }
                    )
                }
            })
            },
            new Weapon() {designation = "2B14 82mm Mortar", Munitions = new ObservableCollection<Ammunition>(new Ammunition[] {
                new Ammunition() { Designation = "HE", Lot="VO-832DU", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromFile(0,(fi  +  "Rangetables\\2B14\\2b14_82mm_HE_Charge0")),
                        Rangetable.FromFile(1,(fi  +  "Rangetables\\2B14\\2b14_82mm_HE_Charge1")),
                        Rangetable.FromFile(2,(fi  +  "Rangetables\\2B14\\2b14_82mm_HE_Charge2")),
                        Rangetable.FromFile(3,(fi  +  "Rangetables\\2B14\\2b14_82mm_HE_Charge3")),
                        Rangetable.FromFile(4,(fi  +  "Rangetables\\2B14\\2b14_82mm_HE_Charge4")),
                        Rangetable.FromFile(5,(fi  +  "Rangetables\\2B14\\2b14_82mm_HE_Charge5")),
                        Rangetable.FromFile(6,(fi  +  "Rangetables\\2B14\\2b14_82mm_HE_Charge6"))
                    }
                    ),
                    Fuzes = new ObservableCollection<Fuze>(new Fuze[] {
                        new Fuze() { Designation = "Impact Burst", Short="Quick", HasTimeFuze=false }
                    }
                    )
                },
                new Ammunition() { Designation = "WP", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromFile(0,(fi  +  "Rangetables\\2B14\\2b14_82mm_WP_Charge0")),
                        Rangetable.FromFile(1,(fi  +  "Rangetables\\2B14\\2b14_82mm_WP_Charge1")),
                        Rangetable.FromFile(2,(fi  +  "Rangetables\\2B14\\2b14_82mm_WP_Charge2")),
                        Rangetable.FromFile(3,(fi  +  "Rangetables\\2B14\\2b14_82mm_WP_Charge3")),
                        Rangetable.FromFile(4,(fi  +  "Rangetables\\2B14\\2b14_82mm_WP_Charge4")),
                        Rangetable.FromFile(5,(fi  +  "Rangetables\\2B14\\2b14_82mm_WP_Charge5")),
                        Rangetable.FromFile(6,(fi  +  "Rangetables\\2B14\\2b14_82mm_WP_Charge6"))
                    }
                    ),
                    Fuzes = new ObservableCollection<Fuze>(new Fuze[] {
                        new Fuze() { Designation = "Point Detonate", Short="Quick", HasTimeFuze=false }
                    }
                    )
                },
                new Ammunition() { Designation = "Illum", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromFile(0,(fi  +  "Rangetables\\2B14\\2b14_82mm_Illum_Charge0")),
                        Rangetable.FromFile(1,(fi  +  "Rangetables\\2B14\\2b14_82mm_Illum_Charge1")),
                        Rangetable.FromFile(2,(fi  +  "Rangetables\\2B14\\2b14_82mm_Illum_Charge2")),
                        Rangetable.FromFile(3,(fi  +  "Rangetables\\2B14\\2b14_82mm_Illum_Charge3"))
                    }
                    ),
                    Fuzes = new ObservableCollection<Fuze>(new Fuze[] {
                        new Fuze() { Designation = "Time Fuze", Short="VT", HasTimeFuze=true}
                    }
                    )
                }
            })
            },
            new Weapon() {designation = "M224 60mm Mortar", Munitions = new ObservableCollection<Ammunition>(new Ammunition[] {
                new Ammunition() { Designation = "HE", Lot="M720A1", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromFile(0,(fi  +  "Rangetables\\M224\\m224_60mm_HE_Charge0")),
                        Rangetable.FromFile(1,(fi  +  "Rangetables\\M224\\m224_60mm_HE_Charge1")),
                        Rangetable.FromFile(2,(fi  +  "Rangetables\\M224\\m224_60mm_HE_Charge2")),
                        Rangetable.FromFile(3,(fi  +  "Rangetables\\M224\\m224_60mm_HE_Charge3")),
                        Rangetable.FromFile(4,(fi  +  "Rangetables\\M224\\m224_60mm_HE_Charge4"))
                    }
                    ),
                    Fuzes = new ObservableCollection<Fuze>(new Fuze[] {
                        new Fuze() { Designation = "Impact Burst", Short="Quick", HasTimeFuze=false },
                        new Fuze() { Designation = "Near-Surface Burst", Short="Near-Surface", HasTimeFuze=false },
                        new Fuze() { Designation = "Proximity Burst", Short="Proximity", HasTimeFuze=false },
                        new Fuze() { Designation = "Delay", Short="Delay", HasTimeFuze=false }
                    }
                    )
                },
                new Ammunition() { Designation = "M224 WP", Lot="M722", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromFile(0,(fi  +  "Rangetables\\M224\\m224_60mm_WP_Charge0")),
                        Rangetable.FromFile(1,(fi  +  "Rangetables\\M224\\m224_60mm_WP_Charge1")),
                        Rangetable.FromFile(2,(fi  +  "Rangetables\\M224\\m224_60mm_WP_Charge2")),
                        Rangetable.FromFile(3,(fi  +  "Rangetables\\M224\\m224_60mm_WP_Charge3")),
                        Rangetable.FromFile(4,(fi  +  "Rangetables\\M224\\m224_60mm_WP_Charge4"))
                    }
                    ),
                    Fuzes = new ObservableCollection<Fuze>(new Fuze[] {
                        new Fuze() { Designation = "Point Detonate", Short="Quick", HasTimeFuze=false }
                    }
                    )
                },
                new Ammunition() { Designation = "M224 Illum", Lot="M721", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromFile(1,(fi  +  "Rangetables\\M224\\m224_60mm_Illum_Charge1")),
                        Rangetable.FromFile(2,(fi  +  "Rangetables\\M224\\m224_60mm_Illum_Charge2")),
                        Rangetable.FromFile(3,(fi  +  "Rangetables\\M224\\m224_60mm_Illum_Charge3")),
                        Rangetable.FromFile(4,(fi  +  "Rangetables\\M224\\m224_60mm_Illum_Charge4"))
                    }
                    ),
                    Fuzes = new ObservableCollection<Fuze>(new Fuze[] {
                        new Fuze() { Designation = "Time Fuze", Short="VT", HasTimeFuze=true}
                    }
                    )
                }
            })
            },
            new Weapon() {designation = "M119 105mm Gun", Munitions = new ObservableCollection<Ammunition>(new Ammunition[] {
                new Ammunition() { Designation = "DPICM", Lot="M916", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromFile(1,(fi  +  "Rangetables\\M119\\m119_105mm_DPICM_Charge1")),
                        Rangetable.FromFile(2,(fi  +  "Rangetables\\M119\\m119_105mm_DPICM_Charge2")),
                        Rangetable.FromFile(3,(fi  +  "Rangetables\\M119\\m119_105mm_DPICM_Charge3")),
                        Rangetable.FromFile(4,(fi  +  "Rangetables\\M119\\m119_105mm_DPICM_Charge4")),
                        Rangetable.FromFile(5,(fi  +  "Rangetables\\M119\\m119_105mm_DPICM_Charge5")),
                        Rangetable.FromFile(6,(fi  +  "Rangetables\\M119\\m119_105mm_DPICM_Charge6")),
                        Rangetable.FromFile(7,(fi  +  "Rangetables\\M119\\m119_105mm_DPICM_Charge7")),
                        Rangetable.FromFile(8,(fi  +  "Rangetables\\M119\\m119_105mm_DPICM_Charge8"))
                    }
                    ),
                    Fuzes = new ObservableCollection<Fuze>(new Fuze[] {
                        new Fuze() { Designation = "Time Fuze", Short="VT", HasTimeFuze=true}
                    }
                    )
                },
                new Ammunition() { Designation = "HE", Lot="M1", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromFile(1,(fi  +  "Rangetables\\M119\\m119_105mm_HE_Charge1")),
                        Rangetable.FromFile(2,(fi  +  "Rangetables\\M119\\m119_105mm_HE_Charge2")),
                        Rangetable.FromFile(3,(fi  +  "Rangetables\\M119\\m119_105mm_HE_Charge3")),
                        Rangetable.FromFile(4,(fi  +  "Rangetables\\M119\\m119_105mm_HE_Charge4")),
                        Rangetable.FromFile(5,(fi  +  "Rangetables\\M119\\m119_105mm_HE_Charge5")),
                        Rangetable.FromFile(6,(fi  +  "Rangetables\\M119\\m119_105mm_HE_Charge6")),
                        Rangetable.FromFile(7,(fi  +  "Rangetables\\M119\\m119_105mm_HE_Charge7")),
                        Rangetable.FromFile(8,(fi  +  "Rangetables\\M119\\m119_105mm_HE_Charge8"))
                    }
                    ),
                    Fuzes = new ObservableCollection<Fuze>(new Fuze[] {
                        new Fuze() { Designation = "Point Detonate", Short="Quick", HasTimeFuze=false },
                        new Fuze() { Designation = "Proximity Burst", Short="Proximity", HasTimeFuze=false },
                        new Fuze() { Designation = "Delay", Short="Delay", HasTimeFuze=false },
                        new Fuze() { Designation = "Time Fuze", Short="VT", HasTimeFuze=true}
                    }
                    )
                },
                new Ammunition() { Designation = "Bursting WP / HC Smoke", Lot="M60A2 / M84A1",  Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromFile(1,(fi  +  "Rangetables\\M119\\m119_105mm_WP_Charge1")),
                        Rangetable.FromFile(2,(fi  +  "Rangetables\\M119\\m119_105mm_WP_Charge2")),
                        Rangetable.FromFile(3,(fi  +  "Rangetables\\M119\\m119_105mm_WP_Charge3")),
                        Rangetable.FromFile(4,(fi  +  "Rangetables\\M119\\m119_105mm_WP_Charge4")),
                        Rangetable.FromFile(1,(fi  +  "Rangetables\\M119\\m119_105mm_WP_Charge5")),
                        Rangetable.FromFile(2,(fi  +  "Rangetables\\M119\\m119_105mm_WP_Charge6")),
                        Rangetable.FromFile(3,(fi  +  "Rangetables\\M119\\m119_105mm_WP_Charge7"))
                    }
                    ),
                    Fuzes = new ObservableCollection<Fuze>(new Fuze[] {
                        new Fuze() { Designation = "Point Detonate (WP only)", Short="Quick", HasTimeFuze=false },
                        new Fuze() { Designation = "Time Fuze (HC Smoke only)", Short="VT", HasTimeFuze=true}
                    }
                    )
                },
                new Ammunition() { Designation = "M119 Illum", Lot="M314A3", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromFile(1,(fi  +  "Rangetables\\M119\\m119_105mm_Illum_Charge1")),
                        Rangetable.FromFile(2,(fi  +  "Rangetables\\M119\\m119_105mm_Illum_Charge2")),
                        Rangetable.FromFile(3,(fi  +  "Rangetables\\M119\\m119_105mm_Illum_Charge3")),
                        Rangetable.FromFile(4,(fi  +  "Rangetables\\M119\\m119_105mm_Illum_Charge4")),
                        Rangetable.FromFile(1,(fi  +  "Rangetables\\M119\\m119_105mm_Illum_Charge5")),
                        Rangetable.FromFile(2,(fi  +  "Rangetables\\M119\\m119_105mm_Illum_Charge6")),
                        Rangetable.FromFile(3,(fi  +  "Rangetables\\M119\\m119_105mm_Illum_Charge7"))
                    }
                    ),
                    Fuzes = new ObservableCollection<Fuze>(new Fuze[] {
                        new Fuze() { Designation = "Time Fuze", HasTimeFuze=true}
                    }
                    )
                }
            })
            }
        });

        public static ObservableCollection<Weapon> GetDefinedWeapons()
        {
            return DefinedWeapons;
        }

        public ObservableCollection<Ammunition> Munitions { get; set; }

        private string designation;

        public string Designation
        {
            get { return designation; }
            set { designation = value; }
        }

    }

    public class Ammunition
    {
        private string designation;

        public string Lot { get; set; }

        public string Designation
        {
            get { return designation; }
            set { designation = value; }
        }

        private List<Rangetable> rangetables;

        public List<Rangetable> Rangetables
        {
            get { return rangetables; }
            set { rangetables = value; }
        }

        private ObservableCollection<Fuze> fuzes;

        public ObservableCollection<Fuze> Fuzes
        {
            get { return fuzes; }
            set { fuzes = value; }
        }

        public override string ToString()
        {
            return "Ammunition: " + Designation;
        }

    }

    public class Fuze
    {
        public string Designation { get; set; }
        public string Short { get; set; }
        public bool HasTimeFuze { get; set; }

        public override string ToString()
        {
            return Designation;
        }
    }

    public class Rangetable
    {
        private CubicSpline elevSpline = null;
        private CubicSpline timeSpline = null;
        private CubicSpline elevAdjustSpline = null;
        private CubicSpline timeAdjustSpline = null;

        public CubicSpline ElevSpline
        {
            get
            {
                if (elevSpline == null)
                    InitSpline();

                return elevSpline;
            }
        }
        public CubicSpline TimeSpline
        {
            get
            {
                if (timeSpline == null)
                    InitSpline();

                return timeSpline;
            }
        }
        public CubicSpline ElevAdjustSpline
        {
            get
            {
                if (elevAdjustSpline == null)
                    InitSpline();

                return elevAdjustSpline;
            }
        }
        public CubicSpline TimeAdjustSpline
        {
            get
            {
                if (timeAdjustSpline == null)
                    InitSpline();

                return timeAdjustSpline;
            }
        }

        private void InitSpline()
        {
            float[] x = new float[table.Count];
            float[] elev = new float[table.Count];
            float[] time = new float[table.Count];
            float[] dElev = new float[table.Count];
            float[] dTime = new float[table.Count];
            
            //range spline
            for (int i = 0; i < table.Count; i++)
            {
                x[i] = table[i].Range;
                elev[i] = table[i].Elev;
                time[i] = table[i].Time;
                dElev[i] = table[i].ElevationAdjust;
                dTime[i] = table[i].TimeAdjust;
            }

            elevSpline = new CubicSpline();
            timeSpline = new CubicSpline();
            elevAdjustSpline = new CubicSpline();
            timeAdjustSpline = new CubicSpline();

            elevSpline.Fit(x, elev);
            timeSpline.Fit(x, time);
            elevAdjustSpline.Fit(x, dElev);
            timeAdjustSpline.Fit(x, dTime);
        }

        public int Charge {get; private set;}

        private List<RangetableRow> table = new List<RangetableRow>();

        public List<RangetableRow> Table
        {
            get { return table; }
            set { table = value; }
        }

        public Rangetable(int charge)
        {
            this.Charge = charge;
        }

        public override string ToString()
        {
            return "Charge " + Charge;
        }

        public static Rangetable FromFile(int charge, string file)
        {
            //System.Console.WriteLine("Loading charge " + charge + " from file " + file);

            try
            {
                FileStream fs = new FileStream(file, FileMode.Open);

                return FromStream(charge, fs);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                if (e.InnerException != null)
                {
                    System.Console.WriteLine(e.InnerException.Message);
                    if (e.InnerException.InnerException != null)
                    {
                        System.Console.WriteLine(e.InnerException.InnerException.Message);
                    }
                }
                throw;
            }
        }

        public static Rangetable FromStream(int charge, Stream source)
        {
            var rt = new Rangetable(charge);

            using (StreamReader rd = new StreamReader(source))
            {
                while (!rd.EndOfStream)
                {
                    var l = rd.ReadLine().Split('\t');

                    int r;
                    if (int.TryParse(l[0], out r))
                    {
                        int elev = int.Parse(l[1]);
                        int dElev = int.Parse(l[2]);
                        int dTime = (int)(double.Parse(l[3],NumberFormatInfo.InvariantInfo) * 1000);
                        int time = (int)(double.Parse(l[4], NumberFormatInfo.InvariantInfo) * 1000);

                        
                        

                        rt.table.Add(new RangetableRow()
                        {
                            Range = r,
                            Elev = elev,
                            ElevationAdjust = dElev,
                            Time = time,
                            TimeAdjust = dTime
                        }
                        );
                    }
                }
            }

            return rt;
        }
    }

    public struct RangetableRow
    {
        public int Range { get; set; }
        public int Elev { get; set; }
        public int ElevationAdjust { get; set; }
        public int Time { get; set; }
        public int TimeAdjust { get; set; }
    }
}
