using mrrslib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrrswpf.ViewModels
{
    public class InspectorActivityViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<InspectorActivity> _inspectorActivities;
        public ObservableCollection<InspectorActivity> InspectorActivities {
            get { return _inspectorActivities; }
            set
            {
                _inspectorActivities = value;
                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs(nameof(InspectorActivities)));
            }
        }

        public InspectorActivityViewModel()
        {
            string dbPath = @"C:\Users\atoss\Programming\MRRS-Tracker\testroot\MRRS.db";
            MRRS mrrs = new MRRS(dbPath);
            InspectorActivities = mrrs.GetActivityList();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
