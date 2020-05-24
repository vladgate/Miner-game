using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinerLogic
{
    internal sealed class GameManager
    {
        private static GameManager _instance;

        private GameField _gameField;
        private OptionsManager _optionsManager;
        private int _elapsedTime; //отображает затраченное время на форме
        private int _minesLeft; //отображает количество оставшихся мин на форме
        private Options _currentOptions;

        private GameManager() //конструктор
        {
            _optionsManager = new OptionsManager();
            _currentOptions = _optionsManager.CurrentOptions;
            _gameField = new GameField(_currentOptions.Width, _currentOptions.Height, _currentOptions.MinesAmount);
        }

        internal Options GetCurrentOptions()
        {
            return _optionsManager.CurrentOptions;
        }

        /// <summary>
        /// возвращает экземпляр класса GameManager как синглтон
        /// </summary>
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameManager();
                }
                return _instance;
            }
        }

        /// <summary>
        /// возвращает прошедшее с начала игры время в секундах
        /// </summary>
        public int ElapsedTime
        {
            get { return _elapsedTime; }
        }

        internal CellValue GetCurrentCellState(int indexX, int indexY)
        {
            return _gameField.Cells[indexX,indexY].CellValue;
        }

        internal void SetCellValue(int indexX, int indexY, CellValue value)
        {
            _gameField.Cells[indexX,indexY].CellValue = value;
        }

        /// <summary>
        /// возвращает количество оставшихся мин с учетом открытых и отмеченых ячеек (для отображения на форме)
        /// </summary>
        public int MinesLeft
        {
            get { return _minesLeft; }
        }


        public void StartNewGame()
        {

        }

        public void SaveGame(GameData gameData)
        {

        }

        public GameData LoadGame()
        {
            throw new NotImplementedException();
        }
    }
    internal enum GameType : byte
    {
        Easy,
        Medium,
        Hard,
        Custom
    }
}
