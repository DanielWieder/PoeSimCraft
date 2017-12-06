using System.Windows.Controls;
using PoeCrafting.Infrastructure;

namespace WorkspacesModule.ItemBase
{
    /// <summary>
    /// Interaction logic for ItemBaseView.xaml
    /// </summary>
    public partial class ItemBaseView : UserControl, IView
    {
        public ItemBaseView(ItemBaseViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }

        public IViewModel ViewModel { get
            {
                return (IViewModel)DataContext;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
