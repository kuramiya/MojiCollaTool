using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
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
        /// <summary>
        /// 文字のID
        /// </summary>
        public int Id => MojiData.Id;

        /// <summary>
        /// 代表文字列
        /// </summary>
        public string ExampleText => MojiData.ExampleText;

        public MojiData MojiData { get; set; }

        public MojiWindow? MojiWindow { get; set; }

        /// <summary>
        /// 文字列を配置するパネル
        /// </summary>
        private StackPanel stackPanel = new StackPanel();

        private MainWindow mainWindow;

        private Nullable<Point> dragStart = null;

        /// <summary>
        /// パネルがダブルクリックされたことを示すフラグ
        /// </summary>
        private bool panelDoubleClicked = false;

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
            MojiWindow = new MojiWindow(this);

            stackPanel.VerticalAlignment = VerticalAlignment.Center;
            stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            AddChild(stackPanel);

            MouseDown += MojiPanel_MouseDown;
            MouseUp += MojiPanel_MouseUp;
            MouseMove += MojiPanel_MouseMove;
            MouseDoubleClick += MojiPanel_MouseDoubleClick;
            Unloaded += MojiPanel_Unloaded;

            UpdateMojiView();
        }

        /// <summary>
        /// 文字を複製する
        /// </summary>
        public void Reproduction()
        {
            mainWindow.ReproductionMoji(this);
        }

        /// <summary>
        /// 文字を削除する
        /// </summary>
        public void Remove()
        {
            mainWindow.RemoveMojiPanel(this);
        }

        private void MojiPanel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MojiWindow?.Show();

            //  フォーカスを設定する処理は別の処理で奪われるため、フラグだけ立てて後で処理させる
            panelDoubleClicked = true;
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

            //  パネルがダブルクリックされた場合の後処理
            //  文字画面へのフォーカスを設定する処理を行う
            if(panelDoubleClicked)
            {
                if(MojiWindow != null)
                {
                    MojiWindow.Topmost = true;
                    MojiWindow.Topmost = false;
                    MojiWindow.Activate();
                }

                panelDoubleClicked = false;
            }
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
                //  これにより、ウィンドウ保持によるアプリが終わらない問題を回避する
                MojiWindow.IsHideOnly = false;
                MojiWindow.Close();
                MojiWindow = null;
            }
        }

        /// <summary>
        /// 文字パネルの位置を更新sる
        /// </summary>
        public void UpdateXYView()
        {
            //  文字パネルの位置を設定する
            Margin = new Thickness(MojiData.X, MojiData.Y, 0, 0);
        }

        /// <summary>
        /// 文字の表示を更新する
        /// </summary>
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

                //  空の行だった場合、配置されずにずれるため、全角スペースを入れておく
                if (characters.Count <= 0)
                {
                    characters.Add('　');
                }

                foreach (var character in characters)
                {
                    //  縦書きのために、１文字ずつ文字を作成する
                    DecorationTextControl decorationTextControl = new DecorationTextControl(character, MojiData);

                    //  行パネルに追加する
                    linePanel.Children.Add(decorationTextControl);
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

            //  背景色を設定する
            if(MojiData.IsBackgroundBoxExists)
            {
                stackPanel.Background = new SolidColorBrush(MojiData.BackgroundBoxColor);
                //  todo    背景の幅の付け方を考える必要あり
            }
            else
            {
                //  透明色を設定しておく
                //  これによりマウスのヒットボックスが背景にも及ぶようになる
                stackPanel.Background = Brushes.Transparent;
            }
        }
    }
}
