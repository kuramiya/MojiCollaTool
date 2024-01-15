using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MojiCollaTool
{
    public enum TextDirection : int
    {
        Yokogaki = 0,
        Tategaki = 1,
    }

    [Serializable]
    public class MojiData
    {
        public int Id { get; private set; }

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
        }

        public MojiData Reproduct(int id)
        {
            var reproductionMojiData = new MojiData(id);

            reproductionMojiData.Copy(this);

            //  わかりやすくするために、座標を少しずらす
            reproductionMojiData.X += 10;
            reproductionMojiData.Y += 10;

            return reproductionMojiData;
        }
    }
}
