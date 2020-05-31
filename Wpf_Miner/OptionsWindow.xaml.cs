using MinerLogic.CommonPublic;
using MinerLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Wpf_Miner
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window, IOptionsView
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
        public OptionsWindow()
        {
            InitializeComponent();
        }
        public event EventHandler NotCustomGameSelect;
        public event EventHandler CustomGameSelect;
        public event EventHandler<ChangeOptionsEventArgs> ConfirmSelectionClick;
        public short Mines
        {
            get => short.Parse(_txtBoxAmountMines.Text);
            set => _txtBoxAmountMines.Text = value.ToString();
        }
        public byte FieldWidth
        {
            get => byte.Parse(_txtWidth.Text);
            set => _txtWidth.Text = value.ToString();
        }
        public byte FieldHeight
        {
            get => byte.Parse(_txtHeight.Text);
            set => _txtHeight.Text = value.ToString();
        }

        public void EnableCustomValues(bool enable)
        {
            _txtBoxAmountMines.IsEnabled = enable;
            _txtWidth.IsEnabled = enable;
            _txtHeight.IsEnabled = enable;
        }

        public void SetSelectedOption(GameType currentGameType)
        {
            switch (currentGameType)
            {
                case GameType.Easy:
                    _radioBtnEasy.IsChecked = true;
                    break;
                case GameType.Medium:
                    _radioBtnMedium.IsChecked = true;
                    break;
                case GameType.Hard:
                    _radioBtnHard.IsChecked = true;
                    break;
                case GameType.Custom:
                    _radioBtnCustom.IsChecked = true;
                    break;
            };
        }

        void IOptionsView.ShowDialog()
        {
            this.ShowDialog();
        }

        private void RadioBtnCustom_Click(object sender, RoutedEventArgs e)
        {
            CustomGameSelect?.Invoke(sender, e);
        }

        private void RadioBtNotCustom_Click(object sender, RoutedEventArgs e)
        {
            NotCustomGameSelect?.Invoke(sender, e);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            GameType currentGameType;
            if (_radioBtnEasy.IsChecked.HasValue && _radioBtnEasy.IsChecked.Value)
            {
                currentGameType = GameType.Easy;
            }
            else if (_radioBtnMedium.IsChecked.HasValue && _radioBtnMedium.IsChecked.Value)
            {
                currentGameType = GameType.Medium;
            }
            else if (_radioBtnHard.IsChecked.HasValue && _radioBtnHard.IsChecked.Value)
            {
                currentGameType = GameType.Hard;
            }
            else
            {
                currentGameType = GameType.Custom;
            }

            ConfirmSelectionClick?.Invoke(sender, new ChangeOptionsEventArgs(currentGameType));

        }

        private void PreviewTxtBox_KeyDown(object sender, KeyEventArgs e)
        {
            bool FirstZero() //проверяем, есть ли ноль первой цифрой
            {
                if ((e.Key == Key.D0 || e.Key == Key.NumPad0) && ((TextBox)sender).Text == "")
                {
                    return true;
                }
                return false;
            }

            bool TextTooMuchLong()
            {
                if (((TextBox)sender).Text.Length >= 4)
                {
                    return true;
                }
                return false;
            }

            if (e.Key == Key.Back)
            {
                return;
            }
            if (FirstZero() || TextTooMuchLong() || !IsDigit(e.Key))
            {
                e.Handled = true;
            }
        }

        private bool IsDigit(Key key)
        {
            int intKey = (int)key;
            if ((intKey >= 34 && intKey <= 43) || (intKey >= 74 && intKey <= 83))
            {
                return true;
            }
            return false;
        }

        private void _txtBoxAmountMines_GotFocus(object sender, RoutedEventArgs e)
        {
            _initMines = Mines;
        }

        private void _txtWidth_GotFocus(object sender, RoutedEventArgs e)
        {
            _initWidth = FieldWidth;
        }

        private void _txtHeight_GotFocus(object sender, RoutedEventArgs e)
        {
            _initHeigth = FieldHeight;
        }

        private void _txtBoxAmountMines_LostFocus(object sender, RoutedEventArgs e)
        {
            bool success = short.TryParse(_txtBoxAmountMines.Text, out short mines);
            if (!success || mines < MIN_MINES || mines > MAX_MINES)
            {
                _txtBoxAmountMines.Text = _initMines.ToString();
            }
            else
            {
                _txtBoxAmountMines.Text = mines.ToString();
            }
        }

        private void _txtWidth_LostFocus(object sender, RoutedEventArgs e)
        {
            bool success = byte.TryParse(_txtWidth.Text, out byte width);
            if (!success || width < MIN_WIDTH || width > MAX_WIDTH)
            {
                _txtWidth.Text = _initWidth.ToString();
            }
            else
            {
                _txtWidth.Text = width.ToString();
            }
        }

        private void _txtHeight_LostFocus(object sender, RoutedEventArgs e)
        {
            bool success = byte.TryParse(_txtHeight.Text, out byte height);
            if (!success || height < MIN_HEIGHT || height > MAX_HEIGHT)
            {
                _txtHeight.Text = _initHeigth.ToString();
            }
            else
            {
                _txtHeight.Text = height.ToString();
            }

        }
    }
}
