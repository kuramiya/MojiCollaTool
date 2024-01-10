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

    public class Moji
    {
        public int Id { get; private set; }

        public string FullText { get; set; } = "サンプル";

        public string ExampleText => FullText.Replace(Environment.NewLine, string.Empty).Substring(0, 10);

        public StackPanel MojiPanel { get; set; } = new StackPanel();

        public Point Location { get; set; } = new Point(0, 0);

        public TextDirection TextDirection { get; set; } = TextDirection.Yokogaki;

        public double CharacterMargin { get; set; } = 0;

        public double LineMargin { get; set; } = 0;

        public Moji(int id)
        {
            Id = id;
            FullText = $"サンプル{id}";

            UpdateMojiPanel();
        }

        public void UpdateMojiPanel()
        {
            //  文字パネルの中身を初期化する
            MojiPanel.Children.Clear();

            //  文字パネルの位置を設定する
            MojiPanel.Margin = new Thickness(Location.X, Location.Y, 0, 0);

            //  改行ごとに分ける
            var lines = FullText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            List<Panel> linePanels = new List<Panel>();

            foreach (var line in lines)
            {
                //  行ごとにスタックパネルを用意する
                var linePanel = new StackPanel();

                //  配置方向を設定する
                //  行間を設定する
                //  縦書き横書きで異なる
                switch (TextDirection)
                {
                    case TextDirection.Yokogaki:
                        linePanel.Margin = new Thickness(0, 0, 0, LineMargin);
                        linePanel.Orientation = Orientation.Horizontal;
                        break;
                    case TextDirection.Tategaki:
                        linePanel.Margin = new Thickness(LineMargin, 0, 0, 0);
                        linePanel.Orientation = Orientation.Vertical;
                        break;
                    default:
                        break;
                }

                foreach (var character in line)
                {
                    //  文字ごとにラベルを用意する
                    var charLabel = new Label();

                    //  todo 装飾を付加する必要あり、おそらくFormattedTextを使用する
                    charLabel.Content = character;

                    //  文字間隔を設定する
                    //  縦書き横書きで異なる
                    //  todo 縦書きの処理が必要
                    switch (TextDirection)
                    {
                        case TextDirection.Yokogaki:
                            charLabel.Margin = new Thickness(0, 0, CharacterMargin, 0);
                            break;
                        case TextDirection.Tategaki:
                            charLabel.Margin = new Thickness(0, 0, 0, CharacterMargin);
                            break;
                        default:
                            break;
                    }

                    //  行パネルに追加する
                    linePanel.Children.Add(charLabel);
                }

                linePanels.Add(linePanel);
            }

            //  縦書きの場合、逆順に配置する必要あり
            if(TextDirection == TextDirection.Tategaki)
            {
                linePanels.Reverse();
            }
            foreach (var linePanel in linePanels)
            {
                MojiPanel.Children.Add(linePanel);
            }
        }

    }
}
