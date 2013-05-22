using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ArtyCalc.UI
{
    /// <summary>
    /// Interaction logic for RangetableWindow.xaml
    /// </summary>
    public partial class RangetableWindow : Window, INotifyPropertyChanged
    {
        private Weapon selectedWeapon;

        private Ammunition selectedMunition;

        private Rangetable selectedRangetable;

        public Rangetable SelectedRangetable
        {
            get { return selectedRangetable; }
            set
            {
                selectedRangetable = value;
                OnPropertyChanged("SelectedRangetable");
            }
        }


        public Ammunition SelectedMunition
        {
            get { return selectedMunition; }
            set
            {
                selectedMunition = value;
                OnPropertyChanged("SelectedMunition");
            }
        }


        public Weapon SelectedWeapon
        {
            get { return selectedWeapon; }
            set
            {
                selectedWeapon = value;
                OnPropertyChanged("SelectedWeapon");
            }
        }





        public RangetableWindow()
        {
            InitializeComponent();
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
