using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ArtyCalc.Model
{
    public class KnownPoint : INotifyPropertyChanged
    {
        private Coordinate coord;

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        

        public Coordinate Coord
        {
            get { return coord; }
            set
            {
                coord = value;
                OnPropertyChanged("Coord");
            }
        }

        public KnownPoint(Coordinate coord, string name)
        {
            this.coord = coord;
            this.name = name;
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
