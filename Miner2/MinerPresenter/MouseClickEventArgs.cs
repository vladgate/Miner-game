using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinerLogic.MinerPresenter
{
    public sealed class MouseClickEventArgs : EventArgs
    {
        /// <summary>
        /// класс для инкапсуляции координат нажатия мышки относительно всего игрового поля
        /// </summary>
        /// <param name="x">x-ккордината в пределах всего игрового поля</param>
        /// <param name="y">y-ккордината в пределах всего игрового поля</param>
        public MouseClickEventArgs(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

    }

    //public enum MouseButton
    //{
    //    Left = 0,
    //    Right = 1
    //}
}
