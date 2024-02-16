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

        public bool IsHideOnly = true;

        private bool runEvent = false;

        public MojiWindow(MojiPanel mojiPanel)
        {
            this.mojiPanel= mojiPanel;

            InitializeComponent();

            FontFamilyComboBox.ItemsSource = FontUtil.FontTextBlocks;
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

            Title = $"[{mojiData.Id}] {mojiData.ExampleText}";
            IDLabel.Content = $"Moji ID:{mojiData.Id}";
            TextTextBox.Text = mojiData.FullText;
            LocationXTextBox.SetValue((int)mojiData.X, false);
            LocationYTextBox.SetValue((int)mojiData.Y, false);
            FontSizeTextBox.SetValue(mojiData.FontSize, false);
            if (FontUtil.GetFontFamilies().ContainsKey(mojiData.FontFamilyName))
            {
                FontFamilyComboBox.SelectedValue = mojiData.FontFamilyName;
            }
            BoldCheckBox.IsChecked = mojiData.IsBold;
            ItalicCheckBox.IsChecked = mojiData.IsItalic;
            CharacterMarginTextBox.SetValue((int)mojiData.CharacterMargin);
            LineMarginTextBox.SetValue((int)mojiData.LineMargin);
            ForeColorButton.Background = new SolidColorBrush(mojiData.ForeColor);

            runEvent = true;
        }

        public void UpdateXY(double x, double y)
        {
            runEvent = false;

            LocationXTextBox.SetValue((int)x, false);
            LocationYTextBox.SetValue((int)y, false);

            runEvent = true;
        }

        private void ReproductionButton_Click(object sender, RoutedEventArgs e)
        {
            mojiPanel.Reproduction();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            mojiPanel.Remove();
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

        private void DirectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (runEvent == false) return;

            mojiPanel.MojiData.TextDirection = (TextDirection)DirectionComboBox.SelectedIndex;
            mojiPanel.UpdateMojiView();
        }

        private void FontStyleCheckBox_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (runEvent == false) return;

            mojiPanel.MojiData.IsBold = (BoldCheckBox.IsChecked == true);
            mojiPanel.MojiData.IsItalic = (ItalicCheckBox.IsChecked == true);
            mojiPanel.UpdateMojiView();
        }

        private void MarginTextBox_ValueChanged(object sender, UpDownTextBoxEvent e)
        {
            if (runEvent == false) return;

            mojiPanel.MojiData.LineMargin = LineMarginTextBox.Value;
            mojiPanel.MojiData.CharacterMargin = CharacterMarginTextBox.Value;
            mojiPanel.UpdateMojiView();
        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mojiPanel.MojiData.FontFamilyName = (string)FontFamilyComboBox.SelectedValue;
            mojiPanel.UpdateMojiView();
        }

        private void ForeColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorSelector.ColorSelectorWindow colorSelectorWindow = new ColorSelector.ColorSelectorWindow(ForeBack.Fore, mojiPanel.MojiData.ForeColor, mojiPanel.MojiData.BackgroundColor);
            var dialogResult = colorSelectorWindow.ShowDialog();

            if(dialogResult.HasValue && dialogResult.Value) 
            {
                mojiPanel.MojiData.ForeColor = colorSelectorWindow.GetNextColor();
                ForeColorButton.Background = new SolidColorBrush(mojiPanel.MojiData.ForeColor);
                mojiPanel.UpdateMojiView();
            }
        }
    }
}
