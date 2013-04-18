using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows;

namespace ArtyCalc.Model
{
    public class BallisticModel
    {

    }

    public class Weapon
    {
#if DEBUG
        static string fi = @"C:\Users\luke\Documents\Visual Studio 2010\Projects\ArtyCalc\ArtyCalc\bin\Debug";
#else
        const string fi = "";
#endif


        public static ObservableCollection<Weapon> DefinedWeapons = new ObservableCollection<Weapon>(new Weapon[] {
            new Weapon() {designation = "M252 81mm Mortar", Munitions = new List<Ammunition>(new Ammunition[] {
                new Ammunition() { Designation = "HE", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromStream(0,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_HE_Charge0",FileMode.Open)),
                        Rangetable.FromStream(1,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_HE_Charge1",FileMode.Open)),
                        Rangetable.FromStream(2,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_HE_Charge2",FileMode.Open)),
                        Rangetable.FromStream(3,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_HE_Charge3",FileMode.Open)),
                        Rangetable.FromStream(4,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_HE_Charge4",FileMode.Open))
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
                    )
                },
                new Ammunition() { Designation = "Illum", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromStream(1,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_Illum_Charge1",FileMode.Open)),
                        Rangetable.FromStream(2,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_Illum_Charge2",FileMode.Open)),
                        Rangetable.FromStream(3,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_Illum_Charge3",FileMode.Open)),
                        Rangetable.FromStream(4,new FileStream(fi  +  "\\Rangetables\\M252\\m252_81mm_Illum_Charge4",FileMode.Open))
                    }
                    )
                }
            })
            },
            new Weapon() {designation = "2B14 81mm Mortar", Munitions = new List<Ammunition>(new Ammunition[] {
                new Ammunition() { Designation = "HE", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromStream(0,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_HE_Charge0",FileMode.Open)),
                        Rangetable.FromStream(1,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_HE_Charge1",FileMode.Open)),
                        Rangetable.FromStream(2,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_HE_Charge2",FileMode.Open)),
                        Rangetable.FromStream(3,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_HE_Charge3",FileMode.Open)),
                        Rangetable.FromStream(4,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_HE_Charge4",FileMode.Open))
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
                    )
                },
                new Ammunition() { Designation = "Illum", Rangetables = new List<Rangetable>(
                    new Rangetable[] {
                        Rangetable.FromStream(0,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_Illum_Charge0",FileMode.Open)),
                        Rangetable.FromStream(1,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_Illum_Charge1",FileMode.Open)),
                        Rangetable.FromStream(2,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_Illum_Charge2",FileMode.Open)),
                        Rangetable.FromStream(3,new FileStream(fi  +  "\\Rangetables\\2B14\\2b14_82mm_Illum_Charge3",FileMode.Open))
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

        public List<Ammunition> Munitions { get; set; }

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
    }

    public class Rangetable
    {
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
                        int time = (int)(float.Parse(l[3]) * 1000);
                        int dTime = (int)(float.Parse(l[4]) * 1000);

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
