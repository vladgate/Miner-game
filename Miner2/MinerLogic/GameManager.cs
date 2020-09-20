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
      private int _elapsedTime; //for displaying elapsed time in the view
      private short _minesLeft; //for displaying mines left in the view
      private Options _currentOptions;
      private GameState _gameState;
      private GameType _gameType;
      private Cell[,] _cells;
      private List<Cell> _changedCells; // changed cells storage
      private List<Cell> _recursionInvokedCells;//cell's storage which have been recursively handled after left mouse click
      private bool[,] _visited;
      private Timer _timer;
      internal event EventHandler<ElapsedTimeChangedEventArgs> ElapsedTimeChanged;

      private GameManager() //contsructor
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
      }
      internal GameType GetGameType()
      {
         return _gameType;
      }
      internal GameState GameState { get => _gameState; }

      /// <summary>
      ///save current app settings
      /// </summary>
      internal void SaveCurrentSettings()
      {
         _optionsManager.CurrentOptions = _currentOptions;
         _optionsManager.SaveCurrentOptions();
      }

      /// <summary>
      /// returns GameManager as singleton
      /// </summary>
      internal static GameManager Instance
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
      /// indicate that cell at specified index has flag, question mark or closed
      /// </summary>
      internal bool IsClosedCell(int indexX, int indexY)
      {
         CellValue current = _cells[indexX, indexY].CellValue;
         return current == CellValue.Closed || current == CellValue.Flag || current == CellValue.Question;
      }

      /// <summary>
      /// gets all not closed cells
      /// </summary>
      /// <returns>array not closed cells</returns>
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
      /// indicate that cell at specified index has been marked with flag
      /// </summary>
      internal bool IsFlagCell(int indexX, int indexY)
      {
         return _cells[indexX, indexY].CellValue == CellValue.Flag;
      }

      /// <summary>
      /// 
      /// the cell at the specified index is opened by left-clicking
      /// </summary>
      /// <returns>array with cells that have been changed their values</returns>
      internal Cell[] ApplyLeftClick(int indexX, int indexY)
      {
         _changedCells.Clear();// clear storage for changed cells
         _recursionInvokedCells.Clear(); //clear storage for recursively handled cells
         if (_gameState == GameState.NotStarted) //if not started - need to place mines on the gamefield
         {
            _gameField.PlaceMines(indexX, indexY); //place mines
            HandleEmptyCell(indexX, indexY);
            _gameState = GameState.InProgress; // start game
            _timer.Change(0, 1000);
            if (_gameField.IsLastCellLeft())
            {
               WinGame();
            }
         }
         else
         {
            if (_cells[indexX, indexY].IsMine) // click on mine
            {
               LooseGame(indexX, indexY);
               _timer.Change(-1, 1000);
            }
            else
            {
               HandleEmptyCell(indexX, indexY);
               if (_gameState == GameState.Loaded) //game was just started - stat the timer
               {
                  _timer.Change(0, 1000);
               }

               if (_gameField.IsLastCellLeft())
               {
                  WinGame();
                  _timer.Change(-1, 1000); //stop timer
               }
            }
         }
         return _changedCells.ToArray();
      }

      /// <summary>
      /// handle click on mine - loosing
      /// </summary>
      /// <param name="cellX">х-horizontal index of cell</param>
      /// <param name="cellY">у-vertical index of cell</param>
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
      /// win game handler
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
      /// click on empty cell handler
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
      /// right click handler - change cell at specified index
      /// </summary>
      /// <returns>new cell's value</returns>
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
      ///returns elapsed time from game start
      /// </summary>
      public int ElapsedTime
      {
         get { return _elapsedTime; }
      }

      /// <summary>
      /// returns mines left amount taking into account open and marked cells (for display in view)
      /// </summary>
      public short MinesLeft
      {
         get { return _minesLeft; }
      }

      /// <summary>
      /// init new game with current settings
      /// </summary>
      public void StartNewGame()
      {
         StartNewGame(_currentOptions);
      }
      /// <summary>
      /// init new game with new settings
      /// </summary>
      public void StartNewGame(Options options)
      {
         _timer.Change(-1, 1000); //stop timer
         _currentOptions = options; //save new options
         _gameType = _optionsManager.GetGameType(_currentOptions);
         _gameField = new GameField(_currentOptions.Width, _currentOptions.Height, _currentOptions.MinesAmount);
         _cells = _gameField.InitCellArray();
         _minesLeft = _currentOptions.MinesAmount;
         _elapsedTime = 0;
         _gameState = GameState.NotStarted;
         _visited = new bool[_currentOptions.Width, _currentOptions.Height];
      }

      /// <summary>
      /// resume saved game
      /// </summary>
      /// <param name="gameData">data of the game</param>
      public void ContinueSavedGame(GameData gameData)
      {
         _timer.Change(-1, 1000); //stop timer
         _currentOptions = gameData.Options;
         _gameType = _optionsManager.GetGameType(_currentOptions);
         _gameField = gameData.GameField;
         _cells = _gameField.Cells;
         _minesLeft = gameData.MinesLeft;
         _elapsedTime = gameData.ElapsedTime;
         if (_gameField.MinesPlaced) //mines have been already placed on the gamefield
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
      /// save current game
      /// </summary>
      /// <returns>true if successfully saved</returns>
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
      /// gets the saved game data
      /// </summary>
      /// <returns>saved game data</returns>
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
      NotStarted = 0, // no open cells
      InProgress = 1, // cells have been changed (opened)
      Win = 2, // game won
      Loose = 4, // game is lost
      Loaded = 8 //game just has been loaded
   }
}
