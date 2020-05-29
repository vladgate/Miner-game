using MinerLogic.CommonPublic;
using System;
using System.Collections;
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

        //static int count = 0;
        private GameManager() //конструктор
        {
            _optionsManager = new OptionsManager(OptionsManager.OptionsFromInnerSettings());
            _currentOptions = _optionsManager.CurrentOptions;
            _changedCells = new List<Cell>(_currentOptions.Width * _currentOptions.Height >> 2);
            _recursionInvokedCells = new List<Cell>(_currentOptions.Width * _currentOptions.Height >> 2);
        }

        internal Options GetCurrentOptions()
        {
            return _currentOptions;
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
                if (!_cells[posX, posY].IsMine)
                {
                    _cells[posX, posY].IsMine = true;
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
            _recursionInvokedCells.Clear(); // очищаем хранилище для рекурсивно обработаных ячеек
            if (_gameState == GameState.NotStarted) //надо провести начальное размещение мин на игровом поле
            {
                PlaceMines(indexX, indexY); //размещение мин на поле
                //HandleEmptyCell(indexX, indexY); // обработка незанятой ячейки
                //HandleEmptyCellQueue(indexX, indexY);//22.6
                HandleEmptyCellStack(indexX, indexY);//20.4/20.6/
                //таймер надо запустить где-то здесь
                _gameState = GameState.InProgress; // игра началась
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
                }
                else
                {
                    //HandleEmptyCell(indexX, indexY);
                    //HandleEmptyCellQueue(indexX, indexY);
                    HandleEmptyCellStack(indexX, indexY);
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

        private void HandleEmptyCellQueue(int cellX, int cellY)
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

            Queue<Cell> queue = new Queue<Cell>(8);
            queue.Enqueue(_cells[cellX, cellY]);
            while (queue.Count != 0)
            {
                Cell current = queue.Dequeue();

                //if (!(_cells[cellX, cellY].CellValue == CellValue.Closed || _cells[cellX, cellY].CellValue == CellValue.Question))
                //{
                //    continue;
                //}
                //count++;
                byte minesAround = GetMinesAround(current);
                if (minesAround == 0)
                {
                    _cells[current.X, current.Y].CellValue = CellValue.EmptyOpen;
                    if (current.X != 0)
                    {
                        if (!_changedCells.Contains(_cells[current.X - 1, current.Y]) && !queue.Contains(_cells[current.X - 1, current.Y]) && !_cells[current.X - 1, current.Y].IsMine) //left
                        {
                            queue.Enqueue(_cells[current.X - 1, current.Y]);
                        }
                    }
                    if (current.X != _currentOptions.Width - 1)
                    {
                        if (!_changedCells.Contains(_cells[current.X + 1, current.Y]) && !queue.Contains(_cells[current.X + 1, current.Y]) && !_cells[current.X + 1, current.Y].IsMine) //right
                        {
                            queue.Enqueue(_cells[current.X + 1, current.Y]);
                        }
                    }
                    if (current.Y != 0)
                    {
                        if (!_changedCells.Contains(_cells[current.X, current.Y - 1]) && !queue.Contains(_cells[current.X, current.Y - 1]) && !_cells[current.X, current.Y - 1].IsMine) //up
                        {
                            queue.Enqueue(_cells[current.X, current.Y - 1]);
                        }
                    }
                    if (current.Y != _currentOptions.Height - 1)
                    {
                        if (!_changedCells.Contains(_cells[current.X, current.Y + 1]) && !queue.Contains(_cells[current.X, current.Y + 1])&& !_cells[current.X, current.Y + 1].IsMine) //down
                        {
                            queue.Enqueue(_cells[current.X, current.Y + 1]);
                        }
                    }
                    if (current.X != 0 && current.Y != 0)
                    {
                        if (!_changedCells.Contains(_cells[current.X - 1, current.Y - 1]) && !queue.Contains(_cells[current.X - 1, current.Y - 1])&& !_cells[current.X - 1, current.Y - 1].IsMine) //left, up 
                        {
                            queue.Enqueue(_cells[current.X - 1, current.Y - 1]);
                        }
                    }

                    if (current.X != 0 && current.Y != _currentOptions.Height - 1)
                    {
                        if (!_changedCells.Contains(_cells[current.X - 1, current.Y + 1]) && !queue.Contains(_cells[current.X - 1, current.Y + 1])&& !_cells[current.X - 1, current.Y + 1].IsMine) //left, down
                        {
                            queue.Enqueue(_cells[current.X - 1, current.Y + 1]);
                        }
                    }

                    if (current.X != _currentOptions.Width - 1 && current.Y != 0)
                    {
                        if (!_changedCells.Contains(_cells[current.X + 1, current.Y - 1]) && !queue.Contains(_cells[current.X + 1, current.Y - 1])&& !_cells[current.X + 1, current.Y - 1].IsMine) //right, up
                        {
                            queue.Enqueue(_cells[current.X + 1, current.Y - 1]);
                        }
                    }
                    if (current.X != _currentOptions.Width - 1 && current.Y != _currentOptions.Height - 1)
                    {
                        if (!_changedCells.Contains(_cells[current.X + 1, current.Y + 1]) && !queue.Contains(_cells[current.X + 1, current.Y + 1]) && !_cells[current.X + 1, current.Y + 1].IsMine) //right, down
                        {
                            queue.Enqueue(_cells[current.X + 1, current.Y + 1]);
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
        private void HandleEmptyCellStack(int cellX, int cellY)
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

            Stack<Cell> queue = new Stack<Cell>(8);
            queue.Push(_cells[cellX, cellY]);
            while (queue.Count != 0)
            {
                Cell current = queue.Pop();

                //if (!(_cells[cellX, cellY].CellValue == CellValue.Closed || _cells[cellX, cellY].CellValue == CellValue.Question))
                //{
                //    continue;
                //}
                //count++;
                byte minesAround = GetMinesAround(current);
                if (minesAround == 0)
                {
                    _cells[current.X, current.Y].CellValue = CellValue.EmptyOpen;
                    if (current.X != 0)
                    {
                        if (!_changedCells.Contains(_cells[current.X - 1, current.Y]) && !queue.Contains(_cells[current.X - 1, current.Y]) && !_cells[current.X - 1, current.Y].IsMine) //left
                        {
                            queue.Push(_cells[current.X - 1, current.Y]);
                        }
                    }
                    if (current.X != _currentOptions.Width - 1)
                    {
                        if (!_changedCells.Contains(_cells[current.X + 1, current.Y]) && !queue.Contains(_cells[current.X + 1, current.Y]) && !_cells[current.X + 1, current.Y].IsMine) //right
                        {
                            queue.Push(_cells[current.X + 1, current.Y]);
                        }
                    }
                    if (current.Y != 0)
                    {
                        if (!_changedCells.Contains(_cells[current.X, current.Y - 1]) && !queue.Contains(_cells[current.X, current.Y - 1]) && !_cells[current.X, current.Y - 1].IsMine) //up
                        {
                            queue.Push(_cells[current.X, current.Y - 1]);
                        }
                    }
                    if (current.Y != _currentOptions.Height - 1)
                    {
                        if (!_changedCells.Contains(_cells[current.X, current.Y + 1]) && !queue.Contains(_cells[current.X, current.Y + 1]) && !_cells[current.X, current.Y + 1].IsMine) //down
                        {
                            queue.Push(_cells[current.X, current.Y + 1]);
                        }
                    }
                    if (current.X != 0 && current.Y != 0)
                    {
                        if (!_changedCells.Contains(_cells[current.X - 1, current.Y - 1]) && !queue.Contains(_cells[current.X - 1, current.Y - 1]) && !_cells[current.X - 1, current.Y - 1].IsMine) //left, up 
                        {
                            queue.Push(_cells[current.X - 1, current.Y - 1]);
                        }
                    }

                    if (current.X != 0 && current.Y != _currentOptions.Height - 1)
                    {
                        if (!_changedCells.Contains(_cells[current.X - 1, current.Y + 1]) && !queue.Contains(_cells[current.X - 1, current.Y + 1]) && !_cells[current.X - 1, current.Y + 1].IsMine) //left, down
                        {
                            queue.Push(_cells[current.X - 1, current.Y + 1]);
                        }
                    }

                    if (current.X != _currentOptions.Width - 1 && current.Y != 0)
                    {
                        if (!_changedCells.Contains(_cells[current.X + 1, current.Y - 1]) && !queue.Contains(_cells[current.X + 1, current.Y - 1]) && !_cells[current.X + 1, current.Y - 1].IsMine) //right, up
                        {
                            queue.Push(_cells[current.X + 1, current.Y - 1]);
                        }
                    }
                    if (current.X != _currentOptions.Width - 1 && current.Y != _currentOptions.Height - 1)
                    {
                        if (!_changedCells.Contains(_cells[current.X + 1, current.Y + 1]) && !queue.Contains(_cells[current.X + 1, current.Y + 1]) && !_cells[current.X + 1, current.Y + 1].IsMine) //right, down
                        {
                            queue.Push(_cells[current.X + 1, current.Y + 1]);
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
        /// обработка открытия пустой ячейки
        /// </summary>
        private void HandleEmptyCell(int cellX, int cellY)
        {
            if (!(_cells[cellX, cellY].CellValue == CellValue.Closed || _cells[cellX, cellY].CellValue == CellValue.Question))
            {
                return;
            }
            //count++;

            byte amountMinesAround = 0;
            List<Cell> recursionList = new List<Cell>(); // list for recursion

            if (_cells[cellX, cellY].X != 0)
            {
                if (_cells[cellX - 1, cellY].IsMine) //left
                {
                    amountMinesAround++;
                }
                else
                {
                    if (!_changedCells.Contains(_cells[cellX - 1, cellY]))
                        recursionList.Add(_cells[cellX - 1, cellY]);
                }
            }
            if (_cells[cellX, cellY].X != _currentOptions.Width - 1)
            {
                if (_cells[cellX + 1, cellY].IsMine) //right
                    amountMinesAround++;
                else
                {
                    if (!_changedCells.Contains(_cells[cellX + 1, cellY]))
                        recursionList.Add(_cells[cellX + 1, cellY]);
                }
            }
            if (_cells[cellX, cellY].Y != 0)
            {
                if (_cells[cellX, cellY - 1].IsMine) //up
                    amountMinesAround++;
                else
                {
                    if (!_changedCells.Contains(_cells[cellX, cellY - 1]))
                        recursionList.Add(_cells[cellX, cellY - 1]);
                }
            }
            if (_cells[cellX, cellY].Y != _currentOptions.Height - 1)
            {
                if (_cells[cellX, cellY + 1].IsMine) //down
                    amountMinesAround++;
                else
                {
                    if (!_changedCells.Contains(_cells[cellX, cellY + 1]))
                        recursionList.Add(_cells[cellX, cellY + 1]);
                }
            }
            if (_cells[cellX, cellY].X != 0 && _cells[cellX, cellY].Y != 0)
            {
                if (_cells[cellX - 1, cellY - 1].IsMine) //left, up 
                    amountMinesAround++;
                else
                {
                    if (!_changedCells.Contains(_cells[cellX - 1, cellY - 1]))
                        recursionList.Add(_cells[cellX - 1, cellY - 1]);
                }
            }
            if (_cells[cellX, cellY].X != 0 && _cells[cellX, cellY].Y != _currentOptions.Height - 1)
            {
                if (_cells[cellX - 1, cellY + 1].IsMine) //left, down
                    amountMinesAround++;
                else
                {
                    if (!_changedCells.Contains(_cells[cellX - 1, cellY + 1]))
                        recursionList.Add(_cells[cellX - 1, cellY + 1]);
                }
            }
            if (_cells[cellX, cellY].X != _currentOptions.Width - 1 && _cells[cellX, cellY].Y != 0)
            {
                if (_cells[cellX + 1, cellY - 1].IsMine) //right, up
                    amountMinesAround++;
                else
                {
                    if (!_changedCells.Contains(_cells[cellX + 1, cellY - 1]))
                        recursionList.Add(_cells[cellX + 1, cellY - 1]);
                }
            }
            if (_cells[cellX, cellY].X != _currentOptions.Width - 1 && _cells[cellX, cellY].Y != _currentOptions.Height - 1)
            {
                if (_cells[cellX + 1, cellY + 1].IsMine) //right, down
                    amountMinesAround++;
                else
                {
                    if (!_changedCells.Contains(_cells[cellX + 1, cellY + 1]))
                        recursionList.Add(_cells[cellX + 1, cellY + 1]);
                }
            }

            if (amountMinesAround == 0)
            {
                _cells[cellX, cellY].CellValue = CellValue.EmptyOpen;
                _changedCells.Add(_cells[cellX, cellY]);
                foreach (Cell c in recursionList)
                {
                    if (_recursionInvokedCells.Contains(c))
                    {
                        continue;
                    }
                    else
                    {
                        _recursionInvokedCells.Add(c);
                    }
                    HandleEmptyCell(c.X, c.Y);
                }
            }
            else
            {
                switch (amountMinesAround)
                {
                    case 1:
                        _cells[cellX, cellY].CellValue = CellValue.One; break;
                    case 2:
                        _cells[cellX, cellY].CellValue = CellValue.Two; break;
                    case 3:
                        _cells[cellX, cellY].CellValue = CellValue.Three; break;
                    case 4:
                        _cells[cellX, cellY].CellValue = CellValue.Four; break;
                    case 5:
                        _cells[cellX, cellY].CellValue = CellValue.Five; break;
                    case 6:
                        _cells[cellX, cellY].CellValue = CellValue.Six; break;
                    case 7:
                        _cells[cellX, cellY].CellValue = CellValue.Seven; break;
                    case 8:
                        _cells[cellX, cellY].CellValue = CellValue.Eight; break;
                }
                _changedCells.Add(_cells[cellX, cellY]);
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
            _gameField = new GameField(_currentOptions.Width, _currentOptions.Height, _currentOptions.MinesAmount);
            _gameType = _optionsManager.GetGameType(_currentOptions);
            _gameField.Init();
            _cells = _gameField.Cells;
            _minesLeft = _currentOptions.MinesAmount;
            _elapsedTime = 0;
            _gameState = GameState.NotStarted;
        }
        /// <summary>
        /// инициализация значений для новой игры с новыми настройками
        /// </summary>
        public void StartNewGame(Options options)
        {
            _currentOptions = options; //сохраняем новые значения настроек
            _gameType = _optionsManager.GetGameType(_currentOptions);
            _gameField = new GameField(_currentOptions.Width, _currentOptions.Height, _currentOptions.MinesAmount);
            _gameField.Init();
            _cells = _gameField.Cells;
            _minesLeft = _currentOptions.MinesAmount;
            _elapsedTime = 0;
            _gameState = GameState.NotStarted;
        }

        /// <summary>
        /// продолжаем ранее сохраненную игру
        /// </summary>
        /// <param name="gameData">данные игры</param>
        public void ContinueSavedGame(GameData gameData)
        {
            _currentOptions = gameData.Options;
            _gameType = _optionsManager.GetGameType(_currentOptions);
            _gameField = gameData.GameField;
            _cells = _gameField.Cells;
            _minesLeft = gameData.MinesLeft;
            _elapsedTime = gameData.ElapsedTime;
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
        public bool SaveCurrentGame()
        {
            GameData gameData = new GameData(_currentOptions, _gameField, _elapsedTime, _minesLeft, _gameState);
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
        Win = 2,
        Loose = 4
    }
}
