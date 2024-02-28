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
            BorderThicknessTextBox.SetValue((int)mojiData.BorderThickness);
            BorderColorButton.Background = new SolidColorBrush(mojiData.BorderColor);

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

        public void UpdateMojiView()
        {
            mojiPanel.MojiData.FullText = TextTextBox.Text;
            Title = $"[{mojiPanel.MojiData.Id}] {mojiPanel.MojiData.ExampleText}";
            mojiPanel.MojiData.FontSize = FontSizeTextBox.Value;
            mojiPanel.MojiData.X = LocationXTextBox.Value;
            mojiPanel.MojiData.Y = LocationYTextBox.Value;
            mojiPanel.MojiData.TextDirection = (TextDirection)DirectionComboBox.SelectedIndex;
            mojiPanel.MojiData.IsBold = (BoldCheckBox.IsChecked == true);
            mojiPanel.MojiData.IsItalic = (ItalicCheckBox.IsChecked == true);
            mojiPanel.MojiData.LineMargin = LineMarginTextBox.Value;
            mojiPanel.MojiData.CharacterMargin = CharacterMarginTextBox.Value;
            mojiPanel.MojiData.FontFamilyName = (string)FontFamilyComboBox.SelectedValue;
            mojiPanel.MojiData.BorderThickness = BorderThicknessTextBox.Value;
            mojiPanel.MojiData.BorderBlurrRadius = BorderBlurrRadiusTextBox.Value;
            mojiPanel.MojiData.IsBackgroundBoxExists = (BackgroundCheckBox.IsChecked == true);
            mojiPanel.MojiData.BackgoundBoxPadding = BackgroundBoxPaddingTextBox.Value;

            mojiPanel.UpdateMojiView();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (runEvent == false) return;

            UpdateMojiView();
        }

        private void TextBox_ValueChanged(object sender, UpDownTextBoxEvent e)
        {
            if (runEvent == false) return;

            UpdateMojiView();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (runEvent == false) return;

            UpdateMojiView();
        }

        private void CheckBox_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (runEvent == false) return;

            UpdateMojiView();
        }


        private void ColorButton_Click(Color currentColor, Action<Color> action)
        {
            if (runEvent == false) return;

            ColorSelector.ColorSelectorWindow colorSelectorWindow = new ColorSelector.ColorSelectorWindow(currentColor, action);
            var dialogResult = colorSelectorWindow.ShowDialog();

            if (dialogResult.HasValue == false)
            {
                //  色を元に戻す
                action(currentColor);
            }
            else if(dialogResult.Value == false)
            {
                //  色を元に戻す
                action(currentColor);
            }
        }

        private void ForeColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorButton_Click(mojiPanel.MojiData.ForeColor, (color) =>
            {
                mojiPanel.MojiData.ForeColor = color;
                ((Button)sender).Background = new SolidColorBrush(color);
                mojiPanel.UpdateMojiView();
            });
        }

        private void BorderColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorButton_Click(mojiPanel.MojiData.BorderColor, (color) =>
            {
                mojiPanel.MojiData.BorderColor = color;
                ((Button)sender).Background = new SolidColorBrush(color);
                mojiPanel.UpdateMojiView();
            });
        }

        private void BackgroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorButton_Click(mojiPanel.MojiData.BackgroundBoxColor, (color) =>
            {
                mojiPanel.MojiData.BackgroundBoxColor = color;
                ((Button)sender).Background = new SolidColorBrush(color);
                mojiPanel.UpdateMojiView();
            });
        }
    }
}
