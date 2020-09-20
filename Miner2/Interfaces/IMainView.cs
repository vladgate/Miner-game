using MinerLogic.CommonPublic;
using MinerLogic.MinerPresenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinerLogic.Interfaces
{
   public interface IMainView
   {
      short MinesLeft { set; }
      int ElapsedTime { set; }

      event EventHandler<MouseClickEventArgs> LeftMouseClick;
      event EventHandler<MouseClickEventArgs> RightMouseClick;
      event EventHandler NewGameClick;
      event EventHandler SaveGameClick;
      event EventHandler LoadGameClick;
      event EventHandler OptionsClick;
      event EventHandler AboutClick;
      event EventHandler<ExitGameEventArgs> ExitClick;

      /// <summary>
      /// init images of cells according to specified cell size (width=height)
      /// </summary>
      void InitializeImages(int cellSize);

      /// <summary>
      /// adjust view size and gamefield according to amount of cells horizontally and vertically
      /// </summary>
      void AdjustViewToCellsAmount(int amountX, int amountY);

      void CreateAndAddCell(int indexX, int indexY);
      void RemoveCell(int indexX, int indexY);

      /// <summary>
      /// init and draw initial empty gamefield with closed cells
      /// </summary>
      void DrawEmptyGameField();

      /// <summary>
      /// clear opened cell - draw initial empty gamefield with closed cells without repeated initialization
      /// </summary>
      void ClearGameField();

      /// <summary>
      /// close view and app
      /// </summary>
      void Close();

      /// <summary>
      /// draw closed cell at specified index
      /// </summary>
      void SetClosedCell(int indexX, int indexY);

      /// <summary>
      /// draw opened cell at specified index
      /// </summary>
      void SetOpenEmptyCell(int indexX, int indexY);

      /// <summary>
      /// draw cell with flag at specified index
      /// </summary>
      void SetFlag(int indexX, int indexY);

      /// <summary>
      /// draw cell with wrong flag at specified index
      /// </summary>
      void SetWrongFlag(int indexX, int indexY);

      /// <summary>
      /// draw cell with question mark at specified index
      /// </summary>
      void SetQuestion(int indexX, int indexY);

      /// <summary>
      /// draw cell with '1' at specified index
      /// </summary>
      void SetOne(int indexX, int indexY);

      /// <summary>
      /// draw cell with '2' at specified index
      /// </summary>
      void SetTwo(int indexX, int indexY);

      /// <summary>
      /// draw cell with '3' at specified index
      /// </summary>
      void SetThree(int indexX, int indexY);

      /// <summary>
      /// draw cell with '4' at specified index
      /// </summary>
      void SetFour(int indexX, int indexY);

      /// <summary>
      /// draw cell with '5' at specified index
      /// </summary>
      void SetFive(int indexX, int indexY);

      /// <summary>
      /// draw cell with '6' at specified index
      /// </summary>
      void SetSix(int indexX, int indexY);

      /// <summary>
      /// draw cell with '7' at specified index
      /// </summary>
      void SetSeven(int indexX, int indexY);

      /// <summary>
      /// draw cell with '8' at specified index
      /// </summary>
      void SetEight(int indexX, int indexY);

      /// <summary>
      /// draw cell with exploded image at specified index
      /// </summary>
      void SetExploded(int indexX, int indexY);

      /// <summary>
      /// draw cell with mine at specified index
      /// </summary>
      void SetMine(int indexX, int indexY);

      /// <summary>
      /// creates options view
      /// </summary>
      /// <returns>new options view</returns>
      IOptionsView CreateOptionsView();
   }
}
