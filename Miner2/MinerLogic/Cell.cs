using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinerLogic
{
    public struct Cell
    {
        public CellValue CellValue;
        public bool IsMine;
        public Cell(CellValue cellValue, bool isMine)
        {
            this.CellValue = cellValue;
            this.IsMine = isMine;
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
        FrongFlag = 0x1000,
        Mine = 0x2000
    }
}
