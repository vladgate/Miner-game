using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinerLogic
{
    [Serializable]
    public struct Cell
    {
        public CellValue CellValue;
        public bool IsMine;
        public byte X, Y; // для сохранения расположение в массиве
        public Cell(CellValue cellValue, bool isMine, byte x, byte y)
        {
            this.CellValue = cellValue;
            this.IsMine = isMine;
            X = x;
            Y = y;
        }
        public override int GetHashCode()
        {
            return X<<4^Y;
        }
    }

    [Flags]
    public enum CellValue
    {
        Closed = 0x0000,
        EmptyOpen = 0x0001,
        One = 0x0002,
        Two = 0x0004,
        Three = 0x0008,
        Four = 0x0010,
        Five = 0x0020,
        Six = 0x0040,
        Seven = 0x0080,
        Eight = 0x0100,
        Exploded = 0x0200,
        Question = 0x0400,
        Flag = 0x0800,
        WrongFlag = 0x1000,
        Mine = 0x2000
    }
}
