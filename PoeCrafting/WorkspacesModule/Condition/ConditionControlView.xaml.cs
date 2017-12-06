using System.Windows.Controls;
using PoeCrafting.Infrastructure;

namespace WorkspacesModule.Condition
{
    /// <summary>
    /// Interaction logic for ConditionControl.xaml
    /// </summary>
    public partial class ConditionControlView : UserControl, IView
    {
        public ConditionControlView(ConditionControlViewModel viewModel)
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
    }
}
