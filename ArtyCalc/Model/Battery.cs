using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace ArtyCalc.Model
{
    public enum BatteryType {
        Mortar,
        M119
    }

    public class Battery : INotifyPropertyChanged
    {
        private string name;
        private string callsign;

        private BatteryType bType;
        private Coordinate coords;
        private double dir;

        private string prefix;
        private int start;

        private ObservableCollection<KnownPoint> observers;
        private ObservableCollection<KnownPoint> knownpoints;

        #region Properties

        

        public ObservableCollection<KnownPoint> Observers
        {
            get { return observers; }
            set
            {
                observers = value;
                OnPropertyChanged("Observers");
            }
        }

        

        public ObservableCollection<KnownPoint> Knownpoints
        {
            get { return knownpoints; }
            set
            {
                knownpoints = value;
                OnPropertyChanged("Knownpoints");
            }
        }



        public BatteryType BType
        {
            get { return bType; }
            set
            {
                bType = value;
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

        public double Dir
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

        public Battery(string name, string callsign, BatteryType btype, Coordinate coords, double dir, string prefix, int start)
        {
            this.name = name;
            this.callsign = callsign;
            this.bType = btype;
            this.coords = coords;
            this.dir = dir;
            this.prefix = prefix;
            this.start = start;

            this.observers = new ObservableCollection<KnownPoint>();
            this.knownpoints = new ObservableCollection<KnownPoint>();
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
