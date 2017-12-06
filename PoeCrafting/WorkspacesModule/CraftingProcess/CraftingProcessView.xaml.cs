using System.Windows.Controls;
using PoeCrafting.Infrastructure;

namespace WorkspacesModule.CraftingProcess
{
    /// <summary>
    /// Interaction logic for CraftingProcessView.xaml
    /// </summary>
    public partial class CraftingProcessView : UserControl, IView
    {
        public CraftingProcessView(CraftingProcessViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }

        public IViewModel ViewModel
        {
            get
            {
                return (IViewModel)DataContext;
            }
            set
            {
                DataContext = value;
            }
        }

        private void OnInsertOptionSelected(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
