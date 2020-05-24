using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
        private GameState _gameState;
        private List<Cell> _changedCells; // хранилище ячеек, которые изменились
        
        private GameManager() //конструктор
        {
            _optionsManager = new OptionsManager(OptionsManager.OptionsFromInnerSettings());
            _currentOptions = _optionsManager.CurrentOptions;
            _changedCells = new List<Cell>(_currentOptions.Width * _currentOptions.Height >> 2);
        }

        internal Options GetCurrentOptions()
        {
            return _currentOptions;
        }

        internal void SaveCurrentSettings()
        {
            _optionsManager.SaveCurrentOptions();
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
        /// показывает, является ли ячейка по указанному индексу отмеченной флагом наличия мины, знаком вопроса или закрытой
        /// </summary>
        internal bool IsClosedCell(int indexX, int indexY)
        {
            CellValue current = _gameField.Cells[indexX, indexY].CellValue;
            return current == CellValue.Closed || current == CellValue.Flag || current == CellValue.Question;
        }

        /// <summary>
        /// показывает, является ли ячейка по указанному индексу отмеченная флагом наличия мины
        /// </summary>
        internal bool IsFlagCell(int indexX, int indexY)
        {
            return _gameField.Cells[indexX, indexY].CellValue == CellValue.Flag;
        }

        /// <summary>
        /// начальное размещение мин на поле, с учетом индекса первой ячейки, на которую щелкнули для избежания проигрыша при первом щелчке
        /// </summary>
        /// <param name="busyX">х-индекс первой ячейки</param>
        /// <param name="busyY">у-индекс первой ячейки</param>
        private void PlaceMines(int busyX, int busyY) //
        {
            Random rnd = new Random();
            int posX, posY, alreadyMinesPlaced = 0;
            while (_minesLeft > alreadyMinesPlaced)
            {
                posX = rnd.Next(_currentOptions.Width);
                posY = rnd.Next(_currentOptions.Height);
                if (posX == busyX && posY == busyY)
                    continue;
                if (!_gameField.Cells[posX, posY].IsMine)
                {
                    _gameField.Cells[posX, posY].IsMine = true;
                    alreadyMinesPlaced++;
                }
            }
        }

        /// <summary>
        /// происходит открытие ячейки по указанному индексу по левому щелчку мыши
        /// </summary>
        /// <returns>возращает массив ячеек, которые изменили свои значения</returns>
        internal Cell[] ApplyLeftClick(int indexX, int indexY)
        {
            _changedCells.Clear();// очищаем хранилище для измененных ячеек
            if (_gameState == GameState.NotStarted) //надо провести начальное размещение мин на игровом поле
            {
                PlaceMines(indexX, indexY); //размещение мин на поле
                HandleEmptyCell(indexX, indexY); // обработка незанятой ячейки
                //таймер надо запустить где-то здесь
                _gameState = GameState.InProgress; // игра началась
            }
            else
            {
                if (_gameField.Cells[indexX, indexY].IsMine) //попали на мину
                {
                    LooseGame(indexX, indexY);
                }
                else
                {
                    HandleEmptyCell(indexX, indexY);
                    //SetImages();
                    if (_gameField.IsLastCellLeft())
                    {
                        WinGame();
                    }
                }
            }
            return _changedCells.ToArray();
        }

        /// <summary>
        /// обработка попадания на мину - проигрыша
        /// </summary>
        /// <param name="cellX">х-индекс ячейки, на которой подорвались</param>
        /// <param name="cellY">у-индекс ячейки, на которой подорвались</param>
        private void LooseGame(int cellX, int cellY)
        {
            for (int i = 0; i < _currentOptions.Width; i++)
            {
                for (int j = 0; j < _currentOptions.Height; j++)
                {
                    if (i == cellX && j == cellY)
                    {
                        _gameField.Cells[i, j].CellValue = CellValue.Exploded;
                        _changedCells.Add(_gameField.Cells[i, j]);
                        continue;
                    }
                    else if (_gameField.Cells[i, j].CellValue == CellValue.Flag && !_gameField.Cells[i, j].IsMine)
                    {
                        _gameField.Cells[i, j].CellValue = CellValue.WrongFlag;
                        _changedCells.Add(_gameField.Cells[i, j]);
                        continue;
                    }
                    else if (_gameField.Cells[i, j].IsMine)
                    {
                        _gameField.Cells[i, j].CellValue = CellValue.Mine;
                        _changedCells.Add(_gameField.Cells[i, j]);
                        continue;
                    }
                }
            }
            _gameState = GameState.Loose;
        }

        /// <summary>
        /// обработка выигрыша
        /// </summary>
        private void WinGame()
        {
            for (int i = 0; i < _currentOptions.Width; i++)
            {
                for (int j = 0; j < _currentOptions.Height; j++)
                {
                    if (_gameField.Cells[i, j].CellValue != CellValue.Flag && _gameField.Cells[i, j].IsMine)
                    {
                        _gameField.Cells[i, j].CellValue = CellValue.Mine;
                        _changedCells.Add(_gameField.Cells[i, j]);
                    }
                }
            }
            _gameState = GameState.Win;
        }

        /// <summary>
        /// обработка открытия пустой ячейки
        /// </summary>
        private void HandleEmptyCell(int cellX, int cellY)
        {
            Cell[,] cells = _gameField.Cells;
            if (!(cells[cellX, cellY].CellValue == CellValue.Closed || cells[cellX, cellY].CellValue == CellValue.Question))
            {
                return;
            }
            int amountMinesAround = 0;
            List<Cell> recursionList = new List<Cell>(); // list for recursion

            if (cells[cellX, cellY].X != 0)
            {
                if (cells[cellX - 1, cellY].IsMine)//left
                {
                    amountMinesAround++;
                }
                else
                {
                    if (!_changedCells.Contains(cells[cellX - 1, cellY]))
                        recursionList.Add(cells[cellX - 1, cellY]);
                }
            }
            if (cells[cellX, cellY].X != _currentOptions.Width - 1)
            {
                if (cells[cellX + 1, cellY].IsMine) //right
                    amountMinesAround++;
                else
                {
                    if (!_changedCells.Contains(cells[cellX + 1, cellY]))
                        recursionList.Add(cells[cellX + 1, cellY]);
                }
            }
            if (cells[cellX, cellY].Y != 0)
            {
                if (cells[cellX, cellY - 1].IsMine) //up
                    amountMinesAround++;
                else
                {
                    if (!_changedCells.Contains(cells[cellX, cellY - 1]))
                        recursionList.Add(cells[cellX, cellY - 1]);
                }

            }
            if (cells[cellX, cellY].Y != _currentOptions.Height - 1)
            {
                if (cells[cellX, cellY + 1].IsMine) //down
                    amountMinesAround++;
                else
                {
                    if (!_changedCells.Contains(cells[cellX, cellY + 1]))
                        recursionList.Add(cells[cellX, cellY + 1]);
                }
            }
            if (cells[cellX, cellY].X != 0 && cells[cellX, cellY].Y != 0)
            {
                if (cells[cellX - 1, cellY - 1].IsMine) //left, up 
                    amountMinesAround++;
                else
                {
                    if (!_changedCells.Contains(cells[cellX - 1, cellY - 1]))
                        recursionList.Add(cells[cellX - 1, cellY - 1]);
                }
            }
            if (cells[cellX, cellY].X != 0 && cells[cellX, cellY].Y != _currentOptions.Height - 1)
            {
                if (cells[cellX - 1, cellY + 1].IsMine) //left, down
                    amountMinesAround++;
                else
                {
                    if (!_changedCells.Contains(cells[cellX - 1, cellY + 1]))
                        recursionList.Add(cells[cellX - 1, cellY + 1]);
                }
            }
            if (cells[cellX, cellY].X != _currentOptions.Width - 1 && cells[cellX, cellY].Y != 0)
            {
                if (cells[cellX + 1, cellY - 1].IsMine) //right, up
                    amountMinesAround++;
                else
                {
                    if (!_changedCells.Contains(cells[cellX + 1, cellY - 1]))
                        recursionList.Add(cells[cellX + 1, cellY - 1]);
                }
            }
            if (cells[cellX, cellY].X != _currentOptions.Width - 1 && cells[cellX, cellY].Y != _currentOptions.Height - 1)
            {
                if (cells[cellX + 1, cellY + 1].IsMine) //right, down
                    amountMinesAround++;
                else
                {
                    if (!_changedCells.Contains(cells[cellX + 1, cellY + 1]))
                        recursionList.Add(cells[cellX + 1, cellY + 1]);
                }
            }

            if (amountMinesAround == 0)
            {
                cells[cellX, cellY].CellValue = CellValue.EmptyOpen;
                _changedCells.Add(cells[cellX, cellY]);
                foreach (Cell c in recursionList)
                {
                    HandleEmptyCell(c.X, c.Y);
                }
            }
            else
            {
                switch (amountMinesAround)
                {
                    case 1:
                        cells[cellX, cellY].CellValue = CellValue.One; break;
                    case 2:
                        cells[cellX, cellY].CellValue = CellValue.Two; break;
                    case 3:
                        cells[cellX, cellY].CellValue = CellValue.Three; break;
                    case 4:
                        cells[cellX, cellY].CellValue = CellValue.Four; break;
                    case 5:
                        cells[cellX, cellY].CellValue = CellValue.Five; break;
                    case 6:
                        cells[cellX, cellY].CellValue = CellValue.Six; break;
                    case 7:
                        cells[cellX, cellY].CellValue = CellValue.Seven; break;
                    case 8:
                        cells[cellX, cellY].CellValue = CellValue.Eight; break;
                }
                _changedCells.Add(cells[cellX, cellY]);
            }
        }

        /// <summary>
        /// происходят изменения элемента по указанному индексу по правому щелчку мыши
        /// </summary>
        /// <returns>возращает новое значение в ячейке</returns>
        internal Cell ApplyRightClick(int indexX, int indexY)
        {
            Cell currentCell = _gameField.Cells[indexX, indexY];
            switch (currentCell.CellValue)
            {
                case CellValue.Closed:
                    _gameField.Cells[indexX, indexY].CellValue = CellValue.Flag;
                    _minesLeft--;
                    break;
                case CellValue.Flag:
                    _gameField.Cells[indexX, indexY].CellValue = CellValue.Question;
                    _minesLeft++;
                    break;
                case CellValue.Question:
                    _gameField.Cells[indexX, indexY].CellValue = CellValue.Closed;
                    break;
            }
            return _gameField.Cells[indexX, indexY];
        }

        /// <summary>
        /// возвращает прошедшее с начала игры время в секундах
        /// </summary>
        public int ElapsedTime
        {
            get { return _elapsedTime; }
        }

        /// <summary>
        /// возвращает количество оставшихся мин с учетом открытых и отмеченых ячеек (для отображения на форме)
        /// </summary>
        public int MinesLeft
        {
            get { return _minesLeft; }
        }

        internal GameState GameState { get => _gameState;}

        /// <summary>
        /// инициализация значений для новой игры
        /// </summary>
        /// <param name="gameData">данные игры</param>
        public void StartNewGame()
        {
            _gameField = new GameField(_currentOptions.Width, _currentOptions.Height, _currentOptions.MinesAmount);
            _gameField.Init();
            _minesLeft = _currentOptions.MinesAmount;
            _elapsedTime = 0;
            _gameState = GameState.NotStarted;
        }

        public void ContinueSavedGame(GameData gameData)
        {
            _currentOptions = gameData.Options;
            _minesLeft = gameData.MinesLeft;
            _elapsedTime = gameData.ElapsedTime;
            _gameField = gameData.GameField;
            _gameState = gameData.GameState;
        }

        /// <summary>
        /// возвращает последние настройки
        /// </summary>
        public GameData DefaultSettings()
        {
            _currentOptions = _optionsManager.CurrentOptions;
            return new GameData(_currentOptions, null, 0, _currentOptions.MinesAmount, GameState.NotStarted);
        }

        /// <summary>
        /// сохраняет данные текущей игры
        /// </summary>
        /// <param name="gameData">данные игры для сохранения</param>
        /// <returns>true если успешно сохранено</returns>
        public bool SaveGame(GameData gameData)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            bool success;
            try
            {
                using (FileStream stream = new FileStream("game.save", FileMode.Create, FileAccess.Write))
                {
                    formatter.Serialize(stream, gameData);
                }
                success = true;
            }
            catch (Exception)
            {
                throw;
            }
            return success;
        }

        /// <summary>
        /// получает сохраненные данные для игры
        /// </summary>
        /// <returns>данные игры</returns>
        public GameData LoadGame()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            GameData data;
            if (!File.Exists("game.save"))
            {
                throw new FileNotFoundException();
            }
            using (FileStream stream = new FileStream("game.save", FileMode.Open, FileAccess.Read))
            {
                data = (GameData)formatter.Deserialize(stream);
            }
            return data;

        }
    }
    internal enum GameType : byte
    {
        Easy = 0,
        Medium = 1,
        Hard = 2,
        Custom = 4
    }
    internal enum GameState : byte
    {
        NotStarted = 0, // ни одна ячейка еще не открыта
        InProgress = 1, // ячейки уже открывались
        Win = 2,
        Loose = 4
    }
}
