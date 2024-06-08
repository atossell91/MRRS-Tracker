using mrrslib;
using mrrswpf.Commands;
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

        public ICommand CmdOpenAddInspectorActivity { get; set; }

        private Timer _timer;
        private MRRS mrrs;
        private DateTime _lastUpdated;
        private AddInspectorActivity _addInspectorActivity;

        public InspectorActivityViewModel()
        {
            string dbPath = Properties.Settings.Default.DbPath;
            mrrs = new MRRS(dbPath);
            InspectorActivities = mrrs.GetActivityList();
            _lastUpdated = DateTime.Now;
            _timer = new Timer(new TimerCallback(_ => TryupdateList()), null, 0, 5000);
            _addInspectorActivity = new AddInspectorActivity();
            CmdOpenAddInspectorActivity = new OpenAddInspectorActivityWindow(_addInspectorActivity);
        }

        private void refreshList()
        {
            InspectorActivities = mrrs.GetActivityList();
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
