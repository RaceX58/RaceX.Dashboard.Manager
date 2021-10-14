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
                return new ObservableCollection<CarDashboard>( new ObservableCollection<CarDashboard>( Settings.CarDashboards.Where(x => x.DeviceType == "Vocore")).OrderBy(x=>x.Game).ThenBy(x=>x.Car));
            }
        }
        public ObservableCollection<CarDashboard> USBD480CarDashboards
        {
            get
            {
                return new ObservableCollection<CarDashboard>(new ObservableCollection<CarDashboard>(Settings.CarDashboards.Where(x => x.DeviceType == "USBD480")).OrderBy(x => x.Game).ThenBy(x => x.Car));
            }
        }
        public ObservableCollection<CarDashboard> AX206CarDashboards
        {
            get
            {
                return new ObservableCollection<CarDashboard>(new ObservableCollection<CarDashboard>(Settings.CarDashboards.Where(x => x.DeviceType == "AX206")).OrderBy(x => x.Game).ThenBy(x => x.Car));
            }
        }
        public ObservableCollection<CarDashboard> CorsairNexusCarDashboards
        {
            get
            {
                return new ObservableCollection<CarDashboard>(new ObservableCollection<CarDashboard>(Settings.CarDashboards.Where(x => x.DeviceType == "CorsairNexus")).OrderBy(x => x.Game).ThenBy(x => x.Car));
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
            OnPropertyChanged("USBD480CarDashboards");
            OnPropertyChanged("AX206CarDashboards");
            OnPropertyChanged("CorsairNexusCarDashboards");
            OnPropertyChanged("SecondMonitorCarDashboards");
            OnPropertyChanged("ThirdMonitorCarDashboards");
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
            OnPropertyChanged("USBD480CarDashboards");
            OnPropertyChanged("AX206CarDashboards");
            OnPropertyChanged("CorsairNexusCarDashboards");
        }
        #endregion


    }
}
