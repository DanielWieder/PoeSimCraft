using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCrafting.Infrastructure
{
    public interface IWorkspaceFactory
    {
        List<CommandViewModel> Workspaces { get; }

        Action<WorkspaceViewModel> ActivateWorkspace { get; set; }

        void SubscribeConfigChanged(EventHandler configChanged);
    }
}
