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
using System.Windows.Shapes;
using ArtyCalc.Model;
using System.ComponentModel;
using System.Timers;
using System.Threading;

namespace ArtyCalc
{
    /// <summary>
    /// Interaction logic for MissionWindow.xaml
    /// </summary>
    public partial class MissionWindow : Window, INotifyPropertyChanged
    {
        BatteryWindow batterywindow;

        
        //private MissionSpec mission;

        /*
        public MissionSpec Mission
        {
            get {
                if (batterywindow != null && batterywindow.SelectedBattery != null)
                {
                    return batterywindow.SelectedBattery.CurrentMission;
                }
                else
                    return null;
            }
            set
            {
                if (batterywindow != null && batterywindow.SelectedBattery != null)
                {
                    batterywindow.SelectedBattery.CurrentMission = value;
                    OnPropertyChanged("Mission");
                }
            }
        }*/

        public BatteryWindow BatteryWindow
        {
            get { return batterywindow; }
            set 
            {
                batterywindow = value;
                OnPropertyChanged("BatteryWindow");
            }
        }

        Thread uithread;

        public MissionWindow(BatteryWindow battery)
        {
            this.batterywindow = battery;
            this.batterywindow.SelectedBattery.PropertyChanged += new PropertyChangedEventHandler(MissionWindow_PropertyChanged);

            t.Elapsed += t_Elapsed;

            uithread = Thread.CurrentThread;

            InitializeComponent();
        }

        void MissionWindow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentMission")
            {
                var Mission = batterywindow.SelectedBattery.CurrentMission;

                if (Mission != null)
                {
                    if (Mission is MissionGridSpec)
                    {
                        EMissionType.SelectedIndex = 0;
                    }
                    else if (Mission is MissionPolarSpec)
                    {
                        EMissionType.SelectedIndex = 1;
                    }
                    else if (Mission is MissionShiftSpec)
                    {
                        EMissionType.SelectedIndex = 2;
                    }
                }
            }
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void EAmmo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //mission.Ammunition = (Ammunition)EAmmo.SelectedItem;
            //Console.WriteLine("Changed item - selected item: " + EAmmo.SelectedItem + " - selected value: " + EAmmo.SelectedValue);
        }

        private void AdjustApply_Click(object sender, RoutedEventArgs e)
        {
            var m = batterywindow.SelectedBattery.CurrentMission;

            m.Adjustment = m.Adjustment.Shift(m.AdjustOTDir.GetRadiansRepresentation(), m.AdjustAdd, m.AdjustRight, m.AdjustUp);
        }

        private void AdjustReset_Click(object sender, RoutedEventArgs e)
        {
            batterywindow.SelectedBattery.CurrentMission.Adjustment = Coordinate.Zero;
        }

        double timeLeft = 0;
        System.Timers.Timer t = new System.Timers.Timer(100);

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CurrentSolutionBox.IsEnabled = false;

            var m = batterywindow.SelectedBattery.CurrentMission;

            timeLeft = m.CurrentSolution.Time;
          
            t.Start();

            TimerStopButton.IsEnabled = true;

            if (m.RoundsLeft > 0)
            {
                m.RoundsLeft = m.RoundsLeft - 1;
            }
        }

        public delegate void Tick();

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            Tick tick = delegate
            {
                timeLeft = timeLeft - 100;

                var display = timeLeft / 1000;

                SplashTimeBlock.Text = display.ToString("F2");
            };

            this.Dispatcher.Invoke(tick);
        }

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

        private void Adjust_Fire_Click(object sender, RoutedEventArgs e)
        {
            var m = batterywindow.SelectedBattery.CurrentMission;

            m.RoundsLeft = m.AdjustRounds;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = !batterywindow.closing;
        }

        private void AdjustRecord_Click(object sender, RoutedEventArgs e)
        {
            MissionGridSpec mission = new MissionGridSpec(batterywindow.SelectedBattery);

            batterywindow.SelectedBattery.CurrentMission.CopyTo(mission);
            mission.TargetNumber = mission.TargetNumber + " Recorded";
            mission.Grid = batterywindow.SelectedBattery.CurrentMission.AdjustedCoords;
            mission.Adjustment = Coordinate.Zero;

            batterywindow.SelectedBattery.Missions.Add(mission);

            KnownPoint p = new KnownPoint(mission.Grid, mission.TargetNumber);
            batterywindow.SelectedBattery.Knownpoints.Add(p);
        }
    }
}
