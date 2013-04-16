using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ArtyCalc.Model
{
    public abstract class MissionSpec : INotifyPropertyChanged
    {
        protected abstract Coordinate GetCoords();

        public Coordinate Coords
        {
            get { return GetCoords(); }
        }

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

    public class Mission
    {
    }
}
