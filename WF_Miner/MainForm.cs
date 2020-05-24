﻿using MinerLogic.Interfaces;
using MinerLogic.MinerPresenter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WF_Miner.Properties;

namespace WF_Miner
{
    public partial class MainForm : Form, IMainView
    {
        private const int WIDTH_CORRECTION = 16;
        private const int HEIGHT_CORRECTION = 85;
        private int _cellSize;
        private int _amountX, _amountY;
        private PictureBox[,] _imagesArray;
        private Bitmap _cell_closed;
        private Bitmap _cell_1;
        private Bitmap _cell_2;
        private Bitmap _cell_3;
        private Bitmap _cell_4;
        private Bitmap _cell_5;
        private Bitmap _cell_6;
        private Bitmap _cell_7;
        private Bitmap _cell_8;
        private Bitmap _cell_flag;
        private Bitmap _cell_mine;
        private Bitmap _cell_empty;
        private Bitmap _cell_question;
        private Bitmap _cell_exploded;
        private Bitmap _cell_wrongFlag;

        public MainForm()
        {
            InitializeComponent();
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            Point ctrlPoint = (sender as Control).Location;
            if (e.Button == MouseButtons.Left)
            {
                LeftMouseClick?.Invoke(sender, new MouseClickEventArgs(ctrlPoint.X, ctrlPoint.Y));
            }
            else if (e.Button == MouseButtons.Right)
            {
                RightMouseClick?.Invoke(sender, new MouseClickEventArgs(ctrlPoint.X, ctrlPoint.Y));
            }
        }


        #region IMainView

        public event EventHandler<MouseClickEventArgs> LeftMouseClick;
        public event EventHandler<MouseClickEventArgs> RightMouseClick;
        public void InitializeImages(int cellSize)
        {
            _cellSize = cellSize;
            Size size = new Size(_cellSize, _cellSize);
            _cell_closed = new Bitmap(Resources.img_closed, size);
            _cell_1 = new Bitmap(Resources.img_1, size);
            _cell_2 = new Bitmap(Resources.img_2, size);
            _cell_3 = new Bitmap(Resources.img_3, size);
            _cell_4 = new Bitmap(Resources.img_4, size);
            _cell_5 = new Bitmap(Resources.img_5, size);
            _cell_6 = new Bitmap(Resources.img_6, size);
            _cell_7 = new Bitmap(Resources.img_7, size);
            _cell_8 = new Bitmap(Resources.img_8, size);
            _cell_flag = new Bitmap(Resources.img_flag, size);
            _cell_mine = new Bitmap(Resources.img_mine, size);
            _cell_empty = new Bitmap(Resources.img_empty, size);
            _cell_question = new Bitmap(Resources.img_question, size);
            _cell_exploded = new Bitmap(Resources.img_exploded, size);
            _cell_wrongFlag = new Bitmap(Resources.img_wrongflag, size);
        }

        public void AdjustViewToCellsAmount(int amountX, int amountY)
        {
            _amountX = amountX;
            _amountY = amountY;

            this.Width = _cellSize * amountX + WIDTH_CORRECTION;
            this.Height = _cellSize * amountY + HEIGHT_CORRECTION;
            _mainPanel.Width = _cellSize * amountX;
            _mainPanel.Height = _cellSize * amountY;
        }

        public void DrawEmptyGameField()
        {
            _imagesArray = new PictureBox[_amountX, _amountY];

            Size size = new Size(_cellSize, _cellSize);
            for (int i = 0; i < _amountX; i++)
            {
                for (int j = 0; j < _amountY; j++)
                {
                    PictureBox pb = new PictureBox();
                    pb.Size = size;
                    pb.Location = new Point(i * _cellSize, j * _cellSize);
                    pb.Image = _cell_closed;
                    pb.Name = i + "_" + j;
                    pb.MouseDown += PictureBox_MouseDown;
                    _imagesArray[i, j] = pb;
                    _mainPanel.Controls.Add(pb);
                }
            }
        }

        public void ClearGameField()
        {
            for (int i = 0; i < _amountX; i++)
            {
                for (int j = 0; j < _amountY; j++)
                {
                    _imagesArray[i, j].Image = _cell_closed;
                }
            }
        }

        public void SetClosedCell(int indexX, int indexY)
        {
            _imagesArray[indexX,indexY].Image = _cell_closed;
        }

        public void SetOpenEmptyCell(int indexX, int indexY)
        {
            _imagesArray[indexX, indexY].Image = _cell_empty;
        }

        public void SetFlag(int indexX, int indexY)
        {
            _imagesArray[indexX, indexY].Image = _cell_flag;
        }

        public void SetQuestion(int indexX, int indexY)
        {
            _imagesArray[indexX, indexY].Image = _cell_question;
        }

        public void SetWrongFlag(int indexX, int indexY)
        {
            _imagesArray[indexX, indexY].Image = _cell_wrongFlag;
        }

        public void SetOne(int indexX, int indexY)
        {
            _imagesArray[indexX, indexY].Image = _cell_1;
        }

        public void SetTwo(int indexX, int indexY)
        {
            _imagesArray[indexX, indexY].Image = _cell_2;
        }

        public void SetThree(int indexX, int indexY)
        {
            _imagesArray[indexX, indexY].Image = _cell_3;
        }

        public void SetFour(int indexX, int indexY)
        {
            _imagesArray[indexX, indexY].Image = _cell_4;
        }

        public void SetFive(int indexX, int indexY)
        {
            _imagesArray[indexX, indexY].Image = _cell_5;
        }

        public void SetSix(int indexX, int indexY)
        {
            _imagesArray[indexX, indexY].Image = _cell_6;
        }

        public void SetSeven(int indexX, int indexY)
        {
            _imagesArray[indexX, indexY].Image = _cell_7;
        }

        public void SetEight(int indexX, int indexY)
        {
            _imagesArray[indexX, indexY].Image = _cell_8;
        }

        public void SetExploded(int indexX, int indexY)
        {
            _imagesArray[indexX, indexY].Image = _cell_exploded;
        }

        public void SetMine(int indexX, int indexY)
        {
            _imagesArray[indexX, indexY].Image = _cell_mine;
        }

        #endregion

    }
}
