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

        public MissionWindow(BatteryWindow battery)
        {
            this.batterywindow = battery;
            this.batterywindow.SelectedBattery.PropertyChanged += new PropertyChangedEventHandler(MissionWindow_PropertyChanged);

            InitializeComponent();
        }

        void MissionWindow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Mission")
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
    }
}
