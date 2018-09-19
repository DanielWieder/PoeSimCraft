using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
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
