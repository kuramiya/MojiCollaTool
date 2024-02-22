using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Effects;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MojiCollaTool
{
    public class DecorationTextControl : FrameworkElement
    {
        // Create a collection of child visual objects.
        private readonly VisualCollection _children = null!;

        public DecorationTextControl(char character, MojiData mojiData)
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

            //  サイズを設定する
            Width = formattedText.WidthIncludingTrailingWhitespace;
            Height = formattedText.Height;

            //  文字のジオメトリを作成する
            Geometry characterGeometry = formattedText.BuildGeometry(new Point(0, 0));
            //  _textHighLightGeometry = formattedText.BuildHighlightGeometry(new System.Windows.Point(0, 0));

            _children = new VisualCollection(this);

            if(mojiData.IsBorderExists)
            {
                _children.Add(CreateCharacterBorderDrawingVisual(characterGeometry, mojiData.BorderThickness, mojiData.BorderColor, mojiData.BorderBlurrRadius));
            }

            _children.Add(CreateCharacterDrawingVisual(characterGeometry, mojiData.ForeColor));
        }

        //public void SetText(char character, MojiData mojiData)
        //{
        //    FormattedText formattedText = new FormattedText(
        //        character.ToString(),   //  文字
        //        CultureInfo.GetCultureInfo("ja-JP"),    //  文字文化、日本人しか使用しないと思うので日本語固定で良い
        //        FlowDirection.LeftToRight,  //  日本語、英語ともに左から右の配置で問題ない
        //        new Typeface(   //  文字の表示形式を示すクラスらしい
        //            new FontFamily(mojiData.FontFamilyName),    //  フォント
        //            mojiData.IsItalic ? FontStyles.Italic : FontStyles.Normal,  //  イタリック
        //            mojiData.IsBold ? FontWeights.Bold : FontWeights.Normal,    //  太文字
        //            FontStretches.Normal    //  使える要素かもしれない
        //            ),
        //        mojiData.FontSize,  //  フォントサイズ
        //        Brushes.White,    //  文字色、これは使用されない
        //        VisualTreeHelper.GetDpi(this).PixelsPerDip  //  ？
        //    );

        //    // Build the geometry object that represents the text.
        //    _textGeometry = formattedText.BuildGeometry(new System.Windows.Point(0, 0));
        //    //  _textHighLightGeometry = formattedText.BuildHighlightGeometry(new System.Windows.Point(0, 0));

        //    Width = formattedText.WidthIncludingTrailingWhitespace;
        //    Height = formattedText.Height;
        //}

        protected override int VisualChildrenCount => _children.Count;

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }

        private DrawingVisual CreateCharacterDrawingVisual(Geometry characterGeometry, Color foreColor)
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawGeometry(new SolidColorBrush(foreColor), null, characterGeometry);
            }

            return drawingVisual;
        }

        private DrawingVisual CreateCharacterBorderDrawingVisual(Geometry characterGeometry, double borderThickness, Color borderColor, double borderBlurrRadius)
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            using (var drawingContext = drawingVisual.RenderOpen())
            {
                var pen = new Pen(new SolidColorBrush(borderColor), borderThickness);
                pen.LineJoin = PenLineJoin.Round;

                drawingContext.DrawGeometry(Brushes.Transparent, pen, characterGeometry);
            }

            if(borderBlurrRadius > 0)
            {
                drawingVisual.Effect = new BlurEffect { Radius = borderBlurrRadius };
            }

            return drawingVisual;
        }

        //protected override void OnRender(DrawingContext drawingContext)
        //{
        //    //  縁取りを描画する
        //    if (_borderColorPen.Thickness > 0)
        //    {
        //        //drawingContext.DrawGeometry(null, _borderColorPen, _textGeometry);

        //        DrawingVisual drawingVisual = new DrawingVisual();

        //        using(var dc = drawingVisual.RenderOpen())
        //        {
        //            dc.DrawGeometry(Brushes.Transparent, _borderColorPen, _textGeometry);
        //        }

        //        drawingVisual.Effect = new BlurEffect { Radius = 20 };

        //        drawingContext.DrawDrawing(drawingVisual.Drawing);
        //    }

        //    //  文字本体を描画する
        //    drawingContext.DrawGeometry(_foreColorBrush, null, _textGeometry);
        //}
    }
}
