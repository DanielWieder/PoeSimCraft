using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Threading;
using PoeCrafting.Entities;
using PoeCrafting.Infrastructure;

namespace WorkspacesModule.Condition
{
    /// <summary>
    /// Interaction logic for ConditionAffixControl.xaml
    /// </summary>
    public partial class SubconditionAffixesControlView : UserControl, IView
    {
        public SubconditionAffixesControlView(SubconditionAffixesControlViewModel viewModel)
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
