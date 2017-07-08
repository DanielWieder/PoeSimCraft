using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using PoeCrafting.Entities;
using PoeCrafting.UI.Annotations;
using PoeCrafting.UI.Controls;
using PoeCrafting.UI.Models;

namespace PoeCrafting.UI
{
    /// <summary>
    /// Interaction logic for ItemControl.xaml
    /// </summary>
    public partial class ItemControl : UserControl, INotifyPropertyChanged
    {
        public ItemPrototypeModel Model { get; set; }

        public ConditionControl Condition { get; set; }

        public ItemControl(ItemPrototypeModel model, List<Affix> affixes, ItemBase itemBase)
        {
            Model = model;
            Condition = new ConditionControl(model.Condition, itemBase, affixes);
            OnPropertyChanged(nameof(Condition));
            DataContext = this;
            InitializeComponent();
        }

        public void Save()
        {
            Condition.Save();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
