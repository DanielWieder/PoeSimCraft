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
using System.Windows.Shapes;
using PoeCrafting.Infrastructure;
namespace MainWindowModule.MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WorkspaceManagerView : UserControl, IView
    {
        public IViewModel ViewModel {
            get { return (IViewModel)DataContext; }
            set { DataContext = value; } }

        public WorkspaceManagerView(WorkspaceManagerViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();

            //EventHandler handler = null;
            //handler = delegate
            //{
            //    viewModel.RequestClose -= handler;
            //  //  this.Close();
            //};
            //viewModel.RequestClose += handler;
        }
    }
}
