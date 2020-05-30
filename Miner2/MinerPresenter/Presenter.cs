using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MinerLogic;
using MinerLogic.CommonPublic;
using MinerLogic.Interfaces;
using MinerLogic.MinerLogic;
using MinerLogic.MinerPresenter;

namespace MinerPresenter
{
    public sealed class Presenter
    {
        private const int CELL_SIZE = 16;
        private const float MINES_CORRECTION = 0.5f;

        private IMainView _mainView;
        private IMessageService _messageService;
        IOptionsView _optionsView;
        private GameManager _gameManager;

        public Presenter(IMainView mainView, IMessageService messageService)
        {
            _mainView = mainView;
            _messageService = messageService;
            _gameManager = GameManager.Instance;
            _gameManager.StartNewGame();
            _gameManager.ElapsedTimeChanged += ElapsedTimeHandler;
            InitializeMainView();
        }

        private void ElapsedTimeHandler(object sender, ElapsedTimeChangedEventArgs e)
        {
            Task.Run(() => _mainView.ElapsedTime = e.ElapsedTime);
        }

        /// <summary>
        /// вызывае, когда проиграли
        /// </summary>
        private void LooseGame()
        {
            bool wantNewGame = _messageService.LooseGame("You loose! Start a new game?", "Looser!");
            ProposeNewGame(wantNewGame);
        }

        /// <summary>
        /// вызывае, когда выиграли
        /// </summary>
        private void WinGame()
        {
            _mainView.MinesLeft = 0;
            bool wantNewGame = _messageService.LooseGame("You win! Start a new game?", "Winner!");
            ProposeNewGame(wantNewGame);
        }

        /// <summary>
        /// предложить новую игру
        /// </summary>
        /// <param name="wantNewGame">true усли хотим новую игру</param>
        private void ProposeNewGame(bool wantNewGame)
        {
            if (wantNewGame)
            {
                WantNewGame();
            }
            else
            {
                ExitGame(true);
            }
        }

        /// <summary>
        /// вызывается при положительном ответе на предложение новой игры
        /// </summary>
        private void WantNewGame()
        {
            ClearGameFieldSelected();
            _mainView.MinesLeft = _gameManager.CurrentOptions.MinesAmount;
            _mainView.ElapsedTime = 0;
            _gameManager.StartNewGame();
        }

        /// <summary>
        /// очищаем игровое поле только там, где ячейки открыты
        /// </summary>
        private void ClearGameFieldSelected()
        {
            Cell[] cells = _gameManager.GetNotClosedCells();
            for (int i = 0; i < cells.Length; i++)
            {
                _mainView.SetClosedCell(cells[i].X, cells[i].Y);
            }
        }

        /// <summary>
        /// вызывается при изменении с подтверждением настроек игры в форме настроек
        /// </summary>
        private void WantNewGameWithChangedOptions(Options newOptions)
        {
            Options prevOptions = _gameManager.CurrentOptions;
            if (newOptions.Width != prevOptions.Width || newOptions.Height != prevOptions.Height) // габариты игрового поля поменялись
            {
                _mainView.AdjustViewToCellsAmount(newOptions.Width, newOptions.Height);
                DrawEmptyGameFieldAdvanced(newOptions.Width, prevOptions.Width, newOptions.Height, prevOptions.Height);
                ClearGameFieldSelected();
            }
            else
            {
                ClearGameFieldSelected(); // габариты поля не поменялись - только меняем изображения открытых ячеек
            }
            _gameManager.StartNewGame(newOptions);
            _mainView.MinesLeft = newOptions.MinesAmount;
            _mainView.ElapsedTime = 0;
            //_mainView.ClearGameField(); //очищаем открытые ячейки
            //Options current = _gameManager.GetCurrentOptions();
            //DrawEmptyGameFieldAdvanced(newOptions.Width, current.Width, newOptions.Height, current.Height);
        }

        /// <summary>
        /// инициация выхода из игры
        /// </summary>
        private void ExitGame(bool closeView)
        {
            _gameManager.SaveCurrentSettings();
            if (closeView)
            {
                _mainView.Close();
            }
        }

