using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows;
using Geometry.Interpolation;
using System.ComponentModel;

namespace ArtyCalc.Model
{
    public class FireSolution : INotifyPropertyChanged
    {
        private int charge;
        private double deflection;
        private double elevation;
        private double time;

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

        public double Time
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

            up = mission.Coords.Altitude - battery.Coords.Altitude;

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

            var deflection = (3200.0 + (azimuth - battery.Dir.GetRadiansRepresentation())) % 6400.0;

            foreach (var rt in munition.Rangetables)
            {
                var min = rt.table.Min(rw => rw.Range);
                var max = rt.table.Max(rw => rw.Range);

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

                var elev = rt.ElevSpline.Eval(new float[] { (float)rangedata.Item1 });
                var time = rt.TimeSpline.Eval(new float[] { (float)rangedata.Item1 });
                //TODO: adjust for height diff

                res.Add(new FireSolution()
                {
                    Charge = rt.Charge,
                    Deflection = deflection,
                    Elevation = elev[0],
                    Time = time[0]
                }
                );                
            }

            return res;
        }
    }

    public class Weapon
    {
#if DEBUG
        static string fi = @"C:\Users\luke\Documents\Visual Studio 2010\Projects\ArtyCalc\ArtyCalc\bin\Debug";
#else
        const string fi = "";
#endif


        public static ObservableCollection<Weapon> DefinedWeapons = new ObservableCollection<Weapon>(new Weapon[] {
            new Weapon() {designation = "M252 81mm Mortar", Munitions = new ObservableCollection<Ammunition>(new Ammunition[] {
                new Ammunition() { Designation = "HE", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromStream(0,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_HE_Charge0",FileMode.Open)),
                        Rangetable.FromStream(1,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_HE_Charge1",FileMode.Open)),
                        Rangetable.FromStream(2,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_HE_Charge2",FileMode.Open)),
                        Rangetable.FromStream(3,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_HE_Charge3",FileMode.Open)),
                        Rangetable.FromStream(4,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_HE_Charge4",FileMode.Open))
                    }
                    ),
                    Fuzes = new ObservableCollection<Fuze>(new Fuze[] {
                        new Fuze() { Designation = "Impact Burst", HasTimeFuze=false },
                        new Fuze() { Designation = "Near-Surface Burst", HasTimeFuze=false },
                        new Fuze() { Designation = "Proximity Burst", HasTimeFuze=false },
                        new Fuze() { Designation = "Delay", HasTimeFuze=false }
                    }
                    )
                },
                new Ammunition() { Designation = "WP", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromStream(0,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_WP_Charge0",FileMode.Open)),
                        Rangetable.FromStream(1,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_WP_Charge1",FileMode.Open)),
                        Rangetable.FromStream(2,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_WP_Charge2",FileMode.Open)),
                        Rangetable.FromStream(3,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_WP_Charge3",FileMode.Open)),
                        Rangetable.FromStream(4,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_WP_Charge4",FileMode.Open))
                    }
                    ),
                    Fuzes = new ObservableCollection<Fuze>(new Fuze[] {
                        new Fuze() { Designation = "Impact Burst", HasTimeFuze=false },
                        new Fuze() { Designation = "Near-Surface Burst", HasTimeFuze=false },
                        new Fuze() { Designation = "Proximity Burst", HasTimeFuze=false },
                        new Fuze() { Designation = "Time Fuze", HasTimeFuze=true}
                    }
                    )
                },
                new Ammunition() { Designation = "Illum", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromStream(1,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_Illum_Charge1",FileMode.Open)),
                        Rangetable.FromStream(2,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_Illum_Charge2",FileMode.Open)),
                        Rangetable.FromStream(3,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_Illum_Charge3",FileMode.Open)),
                        Rangetable.FromStream(4,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_Illum_Charge4",FileMode.Open))
                    }
                    ),
                    Fuzes = new ObservableCollection<Fuze>(new Fuze[] {
                        new Fuze() { Designation = "Time Fuze", HasTimeFuze=true}
                    }
                    )
                }
            })
            },
            new Weapon() {designation = "2B14 81mm Mortar", Munitions = new ObservableCollection<Ammunition>(new Ammunition[] {
                new Ammunition() { Designation = "HE", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromStream(0,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_HE_Charge0",FileMode.Open)),
                        Rangetable.FromStream(1,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_HE_Charge1",FileMode.Open)),
                        Rangetable.FromStream(2,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_HE_Charge2",FileMode.Open)),
                        Rangetable.FromStream(3,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_HE_Charge3",FileMode.Open)),
                        Rangetable.FromStream(4,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_HE_Charge4",FileMode.Open))
                    }
                    ),
                    Fuzes = new ObservableCollection<Fuze>(new Fuze[] {
                        new Fuze() { Designation = "Impact Burst", HasTimeFuze=false },
                        new Fuze() { Designation = "Near-Surface Burst", HasTimeFuze=false },
                        new Fuze() { Designation = "Proximity Burst", HasTimeFuze=false },
                        new Fuze() { Designation = "Delay", HasTimeFuze=false }
                    }
                    )
                },
                new Ammunition() { Designation = "WP", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromStream(0,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_WP_Charge0",FileMode.Open)),
                        Rangetable.FromStream(1,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_WP_Charge1",FileMode.Open)),
                        Rangetable.FromStream(2,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_WP_Charge2",FileMode.Open)),
                        Rangetable.FromStream(3,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_WP_Charge3",FileMode.Open)),
                        Rangetable.FromStream(4,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_WP_Charge4",FileMode.Open))
                    }
                    ),
                    Fuzes = new ObservableCollection<Fuze>(new Fuze[] {
                        new Fuze() { Designation = "Impact Burst", HasTimeFuze=false },
                        new Fuze() { Designation = "Near-Surface Burst", HasTimeFuze=false },
                        new Fuze() { Designation = "Proximity Burst", HasTimeFuze=false },
                        new Fuze() { Designation = "Time Fuze", HasTimeFuze=true}
                    }
                    )
                },
                new Ammunition() { Designation = "Illum", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromStream(0,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_Illum_Charge0",FileMode.Open)),
                        Rangetable.FromStream(1,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_Illum_Charge1",FileMode.Open)),
                        Rangetable.FromStream(2,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_Illum_Charge2",FileMode.Open)),
                        Rangetable.FromStream(3,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_Illum_Charge3",FileMode.Open))
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
        public CubicSpline RangeSpline
        {
            get
            {
                if (elevSpline == null)
                    InitSpline();

                return elevSpline;
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

        public int Charge;

        public List<RangetableRow> table = new List<RangetableRow>();

        public Rangetable(int charge)
        {
            this.Charge = charge;
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
                        int dTime = (int)(float.Parse(l[3]) * 1000);
                        int time = (int)(float.Parse(l[4]) * 1000);
                        

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
        public int Range;
        public int Elev;
        public int ElevationAdjust;
        public int Time;
        public int TimeAdjust;
    }
}
