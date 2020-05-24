using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using MinerLogic;
using MinerLogic.Interfaces;

namespace MinerPresenter
{
    public sealed class Presenter
    {
        const int CELL_SIZE = 16;

        IMainView _mainView;
        IMessageService _messageService;
        GameManager _gameManager;

        public Presenter(IMainView mainView, IMessageService messageService)
        {
            _mainView = mainView;
            _messageService = messageService;
            _gameManager = GameManager.Instance;
            _gameManager.StartNewGame();
            InitializeMainView();
        }

        private void LooseGameMessage()
        {
            bool wantNewGame = _messageService.LooseGame("Вы проиграли! Начать новую игру?", "Проигрыш!");
            ProposeNewGame(wantNewGame);
        }

        private void WinGameMessage()
        {
            bool wantNewGame = _messageService.LooseGame("Вы выиграли! Начать новую игру?", "Выигрыш!");
            ProposeNewGame(wantNewGame);
        }

        private void ProposeNewGame(bool wantNewGame)
        {
            if (wantNewGame)
            {
                _mainView.ClearGameField();
                _gameManager.StartNewGame();
            }
            else
            {
                _gameManager.SaveCurrentSettings();
                _mainView.Close();
            }
        }

        /// <summary>
        /// начальная инициализация представления
        /// </summary>
        private void InitializeMainView()
        {
            _mainView.InitializeImages(CELL_SIZE);
            Options currentOptions = _gameManager.GetCurrentOptions();
            _mainView.AdjustViewToCellsAmount(currentOptions.Width, currentOptions.Height);
            _mainView.LeftMouseClick += MainView_LeftMouseClickHandler;
            _mainView.RightMouseClick += MainView_RightMouseClickHandler;
            _mainView.DrawEmptyGameField();
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
                switch(changedCells[i].CellValue)
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
                WinGameMessage();
            }
            else if (_gameManager.GameState == GameState.Loose)
            {
                LooseGameMessage();
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

        }

    }
}
