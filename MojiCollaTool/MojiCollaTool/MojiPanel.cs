using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MojiCollaTool
{
    public class MojiPanel : StackPanel
    {
        public int Id => MojiData.Id;

        public string ExampleText => MojiData.ExampleText;

        public MojiData MojiData { get; set; }

        public MojiWindow? MojiWindow { get; set; }

        private Nullable<Point> dragStart = null;

        public MojiPanel(int id, MainWindow mainWindow)
        {
            MojiData = new MojiData(id);
            MojiWindow = new MojiWindow(this);

            PreviewMouseDown += MojiPanel_PreviewMouseDown;
            Unloaded += MojiPanel_Unloaded;

            MouseButtonEventHandler mouseDown = (sender, args) => {
                var element = (UIElement)sender;
                dragStart = args.GetPosition(element);
                element.CaptureMouse();
            };
            MouseButtonEventHandler mouseUp = (sender, args) => {
                var element = (UIElement)sender;
                dragStart = null;
                element.ReleaseMouseCapture();
            };
            MouseEventHandler mouseMove = (sender, args) => {
                if (dragStart != null && args.LeftButton == MouseButtonState.Pressed)
                {
                    var element = (UIElement)sender;
                    var p2 = args.GetPosition(mainWindow.MainCanvas);
                    Canvas.SetLeft(element, p2.X - dragStart.Value.X);
                    Canvas.SetTop(element, p2.Y - dragStart.Value.Y);
                }
            };

            Action<UIElement> enableDrag = (element) => {
                element.MouseDown += mouseDown;
                element.MouseMove += mouseMove;
                element.MouseUp += mouseUp;
            };

            enableDrag(this);

            UpdateMojiView();
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

        public MojiPanel(MojiData mojiData, MainWindow mainWindow)
        {
            MojiData = mojiData;
            MojiWindow = new MojiWindow(this);

            PreviewMouseDown += MojiPanel_PreviewMouseDown;

            UpdateMojiView();
        }

        private void MojiPanel_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //  右ボタンで詳細表示
            if(e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                MojiWindow?.Show();
            }
        }

        public void UpdateMojiView()
        {
            //  文字パネルの中身を初期化する
            Children.Clear();

            //  文字パネルの位置を設定する
            Margin = new Thickness(MojiData.Location.X, MojiData.Location.Y, 0, 0);

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
                        linePanel.Margin = new Thickness(MojiData.LineMargin, 0, 0, 0);
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
                    var charLabel = new Label();

                    //  todo 装飾を付加する必要あり、おそらくFormattedTextを使用する
                    charLabel.Content = character;

                    charLabel.FontSize = MojiData.FontSize;
                    if(MojiData.IsBold)
                    {
                        charLabel.FontWeight = FontWeights.Bold;
                    }
                    else
                    {
                        charLabel.FontWeight = FontWeights.Normal;
                    }
                    if(MojiData.IsItalic)
                    {
                        charLabel.FontStyle = FontStyles.Italic;
                    }
                    else
                    {
                        charLabel.FontStyle = FontStyles.Normal;
                    }

                    //  文字間隔を設定する
                    //  縦書き横書きで異なる
                    //  todo 縦書きの処理が必要
                    switch (MojiData.TextDirection)
                    {
                        case TextDirection.Yokogaki:
                            charLabel.Margin = new Thickness(0, 0, MojiData.CharacterMargin, 0);
                            break;
                        case TextDirection.Tategaki:
                            charLabel.Margin = new Thickness(0, 0, 0, MojiData.CharacterMargin);
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
            if (MojiData.TextDirection == TextDirection.Tategaki)
            {
                linePanels.Reverse();
            }
            foreach (var linePanel in linePanels)
            {
                Children.Add(linePanel);
            }
        }
    }
}
