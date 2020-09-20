using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinerLogic.MinerPresenter
{
    public sealed class MouseClickEventArgs : EventArgs
    {
      /// <summary>
      /// class for incapsulation of coordinates mouse click relative to entire gamefield
      /// </summary>
      /// <param name="x">x-coord relatively to gamefield</param>
      /// <param name="y">x-coord relatively to gamefield</param>
      public MouseClickEventArgs(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

    }
}
