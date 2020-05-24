﻿using MinerLogic.MinerPresenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinerLogic.Interfaces
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
        /// инициализирует и отрисовывает начальное игровое поле с закрытыми ячейками
        /// </summary>
        void DrawEmptyGameField();

        /// <summary>
        /// отрисовывает начальное игровое поле с закрытыми ячейками без повторной инициализации
        /// </summary>
        void ClearGameField();

        /// <summary>
        /// закрытие приложения
        /// </summary>
        void Close();

        /// <summary>
        /// рисует закрытую ячейку по указанному расположению 
        /// </summary>
        void SetClosedCell(int indexX, int indexY);

        /// <summary>
        /// рисует открытую пустую ячейку по указанному расположению 
        /// </summary>
        void SetOpenEmptyCell(int indexX, int indexY);
        
        /// <summary>
        /// рисует ячейку с пометкой (флагом) 
        /// </summary>
        void SetFlag(int indexX, int indexY);

        /// <summary>
        /// рисует ячейку с ошибочной пометкой (флагом) 
        /// </summary>
        void SetWrongFlag(int indexX, int indexY);

        /// <summary>
        /// рисует ячейку с пометкой-знаком вопроса
        /// </summary>
        void SetQuestion(int indexX, int indexY);

        /// <summary>
        /// рисует ячейку с единицей
        /// </summary>
        void SetOne(int indexX, int indexY);

        /// <summary>
        /// рисует ячейку с двойкой
        /// </summary>
        void SetTwo(int indexX, int indexY);

        /// <summary>
        /// рисует ячейку с тройокй
        /// </summary>
        void SetThree(int indexX, int indexY);

        /// <summary>
        /// рисует ячейку с четверкой
        /// </summary>
        void SetFour(int indexX, int indexY);

        /// <summary>
        /// рисует ячейку с пятеркой
        /// </summary>
        void SetFive(int indexX, int indexY);

        /// <summary>
        /// рисует ячейку с шестеркой
        /// </summary>
        void SetSix(int indexX, int indexY);

        /// <summary>
        /// рисует ячейку с семеркой
        /// </summary>
        void SetSeven(int indexX, int indexY);

        /// <summary>
        /// рисует ячейку с восьмеркой
        /// </summary>
        void SetEight(int indexX, int indexY);

        /// <summary>
        /// рисует ячейку со взорваной миной
        /// </summary>
        void SetExploded(int indexX, int indexY);

        /// <summary>
        /// рисует ячейку с миной
        /// </summary>
        void SetMine(int indexX, int indexY);
    }
}