using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCrafting.UI.Controls
{
    public interface ISimulationControl
    {
        bool IsReady();
        void Save();
    }
}
