using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using PoeCrafting.Domain.Condition;
using PoeCrafting.Entities;
using PoeCrafting.Infrastructure;

namespace WorkspacesModule.Condition
{
    /// <summary>
    /// Interaction logic for SubconditionAffixControl.xaml
    /// </summary>
    public partial class SubconditionAffixControlView : UserControl, IView
    {
        public SubconditionAffixControlView(SubconditionAffixControlViewModel viewModel)
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
