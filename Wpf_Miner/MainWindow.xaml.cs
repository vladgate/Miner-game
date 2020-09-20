using MinerLogic.Interfaces;
using MinerLogic.MinerPresenter;
using MinerPresenter;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wpf_Miner
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window, IMainView
   {
      private readonly int WIDTH_CORRECTION = 17;
      private readonly int HEIGHT_CORRECTION = 106;
      private int _cellSize;
      private int _amountX, _amountY;
      private Image[,] _imagesArray;
      private BitmapImage _cell_closed;
      private BitmapImage _cell_1;
      private BitmapImage _cell_2;
      private BitmapImage _cell_3;
      private BitmapImage _cell_4;
      private BitmapImage _cell_5;
      private BitmapImage _cell_6;
      private BitmapImage _cell_7;
      private BitmapImage _cell_8;
      private BitmapImage _cell_flag;
      private BitmapImage _cell_mine;
      private BitmapImage _cell_empty;
      private BitmapImage _cell_question;
      private BitmapImage _cell_exploded;
      private BitmapImage _cell_wrongFlag;

      public MainWindow()
      {
         InitializeComponent();
         IMessageService messageService = new WpfMessageService();
         Presenter presenter = new Presenter(this, messageService);
         _mainCanvas.MouseUp += MainCanvas_MouseUp;
         _mainCanvas.MouseDown += MainCanvas_MouseDown;
      }

      public event EventHandler<MouseClickEventArgs> LeftMouseClick;
      public event EventHandler<MouseClickEventArgs> RightMouseClick;
      public event EventHandler NewGameClick;
      public event EventHandler SaveGameClick;
      public event EventHandler LoadGameClick;
      public event EventHandler OptionsClick;
      public event EventHandler AboutClick;
      public event EventHandler<ExitGameEventArgs> ExitClick;

      public short MinesLeft
      {
         set => _lblMinesLeft.Content = value.ToString();
      }
      public int ElapsedTime
      {
         set => _lblElapsedTime.Dispatcher.BeginInvoke((Action)(() => _lblElapsedTime.Content = value.ToString()));
      }

      public void AdjustViewToCellsAmount(int amountX, int amountY)
      {
         Image[,] newImagesArray = new Image[amountX, amountY];
         for (int i = 0; i < Math.Min(amountX, _amountX); i++)
         {
            for (int j = 0; j < Math.Min(amountY, _amountY); j++)
            {
               newImagesArray[i, j] = _imagesArray[i, j];
            }
         }
         _amountX = amountX;
         _amountY = amountY;
         _imagesArray = newImagesArray;

         this.Width = _cellSize * amountX + WIDTH_CORRECTION;
         this.Height = _cellSize * amountY + HEIGHT_CORRECTION;
         _mainCanvas.Width = _cellSize * amountX;
         _mainCanvas.Height = _cellSize * amountY;
      }

      public void ClearGameField()
      {
         for (int i = 0; i < _amountX; i++)
         {
            for (int j = 0; j < _amountY; j++)
            {
               _imagesArray[i, j].Source = _cell_closed;
            }
         }
      }

      public void CreateAndAddCell(int indexX, int indexY)
      {
         Image im = new Image();
         im.Name = "x" + indexX + "y" + indexY;
         im.Source = _cell_closed;
         im.Width = _cellSize;
         im.Height = _cellSize;
         im.MouseEnter += Image_MouseEnter;
         im.MouseLeave += Image_MouseLeave;
         Canvas.SetLeft(im, indexX * _cellSize);
         Canvas.SetTop(im, indexY * _cellSize);
         _imagesArray[indexX, indexY] = im;
         _mainCanvas.Children.Add(im);
      }

      private void MainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
      {
         e.Handled = true;
         Point ctrlPoint = e.GetPosition(_mainCanvas);
         if (e.ChangedButton == MouseButton.Left)
         {
            LeftMouseClick?.Invoke(sender, new MouseClickEventArgs((int)ctrlPoint.X, (int)ctrlPoint.Y));
         }
         int x = (int)ctrlPoint.X;
         int y = (int)ctrlPoint.Y;
         _imagesArray[x / _cellSize, y / _cellSize].Opacity = 1;
      }

      private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
      {
         e.Handled = true;
         Point ctrlPoint = e.GetPosition(_mainCanvas);
         if (e.ChangedButton == MouseButton.Right)
         {
            RightMouseClick?.Invoke(sender, new MouseClickEventArgs((int)ctrlPoint.X, (int)ctrlPoint.Y));
         }
         int x = (int)ctrlPoint.X;
         int y = (int)ctrlPoint.Y;
         _imagesArray[x / _cellSize, y / _cellSize].Opacity = 1;
      }

      private void Image_MouseLeave(object sender, MouseEventArgs e)
      {
         (sender as Image).Opacity = 1;
      }

      private void Image_MouseEnter(object sender, RoutedEventArgs e)
      {
         if ((sender as Image).Source == _cell_closed)
         {
            (sender as Image).Opacity = 0.65;
         }
      }

      private void Image_MouseUp(object sender, MouseButtonEventArgs e)
      {
         Point ctrlPoint = (sender as Image).TransformToAncestor(_mainCanvas).Transform(new Point(0, 0));
         if (e.ChangedButton == MouseButton.Left)
         {
            LeftMouseClick?.Invoke(sender, new MouseClickEventArgs((int)ctrlPoint.X, (int)ctrlPoint.Y));
         }
         (sender as Image).Opacity = 1;
      }

      private void Image_MouseDown(object sender, MouseButtonEventArgs e)
      {
         Point ctrlPoint = (sender as Image).TransformToAncestor(_mainCanvas).Transform(new Point(0, 0));
         if (e.ChangedButton == MouseButton.Right)
         {
            RightMouseClick?.Invoke(sender, new MouseClickEventArgs((int)ctrlPoint.X, (int)ctrlPoint.Y));
         }
         (sender as Image).Opacity = 1;
      }

      public IOptionsView CreateOptionsView()
      {
         OptionsWindow optionsView = new OptionsWindow();
         optionsView.Left = this.Left + this.Width / 2 - optionsView.Width / 2;
         optionsView.Top = this.Top + this.Height / 2 - optionsView.Height / 2;
         return optionsView;
      }

      public void DrawEmptyGameField()
      {
         _mainCanvas.Children.Clear();
         for (int i = 0; i < _amountX; i++)
         {
            for (int j = 0; j < _amountY; j++)
            {
               CreateAndAddCell(i, j);
            }
         }
      }

      public void InitializeImages(int cellSize)
      {
         _cellSize = cellSize;

         _cell_closed = new BitmapImage(new Uri("pack://application:,,,/Resources/img_closed.jpg", UriKind.Absolute));
         _cell_closed.DecodePixelWidth = cellSize;
         _cell_closed.DecodePixelHeight = cellSize;

         _cell_1 = new BitmapImage(new Uri("pack://application:,,,/Resources/img_1.jpg", UriKind.Absolute));
         _cell_1.DecodePixelWidth = cellSize;
         _cell_1.DecodePixelHeight = cellSize;

         _cell_2 = new BitmapImage(new Uri("pack://application:,,,/Resources/img_2.jpg", UriKind.Absolute));
         _cell_2.DecodePixelWidth = cellSize;
         _cell_2.DecodePixelHeight = cellSize;

         _cell_3 = new BitmapImage(new Uri("pack://application:,,,/Resources/img_3.jpg", UriKind.Absolute));
         _cell_3.DecodePixelWidth = cellSize;
         _cell_3.DecodePixelHeight = cellSize;

         _cell_4 = new BitmapImage(new Uri("pack://application:,,,/Resources/img_4.jpg", UriKind.Absolute));
         _cell_4.DecodePixelWidth = cellSize;
         _cell_4.DecodePixelHeight = cellSize;

         _cell_5 = new BitmapImage(new Uri("pack://application:,,,/Resources/img_5.jpg", UriKind.Absolute));
         _cell_5.DecodePixelWidth = cellSize;
         _cell_5.DecodePixelHeight = cellSize;

         _cell_6 = new BitmapImage(new Uri("pack://application:,,,/Resources/img_6.jpg", UriKind.Absolute));
         _cell_6.DecodePixelWidth = cellSize;
         _cell_6.DecodePixelHeight = cellSize;

         _cell_7 = new BitmapImage(new Uri("pack://application:,,,/Resources/img_7.jpg", UriKind.Absolute));
         _cell_7.DecodePixelWidth = cellSize;
         _cell_7.DecodePixelHeight = cellSize;

         _cell_8 = new BitmapImage(new Uri("pack://application:,,,/Resources/img_8.jpg", UriKind.Absolute));
         _cell_8.DecodePixelWidth = cellSize;
         _cell_8.DecodePixelHeight = cellSize;

         _cell_flag = new BitmapImage(new Uri("pack://application:,,,/Resources/img_flag.jpg", UriKind.Absolute));
         _cell_flag.DecodePixelWidth = cellSize;
         _cell_flag.DecodePixelHeight = cellSize;

         _cell_mine = new BitmapImage(new Uri("pack://application:,,,/Resources/img_mine.jpg", UriKind.Absolute));
         _cell_mine.DecodePixelWidth = cellSize;
         _cell_mine.DecodePixelHeight = cellSize;

         _cell_empty = new BitmapImage(new Uri("pack://application:,,,/Resources/img_empty.jpg", UriKind.Absolute));
         _cell_empty.DecodePixelWidth = cellSize;
         _cell_empty.DecodePixelHeight = cellSize;

         _cell_question = new BitmapImage(new Uri("pack://application:,,,/Resources/img_question.jpg", UriKind.Absolute));
         _cell_question.DecodePixelWidth = cellSize;
         _cell_question.DecodePixelHeight = cellSize;

         _cell_exploded = new BitmapImage(new Uri("pack://application:,,,/Resources/img_exploded.jpg", UriKind.Absolute));
         _cell_exploded.DecodePixelWidth = cellSize;
         _cell_exploded.DecodePixelHeight = cellSize;

         _cell_wrongFlag = new BitmapImage(new Uri("pack://application:,,,/Resources/img_wrongflag.jpg", UriKind.Absolute));
         _cell_wrongFlag.DecodePixelWidth = cellSize;
         _cell_wrongFlag.DecodePixelHeight = cellSize;
      }

      public void RemoveCell(int indexX, int indexY)
      {
         UIElement el = _mainCanvas.FindName("x" + indexX + "y" + indexY) as Image;
         _mainCanvas.Children.Remove(el);
      }

      public void SetClosedCell(int indexX, int indexY)
      {
         if (indexX > _amountX - 1 || indexY > _amountY - 1)
         {
            return;
         }
         _imagesArray[indexX, indexY].Source = _cell_closed;
      }

      public void SetOne(int indexX, int indexY)
      {
         _imagesArray[indexX, indexY].Source = _cell_1;
      }

      public void SetTwo(int indexX, int indexY)
      {
         _imagesArray[indexX, indexY].Source = _cell_2;
      }

      public void SetThree(int indexX, int indexY)
      {
         _imagesArray[indexX, indexY].Source = _cell_3;
      }

      public void SetFour(int indexX, int indexY)
      {
         _imagesArray[indexX, indexY].Source = _cell_4;
      }

      public void SetFive(int indexX, int indexY)
      {
         _imagesArray[indexX, indexY].Source = _cell_5;
      }

      public void SetSix(int indexX, int indexY)
      {
         _imagesArray[indexX, indexY].Source = _cell_6;
      }

      public void SetSeven(int indexX, int indexY)
      {
         _imagesArray[indexX, indexY].Source = _cell_7;
      }

      public void SetEight(int indexX, int indexY)
      {
         _imagesArray[indexX, indexY].Source = _cell_8;
      }

      public void SetExploded(int indexX, int indexY)
      {
         _imagesArray[indexX, indexY].Source = _cell_exploded;
      }

      public void SetFlag(int indexX, int indexY)
      {
         _imagesArray[indexX, indexY].Source = _cell_flag;
      }

      public void SetMine(int indexX, int indexY)
      {
         _imagesArray[indexX, indexY].Source = _cell_mine;
      }

      public void SetOpenEmptyCell(int indexX, int indexY)
      {
         _imagesArray[indexX, indexY].Source = _cell_empty;
      }

      public void SetQuestion(int indexX, int indexY)
      {
         _imagesArray[indexX, indexY].Source = _cell_question;
      }

      public void SetWrongFlag(int indexX, int indexY)
      {
         _imagesArray[indexX, indexY].Source = _cell_wrongFlag;
      }

      private void Options_Click(object sender, RoutedEventArgs e)
      {
         OptionsClick?.Invoke(sender, e);
      }

      private void LoadGame_Click(object sender, RoutedEventArgs e)
      {
         LoadGameClick?.Invoke(sender, e);
      }

      private void SaveGame_Click(object sender, RoutedEventArgs e)
      {
         SaveGameClick?.Invoke(sender, e);
      }

      private void About_Click(object sender, RoutedEventArgs e)
      {
         AboutClick?.Invoke(sender, e);
      }

      private void Exit_Click(object sender, RoutedEventArgs e)
      {
         ExitClick?.Invoke(sender, new ExitGameEventArgs(true));
      }

      private void NewGame_Click(object sender, RoutedEventArgs e)
      {
         NewGameClick?.Invoke(sender, e);
      }

      private void Form_Closed(object sender, EventArgs e)
      {
         ExitClick?.Invoke(sender, new ExitGameEventArgs(false));
      }
   }
}
