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

namespace MojiCollaTool
{
    /// <summary>
    /// CanvasEditWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class CanvasEditWindow : Window
    {
        /// <summary>
        /// キャンバスデータ
        /// </summary>
        public CanvasData CanvasData { get; set; }

        private MainWindow? _mainWindow;

        private bool _runEvent = true;

        public CanvasEditWindow()
        {
            CanvasData = new CanvasData();

            InitializeComponent();
        }

        public CanvasEditWindow(CanvasData canvasData, MainWindow mainWindow)
        {
            CanvasData = canvasData;
            _mainWindow = mainWindow;

            InitializeComponent();

            UpdateView();
        }

        /// <summary>
        /// キャンバスデータを画面に反映する
        /// </summary>
        /// <param name="canvasData"></param>
        private void UpdateView()
        {
            _runEvent = false;

            if (CanvasData.ImageData2.IsNullData())
            {
                MultiImageLocateGroupBox.IsEnabled = false;
                Image2Rect.Visibility = Visibility.Hidden;
            }
            else
            {
                MultiImageLocateGroupBox.IsEnabled = true;
                Image2Rect.Visibility = Visibility.Visible;
                switch (CanvasData.Image2LocatePosition)
                {
                    case LocatePosition.Left:
                        Image2Rect.SetValue(Grid.RowProperty, 1);
                        Image2Rect.SetValue(Grid.ColumnProperty, 0);
                        break;
                    case LocatePosition.Right:
                        Image2Rect.SetValue(Grid.RowProperty, 1);
                        Image2Rect.SetValue(Grid.ColumnProperty, 2);
                        break;
                    case LocatePosition.Top:
                        Image2Rect.SetValue(Grid.RowProperty, 0);
                        Image2Rect.SetValue(Grid.ColumnProperty, 1);
                        break;
                    case LocatePosition.Bottom:
                        Image2Rect.SetValue(Grid.RowProperty, 2);
                        Image2Rect.SetValue(Grid.ColumnProperty, 1);
                        break;
                    default:
                        break;
                }
            }

            ImageWidthHeighTextBlock.Text = $"←横→:{CanvasData.ImageWidth}px\r\n↑縦↓:{CanvasData.ImageHeight}px";

            CanvasWidthTextBox.Text = CanvasData.CanvasWidth.ToString();
            CanvasHeightTextBox.Text = CanvasData.CanvasHeight.ToString();

            TopTextBox.SetValue((int)CanvasData.ImageMarginTop, false);
            BottomTextBox.SetValue((int)CanvasData.ImageMarginBottom, false);
            LeftTextBox.SetValue((int)CanvasData.ImageMarginLeft, false);
            RightTextBox.SetValue((int)CanvasData.ImageMarginRight, false);

            CanvasColorButton.Background = new SolidColorBrush(CanvasData.CanvasColor);

            _runEvent = true;
        }

        private void Image2LocateButton(object sender, RoutedEventArgs e)
        {
            CanvasData.Image2LocatePosition = Enum.Parse<LocatePosition>((string)((Button)sender).Tag);

            UpdateView();

            CanvasData.ModifyImageSize();

            CanvasData.UpdateCanvasSize();

            _mainWindow?.UpdateCanvas();
        }

        private Color GetCanvasColorButtonColor()
        {
            return ((SolidColorBrush)CanvasColorButton.Background).Color;
        }

        private void CanvasColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorSelector.ColorSelectorWindow colorSelectorWindow = new ColorSelector.ColorSelectorWindow(GetCanvasColorButtonColor(), (color) =>
            {
                CanvasData.CanvasColor = color;
                _mainWindow?.UpdateCanvas();
            });
            colorSelectorWindow.Top = Top;
            colorSelectorWindow.Left = Left;
            colorSelectorWindow.Topmost = Topmost;
            var dialogResult = colorSelectorWindow.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
            {
                CanvasColorButton.Background = new SolidColorBrush(colorSelectorWindow.NextBrush.Color);
            }
            else
            {
                //  元の色に戻す
                CanvasData.CanvasColor = ((SolidColorBrush)CanvasColorButton.Background).Color;
                _mainWindow?.UpdateCanvas();
            }
        }

        private void DirectionTextBox_ValueChanged(object sender, UpDownTextBoxEvent e)
        {
            if(_runEvent == false) return;

            if(sender == TopTextBox)
            {
                CanvasData.ImageMarginTop = TopTextBox.Value;
            }
            else if (sender == BottomTextBox)
            {
                CanvasData.ImageMarginBottom = BottomTextBox.Value;
            }
            else if (sender == LeftTextBox)
            {
                CanvasData.ImageMarginLeft = LeftTextBox.Value;
            }
            else if(sender == RightTextBox)
            {
                CanvasData.ImageMarginRight = RightTextBox.Value;
            }

            CanvasData.UpdateCanvasSize();

            //  画面に設定を反映させる
            UpdateView();

            _mainWindow?.UpdateCanvas();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            //  設定を初期化する
            CanvasData.InitMargin();
            CanvasData.UpdateCanvasSize();

            //  画面に設定を反映させる
            UpdateView();

            _mainWindow?.UpdateCanvas();
        }

    }
}
