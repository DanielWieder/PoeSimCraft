using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace PoeCrafting.UI
{
    /// <summary>
    /// Interaction logic for WindowPage.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {
        private TestbedWindow _testbedWindow;
        private SimulationWindow _simulationWindow;
        public HomeWindow(TestbedWindow testbedWindow, SimulationWindow simulationWindow)
        {
            this._testbedWindow = testbedWindow;
            this._simulationWindow = simulationWindow;

            InitializeComponent();
        }

        private void OnMainWindowClick(object sender, RoutedEventArgs e)
        {
            _testbedWindow.Show();
        }

        private void OnSimulationClick(object sender, RoutedEventArgs e)
        {
            _simulationWindow.Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}
