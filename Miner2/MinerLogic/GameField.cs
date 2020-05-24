using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinerLogic
{
    [Serializable]
    internal sealed class GameField
    {
        private readonly Cell[,] _cells;
        private readonly int _width;
        private readonly int _height;
        private readonly int _mines;

        public int Width { get =>_width; }
        public int Height { get => _height; }
        public int Mines { get => _mines; }
        public Cell[,] Cells { get => _cells; }

        public GameField(int width, int height, int mines)
        {
            //исключение если некорректные опции


            _width = width;
            _height = height;
            _mines = mines;
            _cells = new Cell[_width, _height];
        }

        /// <summary>
        /// очищает ячейки, заполняет массив ячеек начальными значениями
        /// </summary>
        /// <returns></returns>
        public Cell[,] Clear()
        {
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    _cells[i, j].IsMine = false;
                    _cells[i, j].CellValue = CellValue.Closed;
                }
            }
            return _cells;
        }

        /// <summary>
        /// начальное размещение мин на поле
        /// </summary>
        /// <param name="busyX">Индекс ячейки по горизонтали, на которую первую кликнули после начала новой игры</param>
        /// <param name="busyY">Индекс ячейки по вертикали, на которую первую кликнули после начала новой игры</param>
        public void PlaceMines(int busyX, int busyY)
        {
            Random rnd = new Random();
            int posX, posY, alreadyMinesPlaced = 0;
            while (_mines > alreadyMinesPlaced)
            {
                posX = rnd.Next(_width);
                posY = rnd.Next(_height);

                //попали на ячейку, которую первой кликнул игрок
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
        /// показывает, осталась ли только одна неоткрытая ячейка - если да, то игрок выиграл, иначе - игра продолжается
        /// </summary>
        /// <returns></returns>
        public bool IsLastCellLeft()
        {
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    if (!_cells[i, j].IsMine && (_cells[i, j].CellValue == CellValue.Closed || _cells[i, j].CellValue == CellValue.Question || _cells[i, j].CellValue == CellValue.Flag))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
