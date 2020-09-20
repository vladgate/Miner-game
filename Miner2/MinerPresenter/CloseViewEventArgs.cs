using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinerLogic.MinerPresenter
{
    public sealed class ExitGameEventArgs : EventArgs
    {
        public bool NeedCloseView { get; set; } // need to close view when leaving game
        public ExitGameEventArgs(bool needCloseView)
        {
            NeedCloseView = needCloseView;
        }
    }
}
