using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace ArtyCalc.Model
{
    public class Battery : INotifyPropertyChanged
    {
        private string name;
        private string callsign;

        private Weapon bWeapon;
        private Coordinate coords;
        private BaseAngle dir;

        private string prefix;
        private int start;

        private ObservableCollection<KnownPoint> observers = new ObservableCollection<KnownPoint>();
        private ObservableCollection<KnownPoint> knownpoints = new ObservableCollection<KnownPoint>();
        private ObservableCollection<MissionSpec> missions = new ObservableCollection<MissionSpec>();


        private MissionSpec currentMission;
        

        #region Properties

        public ObservableCollection<MissionSpec> Missions
        {
            get { return missions; }
            /*set
            {
                missions = value;
                OnPropertyChanged("Missions");
            }*/
        }
        public MissionSpec CurrentMission
        {
            get { return currentMission; }
            set
            {
                currentMission = value;
                OnPropertyChanged("CurrentMission");
            }
        }
        public ObservableCollection<KnownPoint> Observers
        {
            get { return observers; }
            /*set
            {
                observers = value;
                OnPropertyChanged("Observers");
            }*/
        }
        public ObservableCollection<KnownPoint> Knownpoints
        {
            get { return knownpoints; }
            /*set
            {
                knownpoints = value;
                OnPropertyChanged("Knownpoints");
            }*/
        }
        public Weapon BWeapon
        {
            get { return bWeapon; }
            set
            {
                bWeapon = value;
                OnPropertyChanged("BType");
            }
        }
        public Coordinate Coords
        {
            get { return coords; }
            set
            {
                coords = value;

                OnPropertyChanged("Coords");                
            }
        }

        void coords_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Coords");
        }

        public BaseAngle Dir
        {
            get { return dir; }
            set
            {
                dir = value;
                OnPropertyChanged("Dir");
            }
        }

        public string Prefix
        {
            get { return prefix; }
            set
            {
                prefix = value;
                OnPropertyChanged("Prefix");
            }
        }


        public int Start
        {
            get { return start; }
            set
            {
                start = value;
                OnPropertyChanged("Start");
            }
        }


        public string Name
        {
            get { return name; }
            set
            {
                this.name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Callsign
        {
            get { return callsign; }
            set
            {
                Console.WriteLine("New Callsign: " + value);
                this.callsign = value;
                OnPropertyChanged("Callsign");
            }
        }
        #endregion
        

        public event PropertyChangedEventHandler PropertyChanged;

        public Battery(string name, string callsign, Weapon bweapon, Coordinate coords, BaseAngle dir, string prefix, int start)
        {
            this.name = name;
            this.callsign = callsign;
            this.bWeapon = bweapon;
            this.coords = coords;
            this.dir = dir;
            this.prefix = prefix;
            this.start = start;

            // using initializer instead
            //this.observers = new ObservableCollection<KnownPoint>();
            //this.knownpoints = new ObservableCollection<KnownPoint>();
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
