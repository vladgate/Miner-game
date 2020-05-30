using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinerLogic.MinerLogic
{
    internal class ElapsedTimeChangedEventArgs:EventArgs
    {
        public int ElapsedTime { get; set; }
        public ElapsedTimeChangedEventArgs(int elapsedTime)
        {
            ElapsedTime = elapsedTime;
        }
    }
}
