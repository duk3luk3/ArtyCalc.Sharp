using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ArtyCalc.Model
{
    [Serializable]
    public class Battery : INotifyPropertyChanged
    {
        private string name;
        private string callsign;

        private Weapon bWeapon;

        private Coordinate coords;
        private BaseAngle dir;

        private string prefix;
        private int start;

        private SObservableCollection<KnownPoint> observers = new SObservableCollection<KnownPoint>();
        private SObservableCollection<KnownPoint> knownpoints = new SObservableCollection<KnownPoint>();
        private SObservableCollection<MissionSpec> missions = new SObservableCollection<MissionSpec>();


        private MissionSpec currentMission;


        #region Properties

        public SObservableCollection<MissionSpec> Missions
        {
            get { return missions; }
            /*set
            {
                missions = value;
                OnPropertyChanged("Missions");
            }*/
        }

        [XmlIgnore]
        public MissionSpec CurrentMission
        {
            get { return currentMission; }
            set
            {
                currentMission = value;
                OnPropertyChanged("CurrentMission");
                
            }
        }
        
        public SObservableCollection<KnownPoint> Observers
        {
            get { return observers; }
            /*set
            {
                observers = value;
                OnPropertyChanged("Observers");
            }*/
        }
        public SObservableCollection<KnownPoint> Knownpoints
        {
            get { return knownpoints; }
            /*set
            {
                knownpoints = value;
                OnPropertyChanged("Knownpoints");
            }*/
        }
        
        [XmlIgnore]
        public Weapon BWeapon
        {
            get { return bWeapon; }
            set
            {
                bWeapon = value;
                OnPropertyChanged("BWeapon");
            }
        }


        public string Weapon_Surrogate
        {
            get
            {
                if (BWeapon != null)
                    return BWeapon.Designation;
                else
                    return "";
            }
            set
            {
                var selection = Weapon.DefinedWeapons.Where(w => w.Designation == value);

                if (selection.Count() == 1)
                    BWeapon = selection.Single();
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

        /// <summary>
        /// Parameterless constructor for serialization
        /// </summary>
        public Battery() { }

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

        public void Save(TextWriter wr)
        {

            XmlSerializer serializer = new XmlSerializer(typeof(Battery));

            serializer.Serialize(wr, this);
        }

        public void ReInitialize()
        {
            foreach (var m in missions)
            {
                m.battery = this;

                var ammo = bWeapon.Munitions.Where(mun => mun.Designation == m.Ammunition_Proxy);

                if (ammo.Count() == 1)
                {
                    m.Ammunition = ammo.Single();

                    var fuze = m.Ammunition.Fuzes.Where(f => f.Designation == m.Fuze_Proxy);

                    if (fuze.Count() == 1)
                    {
                        m.Fuze = fuze.Single();
                    }
                }
            }
        }

        public static Battery Load(TextReader rr)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Battery));

            Battery batt = serializer.Deserialize(rr) as Battery;

            foreach (var m in batt.missions)
            {
                m.battery = batt;

                var ammo = batt.bWeapon.Munitions.Where(mun => mun.Designation == m.Ammunition_Proxy);

                if (ammo.Count() == 1)
                    m.Ammunition = ammo.Single();
            }

            return batt;
        }

    }
}
