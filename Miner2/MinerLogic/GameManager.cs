using MinerLogic.CommonPublic;
using MinerLogic.MinerLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MinerLogic
{
    internal sealed class GameManager
    {
        private static GameManager _instance;
        private const string SAVE_FILE_NAME = "game.save";

        private GameField _gameField;
        private OptionsManager _optionsManager;
        private int _elapsedTime; //отображает затраченное время на форме
        private short _minesLeft; //отображает количество оставшихся мин на форме
        private Options _currentOptions;
        private GameState _gameState;
        private GameType _gameType;
        private Cell[,] _cells;
        private List<Cell> _changedCells; // хранилище ячеек, которые изменились
        private List<Cell> _recursionInvokedCells;// хранилище ячеек, которые были рекурсивно обработаны после левого клика мыши
        private bool[,] _visited;
        private Timer _timer;
        internal event EventHandler<ElapsedTimeChangedEventArgs> ElapsedTimeChanged;

        //static int count = 0;
        private GameManager() //конструктор
        {
            _optionsManager = new OptionsManager(OptionsManager.OptionsFromInnerSettings());
            _currentOptions = _optionsManager.CurrentOptions;
            _changedCells = new List<Cell>(_currentOptions.Width * _currentOptions.Height >> 2);
            _recursionInvokedCells = new List<Cell>(_currentOptions.Width * _currentOptions.Height >> 2);
            _timer = new Timer(ChangeElapsedTime, null, -1, 1000);
        }

        private void ChangeElapsedTime(object state)
        {
            _elapsedTime++;
            ElapsedTimeChanged?.Invoke(this, new ElapsedTimeChangedEventArgs(_elapsedTime));
        }

        internal Options CurrentOptions
        {
            get { return _currentOptions; }
            //set { _currentOptions = value; }
        }
        internal GameType GetGameType()
        {
            return _gameType;
        }
        internal GameState GameState { get => _gameState; }

        /// <summary>
        /// сохраняем текущие настройки в приложения
        /// </summary>
        internal void SaveCurrentSettings()
        {
            _optionsManager.CurrentOptions = _currentOptions;
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
            CellValue current = _cells[indexX, indexY].CellValue;
            return current == CellValue.Closed || current == CellValue.Flag || current == CellValue.Question;
        }


        /// <summary>
        /// получает все незакрытые ячейки
        /// </summary>
        /// <returns>масси незакрытых ячеек</returns>
        internal Cell[] GetNotClosedCells()
        {
            List<Cell> cells = new List<Cell>(_currentOptions.Width * _currentOptions.Height / 4);
            for (int i = 0; i < _currentOptions.Width; i++)
            {
                for (int j = 0; j < _currentOptions.Height; j++)
                {
                    if (_cells[i, j].CellValue != CellValue.Closed)
                    {
                        cells.Add(_cells[i, j]);
                    }
                }
            };
            return cells.ToArray();
        }

        /// <summary>
        /// показывает, является ли ячейка по указанному индексу отмеченная флагом наличия мины
        /// </summary>
        internal bool IsFlagCell(int indexX, int indexY)
        {
            return _cells[indexX, indexY].CellValue == CellValue.Flag;
        }

        /// <summary>
        /// начальное размещение мин на поле, с учетом индекса первой ячейки, на которую щелкнули для избежания проигрыша при первом щелчке
        /// </summary>
        /// <param name="busyX">х-индекс первой ячейки</param>
        /// <param name="busyY">у-индекс первой ячейки</param>
        //private void PlaceMines(int busyX, int busyY) //
        //{
        //    Random rnd = new Random();
        //    int posX, posY, alreadyMinesPlaced = 0;
        //    while (_minesLeft > alreadyMinesPlaced)
        //    {
        //        posX = rnd.Next(_currentOptions.Width);
        //        posY = rnd.Next(_currentOptions.Height);
        //        if (posX == busyX && posY == busyY)
        //            continue;
        //        if (!_cells[posX, posY].IsMine)
        //        {
        //            _cells[posX, posY].IsMine = true;
        //            alreadyMinesPlaced++;
        //        }
        //    }
        //}

        /// <summary>
        /// происходит открытие ячейки по указанному индексу по левому щелчку мыши
        /// </summary>
        /// <returns>возращает массив ячеек, которые изменили свои значения</returns>
        internal Cell[] ApplyLeftClick(int indexX, int indexY)
        {
            _changedCells.Clear();// очищаем хранилище для измененных ячеек
            _recursionInvokedCells.Clear(); // очищаем хранилище для рекурсивно обработаных ячеек
            if (_gameState == GameState.NotStarted) //надо провести начальное размещение мин на игровом поле
            {
                _gameField.PlaceMines(indexX, indexY); //размещение мин на поле
                HandleEmptyCell(indexX, indexY);//20.4/20.6/
                _gameState = GameState.InProgress; // игра началась
                _timer.Change(0, 1000);
                if (_gameField.IsLastCellLeft())
                {
                    WinGame();
                }
            }
            else
            {
                if (_cells[indexX, indexY].IsMine) //попали на мину
                {
                    LooseGame(indexX, indexY);
                    _timer.Change(-1, 1000);
                }
                else
                {
                    HandleEmptyCell(indexX, indexY);
                    if (_gameState == GameState.Loaded)
                    {
                        _timer.Change(0, 1000);
                    }

                    if (_gameField.IsLastCellLeft())
                    {
                        WinGame();
                        _timer.Change(-1, 1000);
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
                        _cells[i, j].CellValue = CellValue.Exploded;
                        _changedCells.Add(_cells[i, j]);
                        continue;
                    }
                    else if (_cells[i, j].CellValue == CellValue.Flag && !_cells[i, j].IsMine)
                    {
                        _cells[i, j].CellValue = CellValue.WrongFlag;
                        _changedCells.Add(_cells[i, j]);
                        continue;
                    }
                    else if (_cells[i, j].IsMine)
                    {
                        _cells[i, j].CellValue = CellValue.Mine;
                        _changedCells.Add(_cells[i, j]);
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
                    if (_cells[i, j].CellValue != CellValue.Flag && _cells[i, j].IsMine)
                    {
                        _cells[i, j].CellValue = CellValue.Mine;
                        _changedCells.Add(_cells[i, j]);
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
            byte GetMinesAround(Cell cell)
            {
                byte amountMinesAround = 0;

                if (cell.X != 0)
                {
                    if (_cells[cell.X - 1, cell.Y].IsMine) //left
                        amountMinesAround++;
                }
                if (cell.X != _currentOptions.Width - 1)
                {
                    if (_cells[cell.X + 1, cell.Y].IsMine) //right
                        amountMinesAround++;
                }
                if (cell.Y != 0)
                {
                    if (_cells[cell.X, cell.Y - 1].IsMine) //up
                        amountMinesAround++;
                }
                if (cell.Y != _currentOptions.Height - 1)
                {
                    if (_cells[cell.X, cell.Y + 1].IsMine) //down
                        amountMinesAround++;
                }
                if (cell.X != 0 && cell.Y != 0)
                {
                    if (_cells[cell.X - 1, cell.Y - 1].IsMine) //left, up 
                        amountMinesAround++;
                }
                if (cell.X != 0 && cell.Y != _currentOptions.Height - 1)
                {
                    if (_cells[cell.X - 1, cell.Y + 1].IsMine) //left, down
                        amountMinesAround++;
                }
                if (cell.X != _currentOptions.Width - 1 && cell.Y != 0)
                {
                    if (_cells[cell.X + 1, cell.Y - 1].IsMine) //right, up
                        amountMinesAround++;
                }
                if (cell.X != _currentOptions.Width - 1 && cell.Y != _currentOptions.Height - 1)
                {
                    if (_cells[cell.X + 1, cell.Y + 1].IsMine) //right, down
                        amountMinesAround++;
                }
                return amountMinesAround;
            }

            //bool[,] visited = new bool[_gameField.Width, _gameField.Height];
            _visited[cellX, cellY] = true;
            Stack<Cell> stack = new Stack<Cell>(8);
            stack.Push(_cells[cellX, cellY]);
            while (stack.Count != 0)
            {
                Cell current = stack.Pop();
                byte minesAround = GetMinesAround(current);
                if (minesAround == 0)
                {
                    _cells[current.X, current.Y].CellValue = CellValue.EmptyOpen;
                    if (current.X != 0)
                    {
                        if (!_visited[current.X - 1, current.Y] && !_cells[current.X - 1, current.Y].IsMine) //left
                        {
                            _visited[current.X - 1, current.Y] = true;
                            stack.Push(_cells[current.X - 1, current.Y]);
                        }
                    }
                    if (current.X != _currentOptions.Width - 1)
                    {
                        if (!_visited[current.X + 1, current.Y] && !_cells[current.X + 1, current.Y].IsMine) //right
                        {
                            _visited[current.X + 1, current.Y] = true;
                            stack.Push(_cells[current.X + 1, current.Y]);
                        }
                    }
                    if (current.Y != 0)
                    {
                        if (!_visited[current.X, current.Y - 1] && !_cells[current.X, current.Y - 1].IsMine) //up
                        {
                            _visited[current.X, current.Y - 1] = true;
                            stack.Push(_cells[current.X, current.Y - 1]);
                        }
                    }
                    if (current.Y != _currentOptions.Height - 1)
                    {
                        if (!_visited[current.X, current.Y + 1] && !_cells[current.X, current.Y + 1].IsMine) //down
                        {
                            _visited[current.X, current.Y + 1] = true;
                            stack.Push(_cells[current.X, current.Y + 1]);
                        }
                    }
                    if (current.X != 0 && current.Y != 0)
                    {
                        if (!_visited[current.X - 1, current.Y - 1] && !_cells[current.X - 1, current.Y - 1].IsMine) //left, up 
                        {
                            _visited[current.X - 1, current.Y - 1] = true;
                            stack.Push(_cells[current.X - 1, current.Y - 1]);
                        }
                    }

                    if (current.X != 0 && current.Y != _currentOptions.Height - 1)
                    {
                        if (!_visited[current.X - 1, current.Y + 1] && !_cells[current.X - 1, current.Y + 1].IsMine) //left, down
                        {
                            _visited[current.X - 1, current.Y + 1] = true;
                            stack.Push(_cells[current.X - 1, current.Y + 1]);
                        }
                    }

                    if (current.X != _currentOptions.Width - 1 && current.Y != 0)
                    {
                        if (!_visited[current.X + 1, current.Y - 1] && !_cells[current.X + 1, current.Y - 1].IsMine) //right, up
                        {
                            _visited[current.X + 1, current.Y - 1] = true;
                            stack.Push(_cells[current.X + 1, current.Y - 1]);
                        }
                    }
                    if (current.X != _currentOptions.Width - 1 && current.Y != _currentOptions.Height - 1)
                    {
                        if (!_visited[current.X + 1, current.Y + 1] && !_cells[current.X + 1, current.Y + 1].IsMine) //right, down
                        {
                            _visited[current.X + 1, current.Y + 1] = true;
                            stack.Push(_cells[current.X + 1, current.Y + 1]);
                        }
                    }
                }
                else
                {
                    switch (minesAround)
                    {
                        case 1:
                            _cells[current.X, current.Y].CellValue = CellValue.One; break;
                        case 2:
                            _cells[current.X, current.Y].CellValue = CellValue.Two; break;
                        case 3:
                            _cells[current.X, current.Y].CellValue = CellValue.Three; break;
                        case 4:
                            _cells[current.X, current.Y].CellValue = CellValue.Four; break;
                        case 5:
                            _cells[current.X, current.Y].CellValue = CellValue.Five; break;
                        case 6:
                            _cells[current.X, current.Y].CellValue = CellValue.Six; break;
                        case 7:
                            _cells[current.X, current.Y].CellValue = CellValue.Seven; break;
                        case 8:
                            _cells[current.X, current.Y].CellValue = CellValue.Eight; break;
                    }
                }
                _changedCells.Add(_cells[current.X, current.Y]);
            }
        }

        /// <summary>
        /// происходят изменения элемента по указанному индексу по правому щелчку мыши
        /// </summary>
        /// <returns>возращает новое значение в ячейке</returns>
        internal Cell ApplyRightClick(int indexX, int indexY)
        {
            switch (_cells[indexX, indexY].CellValue)
            {
                case CellValue.Closed:
                    _cells[indexX, indexY].CellValue = CellValue.Flag;
                    _minesLeft--;
                    break;
                case CellValue.Flag:
                    _cells[indexX, indexY].CellValue = CellValue.Question;
                    _minesLeft++;
                    break;
                case CellValue.Question:
                    _cells[indexX, indexY].CellValue = CellValue.Closed;
                    break;
            }
            return _cells[indexX, indexY];
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
        public short MinesLeft
        {
            get { return _minesLeft; }
        }

        /// <summary>
        /// инициализация значений для новой игры с текущими настройками
        /// </summary>
        public void StartNewGame()
        {
            StartNewGame(_currentOptions);
        }
        /// <summary>
        /// инициализация значений для новой игры с новыми настройками
        /// </summary>
        public void StartNewGame(Options options)
        {
            _timer.Change(-1, 1000); //приостанавливает таймер
            _currentOptions = options; //сохраняем новые значения настроек
            _gameType = _optionsManager.GetGameType(_currentOptions);
            _gameField = new GameField(_currentOptions.Width, _currentOptions.Height, _currentOptions.MinesAmount);
            _cells = _gameField.InitCellArray();
            //_cells = _gameField.Cells;
            _minesLeft = _currentOptions.MinesAmount;
            _elapsedTime = 0;
            _gameState = GameState.NotStarted;
            _visited = new bool[_currentOptions.Width, _currentOptions.Height];
        }

        /// <summary>
        /// продолжаем ранее сохраненную игру
        /// </summary>
        /// <param name="gameData">данные игры</param>
        public void ContinueSavedGame(GameData gameData)
        {
            _timer.Change(-1, 1000); //приостанавливает таймер
            _currentOptions = gameData.Options;
            _gameType = _optionsManager.GetGameType(_currentOptions);
            _gameField = gameData.GameField;
            _cells = _gameField.Cells;
            _minesLeft = gameData.MinesLeft;
            _elapsedTime = gameData.ElapsedTime;
            if (_gameField.MinesPlaced) // мины были размещены на игровом поле
            {
                _gameState = GameState.Loaded;
            }
            else
            {
                _gameState = GameState.NotStarted;
            }

            _visited = new bool[_currentOptions.Width, _currentOptions.Height];
        }

        /// <summary>
        /// возвращает последние настройки
        /// </summary>
        public GameData DefaultSettings()
        {
            _currentOptions = _optionsManager.CurrentOptions;
            return new GameData(_currentOptions, null, 0, _currentOptions.MinesAmount);
        }

        /// <summary>
        /// сохраняет данные текущей игры
        /// </summary>
        /// <param name="gameData">данные игры для сохранения</param>
        /// <returns>true если успешно сохранено</returns>
        public bool SaveCurrentGame()
        {

            GameData gameData = new GameData(_currentOptions, _gameField, _elapsedTime, _minesLeft);
            BinaryFormatter formatter = new BinaryFormatter();
            bool success;
            try
            {
                using (FileStream stream = new FileStream(SAVE_FILE_NAME, FileMode.Create, FileAccess.Write))
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
            if (!File.Exists(SAVE_FILE_NAME))
            {
                throw new FileNotFoundException();
            }
            using (FileStream stream = new FileStream(SAVE_FILE_NAME, FileMode.Open, FileAccess.Read))
            {
                data = (GameData)formatter.Deserialize(stream);
            }
            return data;
        }
    }
    internal enum GameState : byte
    {
        NotStarted = 0, // ни одна ячейка еще не открыта
        InProgress = 1, // ячейки уже открывались
        Win = 2, // игра выиграна
        Loose = 4, // игра проиграна
        Loaded = 8 //игра только что загружена
    }
}
