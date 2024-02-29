using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MojiCollaTool
{
    public enum TextDirection : int
    {
        Yokogaki = 0,
        Tategaki = 1,
    }

    /// <summary>
    /// 文字色（前面）or背景色
    /// </summary>
    public enum ForeBack
    {
        Fore,
        Back,
    }

    [Serializable]
    public class MojiData
    {
        public int Id { get; set; }

        public string FullText { get; set; } = "サンプル";

        public string ExampleText => new string(FullText.Replace(Environment.NewLine, string.Empty).Take(10).ToArray());

        public double X { get; set; }

        public double Y { get; set; }

        public int FontSize { get; set; } = 20;

        public string FontFamilyName { get; set; } = "ＭＳ ゴシック";

        public TextDirection TextDirection { get; set; } = TextDirection.Yokogaki;

        public bool IsBold { get; set; } = false;

        public bool IsItalic { get; set; } = false;

        public double CharacterMargin { get; set; } = 0;

        public double LineMargin { get; set; } = 0;

        public Color ForeColor { get; set; } = Colors.Black;

        public Color BorderColor { get; set; } = Colors.White;

        public bool IsBorderExists { get => BorderThickness > 0; }

        public double BorderThickness { get; set; } = 0;

        public double BorderBlurrRadius { get; set; } = 0;

        public bool IsBackgroundBoxExists { get; set; } = false;

        public Color BackgroundBoxColor { get; set; } = Colors.White;

        public double BackgoundBoxPadding { get; set; } = 0;

        public double BackgroundBoxBorderThickness { get; set; } = 0;

        public Color BackgroundBoxBorderColor { get; set; } = Colors.Black;

        public bool IsRotateActive { get => RotateAngle != 0; }

        public double RotateAngle { get; set; } = 0;

        public MojiData()
        {
            //  何もしない
        }

        public MojiData(int id)
        {
            Id = id;
            FullText = $"サンプル{id}";
        }

        public void Copy(MojiData source)
        {
            FullText = source.FullText;
            X = source.X;
            Y = source.Y;
            FontSize = source.FontSize;
            FontFamilyName = source.FontFamilyName;
            TextDirection = source.TextDirection;
            IsBold = source.IsBold;
            IsItalic = source.IsItalic;
            CharacterMargin = source.CharacterMargin;
            LineMargin = source.LineMargin;
            ForeColor = source.ForeColor;
            BorderColor = source.BorderColor;
            BorderThickness = source.BorderThickness;
            BorderBlurrRadius = source.BorderBlurrRadius;
            IsBackgroundBoxExists = source.IsBackgroundBoxExists;
            BackgroundBoxColor = source.BackgroundBoxColor;
            BackgoundBoxPadding = source.BackgoundBoxPadding;
            BackgroundBoxBorderThickness = source.BackgroundBoxBorderThickness;
            BackgroundBoxBorderColor = source.BackgroundBoxBorderColor;
            RotateAngle = source.RotateAngle;
        }

        /// <summary>
        /// 文字を複製する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MojiData Reproduct(int id)
        {
            var reproductionMojiData = new MojiData(id);

            reproductionMojiData.Copy(this);

            //  わかりやすくするために、座標を少しずらす
            reproductionMojiData.X += 10;
            reproductionMojiData.Y += 10;

            return reproductionMojiData;
        }

        /// <summary>
        /// 改行コードがLFだけの場合、CR+LFに復元する
        /// </summary>
        public void RestoreFullTextNewLine()
        {
            if(FullText.Contains("\r") == false)
            {
                FullText = FullText.Replace("\n", Environment.NewLine);
            }
        }
    }
}
