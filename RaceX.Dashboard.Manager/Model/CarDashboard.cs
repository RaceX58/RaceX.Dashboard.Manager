using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceX.Dashboard.Manager.Model
{
    public class CarDashboard : BaseModel
    {
        #region Fields
        private string _Game;
        private string _Car;
        private string _DashboardName;
        private string _DeviceType;
        private bool _IsDefaultDashboard;
        #endregion

        #region Properties
        public string Game
        {
            get { return _Game; }
            set { _Game = value; OnPropertyChanged("Game"); }
        }
        public string Car
        {
            get { return _Car; }
            set { _Car = value; OnPropertyChanged("Car"); }
        }
        public string DashboardName
        {
            get { return _DashboardName; }
            set { _DashboardName = value; OnPropertyChanged("DashboardName"); }
        }
        public string DeviceType
        {
            get { return _DeviceType; }
            set { _DeviceType = value; OnPropertyChanged("DeviceType"); }
        }
        public bool IsDefaultDashboard
        {
            get { return _IsDefaultDashboard; }
            set { _IsDefaultDashboard = value; OnPropertyChanged("IsDefaultDashboard"); }
        }
        #endregion

        #region Constructor

        #endregion
    }
}
