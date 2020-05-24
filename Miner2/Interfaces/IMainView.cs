using MinerLogic.MinerPresenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Interfaces
{
    public interface IMainView
    {
        event EventHandler<MouseClickEventArgs> LeftMouseClick;
        event EventHandler<MouseClickEventArgs> RightMouseClick;

        /// <summary>
        /// инициализирует картинки ячеек с учетом размера ячейки )ширина=высота)
        /// </summary>
        void InitializeImages(int cellSize);

        /// <summary>
        /// подгоняет размеры формы и игрового поля в зависимости от количества ячеек по горизонтали и вертикали
        /// </summary>
        void AdjustViewToCellsAmount(int amountX, int amountY);

        /// <summary>
        /// отрисовывает начальное игровое поле с закрытыми ячейками
        /// </summary>
        void DrawEmptyGameField();

        /// <summary>
        /// рисует закрытую ячейку по указанному расположению 
        /// </summary>
        void SetClosedCell(int indexX, int indexY);

        /// <summary>
        /// рисует открытую пустую ячейку по указанному расположению 
        /// </summary>
        void SetOpenEmptyCell(int indexX, int indexY);
        void SetFlagCell(int indexX, int indexY);
        void SetQuestionCell(int indexX, int indexY);
    }
}
