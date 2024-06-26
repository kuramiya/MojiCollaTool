﻿using System;
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
    public class DecoratedCharacterControl : FrameworkElement
    {
        /// <summary>
        /// 縦書き対応のために90°右回転させる必要のある文字
        /// 主にかっこなど
        /// </summary>
        private const string TATEGAKI_90DEG_ROTATE_TARGET_CHARS = " 　‥…:：;；-=＝≒ー～─━|（）()｟｠⦅⦆❨❩❪❫⸨⸩⦕⦖⦇⦈⦓⦔﴾﴿⸦⸧⎛⎞⎜⎟⎝⎠╭╮┃┃╰╯⁽⁾₍₎︶⁐「」『』⌜⌟⌞⌝﹂［﹄［〈〉⟨⟩《》⟪⟫‹›«»❮❯❬❭❰❱⦉⦊⦑⦒⦓⦔⦖⦕⧼⧽﹀︾｛｝{}❴❵⦃⦄⎧⎫⎨⎬⎩⎭︸［］[]〚〛⟦⟧⦋⦌⦍⦎⦏⦐⁅⁆⎡⎤⎢⎥⎣⎦⸢⸣⸠⸡⸤⸥﹈⎵【】〖〗︼︘〔〕❲❳〘〙⟬⟭⦗⦘︺’’''“”\"\"❛❜❝❞‚‚„„〝　＜＞<>≪≫≦≧≤≥⩽⩾≲≳⪍⪎⪅⪆⋜⋝⪙⪚≶≷⋚⋛⪋⪌";

        /// <summary>
        /// 縦書き対応のために右上にずらす必要のある文字
        /// 主にかっこなど
        /// </summary>
        private const string TATEGAKI_SHIFT_TARGET_CHARS ="｡。､、.．,，";

        /// <summary>
        /// 小さい文字一覧（配置に大して特別な処理が必要かもしれないので置いてある
        /// </summary>
        private const string TATEGAKI_SHIFT_TARGET_CHARS_SMALL = "ぁぃぅぇぉっゃゅょゎゕゖァィゥェォヵㇰヶㇱㇲッㇳㇴㇵㇶㇷㇷ゚ㇸㇹㇺャュョㇻㇼㇽㇾㇿヮ";

        /// <summary>
        /// 文字の種別
        /// </summary>
        public char Character { get; set; }

        // Create a collection of child visual objects.
        private readonly VisualCollection _children = null!;

        /// <summary>
        /// 縦書き対応回転原点変更オブジェクト
        /// </summary>
        private readonly static Point tategakiTransformOrigin = new Point(0.5, 0.5);

        /// <summary>
        /// 縦書き対応90°回転用の変換オブジェクト
        /// </summary>
        private readonly static RotateTransform tategaki90DegRotate = new RotateTransform(90);

        public DecoratedCharacterControl(char character, MojiData mojiData)
        {
            Character = character;

            //  フォーマットされた描画用文字を作成する
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

            //  デバッグ用の箱を追加する
            //_children.Add(CreateDebugBox());

            //  第二の文字の縁取りがある場合、その描画オブジェクトを追加する
            if (mojiData.IsSecondBorderExists)
            {
                _children.Add(CreateCharacterBorderDrawingVisual(characterGeometry, mojiData.SecondBorderThickness, mojiData.SecondBorderColor, mojiData.SecondBorderBlurrRadius));
            }

            //  文字の縁取りがある場合、その描画オブジェクトを追加する
            if (mojiData.IsBorderExists)
            {
                _children.Add(CreateCharacterBorderDrawingVisual(characterGeometry, mojiData.BorderThickness, mojiData.BorderColor, mojiData.BorderBlurrRadius));
            }

            //  文字の描画オブジェクト追加する
            _children.Add(CreateCharacterDrawingVisual(characterGeometry, mojiData.ForeColor));

            //  縦書き、横書きに合わせて配置などの処理を行う
            switch (mojiData.TextDirection)
            {
                case TextDirection.Yokogaki:
                    VerticalAlignment = VerticalAlignment.Bottom;
                    HorizontalAlignment = HorizontalAlignment.Center;

                    //  横書きの場合、文字の間隔は右側だけ広げる
                    Margin = new Thickness(0, 0, mojiData.CharacterMargin, 0);
                    break;
                case TextDirection.Tategaki:
                    VerticalAlignment = VerticalAlignment.Center;
                    HorizontalAlignment = HorizontalAlignment.Center;

                    //  縦書きの場合、文字の間隔は下側だけ広げる
                    Margin = new Thickness(0, 0, 0, mojiData.CharacterMargin);

                    //  縦書きの場合、かっこや点などの記号を回転させる
                    if (TATEGAKI_SHIFT_TARGET_CHARS.Contains(character))
                    {
                        RenderTransform = new TranslateTransform(Width / 2, -Height / 2);
                    }
                    else if(TATEGAKI_SHIFT_TARGET_CHARS_SMALL.Contains(character))
                    {
                        RenderTransform = new TranslateTransform(Width / 8, -Height / 14);
                    }
                    else if (TATEGAKI_90DEG_ROTATE_TARGET_CHARS.Contains(character))
                    {
                        RenderTransformOrigin = tategakiTransformOrigin;
                        RenderTransform = tategaki90DegRotate;
                    }
                    break;
                default:
                    break;
            }
        }

        protected override int VisualChildrenCount => _children.Count;

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }

        /// <summary>
        /// 文字の描画オブジェクトを生成して返す
        /// </summary>
        /// <param name="characterGeometry"></param>
        /// <param name="foreColor"></param>
        /// <returns></returns>
        private DrawingVisual CreateCharacterDrawingVisual(Geometry characterGeometry, Color foreColor)
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawGeometry(new SolidColorBrush(foreColor), null, characterGeometry);
            }

            return drawingVisual;
        }

        /// <summary>
        /// 文字の縁取りオブジェクトを生成して返す
        /// </summary>
        /// <param name="characterGeometry"></param>
        /// <param name="borderThickness"></param>
        /// <param name="borderColor"></param>
        /// <param name="borderBlurrRadius"></param>
        /// <returns></returns>
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

        private DrawingVisual CreateDebugBox()
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(new SolidColorBrush(Colors.Red), null, new Rect(0, 0, Width, Height));
            }

            return drawingVisual;
        }
    }
}
