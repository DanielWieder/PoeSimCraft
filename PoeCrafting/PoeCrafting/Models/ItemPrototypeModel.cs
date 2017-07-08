using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Domain.Condition;
using PoeCrafting.UI.Annotations;

namespace PoeCrafting.UI.Models
{
    public class ItemPrototypeModel : INotifyPropertyChanged
    {
        private string _itemName;

        public string ItemName
        {
            get { return _itemName; }
            set
            {
                _itemName = value;
                OnPropertyChanged(nameof(ItemName));
            }
        }

        private int _value;

        public int Value
        {
            get { return _value; }

            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public int Index { get; set; }
        public CraftingCondition Condition { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
