using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using PoeCrafting.Entities;
using PoeCrafting.Infrastructure;

namespace WorkspacesModule.Condition
{
    /// <summary>
    /// Interaction logic for ConditionControl.xaml
    /// </summary>
    public partial class SubconditionControlView : UserControl, IView
    {
        public SubconditionControlView(SubconditionControlViewModel viewModel)
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
