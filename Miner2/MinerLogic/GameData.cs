using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinerLogic
{
    [Serializable]
    internal sealed class GameData // класс для сохранения/загрузки/начала игры
    {
        private GameField _gameField;
        private Options _options;
        private int _elapsedTime; //отображает затраченное время на форме
        private short _minesLeft; //отображает количество оставшихся мин на форме
        private GameState _gameState; // состояние игры

        public GameData(Options options, GameField gameField, int elapsedTime, short minesLeft, GameState gameState)
        {
            _options = options;
            _gameField = gameField;
            _elapsedTime = elapsedTime;
            _minesLeft = minesLeft;
            _gameState = gameState;
        }

        internal int ElapsedTime { get => _elapsedTime; set => _elapsedTime = value; }
        internal short MinesLeft { get => _minesLeft; set => _minesLeft = value; }
        internal GameField GameField { get => _gameField; set => _gameField = value; }
        internal Options Options { get => _options; set => _options = value; }
        internal GameState GameState { get => _gameState; set => _gameState = value; }
    }

}
