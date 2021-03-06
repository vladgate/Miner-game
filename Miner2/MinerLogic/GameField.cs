﻿using System;
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
      private bool _minesPlaced;

      public int Width { get => _width; }
      public int Height { get => _height; }
      public int Mines { get => _mines; }
      public Cell[,] Cells { get => _cells; }
      public bool MinesPlaced
      {
         get => _minesPlaced;
         private set => _minesPlaced = value;
      }

      public GameField(int width, int height, int mines)
      {
         _width = width;
         _height = height;
         _mines = mines;
         _cells = new Cell[_width, _height];
      }

      /// <summary>
      /// fills array of cells with initial values
      /// </summary>
      /// <returns>array of cells</returns>
      public Cell[,] InitCellArray()
      {
         for (byte i = 0; i < _width; i++)
         {
            for (byte j = 0; j < _height; j++)
            {
               _cells[i, j] = new Cell(CellValue.Closed, false, i, j);
            }
         }
         _minesPlaced = false;
         return _cells;
      }

      /// <summary>
      /// initial placing mines on he gamefield
      /// </summary>
      /// <param name="busyX">x-index of first (after game start) clicked cell</param>
      /// <param name="busyY">y-index of first (after game start) clicked cell</param>
      public void PlaceMines(int busyX, int busyY)
      {
         Random rnd = new Random();
         int posX, posY, alreadyMinesPlaced = 0;
         while (_mines > alreadyMinesPlaced)
         {
            posX = rnd.Next(_width);
            posY = rnd.Next(_height);

            //clicked on the firts clicked cell
            if (posX == busyX && posY == busyY)
               continue;

            if (!_cells[posX, posY].IsMine)
            {
               _cells[posX, posY].IsMine = true;
               alreadyMinesPlaced++;
            }
         }
         _minesPlaced = true;
      }

      /// <summary>
      /// if only one cell left - player win else the game continues
      /// </summary>
      /// <returns>is only one cell left</returns>
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
