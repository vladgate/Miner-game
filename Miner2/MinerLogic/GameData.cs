using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinerLogic
{
    [Serializable]
    internal sealed class GameData // класс для сохранения/загрузки игры
    {
        private GameField _gameField;
        private Options _options;
        private int _elapsedTime; //отображает затраченное время на форме
        private int _minesLeft; //отображает количество оставшихся мин на форме

        public GameData(Options options, GameField gameField, int elapsedTime, int minesLeft)
        {
            _options = options;
            _gameField = gameField;
            _elapsedTime = elapsedTime;
            _minesLeft = minesLeft;
        }
    }

}
