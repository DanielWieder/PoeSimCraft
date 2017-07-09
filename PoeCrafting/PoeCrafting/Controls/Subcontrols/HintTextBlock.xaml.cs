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

namespace PoeCrafting.UI.Controls.Subcontrols
{
    /*
     * TODO: Finish this class. It is currently nonfunctional
     * 
     */


    /// <summary>
    /// Interaction logic for HintTextBlock.xaml
    /// </summary>
    public partial class HintTextBlock : UserControl
    {
        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Identified the Text dependency property
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string),
              typeof(HintTextBlock), new PropertyMetadata(""));


        public Binding TextBinding
        {
            get { return (Binding)GetValue(TextBindingProperty); }
            set { SetValue(TextBindingProperty, value); }
        }

        /// <summary>
        /// Identified the Condition dependency property
        /// </summary>
        public static readonly DependencyProperty TextBindingProperty =
            DependencyProperty.Register("TextBinding", typeof(Binding),
              typeof(HintTextBlock), new PropertyMetadata(null));

        public Binding FocusBinding
        {
            get { return (Binding)GetValue(FocusBindingProperty); }
            set { SetValue(FocusBindingProperty, value); }
        }

        /// <summary>
        /// Identified the Condition dependency property
        /// </summary>
        public static readonly DependencyProperty FocusBindingProperty =
            DependencyProperty.Register("FocusBinding", typeof(Binding),
              typeof(HintTextBlock), new PropertyMetadata(null));


        public HintTextBlock()
        {
            //DataContext = this;
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }
    }
}
