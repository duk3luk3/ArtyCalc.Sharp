using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ArtyCalc.Model
{
    /// <summary>
    /// Shamelessly ripped from http://kentb.blogspot.co.uk/2007/11/serializing-observablecollection.html
    /// </summary>
    /// <typeparam name="T">The type param, duh</typeparam>
    [Serializable]
    public class SObservableCollection<T> : ObservableCollection<T>, INotifyPropertyChanged
    {
        [field: NonSerialized]
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        [field: NonSerialized]
        private PropertyChangedEventHandler _propertyChangedEventHandler;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                _propertyChangedEventHandler = Delegate.Combine(_propertyChangedEventHandler, value) as PropertyChangedEventHandler;
            }
            remove
            {
                _propertyChangedEventHandler = Delegate.Remove(_propertyChangedEventHandler, value) as PropertyChangedEventHandler;
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler handler = CollectionChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = _propertyChangedEventHandler;

            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
