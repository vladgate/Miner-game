using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Interfaces;
using MinerLogic;

namespace MinerPresenter
{
    public sealed class Presenter
    {
        const int CELL_SIZE = 16;

        IMainView _mainView;
        GameManager _gameManager;

        public Presenter(IMainView mainView)
        {
            _mainView = mainView;
            _gameManager = GameManager.Instance;
            InitializeMainView();
        }

        private void InitializeMainView()
        {
            _mainView.InitializeImages(CELL_SIZE);
            Options currentOptions = _gameManager.GetCurrentOptions();
            _mainView.AdjustViewToCellsAmount(currentOptions.Width, currentOptions.Height);
            _mainView.LeftMouseClick += MainView_LeftMouseClickHandler;
            _mainView.RightMouseClick += MainView_RightMouseClickHandler;
        }

        private void MainView_LeftMouseClickHandler(object sender, MinerLogic.MinerPresenter.MouseClickEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void MainView_RightMouseClickHandler(object sender, MinerLogic.MinerPresenter.MouseClickEventArgs e)
        {
            int indexX = e.X / CELL_SIZE;
            int indexY = e.Y / CELL_SIZE;
            CellValue currentCellValue = _gameManager.GetCurrentCellState(indexX, indexY);
            switch (currentCellValue)
            {
                case CellValue.Closed:
                    _gameManager.SetCellValue(indexX, indexY, CellValue.Flag);
                    _mainView.SetFlagCell(indexX, indexY);
                    break;
                case CellValue.Flag:
                    _gameManager.SetCellValue(indexX, indexY, CellValue.Question);
                    _mainView.SetQuestionCell(indexX, indexY);
                    break;
                case CellValue.Question:
                    _gameManager.SetCellValue(indexX, indexY, CellValue.Closed);
                    _mainView.SetClosedCell(indexX, indexY);
                    break;
                default: //уже открытая ячейка
                    return;

            }

        }

    }
}
