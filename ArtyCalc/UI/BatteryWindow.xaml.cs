using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ArtyCalc.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ArtyCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class BatteryWindow : Window, INotifyPropertyChanged
    {
        private MissionWindow mw;

        private ObservableCollection<Battery> batteryList = new ObservableCollection<Battery>();
        public ObservableCollection<Battery> BatteryList
        {
            get { return batteryList; }
            set { batteryList = value; }
        }

        private Battery selectedBattery;

        public Battery SelectedBattery
        {
            get { return selectedBattery; }
            set 
            {
                selectedBattery = value;
                OnPropertyChanged("SelectedBattery");
            }
        }

        private KnownPoint selectedObserver;
        public KnownPoint SelectedObserver
        {
            get { return selectedObserver; }
            set
            {
                selectedObserver = value;
                OnPropertyChanged("SelectedObserver");
            }
        }

        private KnownPoint selectedPoint;

        public KnownPoint SelectedPoint
        {
            get { return selectedPoint; }
            set
            {
                selectedPoint = value;
                OnPropertyChanged("SelectedPoint");
            }
        }


        public BatteryWindow()
        {
            mw = new MissionWindow(this);

            Battery b = new Battery("New Battery", "abc", Weapon.DefinedWeapons[0], new Coordinate("0505",0), 0, "pre", 0);
            b.Observers.Add(new KnownPoint(new Coordinate("0808",100),"obs1"));
            b.Observers.Add(new KnownPoint(new Coordinate("0909", 100), "obs2"));
            BatteryList.Add(b);
            
            Battery b2 = new Battery("New Battery 2", "def", Weapon.DefinedWeapons[0], new Coordinate("0606",100), 0, "pre", 0);
            BatteryList.Add(b2);

            SelectedBattery = b;
            
            InitializeComponent();
        }

        private void EBattSave_Click(object sender, RoutedEventArgs e)
        {
            Battery b = new Battery(EBattName.Text, "", Weapon.DefinedWeapons[0], new Coordinate("0808",200), 0, "pre", 0);
            BatteryList.Add(b);
            SelectedBattery = b;
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void BObserverAdd_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedBattery != null)
            {
                KnownPoint o = new KnownPoint(new Coordinate("00", 0), "New Observer");

                SelectedBattery.Observers.Add(o);
                EObsSelect.SelectedItem = o;
            }
            
        }

        private void BKnownpointAdd_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedBattery != null)
            {
                KnownPoint k = new KnownPoint(new Coordinate("00", 0), "New Known Point");

                SelectedBattery.Knownpoints.Add(k);
                EKPSelect.SelectedItem = k;
            }
        }

        private void BMissionPolar_Click(object sender, RoutedEventArgs e)
        {
            var mission = new MissionPolarSpec();
            selectedBattery.Missions.Add(mission);
            selectedBattery.CurrentMission = mission;

            mw.Show();
            mw.Left = this.Left + this.Width;
            mw.Top = this.Top;
        }

        private void BMissionGrid_Click(object sender, RoutedEventArgs e)
        {
            var mission = new MissionGridSpec();
            selectedBattery.Missions.Add(mission);
            selectedBattery.CurrentMission = mission;

            mw.Show();
            mw.Left = this.Left + this.Width;
            mw.Top = this.Top;
        }

        private void BMissionShift_Click(object sender, RoutedEventArgs e)
        {
            var mission = new MissionShiftSpec();
            selectedBattery.Missions.Add(mission);
            selectedBattery.CurrentMission = mission;

            mw.Show();
            mw.Left = this.Left + this.Width;
            mw.Top = this.Top;
        }
    }
}
