using mrrslib;
using mrrswpf.Commands;
using mrrswpf.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using mrrswpf.Models;
using System.Collections.Specialized;
using mrrswpf.Views;

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

        private ObservableCollection<Inspector> _inspectors;
        public ObservableCollection<Inspector> Inspectors
        {
            get { return _inspectors; }
            set
            {
                _inspectors = value;
                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs(nameof(Inspectors)));
            }
        }

        private ObservableCollection<Activity> _activities;
        public ObservableCollection<Activity> Activities
        {
            get { return _activities; }
            set
            {
                _activities = value;
                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs(nameof(Activities)));
            }
        }

        public ICommand CmdOpenAddInspectorActivity { get; set; }

        private Timer _timer;
        private MRRS mrrs;
        private DateTime _lastUpdated;
        private Configs _configs;

        private AddInspectorActivityDialog _addInspectorActivity;

        public void foo (object sender, NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Foo"));
        }

        public InspectorActivityViewModel()
        {
            _configs = Configs.LoadConfigs("Resources/configs.txt");
            string dbPath = _configs.DatabasePath;
            mrrs = new MRRS(dbPath);
            InspectorActivities = mrrs.GetActivityList();
            _lastUpdated = DateTime.Now;
            _timer = new Timer(new TimerCallback(_ => TryupdateList()), null, 0, 5000);
        }

        private void refreshList()
        {
            InspectorActivities = mrrs.GetActivityList();
            Activities = mrrs.GetActivities();
            Inspectors = mrrs.GetInspectors();
            _lastUpdated = DateTime.Now;
        }

        private void TryupdateList()
        {
            var lastUpdate = mrrs.GetLastDbUpdateTime();
            if (lastUpdate > _lastUpdated)
            {
                refreshList();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
