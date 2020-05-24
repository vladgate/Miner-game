using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinerLogic
{
    [Serializable]
    internal struct Options
    {
        private int _width;
        private int _height;
        private int _minesAmount;

        /// <summary>
        /// возращает настройки игры простой сложности
        /// </summary>
        public static Options Easy
        {
            get { return new Options { _width = 9, _height = 9, _minesAmount = 10 }; }
        }

        /// <summary>
        /// возращает настройки игры средней сложности
        /// </summary>
        public static Options Medium
        {
            get { return new Options { _width = 16, _height = 16, _minesAmount = 40 }; }
        }

        /// <summary>
        /// возращает настройки игры тяжелой сложности
        /// </summary>
        public static Options Hard
        {
            get { return new Options { _width = 30, _height = 16, _minesAmount = 99 }; }
        }

        public Options(int width, int height, int minesAmount)
        {
            _width = width;
            _height = height;
            _minesAmount = minesAmount;
        }

        public override int GetHashCode()
        {
            return (_width << 4 + _minesAmount) ^ _height;
        }
        public override bool Equals(object obj)
        {
            if (obj is Options other)
            {
                return Equals(ref other);
            }
            return false;
        }
        public bool Equals(ref Options other)
        {
            return this._width == other._width && this._height == other._height && this._minesAmount == other._minesAmount;
        }
        public static bool operator ==(Options left, Options right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Options left, Options right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// возвращает количество ячеек по горизонтали
        /// </summary>
        public int Width
        {
            get { return _width; }
        }

        /// <summary>
        /// возвращает количество ячеек по вертикали
        /// </summary>
        public int Height
        {
            get { return _height; }
        }

        /// <summary>
        /// возвращает количество мин на игровом поле
        /// </summary>
        public int MinesAmount
        {
            get { return _minesAmount; }
        }

    }
}
