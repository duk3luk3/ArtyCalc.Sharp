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
using ArtyCalc.UI;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace ArtyCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class BatteryWindow : Window, INotifyPropertyChanged
    {

        private RangetableWindow rw;

        private SObservableCollection<Battery> batteryList = new SObservableCollection<Battery>();
        public SObservableCollection<Battery> BatteryList
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

        Thread uithread;

        public BatteryWindow()
        {

            uithread = Thread.CurrentThread;


            //Battery b = new Battery("New Battery", "abc", Weapon.DefinedWeapons[0], new Coordinate("0505",0), BaseAngle.Create<MilAngle>(0), "pre", 0);
            //b.Observers.Add(new KnownPoint(new Coordinate("0808",100),"obs1"));
            //b.Observers.Add(new KnownPoint(new Coordinate("0909", 100), "obs2"));
            //BatteryList.Add(b);

            //Battery b2 = new Battery("New Battery 2", "def", Weapon.DefinedWeapons[0], new Coordinate("0606", 100), BaseAngle.Create<MilAngle>(0), "pre", 0);
            //BatteryList.Add(b2);

            //SelectedBattery = b;


            try
            {

                rw = new RangetableWindow();



                InitializeComponent();
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    Console.WriteLine(ex.ToString());
                    ex = ex.InnerException;
                }
            }
        }

        private void NewBattery_Click(object sender, RoutedEventArgs e)
        {
            Battery b = new Battery("New Battery", "", Weapon.DefinedWeapons[0], new Coordinate("0000", 0), BaseAngle.Create<MilAngle>(0), "pre", 0);
            b.PropertyChanged += Battery_PropertyChanged;
            BatteryList.Add(b);
            SelectedBattery = b;
        }

        void Battery_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == SelectedBattery && e.PropertyName == "CurrentMission")
            {
                OnPropertyChanged("MissionSelected");

                if (SelectedBattery.CurrentMission is MissionGridSpec)
                {
                    EMissionType.SelectedIndex = 0;
                }
                else if (SelectedBattery.CurrentMission is MissionPolarSpec)
                {
                    EMissionType.SelectedIndex = 1;
                }
                else if (SelectedBattery.CurrentMission is MissionShiftSpec)
                {
                    EMissionType.SelectedIndex = 2;
                }
            }
        }

        public bool BatterySelected
        {
            get
            {
                return SelectedBattery != null;
            }
        }

        public bool MissionSelected
        {
            get
            {
                return SelectedBattery != null && SelectedBattery.CurrentMission != null;
            }
        }

        protected void OnPropertyChanged(string name)
        {
            if (name == "SelectedBattery")
            {
                OnPropertyChanged("BatterySelected");
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NewObserver_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedBattery != null)
            {
                KnownPoint o = new KnownPoint(new Coordinate("00", 0), "New Observer");

                SelectedBattery.Observers.Add(o);
                EObsSelect.SelectedItem = o;
            }

        }

        private void NewKnownpoint_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedBattery != null)
            {
                KnownPoint k = new KnownPoint(new Coordinate("00", 0), "New Known Point");

                SelectedBattery.Knownpoints.Add(k);
                EKPSelect.SelectedItem = k;
            }
        }

        private void NewMissionPolar_Click(object sender, RoutedEventArgs e)
        {
            var mission = new MissionPolarSpec(selectedBattery);
            selectedBattery.Missions.Add(mission);
            selectedBattery.CurrentMission = mission;
        }

        private void NewMissionGrid_Click(object sender, RoutedEventArgs e)
        {
            var mission = new MissionGridSpec(selectedBattery);
            selectedBattery.Missions.Add(mission);
            selectedBattery.CurrentMission = mission;
        }

        private void NewMissionShift_Click(object sender, RoutedEventArgs e)
        {
            var mission = new MissionShiftSpec(selectedBattery);
            selectedBattery.Missions.Add(mission);
            selectedBattery.CurrentMission = mission;
        }

        private void BRangetables_Click(object sender, RoutedEventArgs e)
        {
            rw.Show();
        }

        public bool closing = false;

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            closing = true;

            App.Current.Shutdown(0);
        }

        private void RemoveObserver_Click(object sender, RoutedEventArgs e)
        {
            SelectedBattery.Observers.Remove(SelectedObserver);
        }

        private void RemoveKnownpoint_Click(object sender, RoutedEventArgs e)
        {
            SelectedBattery.Knownpoints.Remove(SelectedPoint);
        }

        private void MissionDelete_Click(object sender, RoutedEventArgs e)
        {
            SelectedBattery.Missions.Remove(SelectedBattery.CurrentMission);
        }

        private void EBattSerialize_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter wr = new StreamWriter(@"D:/batterytest.xml");

            SelectedBattery.Save(wr);

            wr.Close();
        }

        private void EBattDeSerialize_Click(object sender, RoutedEventArgs e)
        {
            StreamReader rr = new StreamReader(@"D:/batterytest.xml");

            Battery b = Battery.Load(rr);

            System.Console.WriteLine("Selected weapon: " + b.BWeapon.Designation);

            BatteryList.Add(b);
            SelectedBattery = b;

        }

        double timeLeft = 0;
        System.Timers.Timer t = new System.Timers.Timer(100);

        private void Timer_Stop(object sender, RoutedEventArgs e)
        {
            if (t.Enabled)
            {
                t.Stop();
                ((Button)sender).Content = "Reset";
            }
            else
            {
                SplashTimeBlock.Text = "00:00";
                ((Button)sender).IsEnabled = false;
                ((Button)sender).Content = "Stop Timer";
                CurrentSolutionBox.IsEnabled = true;
            }
        }

        private void Timer_Start(object sender, RoutedEventArgs e)
        {
            CurrentSolutionBox.IsEnabled = false;

            var m = SelectedBattery.CurrentMission;

            timeLeft = m.CurrentSolution.Time;

            t.Start();

            TimerStopButton.IsEnabled = true;

            if (m.RoundsLeft > 0)
            {
                m.RoundsLeft = m.RoundsLeft - 1;
            }
        }

        private void AdjustReset_Click(object sender, RoutedEventArgs e)
        {
            SelectedBattery.CurrentMission.Adjustment = Coordinate.Zero;
        }

        private void AdjustRecord_Click(object sender, RoutedEventArgs e)
        {
            MissionGridSpec mission = new MissionGridSpec(SelectedBattery);

            SelectedBattery.CurrentMission.CopyTo(mission);
            mission.TargetNumber = mission.TargetNumber + " Recorded";
            mission.Grid = SelectedBattery.CurrentMission.AdjustedCoords;
            mission.Adjustment = Coordinate.Zero;

            SelectedBattery.Missions.Add(mission);

            KnownPoint p = new KnownPoint(mission.Grid, mission.TargetNumber);
            SelectedBattery.Knownpoints.Add(p);
        }

        private void AdjustApply_Click(object sender, RoutedEventArgs e)
        {
            var m = SelectedBattery.CurrentMission;
            m.Adjustment = m.Adjustment.Shift(m.AdjustOTDir.RadiansValue, m.AdjustAdd, m.AdjustRight, m.AdjustUp);
        }

        private void Adjust_Fire_Click(object sender, RoutedEventArgs e)
        {
            var m = SelectedBattery.CurrentMission;
            m.RoundsLeft = m.AdjustRounds;
        }

        private void Fire_for_Effect_Click(object sender, RoutedEventArgs e)
        {
            var m = SelectedBattery.CurrentMission;
            m.RoundsLeft = m.Rounds;
        }

        private void End_Mission_Click(object sender, RoutedEventArgs e)
        {
            var m = SelectedBattery.CurrentMission;
            m.RoundsLeft = 0;
        }

        private string SavePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ArtyCalc\\Battery.xml";
        private const string SaveFilter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {
                ofd.FileName = SavePath;
                ofd.Filter = SaveFilter;

                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    XmlSerializer s = new XmlSerializer(typeof(SObservableCollection<Battery>));

                    SObservableCollection<Battery> loadedBatteries = s.Deserialize(new StreamReader(ofd.FileName)) as SObservableCollection<Battery>;

                    if (((sender as System.Windows.Controls.Control).Tag as string) == "New")
                    {
                        SelectedBattery = null;
                        BatteryList.Clear();
                    }

                    foreach (var b in loadedBatteries)
                    {
                        b.ReInitialize();
                        b.PropertyChanged += Battery_PropertyChanged;
                        BatteryList.Add(b);
                    }



                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            FileInfo f = new FileInfo(SavePath);
            f.Directory.Create();

            using (System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog())
            {
                sfd.FileName = SavePath;
                sfd.CreatePrompt = true;
                sfd.CheckFileExists = false;
                sfd.CheckPathExists = false;
                sfd.Filter = SaveFilter;

                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {



                    var coll = BatteryList;
                    if (((sender as System.Windows.Controls.Control).Tag as string) == "Selected")
                    {
                        coll = new SObservableCollection<Battery>();
                        coll.Add(SelectedBattery);
                    }

                    XmlSerializer s = new XmlSerializer(typeof(SObservableCollection<Battery>));

                    s.Serialize(new StreamWriter(sfd.FileName), coll);
                }
            }
        }

        private void RemoveBattery_Click(object sender, RoutedEventArgs e)
        {
            var b = SelectedBattery;
            SelectedBattery = null;
            BatteryList.Remove(b);
        }

        private void ClearBattery_Click(object sender, RoutedEventArgs e)
        {
            SelectedBattery = null;
            BatteryList.Clear();
        }

        private void ClearObserver_Click(object sender, RoutedEventArgs e)
        {
            SelectedObserver = null;
            SelectedBattery.Observers.Clear();
        }

        private void ClearKnownpoint_Click(object sender, RoutedEventArgs e)
        {
            SelectedPoint = null;
            SelectedBattery.Observers.Clear();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("Mission Changed");

        }
    }
}
