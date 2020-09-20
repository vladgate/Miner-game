using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinerLogic
{
    [Serializable]
    internal sealed class GameData // class for save/load game
    {
        private GameField _gameField;
        private Options _options;
        private int _elapsedTime;
        private short _minesLeft;

        public GameData(Options options, GameField gameField, int elapsedTime, short minesLeft)
        {
            _options = options;
            _gameField = gameField;
            _elapsedTime = elapsedTime;
            _minesLeft = minesLeft;
        }

        internal int ElapsedTime { get => _elapsedTime; set => _elapsedTime = value; }
        internal short MinesLeft { get => _minesLeft; set => _minesLeft = value; }
        internal GameField GameField { get => _gameField; set => _gameField = value; }
        internal Options Options { get => _options; set => _options = value; }
    }

}
