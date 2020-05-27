using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinerLogic.CommonPublic
{
    public class ChangeOptionsEventArgs
    {
        public GameType GameType { get; }
        public ChangeOptionsEventArgs(GameType gameType)
        {
            GameType = gameType;
        }
    }
}