        /// <summary>
        /// начальная инициализация представления
        /// </summary>
        private void InitializeMainView()
        {
            _mainView.InitializeImages(CELL_SIZE);
            Options currentOptions = _gameManager.CurrentOptions;
            _mainView.AdjustViewToCellsAmount(currentOptions.Width, currentOptions.Height);
            _mainView.LeftMouseClick += MainView_LeftMouseClickHandler;
            _mainView.RightMouseClick += MainView_RightMouseClickHandler;
            _mainView.NewGameClick += MainView_NewGameClick;
            _mainView.OptionsClick += MainView_OptionsClick;
            _mainView.SaveGameClick += MainView_SaveGameClick;
            _mainView.LoadGameClick += MainView_LoadGameClick;
            _mainView.AboutClick += MainView_AboutClick;
            _mainView.ExitClick += MainView_ExitClick;
            _mainView.DrawEmptyGameField();
            _mainView.MinesLeft = currentOptions.MinesAmount;
            _mainView.ElapsedTime = 0;
        }

        /// <summary>
        /// устанавливает картинки для всего игрового поля (перебор всех ячеек)
        /// </summary>
        /// <param name="cells">ячейки игрового поля</param>
        private void SetImagesInView(Cell[,] cells)
        {
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    switch (cells[i, j].CellValue)
                    {
                        case CellValue.Closed:
                            _mainView.SetClosedCell(i, j);
                            break;
                        case CellValue.One:
                            _mainView.SetOne(i, j);
                            break;
                        case CellValue.Two:
                            _mainView.SetTwo(i, j);
                            break;
                        case CellValue.Three:
                            _mainView.SetThree(i, j);
                            break;
                        case CellValue.Four:
                            _mainView.SetFour(i, j);
                            break;
                        case CellValue.Five:
                            _mainView.SetFive(i, j);
                            break;
                        case CellValue.Six:
                            _mainView.SetSix(i, j);
                            break;
                        case CellValue.Seven:
                            _mainView.SetSeven(i, j);
                            break;
                        case CellValue.Eight:
                            _mainView.SetEight(i, j);
                            break;
                        case CellValue.EmptyOpen:
                            _mainView.SetOpenEmptyCell(i, j);
                            break;
                        case CellValue.Mine:
                            _mainView.SetMine(i, j);
                            break;
                        case CellValue.Exploded:
                            _mainView.SetExploded(i, j);
                            break;
                        case CellValue.WrongFlag:
                            _mainView.SetWrongFlag(i, j);
                            break;
                    }
                }
            };
        }

        /// <summary>
        /// рисует игровое поле с учетом существующего ранее
        /// </summary>
        private void DrawEmptyGameFieldAdvanced(int newWidth, int prevWidth, int newHeight, int prevHeight)
        {
            if (newWidth < prevWidth && newHeight >= prevHeight)               // new width < prev width
            {
                for (int i = newWidth; i < prevWidth; i++)
                {
                    for (int j = 0; j < prevHeight; j++)
                    {
                        _mainView.RemoveCell(i, j);
                    }
                }
            }
            if (newHeight < prevHeight && newWidth >= prevWidth)              // new height < prev height
            {
                for (int i = 0; i < prevWidth; i++)
                {
                    for (int j = newHeight; j < prevHeight; j++)
                    {
                        _mainView.RemoveCell(i, j);
                    }
                }
            }
            if (newWidth > prevWidth && newHeight <= prevHeight)               // new width > prev width
            {
                for (int i = prevWidth; i < newWidth; i++)
                {
                    for (int j = 0; j < prevHeight; j++)
                    {
                        _mainView.CreateAndAddCell(i, j);
                    }
                }
            }
            if (newHeight > prevHeight && newWidth <= prevWidth)               // new height > prev height
            {
                for (int i = 0; i < prevWidth; i++)
                {
                    for (int j = prevHeight; j < newHeight; j++)
                    {
                        _mainView.CreateAndAddCell(i, j);
                    }
                }
            }
            if (newWidth > prevWidth && newHeight > prevHeight)             //new width > prev width and new height > prev height
            {
                for (int i = prevWidth; i < newWidth; i++)
                {
                    for (int j = 0; j < prevHeight; j++)
                    {
                        _mainView.CreateAndAddCell(i, j);
                    }
                }
                for (int i = 0; i < newWidth; i++)
                {
                    for (int j = prevHeight; j < newHeight; j++)
                    {
                        _mainView.CreateAndAddCell(i, j);
                    }
                }
            }
            if (newWidth < prevWidth && newHeight < prevHeight)             //new width < prev width and new height < prev height
            {
                for (int i = newWidth; i < prevWidth; i++)
                {
                    for (int j = 0; j < newHeight; j++)
                    {
                        _mainView.RemoveCell(i, j);
                    }
                }
                for (int i = 0; i < prevWidth; i++)
                {
                    for (int j = newHeight; j < prevHeight; j++)
                    {
                        _mainView.RemoveCell(i, j);
                    }
                }
            }
            //ClearGameFieldSelected();
        }

        private void MainView_LoadGameClick(object sender, EventArgs e)
        {
            Options prevOptions = _gameManager.CurrentOptions;
            int prevWidth = prevOptions.Width;
            int prevHeight = prevOptions.Height;
            GameData gameData;
            try
            {
                gameData = _gameManager.LoadGame();
            }
            catch (FileNotFoundException)
            {
                _messageService.ShowError("Save game not found", "Error");
                return;
            }
            catch (Exception ex)
            {
                _messageService.ShowError(ex.Message, "Error");
                return;
            }
            _gameManager.ContinueSavedGame(gameData);
            _mainView.ElapsedTime = gameData.ElapsedTime;
            _mainView.MinesLeft = gameData.MinesLeft;
            Options currentOptions = gameData.Options;
            if (prevWidth == currentOptions.Width && prevHeight == currentOptions.Height)
            {
                SetImagesInView(gameData.GameField.Cells);
            }
            else //advanced
            {
                _mainView.AdjustViewToCellsAmount(gameData.Options.Width, gameData.Options.Height);
                DrawEmptyGameFieldAdvanced(currentOptions.Width, prevWidth, currentOptions.Height, prevHeight);
                SetImagesInView(gameData.GameField.Cells);
            }
        }
        private void MainView_ExitClick(object sender, ExitGameEventArgs e)
        {
            ExitGame(e.NeedCloseView);
        }
        private void MainView_AboutClick(object sender, EventArgs e)
        {
            _messageService.ShowMessage("Miner Game v.1.2.0.0. \n\nDeveloped by Vlad Galapats.", "About");
        }
        private void MainView_NewGameClick(object sender, EventArgs e)
        {
            WantNewGame();
        }
        private void MainView_SaveGameClick(object sender, EventArgs e)
        {
            _gameManager.SaveCurrentGame();
            _messageService.ShowMessage("The game was successfully saved!", "Save");
        }
        private void MainView_OptionsClick(object sender, EventArgs e)
        {
            _optionsView = _mainView.CreateOptionsView();
            _optionsView.NotCustomGameSelect += OptionsView_NotCustomGameSelect;
            _optionsView.CustomGameSelect += OptionsView_CustomGameSelect;
            _optionsView.ConfirmSelectionClick += OptionsView_ConfirmSelectionClick;
            GameType currentGameType = _gameManager.GetGameType();
            _optionsView.SetSelectedOption(currentGameType);
            if (currentGameType == GameType.Custom)
            {
                Options currentOptions = _gameManager.CurrentOptions;
                _optionsView.FieldWidth = currentOptions.Width;
                _optionsView.FieldHeight = currentOptions.Height;
                _optionsView.Mines = currentOptions.MinesAmount;
                _optionsView.EnableCustomValues(true);
            }
            else
            {
                _optionsView.EnableCustomValues(false);
            }
            _optionsView.ShowDialog();
        }

        private void OptionsView_ConfirmSelectionClick(object sender, MinerLogic.CommonPublic.ChangeOptionsEventArgs e)
        {
            GameType selectedGameType = e.GameType;
            if (selectedGameType == _gameManager.GetGameType() && selectedGameType != GameType.Custom) //настройки не поменялись - ничего не делаем
            {
                _optionsView.Close();
                return;
            }
            Options options = null;
            switch (selectedGameType)
            {
                case GameType.Easy:
                    options = Options.Easy;
                    break;
                case GameType.Medium:
                    options = Options.Medium;
                    break;
                case GameType.Hard:
                    options = Options.Hard;
                    break;
                case GameType.Custom:
                    byte width = _optionsView.FieldWidth;
                    byte height = _optionsView.FieldHeight;
                    short mines = _optionsView.Mines;
                    if (mines > width * height * MINES_CORRECTION)
                    {
                        mines = (short)(width * height * MINES_CORRECTION); //ограничиваем количество мин в зависимости от размеров игрового поля
                    }
                    options = new Options(width, height, mines);
                    if (options == _gameManager.CurrentOptions)
                    {
                        _optionsView.Close();
                        return;
                    }
                    break;
            }
            _optionsView.Close();
            WantNewGameWithChangedOptions(options);
        }
        private void OptionsView_CustomGameSelect(object sender, EventArgs e)
        {
            _optionsView.EnableCustomValues(true);
        }
        private void OptionsView_NotCustomGameSelect(object sender, EventArgs e)
        {
            _optionsView.EnableCustomValues(false);
        }

        /// <summary>
        /// обработчик левого клика мыши
        /// </summary>
        private void MainView_LeftMouseClickHandler(object sender, MinerLogic.MinerPresenter.MouseClickEventArgs e)
        {
            int indexX = e.X / CELL_SIZE; // /16 = >>4 try
            int indexY = e.Y / CELL_SIZE;
            if (!_gameManager.IsClosedCell(indexX, indexY)) // если ячейка уже открыта - ничего не делаем
            {
                return;
            }
            if (_gameManager.IsFlagCell(indexX, indexY)) //ячейка помеченная флагом - ничего не делаем
            {
                return;
            }

            Cell[] changedCells = _gameManager.ApplyLeftClick(indexX, indexY);
            for (int i = 0; i < changedCells.Length; i++)
            {
                switch (changedCells[i].CellValue)
                {
                    case CellValue.One:
                        _mainView.SetOne(changedCells[i].X, changedCells[i].Y);
                        break;
                    case CellValue.Two:
                        _mainView.SetTwo(changedCells[i].X, changedCells[i].Y);
                        break;
                    case CellValue.Three:
                        _mainView.SetThree(changedCells[i].X, changedCells[i].Y);
                        break;
                    case CellValue.Four:
                        _mainView.SetFour(changedCells[i].X, changedCells[i].Y);
                        break;
                    case CellValue.Five:
                        _mainView.SetFive(changedCells[i].X, changedCells[i].Y);
                        break;
                    case CellValue.Six:
                        _mainView.SetSix(changedCells[i].X, changedCells[i].Y);
                        break;
                    case CellValue.Seven:
                        _mainView.SetSeven(changedCells[i].X, changedCells[i].Y);
                        break;
                    case CellValue.Eight:
                        _mainView.SetEight(changedCells[i].X, changedCells[i].Y);
                        break;
                    case CellValue.Mine:
                        _mainView.SetMine(changedCells[i].X, changedCells[i].Y);
                        break;
                    case CellValue.EmptyOpen:
                        _mainView.SetOpenEmptyCell(changedCells[i].X, changedCells[i].Y);
                        break;
                    case CellValue.Exploded:
                        _mainView.SetExploded(changedCells[i].X, changedCells[i].Y);
                        break;
                    case CellValue.WrongFlag:
                        _mainView.SetWrongFlag(changedCells[i].X, changedCells[i].Y);
                        break;
                }
            }
            if (_gameManager.GameState == GameState.Win)
            {
                WinGame();
            }
            else if (_gameManager.GameState == GameState.Loose)
            {
                LooseGame();
            }
        }

        /// <summary>
        /// обработчик правого клика мыши
        /// </summary>
        private void MainView_RightMouseClickHandler(object sender, MinerLogic.MinerPresenter.MouseClickEventArgs e)
        {
            int indexX = e.X / CELL_SIZE; // /16 = >>4 try
            int indexY = e.Y / CELL_SIZE;
            if (!_gameManager.IsClosedCell(indexX, indexY)) // если ячейка уже открыта, ничего не делаем
            {
                return;
            }

            Cell newCell = _gameManager.ApplyRightClick(indexX, indexY);
            switch (newCell.CellValue)
            {
                case CellValue.Closed:
                    _mainView.SetClosedCell(indexX, indexY);
                    break;
                case CellValue.Flag:
                    _mainView.SetFlag(indexX, indexY);
                    break;
                case CellValue.Question:
                    _mainView.SetQuestion(indexX, indexY);
                    break;
                default: //уже открытая ячейка
                    return;
            }
            _mainView.MinesLeft = _gameManager.MinesLeft;
        }

    }
}
