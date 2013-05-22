using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace ArtyCalc.Model
{
    public abstract class MissionSpec : INotifyPropertyChanged
    {
        #region fields
        protected abstract Coordinate GetCoords();
        private string targetNumber;
        private string targetDescription;
        private double radius;
        private double length;
        private BaseAngle attitude;
        private bool dangerClose;
        private string notes;
        private Ammunition munition;
        private Fuze fuze;
        private int adjustPieces;
        private int adjustRounds;
        private Coordinate adjustment = Coordinate.Zero;
        private ObservableCollection<FireSolution> solutions = new ObservableCollection<FireSolution>();
        private FireSolution currentSolution = null;
        
        private BaseAngle adjustOTDir;
        private double adjustAdd;
        private double adjustRight;
        private double adjustUp;

        private int roundsLeft;

        private int rounds;

        public int Rounds
        {
            get { return rounds; }
            set
            {
                rounds = value;
                OnPropertyChanged("Rounds");
            }
        }

        private string fuzeTime;

        public string FuzeTime
        {
            get { return fuzeTime; }
            set
            {
                fuzeTime = value;
                OnPropertyChanged("FuzeTime");
            }
        }


        #endregion

        #region props
        public int RoundsLeft
        {
            get { return roundsLeft; }
            set
            {
                roundsLeft = value;
                OnPropertyChanged("RoundsLeft");
            }
        }
        public double Distance
        {
            get
            {
                return BallisticModel.RangeAzimuthUp(this.battery, this).Item1;
            }
        }
        public BaseAngle Azimuth
        {
            get
            {
                var az = BallisticModel.RangeAzimuthUp(this.battery, this).Item2;

                var angle = new MilAngle();

                angle.SetRadiansRepresentation(az);

                return angle;
            }
        }
        public Coordinate Coords
        {
            get { return GetCoords(); }
        }
        public string TargetNumber
        {
            get { return targetNumber; }
            set
            {
                targetNumber = value;
                OnPropertyChanged("TargetNumber");
            }
        }
        public string TargetDescription
        {
            get { return targetDescription; }
            set
            {
                targetDescription = value;
                OnPropertyChanged("TargetDescription");
            }
        }
        public double Radius
        {
            get { return radius; }
            set
            {
                radius = value;
                OnPropertyChanged("Radius");
            }
        }
        public double Length
        {
            get { return length; }
            set
            {
                length = value;
                OnPropertyChanged("Length");
            }
        }
        public BaseAngle Attitude
        {
            get { return attitude; }
            set
            {
                attitude = value;
                OnPropertyChanged("Attitude");
            }
        }
        public bool DangerClose
        {
            get { return dangerClose; }
            set
            {
                dangerClose = value;
                OnPropertyChanged("DangerClose");
            }
        }
        public string Notes
        {
            get { return notes; }
            set
            {
                notes = value;
                OnPropertyChanged("Notes");
            }
        }
        public Ammunition Ammunition
        {
            get { return munition; }
            set
            {
                munition = value;
                Console.WriteLine("Ammunition set");
                OnPropertyChanged("Ammunition");
            }
        }
        public Fuze Fuze
        {
            get { return fuze; }
            set
            {
                fuze = value;
                OnPropertyChanged("Fuze");
            }
        }
        public int AdjustPieces
        {
            get { return adjustPieces; }
            set
            {
                adjustPieces = value;
                OnPropertyChanged("AdjustPieces");
            }
        }
        public int AdjustRounds
	{
		get { return adjustRounds;}
		set 
		{
			adjustRounds = value;
			OnPropertyChanged("AdjustRounds");
		}
	}
        public string MTO
        {
            get
            {
                return GetMTO();
            }
        }
        public string MTB
        {
            get
            {
                return GetMTB();
            }
        }
        public Coordinate Adjustment
        {
            get { return adjustment; }
            set
            {
                adjustment = value;
                OnPropertyChanged("Adjustment");
                OnPropertyChanged("AdjustedCoords");
            }
        }
        public Coordinate AdjustedCoords
        {
            get
            {
                return Coordinate.Add(GetCoords(), adjustment);
            }
        }
        public ObservableCollection<FireSolution> Solutions
	    {
		    get { return solutions;}
		    set 
		    {
		        solutions = value;
		        OnPropertyChanged("Solutions");
		    }
	    }
        public FireSolution CurrentSolution
	    {
		    get { return currentSolution;}
		    set 
		    {
			    currentSolution = value;
			    OnPropertyChanged("CurrentSolution");
		    }
	    }
        
        public BaseAngle AdjustOTDir
        {
            get { return adjustOTDir; }
            set
            {
                adjustOTDir = value;
                OnPropertyChanged("AdjustOTDir");
            }
        }
        public double AdjustAdd
        {
            get { return adjustAdd; }
            set
            {
                adjustAdd = value;
                OnPropertyChanged("AdjustAdd");
            }
        }
        public double AdjustRight
        {
            get { return adjustRight; }
            set
            {
                adjustRight = value;
                OnPropertyChanged("AdjustRight");
            }
        }
        public double AdjustUp
        {
            get { return adjustUp; }
            set
            {
                adjustUp = value;
                OnPropertyChanged("AdjustUp");
            }
        }

        #endregion

        public string GetMTO()
        {
            return battery.Callsign + ", battery adjust fire, " + adjustPieces + " pieces, " + adjustRounds + " rounds, Fuze " + fuze + " in effect, target number " + targetNumber + ", over.";
        }

        public string GetMTB()
        {
            if (Ammunition != null && CurrentSolution != null && Fuze != null)
            {

                string pieces = "";
                string ineffect = "";
                if (adjustRounds > 0)
                {
                    pieces = "Battery Adjust, Gun Number " + AdjustPieces + " " + AdjustRounds + " Rounds";
                    ineffect = "" + Rounds + ", " + Fuze.Designation + " in effect";
                }
                else
                    pieces = "Battery " + Rounds + " Rounds";

                string shell = "Shell " + Ammunition.Designation;
                string charge = "Charge " + CurrentSolution.Charge;
                string fuze = "Fuze " + Fuze.Designation;

                if (Fuze.HasTimeFuze)
                {
                    fuze = fuze + ", Time " + fuzeTime;
                }

                return "Firemission! " + pieces + "! " + shell + "! " + charge + "! " + fuze + " Deflection " + CurrentSolution.Deflection + "! " + "Quadrant " + CurrentSolution.Elevation + "! " + ineffect;
            }

            return "";
        }

        public MissionSpec(Battery battery)
        {
            this.battery = battery;

            this.targetNumber = battery.Prefix + battery.Missions.Count;

            PropertyChanged += MissionSpec_PropertyChanged;
        }

        void MissionSpec_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Coords")
            {
                OnPropertyChanged("AdjustedCoords");
                OnPropertyChanged("Adjustment");
                OnPropertyChanged("Distance");
                OnPropertyChanged("Azimuth");
                OnPropertyChanged("MTO");
                OnPropertyChanged("MTB");
            }
            if (e.PropertyName == "Ammunition" || e.PropertyName == "Fuze" || e.PropertyName == "CurrentSolution")
            {
                OnPropertyChanged("MTO");
                OnPropertyChanged("MTB");
            }

            //If Coords changed, recalc fire solutions
            if (e.PropertyName == "AdjustedCoords" || e.PropertyName == "Ammunition")
            {
                //solutions.Clear();
                var new_solutions = BallisticModel.CalcFire(battery, this);

                if (e.PropertyName == "AdjustedCoords" && solutions.Count == new_solutions.Count)
                {
                    for (int i = 0; i < solutions.Count; i++)
                    {
                        solutions[i].Deflection = new_solutions[i].Deflection;
                        solutions[i].Elevation = new_solutions[i].Elevation;
                        solutions[i].Time = new_solutions[i].Time;
                    }
                }
                else
                {
                    solutions.Clear();

                    foreach (var s in new_solutions)
                        solutions.Add(s);
                }
                OnPropertyChanged("Solution");
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

        public Battery battery { get; set; }
    }

    public class MissionGridSpec : MissionSpec
    {
        public MissionGridSpec(Battery battery)
            : base(battery)
        {
            grid.PropertyChanged += grid_PropertyChanged;
        }

        void grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Grid");
            OnPropertyChanged("Coords");
        }

        private Coordinate grid = Coordinate.Zero;

        public Coordinate Grid
        {
            get { return grid; }
            set
            {
                grid = value;
                OnPropertyChanged("Coords");
            }
        }

        protected override Coordinate GetCoords()
        {
            return Grid;
        }
        
    }

    public class MissionPolarSpec : MissionSpec
    {
        public MissionPolarSpec(Battery battery)
            : base(battery)
        {
        }

        private KnownPoint observer;
        private BaseAngle otdir = new MilAngle();
        private double range;
        private double dAlt;
        public KnownPoint Observer
        {
            get { return observer; }
            set
            {
                observer = value;
                OnPropertyChanged("Observer");
                OnPropertyChanged("Coords");
            }
        }

        public BaseAngle OTDir
        {
            get { return otdir; }
            set
            {
                otdir = value;
                OnPropertyChanged("OTDir");
                OnPropertyChanged("Coords");
            }
        }

        public double Range
        {
            get { return range; }
            set
            {
                range = value;
                OnPropertyChanged("Range");
                OnPropertyChanged("Coords");
            }
        }

        public double DAlt
        {
            get { return dAlt; }
            set
            {
                dAlt = value;
                OnPropertyChanged("DAlt");
                OnPropertyChanged("Coords");
            }
        }


        protected override Coordinate GetCoords()
        {
            if (observer != null)
            {
                return Observer.Coord.Shift(OTDir.GetRadiansRepresentation(), Range, 0, DAlt);
            }
            else
                return Coordinate.Zero;
        }
    }

    public class MissionShiftSpec : MissionSpec
    {
        public MissionShiftSpec(Battery battery)
            : base(battery)
        {
        }

        private KnownPoint point;
        private BaseAngle otdir = new MilAngle();
        private double right;
        private double add;
        private double up;

        public KnownPoint Point
        {
            get { return point; }
            set
            {
                point = value;
                OnPropertyChanged("Point");
                OnPropertyChanged("Coords");
            }
        }

        public BaseAngle OTDir
        {
            get { return otdir; }
            set
            {
                otdir = value;
                OnPropertyChanged("OTDir");
                OnPropertyChanged("Coords");
            }
        }

        

        public double Right
        {
            get { return right; }
            set
            {
                right = value;
                OnPropertyChanged("Right");
                OnPropertyChanged("Coords");
            }
        }

        

        public double Add
        {
            get { return add; }
            set
            {
                add = value;
                OnPropertyChanged("Add");
                OnPropertyChanged("Coords");
            }
        }
        

        public double Up
        {
            get { return up; }
            set
            {
                up = value;
                OnPropertyChanged("Up");
                OnPropertyChanged("Coords");
            }
        }


        protected override Coordinate GetCoords()
        {
            if (point != null)
            {
                return point.Coord.Shift(otdir.GetRadiansRepresentation(), add, right, up);
            }
            else
            {
                return Coordinate.Zero;
            }
        }
    }
}
