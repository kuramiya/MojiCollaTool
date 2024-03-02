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

        private bool _runEvent = true;

        public CanvasEditWindow()
        {
            CanvasData = new CanvasData();

            InitializeComponent();
        }

        public CanvasEditWindow(int imageWidth, int imageHeight, CanvasData canvasData)
        {
            _imageWidth = imageWidth;
            _imageHeight = imageHeight;
            CanvasData = canvasData;

            InitializeComponent();

            ImageWidthTextBox.Text = imageWidth.ToString();
            ImageHeightTextBox.Text = imageHeight.ToString();

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
                //  何もしない
            });
            colorSelectorWindow.Top = Top;
            colorSelectorWindow.Left = Left;
            colorSelectorWindow.Topmost = Topmost;
            var dialogResult = colorSelectorWindow.ShowDialog();

            if (dialogResult.HasValue == false || dialogResult.Value == false) return;

            CanvasColorButton.Background = new SolidColorBrush(colorSelectorWindow.NextBrush.Color);
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
                CanvasData.UpdateImageTopMargin(TopTextBox.Value, _imageHeight);
            }
            else if (sender == BottomTextBox)
            {
                CanvasData.UpdateImageBottomMargin(BottomTextBox.Value, _imageHeight);
            }
            else if (sender == LeftTextBox)
            {
                CanvasData.UpdateImageLeftMargin(LeftTextBox.Value, _imageWidth);
            }
            else if(sender == RightTextBox)
            {
                CanvasData.UpdateImageRightMargin(RightTextBox.Value, _imageWidth);
            }

            //  画面に設定を反映させる
            LoadCanvasDataToView(CanvasData);
        }

        private void DirectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (_runEvent == false) return;

            if (sender == TopButton)
            {
                CanvasData.UpdateImageTopMargin(0, _imageHeight);
            }
            else if (sender == BottomButton)
            {
                CanvasData.UpdateImageBottomMargin(0, _imageHeight);
            }
            else if (sender == LeftButton) 
            {
                CanvasData.UpdateImageLeftMargin(0, _imageWidth);
            }
            else if(sender == RightButton) 
            {
                CanvasData.UpdateImageRightMargin(0, _imageWidth);
            }

            //  画面に設定を反映させる
            LoadCanvasDataToView(CanvasData);
        }

        private void CanvasSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_runEvent == false) return;

            int width, height;

            if (int.TryParse(CanvasWidthTextBox.Text, out width) == false) return;
            if (int.TryParse(CanvasHeightTextBox.Text, out height) == false) return;

            //  画像より小さいサイズは処理をしない
            if (width < _imageWidth || height < _imageHeight) return;

            //  設定値を更新する
            CanvasData.Update(_imageWidth, _imageHeight, width, height);

            //  画面に設定を反映させる
            LoadCanvasDataToView(CanvasData);
        }
    }
}
