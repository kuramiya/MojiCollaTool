using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MojiCollaTool
{
    public enum TextDirection
    {
        Yokogaki,
        Tategaki,
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
    }
}
