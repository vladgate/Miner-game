using MinerLogic.CommonPublic;
using MinerLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WF_Miner
{
    public partial class OptionsForm : Form, IOptionsView
    {
        private short _initMines;
        private byte _initWidth;
        private byte _initHeigth;
        private const short MIN_MINES = 10;
        private const short MAX_MINES = 777;
        private const byte MIN_WIDTH = 9;
        private const byte MAX_WIDTH = 65;
        private const byte MIN_HEIGHT = 9;
        private const byte MAX_HEIGHT = 50;
        public OptionsForm()
        {
            InitializeComponent();
        }
        public event EventHandler NotCustomGameSelect;
        public event EventHandler CustomGameSelect;
        public event EventHandler<ChangeOptionsEventArgs> ConfirmSelectionClick;
        public short Mines
        {
            get => short.Parse(txtBoxAmountMines.Text);
            set => txtBoxAmountMines.Text = value.ToString();
        }
        public byte FieldWidth
        {
            get => byte.Parse(txtBoxWidth.Text);
            set => txtBoxWidth.Text = value.ToString();
        }
        public byte FieldHeight
        {
            get => byte.Parse(txtBoxHeight.Text);
            set => txtBoxHeight.Text = value.ToString();
        }

        public void EnableCustomValues(bool enable)
        {
            txtBoxAmountMines.Enabled = enable;
            txtBoxHeight.Enabled = enable;
            txtBoxWidth.Enabled = enable;
        }

        private void RadioBtNotCustom_Click(object sender, EventArgs e)
        {
            NotCustomGameSelect?.Invoke(sender, e);
        }

        private void RadioBtnCustom_Click(object sender, EventArgs e)
        {
            CustomGameSelect?.Invoke(sender, e);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            GameType currentGameType;
            if (radioBtnEasy.Checked)
            {
                currentGameType = GameType.Easy;
            }
            else if (radioBtnMedium.Checked)
            {
                currentGameType = GameType.Medium;
            }
            else if (radioBtnHard.Checked)
            {
                currentGameType = GameType.Hard;
            }
            else
            {
                currentGameType = GameType.Custom;
            }

            ConfirmSelectionClick?.Invoke(sender, new ChangeOptionsEventArgs(currentGameType));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void SetSelectedOption(GameType currentGameType)
        {
            switch (currentGameType)
            {
                case GameType.Easy:
                    radioBtnEasy.Checked = true;
                    break;
                case GameType.Medium:
                    radioBtnMedium.Checked = true;
                    break;
                case GameType.Hard:
                    radioBtnHard.Checked = true;
                    break;
                case GameType.Custom:
                    radioBtnCustom.Checked = true;
                    break;
            };
        }

        private void txtBoxAmountMines_Enter(object sender, EventArgs e)
        {
            _initMines = Mines;
        }

        private void txtBoxHeight_Enter(object sender, EventArgs e)
        {
            _initHeigth = FieldHeight;
        }

        private void txtBoxWidth_Enter(object sender, EventArgs e)
        {
            _initWidth = FieldWidth;
        }

        private void txtBoxAmountMines_Leave(object sender, EventArgs e)
        {
            short mines = short.Parse(txtBoxAmountMines.Text);
            if (mines < MIN_MINES || mines > MAX_MINES)
            {
                txtBoxAmountMines.Text = _initMines.ToString();
            }
        }

        private void txtBoxHeight_Leave(object sender, EventArgs e)
        {
            byte height = byte.Parse(txtBoxHeight.Text);
            if (height < MIN_HEIGHT || height > MAX_HEIGHT)
            {
                txtBoxHeight.Text = _initHeigth.ToString();
            }
        }

        private void txtBoxWidth_Leave(object sender, EventArgs e)
        {
            byte width = byte.Parse(txtBoxWidth.Text);
            if (width < MIN_WIDTH || width > MAX_WIDTH)
            {
                txtBoxWidth.Text = _initWidth.ToString();
            }
        }

        private void txtBoxAmountMines_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b')
            {
                return;
            }
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        void IOptionsView.ShowDialog()
        {
            this.ShowDialog();
        }
    }
}
