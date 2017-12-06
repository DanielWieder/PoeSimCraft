using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;

namespace PoeCrafting.Infrastructure
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// This ViewModelBase subclass requests to be removed 
    /// from the UI when its CloseCommand executes.
    /// This class is abstract.
    /// </summary>
    public abstract class WorkspaceViewModel : ViewModelBase
    {
        ICommand _closeCommand;

        /// <summary>
        /// Returns the command that, when invoked, attempts
        /// to remove this workspace from the user interface.
        /// </summary>
        public ICommand CloseCommand => _closeCommand ?? (_closeCommand = new DelegateCommand(OnRequestClose));

        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}