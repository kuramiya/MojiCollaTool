using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfColorPicker
{
    /// <summary>
    /// UserControl1.xaml の相互作用ロジック
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        #region プロパティ

        /// <summary>
        /// 入力ブラシプロパティ
        /// </summary>
        public static readonly DependencyProperty BeforeBrushProperty =
            DependencyProperty.Register(nameof(BeforeBrush), typeof(SolidColorBrush), typeof(ColorPicker), new PropertyMetadata(Brushes.White, (obj, e)=>
            {
                ColorPicker? cp = obj as ColorPicker;

                if (cp == null) return;

                cp.AfterBrush = cp.BeforeBrush;
                cp.Red = cp.BeforeBrush.Color.R;
                cp.Green = cp.BeforeBrush.Color.G;
                cp.Blue = cp.BeforeBrush.Color.B;
                cp.Alpha = (cp.BeforeBrush.Color.A / 255.0) * 100;

                cp.CalcHSV();
            }));

        /// <summary>
        /// 出力ブラシプロパティ
        /// </summary>
        public static readonly DependencyProperty AfterBrushProperty = 
            DependencyProperty.Register(nameof(AfterBrush), typeof(SolidColorBrush), typeof(ColorPicker), new PropertyMetadata(Brushes.White));

        /// <summary>
        /// 色相プロパティ
        /// </summary>
        public static readonly DependencyProperty HueProperty = 
            DependencyProperty.Register(nameof(Hue), typeof(double), typeof(ColorPicker), new PropertyMetadata(0d, (obj, e)=> 
            {
                ColorPicker? cp = obj as ColorPicker;

                if(cp == null) return;

                double height = ((Canvas)cp.HueThumb.Parent).ActualHeight;
                Canvas.SetTop(cp.HueThumb, (cp.Hue / 100) * height - (cp.HueThumb.ActualHeight / 2));
                cp.CalcRGB();
            }));

        /// <summary>
        /// 彩度プロパティ
        /// </summary>
        public static readonly DependencyProperty SaturationProperty = 
            DependencyProperty.Register(nameof(Saturation), typeof(double), typeof(ColorPicker), new PropertyMetadata(0d, (obj, e)=>
            {
                ColorPicker? cp = obj as ColorPicker;

                if (cp == null) return;

                double width = ((Canvas)cp.ColorBoxThumb.Parent).ActualWidth;
                Canvas.SetLeft(cp.ColorBoxThumb, (cp.Saturation / 100) * width - (cp.ColorBoxThumb.ActualWidth / 2));
                cp.CalcRGB();
            }));

        /// <summary>
        /// 明度プロパティ
        /// </summary>
        public static readonly DependencyProperty ValueProperty = 
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(ColorPicker), new PropertyMetadata(0d, (obj, e)=>
            {
                ColorPicker? cp = obj as ColorPicker;

                if (cp == null) return;

                double height = ((Canvas)cp.ColorBoxThumb.Parent).ActualHeight;
                Canvas.SetTop(cp.ColorBoxThumb, -((cp.Value / 100) * height) + height - (cp.ColorBoxThumb.ActualHeight / 2));
                cp.CalcRGB();
            }));

        /// <summary>
        /// 透明度プロパティ
        /// </summary>
        public static readonly DependencyProperty AlphaProperty = 
            DependencyProperty.Register(nameof(Alpha), typeof(double), typeof(ColorPicker), new PropertyMetadata(100d, (obj, e)=>
            {
                ColorPicker? cp = obj as ColorPicker;

                if (cp == null) return;

                double height = ((Canvas)cp.AlphaThumb.Parent).ActualHeight;
                Canvas.SetTop(cp.AlphaThumb, -((cp.Alpha / 100) * height) + height - (cp.AlphaThumb.ActualHeight / 2));
                cp.CalcRGB();
            }));

        /// <summary>
        /// 赤プロパティ
        /// </summary>
        public static readonly DependencyProperty RedProperty = 
            DependencyProperty.Register(nameof(Red), typeof(byte), typeof(ColorPicker), new PropertyMetadata((byte)0, (obj, e)=>
            {
                ColorPicker? cp = obj as ColorPicker;

                if (cp == null) return;

                cp.CalcHSV();

                double height = ((Canvas)cp.ColorBoxThumb.Parent).ActualHeight;
                double width = ((Canvas)cp.ColorBoxThumb.Parent).ActualWidth;
                Canvas.SetLeft(cp.ColorBoxThumb, (cp.Saturation / 100) * width - (cp.ColorBoxThumb.ActualWidth / 2));
                Canvas.SetTop(cp.ColorBoxThumb, -((cp.Value / 100) * height) + height - (cp.ColorBoxThumb.ActualHeight / 2));

                height = ((Canvas)cp.HueThumb.Parent).ActualHeight;
                Canvas.SetTop(cp.HueThumb, (cp.Hue / 100) * height - (cp.HueThumb.ActualHeight / 2));

                height = ((Canvas)cp.AlphaThumb.Parent).ActualHeight;
                Canvas.SetTop(cp.AlphaThumb, -((cp.Alpha / 100) * height) + height - (cp.AlphaThumb.ActualHeight / 2));
            }));

        /// <summary>
        /// 緑プロパティ
        /// </summary>
        public static readonly DependencyProperty GreenProperty = 
            DependencyProperty.Register(nameof(Green), typeof(byte), typeof(ColorPicker), new PropertyMetadata((byte)0, (obj, e) =>
            {
                ColorPicker? cp = obj as ColorPicker;

                if (cp == null) return;

                cp.CalcHSV();

                double height = ((Canvas)cp.ColorBoxThumb.Parent).ActualHeight;
                double width = ((Canvas)cp.ColorBoxThumb.Parent).ActualWidth;
                Canvas.SetLeft(cp.ColorBoxThumb, (cp.Saturation / 100) * width - (cp.ColorBoxThumb.ActualWidth / 2));
                Canvas.SetTop(cp.ColorBoxThumb, -((cp.Value / 100) * height) + height - (cp.ColorBoxThumb.ActualHeight / 2));

                height = ((Canvas)cp.HueThumb.Parent).ActualHeight;
                Canvas.SetTop(cp.HueThumb, (cp.Hue / 100) * height - (cp.HueThumb.ActualHeight / 2));

                height = ((Canvas)cp.AlphaThumb.Parent).ActualHeight;
                Canvas.SetTop(cp.AlphaThumb, -((cp.Alpha / 100) * height) + height - (cp.AlphaThumb.ActualHeight / 2));
            }));

        /// <summary>
        /// 青プロパティ
        /// </summary>
        public static readonly DependencyProperty BlueProperty = 
            DependencyProperty.Register(nameof(Blue), typeof(byte), typeof(ColorPicker), new PropertyMetadata((byte)0, (obj, e) =>
            {
                ColorPicker? cp = obj as ColorPicker;

                if (cp == null) return;

                cp.CalcHSV();

                double height = ((Canvas)cp.ColorBoxThumb.Parent).ActualHeight;
                double width = ((Canvas)cp.ColorBoxThumb.Parent).ActualWidth;
                Canvas.SetLeft(cp.ColorBoxThumb, (cp.Saturation / 100) * width - (cp.ColorBoxThumb.ActualWidth / 2));
                Canvas.SetTop(cp.ColorBoxThumb, -((cp.Value / 100) * height) + height - (cp.ColorBoxThumb.ActualHeight / 2));

                height = ((Canvas)cp.HueThumb.Parent).ActualHeight;
                Canvas.SetTop(cp.HueThumb, (cp.Hue / 100) * height - (cp.HueThumb.ActualHeight / 2));

                height = ((Canvas)cp.AlphaThumb.Parent).ActualHeight;
                Canvas.SetTop(cp.AlphaThumb, -((cp.Alpha / 100) * height) + height - (cp.AlphaThumb.ActualHeight / 2));
            }));

        /// <summary>
        /// 入力ブラシ
        /// </summary>
        public SolidColorBrush BeforeBrush
        {
            get => (SolidColorBrush)GetValue(BeforeBrushProperty);
            set => SetValue(BeforeBrushProperty, value);
        }

        /// <summary>
        /// 出力ブラシ
        /// </summary>
        public SolidColorBrush AfterBrush
        {
            get => (SolidColorBrush)GetValue(AfterBrushProperty);
            set => SetValue(AfterBrushProperty, value);
        }

        /// <summary>
        /// 色相
        /// </summary>
        public double Hue
        {
            get => (double)GetValue(HueProperty);
            set => SetValue(HueProperty, value);
        }

        /// <summary>
        /// 彩度
        /// </summary>
        public double Saturation
        {
            get => (double)GetValue(SaturationProperty);
            set => SetValue(SaturationProperty, value);
        }

        /// <summary>
        /// 明度
        /// </summary>
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <summary>
        /// 透明度
        /// </summary>
        public double Alpha
        {
            get => (double)GetValue(AlphaProperty);
            set => SetValue(AlphaProperty, value);
        }

        /// <summary>
        /// 赤
        /// </summary>
        public byte Red
        {
            get => (byte)GetValue(RedProperty);
            set => SetValue(RedProperty, value);
        }

        /// <summary>
        /// 緑
        /// </summary>
        public byte Green
        {
            get => (byte)GetValue(GreenProperty);
            set => SetValue(GreenProperty, value);
        }

        /// <summary>
        /// 青
        /// </summary>
        public byte Blue
        {
            get => (byte)GetValue(BlueProperty);
            set => SetValue(BlueProperty, value);
        }

        #endregion

        private bool IsMouseDown = false;
        private bool IsCalcHSV = false;
        private bool IsCalcRGB = false;

        // コンストラクタ
        public ColorPicker()
        {
            InitializeComponent();

            DataContext = this;
        }

        #region イベントハンドラ

        /// <summary>
        /// サイズチェンジ時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double height = ((Canvas)ColorBoxThumb.Parent).ActualHeight;
            double width = ((Canvas)ColorBoxThumb.Parent).ActualWidth;
            Canvas.SetLeft(ColorBoxThumb, (Saturation / 100) * width - (ColorBoxThumb.ActualWidth / 2));
            Canvas.SetTop(ColorBoxThumb, -((Value / 100) * height) + height - (ColorBoxThumb.ActualHeight / 2));

            height = ((Canvas)HueThumb.Parent).ActualHeight;
            Canvas.SetTop(HueThumb, (Hue / 100) * height - (HueThumb.ActualHeight / 2));

            height = ((Canvas)AlphaThumb.Parent).ActualHeight;
            Canvas.SetTop(AlphaThumb, -((Alpha / 100) * height) + height - (AlphaThumb.ActualHeight / 2));
        }

        /// <summary>
        /// HSVA各コントロールのマウスダウン時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HSVA_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = true;
            ((Canvas)sender).CaptureMouse();
            Keyboard.ClearFocus();
        }

        /// <summary>
        /// HSVA各コントロールのマウスアップ時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HSVA_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = false;
            ((Canvas)sender).ReleaseMouseCapture();
        }

        /// <summary>
        /// 色相コントロールのマウスムーブ時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HueBarThumb_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseDown)
            {
                Canvas? canvas = sender as Canvas;

                if (canvas == null) return;

                Grid? grid = canvas.Children[0] as Grid;
                double y = e.GetPosition(canvas).Y;

                y = Math.Max(y, 0);
                y = Math.Min(y, canvas.ActualHeight);

                Canvas.SetTop(grid, y - (HueThumb.ActualHeight / 2));

                Hue = (y / canvas.ActualHeight) * 100;
            }
        }

        /// <summary>
        /// 透明度コントロールのマウスムーブ時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlphaBarThumb_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseDown)
            {
                Canvas? canvas = sender as Canvas;

                if (canvas == null) return;

                Grid? grid = canvas.Children[0] as Grid;
                double y = e.GetPosition(canvas).Y;

                y = Math.Max(y, 0);
                y = Math.Min(y, canvas.ActualHeight);

                Canvas.SetTop(grid, y - (AlphaThumb.ActualHeight / 2));

                Alpha = -((y / canvas.ActualHeight) * 100) + 100;
            }
        }

        /// <summary>
        /// 彩度、明度コントロールのマウスムーブ時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorBoxThumb_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseDown)
            {
                Canvas? canvas = sender as Canvas;

                if (canvas == null) return;

                Grid? grid = canvas.Children[0] as Grid;
                double x = e.GetPosition(canvas).X;
                double y = e.GetPosition(canvas).Y;

                x = Math.Max(x, 0);
                x = Math.Min(x, canvas.ActualWidth);
                y = Math.Max(y, 0);
                y = Math.Min(y, canvas.ActualHeight);

                Canvas.SetLeft(grid, x - (ColorBoxThumb.ActualWidth / 2));
                Canvas.SetTop(grid, y - (ColorBoxThumb.ActualHeight / 2));

                Saturation = (x / canvas.ActualWidth) * 100;
                Value = -((y / canvas.ActualHeight) * 100) + 100;
            }
        }

        /// <summary>
        /// HSVA各テキストボックスのテキスト変更時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HSVA_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;

            if (textBox == null) return;

            if (double.TryParse(textBox.Text, out double num))
            {
                if (num > 100) textBox.Text = "100";
                else if (num < 0)textBox.Text = "0";
            }
            else textBox.Text = "0";
        }

        /// <summary>
        /// RGB各テキストボックスのテキスト変更時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RGB_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;

            if (textBox == null) return;

            if (int.TryParse(textBox.Text, out int num))
            {
                if (num > 255) textBox.Text = "255";
                else if (num < 0) textBox.Text = "0";
            }   
            else textBox.Text = "0";
        }

        #endregion

        /// <summary>
        /// HSVを計算してそれぞれ代入する
        /// </summary>
        private void CalcHSV()
        {
            IsCalcHSV = true;

            if (!IsCalcRGB)
            {
                byte max = Math.Max(Red, Math.Max(Green, Blue));
                byte min = Math.Min(Red, Math.Min(Green, Blue));

                double hue = 0;
                if (Red == Green && Green == Blue) hue = 0;
                else if (Red >= Green && Red >= Blue) hue = 16.667 * ((Green - Blue) / (double)(max - min));
                else if (Green >= Red && Green >= Blue) hue = (16.667 * ((Blue - Red) / (double)(max - min)) + 33.333);
                else if (Blue >= Red && Blue >= Green) hue = (16.667 * ((Red - Green) / (double)(max - min)) + 66.667);

                if (hue < 0) hue += 100;
                Hue = hue;

                try
                {
                    Saturation = ((max - min) / (double)max) * 100;
                    if (double.IsNaN(Saturation)) Saturation = 0;
                }
                catch (DivideByZeroException)
                {
                    Saturation = 0;
                }

                Value = (max / 255.0) * 100;

                AfterBrush = new SolidColorBrush(Color.FromArgb((byte)((Alpha / 100.0) * 255), Red, Green, Blue));
            }

            IsCalcHSV = false;
        }

        /// <summary>
        /// RGBを計算してそれぞれ代入する
        /// </summary>
        private void CalcRGB()
        {
            IsCalcRGB = true;

            if (!IsCalcHSV)
            {
                byte max = (byte)((Value / 100) * 255);
                byte min = (byte)(max - ((Saturation / 100) * max));

                if (Hue <= 16.667)
                {
                    Red = max;
                    Green = (byte)((Hue / 16.667) * (max - min) + min);
                    Blue = min;
                }
                else if (Hue <= 33.333)
                {
                    Red = (byte)(((33.333 - Hue) / 16.667) * (max - min) + min);
                    Green = max;
                    Blue = min;
                }
                else if (Hue <= 50)
                {
                    Red = min;
                    Green = max;
                    Blue = (byte)(((Hue - 33.333) / 16.667) * (max - min) + min);
                }
                else if (Hue <= 66.667)
                {
                    Red = min;
                    Green = (byte)(((66.667 - Hue) / 16.667) * (max - min) + min);
                    Blue = max;
                }
                else if (Hue <= 83.333)
                {
                    Red = (byte)(((Hue - 66.667) / 16.667) * (max - min) + min);
                    Green = min;
                    Blue = max;
                }
                else
                {
                    Red = max;
                    Green = min;
                    Blue = (byte)(((100 - Hue) / 16.667) * (max - min) + min);
                }

                AfterBrush = new SolidColorBrush(Color.FromArgb((byte)((Alpha / 100) * 255), Red, Green, Blue));
            }

            IsCalcRGB = false;
        }
    }
}
