using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MojiCollaTool
{
    public class MojiPanel : ContentControl
    {
        private static readonly char[] TategakiRotateTargetCharacters = { '「', '」', '(', ')', '【', '】' };

        public int Id => MojiData.Id;

        public string ExampleText => MojiData.ExampleText;

        public MojiData MojiData { get; set; }

        public MojiWindow? MojiWindow { get; set; }

        private StackPanel stackPanel = new StackPanel();

        private MainWindow mainWindow;

        private Nullable<Point> dragStart = null;

        private TransformGroup tategakiTransformGroup = new TransformGroup();

        public MojiPanel(int id, MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            MojiData = new MojiData(id);

            Init();
        }

        public MojiPanel(MojiData mojiData, MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            MojiData = mojiData;

            Init();
        }

        private void Init()
        {
            tategakiTransformGroup.Children.Add(new RotateTransform(90));
            tategakiTransformGroup.Children.Add(new TranslateTransform(2, 0));

            MojiWindow = new MojiWindow(this);

            AddChild(stackPanel);

            MouseDown += MojiPanel_MouseDown;
            MouseUp += MojiPanel_MouseUp;
            MouseMove += MojiPanel_MouseMove;
            MouseDoubleClick += MojiPanel_MouseDoubleClick;
            Unloaded += MojiPanel_Unloaded;

            UpdateMojiView();
        }

        public void Reproduction()
        {
            mainWindow.ReproductionMoji(this);
        }

        public void Remove()
        {
            mainWindow.RemoveMoji(this);
        }

        private void MojiPanel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MojiWindow?.Show();
        }

        private void MojiPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragStart != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var element = (UIElement)sender;
                var p2 = e.GetPosition(mainWindow.MainCanvas);

                MojiData.X = p2.X - dragStart.Value.X;
                MojiData.Y = p2.Y - dragStart.Value.Y;

                Margin = new Thickness(MojiData.X, MojiData.Y, 0, 0);
            }
        }

        private void MojiPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;

            if(dragStart != null)
            {
                MojiData.X = VisualOffset.X;
                MojiData.Y = VisualOffset.Y;
                MojiWindow?.UpdateXY(MojiData.X, MojiData.Y);
            }
            dragStart = null;
            element.ReleaseMouseCapture();
        }

        private void MojiPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;
            dragStart = e.GetPosition(element);
            element.CaptureMouse();
        }

        private void MojiPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            if (MojiWindow != null)
            {
                //  パネルがアンロード（削除？）されるタイミングで、内部保持のウィンドウに終了フラグを立て、クローズする
                //  これにより、ウィンドウ保持によるアプリが終わらない問題を回避している
                MojiWindow.IsHideOnly = false;
                MojiWindow.Close();
                MojiWindow = null;
            }
        }

        public void UpdateXYView()
        {
            //  文字パネルの位置を設定する
            Margin = new Thickness(MojiData.X, MojiData.Y, 0, 0);
        }

        public void UpdateMojiView()
        {
            //  文字パネルの中身を初期化する
            stackPanel.Children.Clear();

            //  文字パネルの位置を設定する
            Margin = new Thickness(MojiData.X, MojiData.Y, 0, 0);

            //  縦書き、横書きで配置方法が異なる
            switch (MojiData.TextDirection)
            {
                case TextDirection.Yokogaki:
                    stackPanel.Orientation = Orientation.Vertical;
                    break;
                case TextDirection.Tategaki:
                    stackPanel.Orientation = Orientation.Horizontal;
                    break;
                default:
                    break;
            }

            //  改行ごとに分ける
            var lines = MojiData.FullText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            List<Panel> linePanels = new List<Panel>();

            foreach (var line in lines)
            {
                //  行ごとにスタックパネルを用意する
                var linePanel = new StackPanel();

                //  配置方向を設定する
                //  行間を設定する
                //  縦書き横書きで異なる
                switch (MojiData.TextDirection)
                {
                    case TextDirection.Yokogaki:
                        linePanel.Margin = new Thickness(0, 0, 0, MojiData.LineMargin);
                        linePanel.Orientation = Orientation.Horizontal;
                        break;
                    case TextDirection.Tategaki:
                        linePanel.Margin = new Thickness(0, 0, MojiData.LineMargin, 0);
                        linePanel.Orientation = Orientation.Vertical;
                        break;
                    default:
                        break;
                }

                var characters = new List<Char>(line);

                //  空の行だった場合、配置されずにずれるため、半角スペースを入れておく
                if (characters.Count <= 0)
                {
                    characters.Add(' ');
                }

                foreach (var character in characters)
                {
                    //  文字ごとにラベルを用意する
                    var charTextBlock = new TextBlock();

                    //  中心配置にする
                    charTextBlock.VerticalAlignment = VerticalAlignment.Center;
                    charTextBlock.HorizontalAlignment = HorizontalAlignment.Center;

                    //  todo 装飾を付加する必要あり、おそらくFormattedTextを使用する
                    charTextBlock.Text = character.ToString();

                    charTextBlock.FontSize = MojiData.FontSize;
                    if(MojiData.IsBold)
                    {
                        charTextBlock.FontWeight = FontWeights.Bold;
                    }
                    else
                    {
                        charTextBlock.FontWeight = FontWeights.Normal;
                    }
                    if(MojiData.IsItalic)
                    {
                        charTextBlock.FontStyle = FontStyles.Italic;
                    }
                    else
                    {
                        charTextBlock.FontStyle = FontStyles.Normal;
                    }

                    //  文字間隔を設定する
                    //  縦書き横書きで異なる
                    //  todo 縦書きの処理が必要
                    switch (MojiData.TextDirection)
                    {
                        case TextDirection.Yokogaki:
                            charTextBlock.Margin = new Thickness(0, 0, MojiData.CharacterMargin, 0);
                            break;
                        case TextDirection.Tategaki:
                            charTextBlock.Margin = new Thickness(0, 0, 0, MojiData.CharacterMargin);
                            if(TategakiRotateTargetCharacters.Contains(character))
                            {
                                charTextBlock.RenderTransformOrigin = new Point(0.5, 0.5);
                                charTextBlock.RenderTransform = tategakiTransformGroup;
                            }
                            break;
                        default:
                            break;
                    }

                    //  行パネルに追加する
                    linePanel.Children.Add(charTextBlock);
                }

                linePanels.Add(linePanel);
            }

            //  縦書きの場合、逆順に配置する必要あり
            if (MojiData.TextDirection == TextDirection.Tategaki)
            {
                linePanels.Reverse();

                //  縦書きの末尾の行の要素のマージンは不要、あった場合、マイナス値の場合文字が隠れていく
                linePanels.Last().Margin = new Thickness(0);
            }
            foreach (var linePanel in linePanels)
            {
                stackPanel.Children.Add(linePanel);
            }
        }
    }
}
