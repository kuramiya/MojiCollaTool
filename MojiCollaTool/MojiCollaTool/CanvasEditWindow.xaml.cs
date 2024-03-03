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
        /// 画像の横幅
        /// </summary>
        private int _imageWidth;

        /// <summary>
        /// 画像の縦幅
        /// </summary>
        private int _imageHeight;

        /// <summary>
        /// キャンバスデータ
        /// </summary>
        public CanvasData CanvasData { get; set; }

        private MainWindow _mainWindow;

        private bool _runEvent = true;

        public CanvasEditWindow()
        {
            CanvasData = new CanvasData();

            InitializeComponent();
        }

        public CanvasEditWindow(int imageWidth, int imageHeight, CanvasData canvasData, MainWindow mainWindow)
        {
            _imageWidth = imageWidth;
            _imageHeight = imageHeight;
            CanvasData = canvasData;
            _mainWindow = mainWindow;

            InitializeComponent();

            ImageWidthHeighTextBlock.Text = $"←横→:{imageWidth}px\r\n↑縦↓:{imageHeight}px";

            LoadCanvasDataToView(canvasData);
        }

        /// <summary>
        /// キャンバスデータを画面に反映する
        /// </summary>
        /// <param name="canvasData"></param>
        private void LoadCanvasDataToView(CanvasData canvasData)
        {
            _runEvent = false;

            CanvasWidthTextBox.Text = canvasData.Width.ToString();
            CanvasHeightTextBox.Text = canvasData.Height.ToString();

            TopTextBox.SetValue(canvasData.ImageTopMargin, false);
            BottomTextBox.SetValue(canvasData.ImageBottomMargin, false);
            LeftTextBox.SetValue(canvasData.ImageLeftMargin, false);
            RightTextBox.SetValue(canvasData.ImageRightMargin, false);

            CanvasColorButton.Background = new SolidColorBrush(canvasData.Background);

            _runEvent = true;
        }

        /// <summary>
        /// キャンバスデータを画面から生成する
        /// </summary>
        /// <returns></returns>
        private CanvasData CreateCanvasData()
        {
            return new CanvasData()
            {
                Width = int.Parse(CanvasWidthTextBox.Text),
                Height = int.Parse(CanvasHeightTextBox.Text),
                ImageTopMargin = TopTextBox.Value,
                ImageBottomMargin = BottomTextBox.Value,
                ImageLeftMargin = LeftTextBox.Value,
                ImageRightMargin = RightTextBox.Value,
                Background = GetCanvasColorButtonColor(),
            };
        }

        private Color GetCanvasColorButtonColor()
        {
            return ((SolidColorBrush)CanvasColorButton.Background).Color;
        }

        private void CanvasColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorSelector.ColorSelectorWindow colorSelectorWindow = new ColorSelector.ColorSelectorWindow(GetCanvasColorButtonColor(), (color) =>
            {
                CanvasData.Background = color;
                _mainWindow.UpdateCanvas(CanvasData);
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
                CanvasData.Background = ((SolidColorBrush)CanvasColorButton.Background).Color;
                _mainWindow.UpdateCanvas(CanvasData);
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            CanvasData = CreateCanvasData();
            DialogResult = true;
            Close();
        }

        private void DirectionTextBox_ValueChanged(object sender, UpDownTextBoxEvent e)
        {
            if(_runEvent == false) return;

            if(sender == TopTextBox)
            {
                CanvasData.ImageTopMargin = TopTextBox.Value;
            }
            else if (sender == BottomTextBox)
            {
                CanvasData.ImageBottomMargin = BottomTextBox.Value;
            }
            else if (sender == LeftTextBox)
            {
                CanvasData.ImageLeftMargin = LeftTextBox.Value;
            }
            else if(sender == RightTextBox)
            {
                CanvasData.ImageRightMargin = RightTextBox.Value;
            }

            CanvasData.UpdateCanvasWidthHeight(_imageWidth, _imageHeight);

            //  画面に設定を反映させる
            LoadCanvasDataToView(CanvasData);

            _mainWindow.UpdateCanvas(CanvasData);
        }
    }
}
