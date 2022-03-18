using GameReaderCommon;
using RaceX.Dashboard.Manager.Commands;
using RaceX.Dashboard.Manager.Model;
using RaceX.Dashboard.Manager.View;
using SimHub.Plugins;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace RaceX.Dashboard.Manager.ViewModel
{
    [PluginDescription("Automatically load dashboard based on current car")]
    [PluginAuthor("RaceX")]
    [PluginName("RaceX Dashboard Manager")]
    public class DashboardManagerPlugin : BaseViewModel, IPlugin, IDataPlugin, IWPFSettings
    {
        #region Fields
        private DashboardManagerView _View;
        private DashboardManagerPluginSettings _Settings;
        private static object _syncLock = new object();
        private string _CurrentGame;
        private string _CurrentCar;
        private string _CurrentVOCOREDashboard;
        private string _CurrentVOCORE2Dashboard;
        private string _CurrentVOCORE3Dashboard;
        private string _CurrentVOCORE4Dashboard;
        private string _CurrentUSBD480Dashboard;
        private string _CurrentAX206Dashboard;
        private string _CurrentNexusCorsairDashboard;
        #endregion

        #region Properties
        public PluginManager PluginManager { get; set; }
        public DashboardManagerPluginSettings Settings
        {
            get { return _Settings; }
            set { _Settings = value; OnPropertyChanged("Settings"); }
        }
        public string CurrentGame
        {
            get { return _CurrentGame; }
            set { _CurrentGame = value; OnPropertyChanged("CurrentGame"); }
        }
        public string CurrentCar
        {
            get { return _CurrentCar; }
            set { _CurrentCar = value; OnPropertyChanged("CurrentCar"); }
        }
        public string CurrentVOCOREDashboard
        {
            get { return _CurrentVOCOREDashboard; }
            set { _CurrentVOCOREDashboard = value; OnPropertyChanged("CurrentVOCOREDashboard"); }
        }
        public string CurrentVOCORE2Dashboard
        {
            get { return _CurrentVOCORE2Dashboard; }
            set { _CurrentVOCORE2Dashboard = value; OnPropertyChanged("CurrentVOCORE2Dashboard"); }
        }
        public string CurrentVOCORE3Dashboard
        {
            get { return _CurrentVOCORE3Dashboard; }
            set { _CurrentVOCORE3Dashboard = value; OnPropertyChanged("CurrentVOCORE3Dashboard"); }
        }
        public string CurrentVOCORE4Dashboard
        {
            get { return _CurrentVOCORE4Dashboard; }
            set { _CurrentVOCORE4Dashboard = value; OnPropertyChanged("CurrentVOCORE4Dashboard"); }
        }
        public string CurrentUSBD480Dashboard
        {
            get { return _CurrentUSBD480Dashboard; }
            set { _CurrentUSBD480Dashboard = value; OnPropertyChanged("CurrentUSBD480Dashboard"); }
        }
        public string CurrentAX206Dashboard
        {
            get { return _CurrentAX206Dashboard; }
            set { _CurrentAX206Dashboard = value; OnPropertyChanged("CurrentAX206Dashboard"); }
        }
        public string CurrentNexusCorsairDashboard
        {
            get { return _CurrentNexusCorsairDashboard; }
            set { _CurrentNexusCorsairDashboard = value; OnPropertyChanged("CurrentNexusCorsairDashboard"); }
        }
        public ObservableCollection<CarDashboard> VocoreCarDashboards
        {
            get
            {
                return Settings is null ? null : new ObservableCollection<CarDashboard>( new ObservableCollection<CarDashboard>( Settings.CarDashboards.Where(x => x.DeviceType == "Vocore")).OrderBy(x=>x.Game).ThenBy(x=>x.Car));
            }
        }
        public ObservableCollection<CarDashboard> Vocore2CarDashboards
        {
            get
            {
                return Settings is null ? null : new ObservableCollection<CarDashboard>(new ObservableCollection<CarDashboard>(Settings.CarDashboards.Where(x => x.DeviceType == "Vocore2")).OrderBy(x => x.Game).ThenBy(x => x.Car));
            }
        }
        public ObservableCollection<CarDashboard> Vocore3CarDashboards
        {
            get
            {
                return Settings is null ? null : new ObservableCollection<CarDashboard>(new ObservableCollection<CarDashboard>(Settings.CarDashboards.Where(x => x.DeviceType == "Vocore3")).OrderBy(x => x.Game).ThenBy(x => x.Car));
            }
        }
        public ObservableCollection<CarDashboard> Vocore4CarDashboards
        {
            get
            {
                return Settings is null ? null : new ObservableCollection<CarDashboard>(new ObservableCollection<CarDashboard>(Settings.CarDashboards.Where(x => x.DeviceType == "Vocore4")).OrderBy(x => x.Game).ThenBy(x => x.Car));
            }
        }
        public ObservableCollection<CarDashboard> USBD480CarDashboards
        {
            get
            {
                return Settings is null ? null : new ObservableCollection<CarDashboard>(new ObservableCollection<CarDashboard>(Settings.CarDashboards.Where(x => x.DeviceType == "USBD480")).OrderBy(x => x.Game).ThenBy(x => x.Car));
            }
        }
        public ObservableCollection<CarDashboard> AX206CarDashboards
        {
            get
            {
                return Settings is null ? null : new ObservableCollection<CarDashboard>(new ObservableCollection<CarDashboard>(Settings.CarDashboards.Where(x => x.DeviceType == "AX206")).OrderBy(x => x.Game).ThenBy(x => x.Car));
            }
        }
        public ObservableCollection<CarDashboard> CorsairNexusCarDashboards
        {
            get
            {
                return Settings is null ? null : new ObservableCollection<CarDashboard>(new ObservableCollection<CarDashboard>(Settings.CarDashboards.Where(x => x.DeviceType == "CorsairNexus")).OrderBy(x => x.Game).ThenBy(x => x.Car));
            }
        }
        #endregion


        #region Commands

        #region SetDefaultDashboardCommand
        private DelegateCommand<string> _SetDefaultDashboardCommand;
        public ICommand SetDefaultDashboardCommand
        {
            get
            {
                if (_SetDefaultDashboardCommand == null)
                    _SetDefaultDashboardCommand = new DelegateCommand<string>(new System.Action<string>(SetDefaultDashboardExecuted), new Func<string,bool>(SetDefaultDashboardCanExecute));
                return _SetDefaultDashboardCommand;
            }

        }
        public bool SetDefaultDashboardCanExecute(string deviceType)
        {
            if (_CurrentGame is null)
                return false;
            if (deviceType == "VOCORE")
                return _CurrentVOCOREDashboard != null;
            if (deviceType == "VOCORE2")
                return _CurrentVOCORE2Dashboard != null;
            if (deviceType == "VOCORE3")
                return _CurrentVOCORE3Dashboard != null;
            if (deviceType == "VOCORE4")
                return _CurrentVOCORE4Dashboard != null;
            if (deviceType == "USBD480")
                return _CurrentVOCOREDashboard != null;
            if (deviceType == "AX206")
                return _CurrentVOCOREDashboard != null;
            if (deviceType == "CorsairNexus")
                return _CurrentVOCOREDashboard != null;
            return false;
        }
        public void SetDefaultDashboardExecuted(string deviceType)
        {            
            if (deviceType == "VOCORE")
            {
                CarDashboard VocoreDefaultGameDashboard = _Settings.CarDashboards.FirstOrDefault(x => x.Game == _CurrentGame && x.Car == "default" && x.DeviceType == "Vocore");
                if (VocoreDefaultGameDashboard is null)
                {
                    VocoreDefaultGameDashboard = new CarDashboard { Game = _CurrentGame, Car = "default", DeviceType = "Vocore", IsDefaultDashboard = true };
                    lock (_syncLock)
                    {
                        Settings.CarDashboards.Add(VocoreDefaultGameDashboard);
                        OnPropertyChanged("VocoreCarDashboards");
                    }
                }
                VocoreDefaultGameDashboard.DashboardName = _CurrentVOCOREDashboard;
            }
            if (deviceType == "VOCORE2")
            {
                CarDashboard Vocore2DefaultGameDashboard = _Settings.CarDashboards.FirstOrDefault(x => x.Game == _CurrentGame && x.Car == "default" && x.DeviceType == "Vocore2");
                if (Vocore2DefaultGameDashboard is null)
                {
                    Vocore2DefaultGameDashboard = new CarDashboard { Game = _CurrentGame, Car = "default", DeviceType = "Vocore2", IsDefaultDashboard = true };
                    lock (_syncLock)
                    {
                        Settings.CarDashboards.Add(Vocore2DefaultGameDashboard);
                        OnPropertyChanged("Vocore2CarDashboards");
                    }
                }
                Vocore2DefaultGameDashboard.DashboardName = _CurrentVOCORE2Dashboard;
            }
            if (deviceType == "VOCORE3")
            {
                CarDashboard Vocore3DefaultGameDashboard = _Settings.CarDashboards.FirstOrDefault(x => x.Game == _CurrentGame && x.Car == "default" && x.DeviceType == "Vocore3");
                if (Vocore3DefaultGameDashboard is null)
                {
                    Vocore3DefaultGameDashboard = new CarDashboard { Game = _CurrentGame, Car = "default", DeviceType = "Vocore3", IsDefaultDashboard = true };
                    lock (_syncLock)
                    {
                        Settings.CarDashboards.Add(Vocore3DefaultGameDashboard);
                        OnPropertyChanged("Vocore3CarDashboards");
                    }
                }
                Vocore3DefaultGameDashboard.DashboardName = _CurrentVOCORE3Dashboard;
            }
            if (deviceType == "VOCORE4")
            {
                CarDashboard Vocore4DefaultGameDashboard = _Settings.CarDashboards.FirstOrDefault(x => x.Game == _CurrentGame && x.Car == "default" && x.DeviceType == "Vocore4");
                if (Vocore4DefaultGameDashboard is null)
                {
                    Vocore4DefaultGameDashboard = new CarDashboard { Game = _CurrentGame, Car = "default", DeviceType = "Vocore4", IsDefaultDashboard = true };
                    lock (_syncLock)
                    {
                        Settings.CarDashboards.Add(Vocore4DefaultGameDashboard);
                        OnPropertyChanged("Vocore4CarDashboards");
                    }
                }
                Vocore4DefaultGameDashboard.DashboardName = _CurrentVOCORE4Dashboard;
            }
            else if (deviceType == "USBD480")
            {
                CarDashboard DefaultGameDashboard = _Settings.CarDashboards.FirstOrDefault(x => x.Game == _CurrentGame && x.Car == "default" && x.DeviceType == "USBD480");
                if (DefaultGameDashboard is null)
                {
                    DefaultGameDashboard = new CarDashboard { Game = _CurrentGame, Car = "default", DeviceType = "USBD480", IsDefaultDashboard = true };
                    lock (_syncLock)
                    {
                        Settings.CarDashboards.Add(DefaultGameDashboard);
                        OnPropertyChanged("USBD480CarDashboards");
                    }
                }
                DefaultGameDashboard.DashboardName = _CurrentUSBD480Dashboard;
            }
            else if (deviceType == "AX206")
            {
                CarDashboard DefaultGameDashboard = _Settings.CarDashboards.FirstOrDefault(x => x.Game == _CurrentGame && x.Car == "default" && x.DeviceType == "AX206");
                if (DefaultGameDashboard is null)
                {
                    DefaultGameDashboard = new CarDashboard { Game = _CurrentGame, Car = "default", DeviceType = "AX206", IsDefaultDashboard = true };
                    lock (_syncLock)
                    {
                        Settings.CarDashboards.Add(DefaultGameDashboard);
                        OnPropertyChanged("AX206CarDashboards");
                    }
                }
                DefaultGameDashboard.DashboardName = _CurrentAX206Dashboard;
            }
            else if (deviceType == "CorsairNexus")
            {
                CarDashboard DefaultGameDashboard = _Settings.CarDashboards.FirstOrDefault(x => x.Game == _CurrentGame && x.Car == "default" && x.DeviceType == "CorsairNexus");
                if (DefaultGameDashboard is null)
                {
                    DefaultGameDashboard = new CarDashboard { Game = _CurrentGame, Car = "default", DeviceType = "CorsairNexus", IsDefaultDashboard = true };
                    lock (_syncLock)
                    {
                        Settings.CarDashboards.Add(DefaultGameDashboard);
                        OnPropertyChanged("CorsairNexusCarDashboards");
                    }
                }
                DefaultGameDashboard.DashboardName = _CurrentNexusCorsairDashboard;
            }          
           
        }
        #endregion

        #region DeleteCarDashboardCommand
        private DelegateCommand<CarDashboard> _DeleteCarDashboardCommand;
        public ICommand DeleteCarDashboardCommand
        {
            get
            {
                if (_DeleteCarDashboardCommand == null)
                    _DeleteCarDashboardCommand = new DelegateCommand<CarDashboard>(new System.Action<CarDashboard>(DeleteCarDashboardExecuted), new Func<CarDashboard, bool>(DeleteCarDashboardCanExecute));
                return _DeleteCarDashboardCommand;
            }

        }
        public bool DeleteCarDashboardCanExecute(CarDashboard carDashboard)
        {
            return carDashboard != null;
        }
        public void DeleteCarDashboardExecuted(CarDashboard carDashboard)
        {
            Settings.CarDashboards.Remove(carDashboard);
            OnPropertyChanged("VocoreCarDashboards");
            OnPropertyChanged("Vocore2CarDashboards");
            OnPropertyChanged("Vocore3CarDashboards");
            OnPropertyChanged("Vocore4CarDashboards");
            OnPropertyChanged("USBD480CarDashboards");
            OnPropertyChanged("AX206CarDashboards");
            OnPropertyChanged("CorsairNexusCarDashboards");
 
        }
        #endregion

        #endregion

        #region Methods
        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning)
            {
                CurrentGame = null;
                CurrentCar = null;
                CurrentVOCOREDashboard = null;
                CurrentVOCORE2Dashboard = null;
                CurrentVOCORE3Dashboard = null;
                CurrentVOCORE4Dashboard = null;
                CurrentUSBD480Dashboard = null;
                CurrentAX206Dashboard = null;
                CurrentNexusCorsairDashboard = null;
                return;
            }
                

            if (data.GameName != null && data.NewData.CarModel != null)
            {
                string gameName = data.GameName;
                string carModel = data.NewData.CarModel;

                var dashPlugin = pluginManager.GetPlugin<SimHub.Plugins.OutputPlugins.GraphicalDash.GraphicalDashPlugin>();

                if (Settings.IsVocorePluginEnabled && dashPlugin.Settings.VoCoreSettings.Enabled)
                {
                    CarDashboard VocorecarDashboard = _Settings.CarDashboards.FirstOrDefault(x => x.Game == gameName && x.Car == carModel && x.DeviceType== "Vocore");
                    if (VocorecarDashboard is null)
                    {
                        VocorecarDashboard = _Settings.CarDashboards.FirstOrDefault(x => x.Game == gameName && x.Car == "default" && x.DeviceType == "Vocore");
                        if (VocorecarDashboard is null)
                        {
                            VocorecarDashboard = new CarDashboard { Game = gameName, Car = carModel, DeviceType = "Vocore" };
                            lock (_syncLock)
                            {
                                Settings.CarDashboards.Add(VocorecarDashboard);
                                OnPropertyChanged("VocoreCarDashboards");
                            }
                        }                       
                    }
                    //If a game/car change has occured
                    if (gameName != CurrentGame || carModel != CurrentCar)
                    {
                        //if dashboard is already assigned
                        if (VocorecarDashboard.DashboardName != null)
                        {
                            //Check current dashboard is car dashboard
                            if (VocorecarDashboard.DashboardName != dashPlugin.Settings.VoCoreSettings.CurrentDashboard)
                                dashPlugin.Settings.VoCoreSettings.CurrentDashboard = VocorecarDashboard.DashboardName;                           
                        }
                        else
                        {
                            VocorecarDashboard.DashboardName = dashPlugin.Settings.VoCoreSettings.CurrentDashboard;
                            OnPropertyChanged("VocoreCarDashboards"); 
                        }

                    }
                    else
                    {
                        if (VocorecarDashboard.DashboardName != dashPlugin.Settings.VoCoreSettings.CurrentDashboard)
                        {
                            //if current dashboard is default dash
                            if (VocorecarDashboard.IsDefaultDashboard)
                            {
                                VocorecarDashboard = new CarDashboard { Game = gameName, Car = carModel, DeviceType = "Vocore" };
                                lock (_syncLock)
                                {
                                    Settings.CarDashboards.Add(VocorecarDashboard);
                                    OnPropertyChanged("VocoreCarDashboards");
                                }
                            }
                            VocorecarDashboard.DashboardName = dashPlugin.Settings.VoCoreSettings.CurrentDashboard;
                            OnPropertyChanged("VocoreCarDashboards");                           
                        }
                    }
                    CurrentVOCOREDashboard = dashPlugin.Settings.VoCoreSettings.CurrentDashboard;
                }
                if (Settings.IsVocore2PluginEnabled && dashPlugin.Settings.VoCoreSettings_2.Enabled)
                {
                    CarDashboard Vocore2carDashboard = _Settings.CarDashboards.FirstOrDefault(x => x.Game == gameName && x.Car == carModel && x.DeviceType == "Vocore2");
                    if (Vocore2carDashboard is null)
                    {
                        Vocore2carDashboard = _Settings.CarDashboards.FirstOrDefault(x => x.Game == gameName && x.Car == "default" && x.DeviceType == "Vocore2");
                        if (Vocore2carDashboard is null)
                        {
                            Vocore2carDashboard = new CarDashboard { Game = gameName, Car = carModel, DeviceType = "Vocore2" };
                            lock (_syncLock)
                            {
                                Settings.CarDashboards.Add(Vocore2carDashboard);
                                OnPropertyChanged("Vocore2CarDashboards");
                            }
                        }
                    }
                    //If a game/car change has occured
                    if (gameName != CurrentGame || carModel != CurrentCar)
                    {
                        //if dashboard is already assigned
                        if (Vocore2carDashboard.DashboardName != null)
                        {
                            //Check current dashboard is car dashboard
                            if (Vocore2carDashboard.DashboardName != dashPlugin.Settings.VoCoreSettings_2.CurrentDashboard)
                                dashPlugin.Settings.VoCoreSettings_2.CurrentDashboard = Vocore2carDashboard.DashboardName;
                        }
                        else
                        {
                            Vocore2carDashboard.DashboardName = dashPlugin.Settings.VoCoreSettings_2.CurrentDashboard;
                            OnPropertyChanged("Vocore2CarDashboards");
                        }

                    }
                    else
                    {
                        if (Vocore2carDashboard.DashboardName != dashPlugin.Settings.VoCoreSettings_2.CurrentDashboard)
                        {
                            //if current dashboard is default dash
                            if (Vocore2carDashboard.IsDefaultDashboard)
                            {
                                Vocore2carDashboard = new CarDashboard { Game = gameName, Car = carModel, DeviceType = "Vocore" };
                                lock (_syncLock)
                                {
                                    Settings.CarDashboards.Add(Vocore2carDashboard);
                                    OnPropertyChanged("Vocore2CarDashboards");
                                }
                            }
                            Vocore2carDashboard.DashboardName = dashPlugin.Settings.VoCoreSettings_2.CurrentDashboard;
                            OnPropertyChanged("Vocore2CarDashboards");
                        }
                    }
                    CurrentVOCORE2Dashboard = dashPlugin.Settings.VoCoreSettings_2.CurrentDashboard;
                }
                if (Settings.IsVocore3PluginEnabled && dashPlugin.Settings.VoCoreSettings_3.Enabled)
                {
                    CarDashboard Vocore3carDashboard = _Settings.CarDashboards.FirstOrDefault(x => x.Game == gameName && x.Car == carModel && x.DeviceType == "Vocore3");
                    if (Vocore3carDashboard is null)
                    {
                        Vocore3carDashboard = _Settings.CarDashboards.FirstOrDefault(x => x.Game == gameName && x.Car == "default" && x.DeviceType == "Vocore3");
                        if (Vocore3carDashboard is null)
                        {
                            Vocore3carDashboard = new CarDashboard { Game = gameName, Car = carModel, DeviceType = "Vocore3" };
                            lock (_syncLock)
                            {
                                Settings.CarDashboards.Add(Vocore3carDashboard);
                                OnPropertyChanged("Vocore3CarDashboards");
                            }
                        }
                    }
                    //If a game/car change has occured
                    if (gameName != CurrentGame || carModel != CurrentCar)
                    {
                        //if dashboard is already assigned
                        if (Vocore3carDashboard.DashboardName != null)
                        {
                            //Check current dashboard is car dashboard
                            if (Vocore3carDashboard.DashboardName != dashPlugin.Settings.VoCoreSettings_3.CurrentDashboard)
                                dashPlugin.Settings.VoCoreSettings_3.CurrentDashboard = Vocore3carDashboard.DashboardName;
                        }
                        else
                        {
                            Vocore3carDashboard.DashboardName = dashPlugin.Settings.VoCoreSettings_3.CurrentDashboard;
                            OnPropertyChanged("Vocore3CarDashboards");
                        }

                    }
                    else
                    {
                        if (Vocore3carDashboard.DashboardName != dashPlugin.Settings.VoCoreSettings_3.CurrentDashboard)
                        {
                            //if current dashboard is default dash
                            if (Vocore3carDashboard.IsDefaultDashboard)
                            {
                                Vocore3carDashboard = new CarDashboard { Game = gameName, Car = carModel, DeviceType = "Vocore3" };
                                lock (_syncLock)
                                {
                                    Settings.CarDashboards.Add(Vocore3carDashboard);
                                    OnPropertyChanged("Vocore3CarDashboards");
                                }
                            }
                            Vocore3carDashboard.DashboardName = dashPlugin.Settings.VoCoreSettings_3.CurrentDashboard;
                            OnPropertyChanged("Vocore3CarDashboards");
                        }
                    }
                    CurrentVOCORE3Dashboard = dashPlugin.Settings.VoCoreSettings_3.CurrentDashboard;
                }
                if (Settings.IsVocore4PluginEnabled && dashPlugin.Settings.VoCoreSettings_4.Enabled)
                {
                    CarDashboard Vocore4carDashboard = _Settings.CarDashboards.FirstOrDefault(x => x.Game == gameName && x.Car == carModel && x.DeviceType == "Vocore4");
                    if (Vocore4carDashboard is null)
                    {
                        Vocore4carDashboard = _Settings.CarDashboards.FirstOrDefault(x => x.Game == gameName && x.Car == "default" && x.DeviceType == "Vocore4");
                        if (Vocore4carDashboard is null)
                        {
                            Vocore4carDashboard = new CarDashboard { Game = gameName, Car = carModel, DeviceType = "Vocore4" };
                            lock (_syncLock)
                            {
                                Settings.CarDashboards.Add(Vocore4carDashboard);
                                OnPropertyChanged("Vocore4CarDashboards");
                            }
                        }
                    }
                    //If a game/car change has occured
                    if (gameName != CurrentGame || carModel != CurrentCar)
                    {
                        //if dashboard is already assigned
                        if (Vocore4carDashboard.DashboardName != null)
                        {
                            //Check current dashboard is car dashboard
                            if (Vocore4carDashboard.DashboardName != dashPlugin.Settings.VoCoreSettings_4.CurrentDashboard)
                                dashPlugin.Settings.VoCoreSettings_4.CurrentDashboard = Vocore4carDashboard.DashboardName;
                        }
                        else
                        {
                            Vocore4carDashboard.DashboardName = dashPlugin.Settings.VoCoreSettings_4.CurrentDashboard;
                            OnPropertyChanged("Vocore4CarDashboards");
                        }

                    }
                    else
                    {
                        if (Vocore4carDashboard.DashboardName != dashPlugin.Settings.VoCoreSettings_4.CurrentDashboard)
                        {
                            //if current dashboard is default dash
                            if (Vocore4carDashboard.IsDefaultDashboard)
                            {
                                Vocore4carDashboard = new CarDashboard { Game = gameName, Car = carModel, DeviceType = "Vocore4" };
                                lock (_syncLock)
                                {
                                    Settings.CarDashboards.Add(Vocore4carDashboard);
                                    OnPropertyChanged("Vocore4CarDashboards");
                                }
                            }
                            Vocore4carDashboard.DashboardName = dashPlugin.Settings.VoCoreSettings_4.CurrentDashboard;
                            OnPropertyChanged("Vocore4CarDashboards");
                        }
                    }
                    CurrentVOCORE4Dashboard = dashPlugin.Settings.VoCoreSettings_4.CurrentDashboard;
                }
                if (Settings.IsUSBD480PluginEnabled && dashPlugin.Settings.USBD480Settings.Enabled)
                {
                    CarDashboard USBD480carDashboard = _Settings.CarDashboards.FirstOrDefault(x => x.Game == gameName && x.Car == carModel && x.DeviceType == "USBD480");
                    if (USBD480carDashboard is null)
                    {
                        USBD480carDashboard = _Settings.CarDashboards.FirstOrDefault(x => x.Game == gameName && x.Car == "default" && x.DeviceType == "USBD480");
                        if (USBD480carDashboard is null)
                        {
                            USBD480carDashboard = new CarDashboard { Game = gameName, Car = carModel, DeviceType = "USBD480" };
                            lock (_syncLock)
                            {
                                Settings.CarDashboards.Add(USBD480carDashboard);
                                OnPropertyChanged("USBD480CarDashboards");
                            }
                        }

                          
                    }
                    if (gameName != CurrentGame || carModel != CurrentCar)
                    {
                        if (USBD480carDashboard.DashboardName != null)
                        {
                            if (USBD480carDashboard.DashboardName != dashPlugin.Settings.USBD480Settings.CurrentDashboard)
                                dashPlugin.Settings.USBD480Settings.CurrentDashboard = USBD480carDashboard.DashboardName;                           
                        }
                        else
                        {                           
                            USBD480carDashboard.DashboardName = dashPlugin.Settings.USBD480Settings.CurrentDashboard;
                            OnPropertyChanged("USBD480CarDashboards");
                        }

                    }
                    else
                    {
                        if (USBD480carDashboard.DashboardName != dashPlugin.Settings.USBD480Settings.CurrentDashboard)
                        {
                            //if current dashboard is default dash
                            if (USBD480carDashboard.IsDefaultDashboard)
                            {
                                USBD480carDashboard = new CarDashboard { Game = gameName, Car = carModel, DeviceType = "USBD480" };
                                lock (_syncLock)
                                {
                                    Settings.CarDashboards.Add(USBD480carDashboard);
                                    OnPropertyChanged("VocoreCarDashboards");
                                }
                            }
                            USBD480carDashboard.DashboardName = dashPlugin.Settings.USBD480Settings.CurrentDashboard;
                            OnPropertyChanged("USBD480CarDashboards");
                        }                       
                    }
                    CurrentUSBD480Dashboard = dashPlugin.Settings.USBD480Settings.CurrentDashboard;
                }
                if (Settings.IsAX206PluginEnabled && dashPlugin.Settings.AX206Settings.Enabled)
                {
                    CarDashboard AX206carDashboard = _Settings.CarDashboards.FirstOrDefault(x => x.Game == gameName && x.Car == carModel && x.DeviceType == "AX206");
                    if (AX206carDashboard is null)
                    {
                        AX206carDashboard = _Settings.CarDashboards.FirstOrDefault(x => x.Game == gameName && x.Car == "default" && x.DeviceType == "AX206");
                        if (AX206carDashboard is null)
                        {
                            AX206carDashboard = new CarDashboard { Game = gameName, Car = carModel, DeviceType = "AX206" };
                            lock (_syncLock)
                            {
                                Settings.CarDashboards.Add(AX206carDashboard);
                                OnPropertyChanged("AX206CarDashboards");
                            }
                        }                        
                    }
                    if (gameName != CurrentGame || carModel != CurrentCar)
                    {
                        if (AX206carDashboard.DashboardName != null)
                        {
                            if (AX206carDashboard.DashboardName != dashPlugin.Settings.AX206Settings.CurrentDashboard)
                                dashPlugin.Settings.AX206Settings.CurrentDashboard = AX206carDashboard.DashboardName;
                        }
                        else
                        {
                            AX206carDashboard.DashboardName = dashPlugin.Settings.AX206Settings.CurrentDashboard;
                            OnPropertyChanged("AX206CarDashboards");
                        }

                    }
                    else
                    {
                        if (AX206carDashboard.DashboardName != dashPlugin.Settings.AX206Settings.CurrentDashboard)
                        {
                            //if current dashboard is default dash
                            if (AX206carDashboard.IsDefaultDashboard)
                            {
                                AX206carDashboard = new CarDashboard { Game = gameName, Car = carModel, DeviceType = "AX206" };
                                lock (_syncLock)
                                {
                                    Settings.CarDashboards.Add(AX206carDashboard);
                                    OnPropertyChanged("AX206CarDashboards");
                                }
                            }
                            AX206carDashboard.DashboardName = dashPlugin.Settings.AX206Settings.CurrentDashboard;
                            OnPropertyChanged("AX206CarDashboards");
                        }                        
                    }
                    CurrentAX206Dashboard = dashPlugin.Settings.AX206Settings.CurrentDashboard;
                }
                if (Settings.IsCorsairNexuPluginEnabled && dashPlugin.Settings.CorsairNexusSettings.Enabled)
                {
                    CarDashboard CorsairNexuscarDashboard = _Settings.CarDashboards.FirstOrDefault(x => x.Game == gameName && x.Car == carModel && x.DeviceType == "CorsairNexus");
                    if (CorsairNexuscarDashboard is null)
                    {
                        CorsairNexuscarDashboard = _Settings.CarDashboards.FirstOrDefault(x => x.Game == gameName && x.Car == "default" && x.DeviceType == "CorsairNexus");
                        if (CorsairNexuscarDashboard is null)
                        {
                            CorsairNexuscarDashboard = new CarDashboard { Game = gameName, Car = carModel, DeviceType = "CorsairNexus" };
                            lock (_syncLock)
                            {
                                Settings.CarDashboards.Add(CorsairNexuscarDashboard);
                                OnPropertyChanged("CorsairNexusCarDashboards");
                            }
                        }
                         
                    }
                    if (gameName != CurrentGame || carModel != CurrentCar)
                    {
                        if (CorsairNexuscarDashboard.DashboardName != null)
                        {
                            if (CorsairNexuscarDashboard.DashboardName != dashPlugin.Settings.CorsairNexusSettings.CurrentDashboard)
                                dashPlugin.Settings.CorsairNexusSettings.CurrentDashboard = CorsairNexuscarDashboard.DashboardName;
                        }
                        else
                        {
                            CorsairNexuscarDashboard.DashboardName = dashPlugin.Settings.CorsairNexusSettings.CurrentDashboard;
                            OnPropertyChanged("CorsairNexusCarDashboards");
                        }

                    }
                    else
                    {
                        if (CorsairNexuscarDashboard.DashboardName != dashPlugin.Settings.CorsairNexusSettings.CurrentDashboard)
                        {
                            //if current dashboard is default dash
                            if (CorsairNexuscarDashboard.IsDefaultDashboard)
                            {
                                CorsairNexuscarDashboard = new CarDashboard { Game = gameName, Car = carModel, DeviceType = "CorsairNexus" };
                                lock (_syncLock)
                                {
                                    Settings.CarDashboards.Add(CorsairNexuscarDashboard);
                                    OnPropertyChanged("CorsairNexusCarDashboards");
                                }
                            }
                            CorsairNexuscarDashboard.DashboardName = dashPlugin.Settings.CorsairNexusSettings.CurrentDashboard;
                            OnPropertyChanged("CorsairNexusCarDashboards");
                        }

                    }
                    CurrentNexusCorsairDashboard = dashPlugin.Settings.CorsairNexusSettings.CurrentDashboard;
                }
            }
            CurrentGame = data.GameName;
            CurrentCar = data.NewData.CarModel;
        }
        public void End(PluginManager pluginManager)
        {
            this.SaveCommonSettings("GeneralSettings", Settings);
        }
        public Control GetWPFSettingsControl(PluginManager pluginManager)
        {
            _View = new DashboardManagerView(this);
            return _View;
        }
        public void Init(PluginManager pluginManager)
        {
            PluginManager = pluginManager;

            SimHub.Logging.Current.Info("Starting plugin RaceX.Dashboard.Manager");
            // Load settings
            Settings = this.ReadCommonSettings<DashboardManagerPluginSettings>("GeneralSettings", () => new DashboardManagerPluginSettings());
            Settings.PropertyChanged += Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsVocorePluginEnabled" && !(sender as DashboardManagerPluginSettings).IsVocorePluginEnabled)
                RemoveDeviceEntries("Vocore");
            if (e.PropertyName == "IsVocore2PluginEnabled" && !(sender as DashboardManagerPluginSettings).IsVocore2PluginEnabled)
                RemoveDeviceEntries("Vocore2");
            if (e.PropertyName == "IsVocore3PluginEnabled" && !(sender as DashboardManagerPluginSettings).IsVocore3PluginEnabled)
                RemoveDeviceEntries("Vocore3");
            if (e.PropertyName == "IsVocore4PluginEnabled" && !(sender as DashboardManagerPluginSettings).IsVocore4PluginEnabled)
                RemoveDeviceEntries("Vocore4");
            if (e.PropertyName == "IsUSBD480PluginEnabled" && !(sender as DashboardManagerPluginSettings).IsUSBD480PluginEnabled)
                RemoveDeviceEntries("USBD480");
            if (e.PropertyName == "IsAX206PluginEnabled" && !(sender as DashboardManagerPluginSettings).IsAX206PluginEnabled)
                RemoveDeviceEntries("AX206");
            if (e.PropertyName == "IsCorsairNexuPluginEnabled" && !(sender as DashboardManagerPluginSettings).IsCorsairNexuPluginEnabled)
                RemoveDeviceEntries("CorsairNexus");
        }

        public void RemoveDeviceEntries(string deviceType)
        {
            foreach (var x in Settings.CarDashboards.ToList())
            {
                if (x.DeviceType == deviceType)
                {
                    Settings.CarDashboards.Remove(x);
                }
            }
            UpdateUI();
        }
        void UpdateUI()
        {
            OnPropertyChanged("VocoreCarDashboards");
            OnPropertyChanged("Vocore2CarDashboards");
            OnPropertyChanged("Vocore3CarDashboards");
            OnPropertyChanged("Vocore4CarDashboards");
            OnPropertyChanged("USBD480CarDashboards");
            OnPropertyChanged("AX206CarDashboards");
            OnPropertyChanged("CorsairNexusCarDashboards");
        }
        #endregion


    }
}
