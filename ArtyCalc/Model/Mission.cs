using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

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

        #endregion

        #region props
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

        #endregion

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class MissionGridSpec : MissionSpec
    {
        private Coordinate grid;

        public Coordinate Grid
        {
            get { return grid; }
            set
            {
                grid = value;
                OnPropertyChanged("Grid");
            }
        }

        protected override Coordinate GetCoords()
        {
            return Grid;
        }
        
    }

    public class MissionPolarSpec : MissionSpec
    {
        private KnownPoint observer;

        public KnownPoint Observer
        {
            get { return observer; }
            set
            {
                observer = value;
                OnPropertyChanged("Observer");
            }
        }

        private double otdir;

        public double OTDir
        {
            get { return otdir; }
            set
            {
                otdir = value;
                OnPropertyChanged("OTDir");
            }
        }

        private double range;

        public double Range
        {
            get { return range; }
            set
            {
                range = value;
                OnPropertyChanged("Range");
            }
        }


        private double dAlt;

        public double DAlt
        {
            get { return dAlt; }
            set
            {
                dAlt = value;
                OnPropertyChanged("DAlt");
            }
        }


        protected override Coordinate GetCoords()
        {
            throw new NotImplementedException();
        }
    }

    public class MissionShiftSpec : MissionSpec
    {
        private KnownPoint point;

        public KnownPoint Point
        {
            get { return point; }
            set
            {
                point = value;
                OnPropertyChanged("Point");
            }
        }

        private double otdir;

        public double OTDir
        {
            get { return otdir; }
            set
            {
                otdir = value;
                OnPropertyChanged("OTDir");
            }
        }

        private double right;

        public double Right
        {
            get { return right; }
            set
            {
                right = value;
                OnPropertyChanged("Right");
            }
        }

        private double add;

        public double Add
        {
            get { return add; }
            set
            {
                add = value;
                OnPropertyChanged("Add");
            }
        }
        private double up;

        public double Up
        {
            get { return up; }
            set
            {
                up = value;
                OnPropertyChanged("Up");
            }
        }


        protected override Coordinate GetCoords()
        {
            throw new NotImplementedException();
        }
    }
}
