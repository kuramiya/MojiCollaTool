using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Effects;

namespace MojiCollaTool
{
    public class DecorationTextControl : FrameworkElement
    {
        private Geometry _textGeometry = null!;
        //  private Geometry _textHighLightGeometry = null!;

        private SolidColorBrush _foreColorBrush;
        private Pen _borderColorPen;

        public DecorationTextControl(char character, MojiData mojiData)
        {
            _foreColorBrush = new SolidColorBrush(mojiData.ForeColor);
            _borderColorPen = new Pen(new SolidColorBrush(mojiData.BorderColor), mojiData.BorderThickness);
            _borderColorPen.LineJoin = PenLineJoin.Round;

            SetText(character, mojiData);
        }

        public void SetText(char character, MojiData mojiData)
        {
            FormattedText formattedText = new FormattedText(
                character.ToString(),   //  文字
                CultureInfo.GetCultureInfo("ja-JP"),    //  文字文化、日本人しか使用しないと思うので日本語固定で良い
                FlowDirection.LeftToRight,  //  日本語、英語ともに左から右の配置で問題ない
                new Typeface(   //  文字の表示形式を示すクラスらしい
                    new FontFamily(mojiData.FontFamilyName),    //  フォント
                    mojiData.IsItalic ? FontStyles.Italic : FontStyles.Normal,  //  イタリック
                    mojiData.IsBold ? FontWeights.Bold : FontWeights.Normal,    //  太文字
                    FontStretches.Normal    //  使える要素かもしれない
                    ),
                mojiData.FontSize,  //  フォントサイズ
                Brushes.White,    //  文字色、これは使用されない
                VisualTreeHelper.GetDpi(this).PixelsPerDip  //  ？
            );

            // Build the geometry object that represents the text.
            _textGeometry = formattedText.BuildGeometry(new System.Windows.Point(0, 0));
            //  _textHighLightGeometry = formattedText.BuildHighlightGeometry(new System.Windows.Point(0, 0));

            Width = formattedText.WidthIncludingTrailingWhitespace;
            Height = formattedText.Height;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //  縁取りを描画する
            if(_borderColorPen.Thickness > 0)
            {
                //drawingContext.DrawGeometry(null, _borderColorPen, _textGeometry);

                DrawingVisual borderDrawingVisual = new DrawingVisual();

                using (DrawingContext borderDrawingContext = borderDrawingVisual.RenderOpen())
                {
                    borderDrawingContext.DrawGeometry(null, _borderColorPen, _textGeometry);
                }

                BlurEffect blurEffect = new BlurEffect()
                {
                    Radius = 20,
                };

                borderDrawingVisual.Effect = blurEffect;

                drawingContext.DrawDrawing(borderDrawingVisual.Drawing);
            }

            //  文字本体を描画する
            drawingContext.DrawGeometry(_foreColorBrush, null, _textGeometry);
        }
    }
}
