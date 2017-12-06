using System;
using System.Windows.Input;

namespace PoeCrafting.Infrastructure
{
    public class CommandViewModel : ViewModelBase
    {
        private readonly Func<bool> _isEnabled;

        public CommandViewModel(
            string displayName, 
            ICommand command, 
            Func<bool> isEnabled)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            base.DisplayName = displayName;
            this.Command = command;
            this._isEnabled = isEnabled;
        }

        public void UpdateIsEnabled()
        {
            OnPropertyChanged(nameof(IsEnabled));
        }

        public ICommand Command { get; }

        public bool IsEnabled => _isEnabled();
    }
}
