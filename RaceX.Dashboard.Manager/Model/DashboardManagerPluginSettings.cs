using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceX.Dashboard.Manager.Model
{
    public class DashboardManagerPluginSettings : BaseModel
    {
        #region Fields
        private ObservableCollection<CarDashboard> _CarDashboards = new ObservableCollection<CarDashboard>();
        private bool _IsVocorePluginEnabled;
        private bool _IsUSBD480PluginEnabled;
        private bool _IsAX206PluginEnabled;
        private bool _IsCorsairNexuPluginEnabled;
        #endregion

        #region Properties
        public ObservableCollection<CarDashboard> CarDashboards
        {
            get { return _CarDashboards; }
            set { _CarDashboards = value; OnPropertyChanged("CarDashboards"); }
        }
        public bool IsVocorePluginEnabled
        {
            get { return _IsVocorePluginEnabled; }
            set { _IsVocorePluginEnabled = value; OnPropertyChanged("IsVocorePluginEnabled"); }
        }
        public bool IsUSBD480PluginEnabled
        {
            get { return _IsUSBD480PluginEnabled; }
            set { _IsUSBD480PluginEnabled = value; OnPropertyChanged("IsUSBD480PluginEnabled"); }
        }
        public bool IsAX206PluginEnabled
        {
            get { return _IsAX206PluginEnabled; }
            set { _IsAX206PluginEnabled = value; OnPropertyChanged("IsAX206PluginEnabled"); }
        }
        public bool IsCorsairNexuPluginEnabled
        {
            get { return _IsCorsairNexuPluginEnabled; }
            set { _IsCorsairNexuPluginEnabled = value; OnPropertyChanged("IsCorsairNexuPluginEnabled"); }
        }
        #endregion
    }
}
