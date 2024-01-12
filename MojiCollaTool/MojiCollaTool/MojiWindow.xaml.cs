using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MojiCollaTool
{
    /// <summary>
    /// MojiWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MojiWindow : Window
    {
        private MojiPanel mojiPanel;

        private MainWindow? mainWindow;

        public bool IsHideOnly = true;

        private bool runEvent = false;

        public MojiWindow(MojiPanel mojiPanel)
        {
            this.mojiPanel= mojiPanel;

            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            LoadMojiDataToWindow(mojiPanel.MojiData);

            runEvent = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //  画面が閉じられても、隠すだけにしている
            //  ただしこの場合だとアプリが終わってもウィンドウが保持されたままとなりアプリが終了しない
            //  隠すのをやめるフラグを設定することで終了可能にする
            if(IsHideOnly)
            {
                e.Cancel = true;
                Hide();
            }
        }

        public void LoadMojiDataToWindow(MojiData mojiData)
        {
            runEvent = false;

            Title = $"ID:{mojiData.Id} {mojiData.ExampleText}";
            IDLabel.Content = $"Moji ID:{mojiData.Id}";
            TextTextBox.Text = mojiData.FullText;
            FontSizeTextBox.SetValue(mojiData.FontSize, false);

            runEvent = true;
        }

        public void UpdateXY(double x, double y)
        {
            runEvent = false;

            LocationXTextBox.SetValue((int)x, false);
            LocationYTextBox.SetValue((int)y, false);

            runEvent = true;
        }

        private void TextTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (runEvent == false) return;

            mojiPanel.MojiData.FullText = TextTextBox.Text;
            Title = $"ID:{mojiPanel.MojiData.Id} {mojiPanel.MojiData.ExampleText}";

            mojiPanel.UpdateMojiView();
        }

        private void FontSizeTextBox_ValueChanged(object sender, UpDownTextBoxEvent e)
        {
            if (runEvent == false) return;

            mojiPanel.MojiData.FontSize = e.Value;
            mojiPanel.UpdateMojiView();
        }

        private void LocationTextBox_ValueChanged(object sender, UpDownTextBoxEvent e)
        {
            if (runEvent == false) return;

            mojiPanel.MojiData.X = LocationXTextBox.Value;
            mojiPanel.MojiData.Y = LocationYTextBox.Value;
            mojiPanel.UpdateXYView();
        }
    }
}
