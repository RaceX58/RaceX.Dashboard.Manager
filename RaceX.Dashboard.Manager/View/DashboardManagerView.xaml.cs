using RaceX.Dashboard.Manager.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RaceX.Dashboard.Manager.View
{
    /// <summary>
    /// Logique d'interaction pour DashboardManagerView.xaml
    /// </summary>
    public partial class DashboardManagerView : UserControl
    {
        public DashboardManagerPlugin Plugin { get; }

        public DashboardManagerView()
        {
            InitializeComponent();
        }

        public DashboardManagerView(DashboardManagerPlugin plugin) : this()
        {
            this.Plugin = plugin;
            this.DataContext = Plugin;

        }
    }
}
