using Microsoft.Win32;
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
        private MojiPanel _mojiPanel;

        /// <summary>
        /// 画面を隠すことのみを示すフラグ
        /// 画面の本当の破棄処理を制御するために用意している
        /// </summary>
        public bool IsHideOnly { get; set; } = true;

        private bool _runEvent = false;

        public MojiWindow(MojiPanel mojiPanel)
        {
            this._mojiPanel= mojiPanel;

            InitializeComponent();

            FontFamilyComboBox.ItemsSource = FontUtil.GetFontTextBlocks();

            ShowTopMostCheckBox.IsChecked = mojiPanel.ShowTopmost;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            LoadMojiDataToWindow(_mojiPanel.MojiData);

            _runEvent = true;
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
            _runEvent = false;

            Title = $"[{mojiData.Id}] {mojiData.ExampleText}";
            IDLabel.Content = $"Moji ID:{mojiData.Id}";

            TextTextBox.Text = mojiData.FullText;

            LocationXTextBox.SetValue((int)mojiData.X, false);
            LocationYTextBox.SetValue((int)mojiData.Y, false);

            DirectionComboBox.SelectedIndex = (int)mojiData.TextDirection;
            RotateTextBox.SetValue((int)mojiData.RotateAngle);

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
            BorderBlurrRadiusTextBox.SetValue((int)mojiData.BorderBlurrRadius);

            SecondBorderThicknessTextBox.SetValue((int)mojiData.SecondBorderThickness);
            SecondBorderColorButton.Background = new SolidColorBrush(mojiData.SecondBorderColor);
            SecondBorderBlurrRadiusTextBox.SetValue((int)mojiData.SecondBorderBlurrRadius);

            BackgroundBoxCheckBox.IsChecked = mojiData.IsBackgroundBoxExists;
            BackgroundBoxColorButton.Background = new SolidColorBrush(mojiData.BackgroundBoxColor);
            BackgroundBoxPaddingTextBox.SetValue((int)mojiData.BackgroundBoxPadding);

            BackgroundBoxPaddingCornerRadiusTextBox.SetValue((int)mojiData.BackgroundBoxCornerRadius);

            BackgroundBoxBorderColorButton.Background = new SolidColorBrush(mojiData.BackgroundBoxBorderColor);
            BackgroundBoxBorderThicknessTextBox.SetValue((int)mojiData.BackgroundBoxBorderThickness);

            _runEvent = true;
        }

        public void UpdateXY(double x, double y)
        {
            _runEvent = false;

            LocationXTextBox.SetValue((int)x, false);
            LocationYTextBox.SetValue((int)y, false);

            _runEvent = true;
        }

        private void ReproductionButton_Click(object sender, RoutedEventArgs e)
        {
            _mojiPanel.Reproduction();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.ShowOKCancelDialog("文字を削除してよろしいですか？") == false) return;

            _mojiPanel.Remove();
        }

        public void UpdateMojiView(bool isTextDecoraitonUpdated)
        {
            _mojiPanel.MojiData.FullText = TextTextBox.Text;
            Title = $"[{_mojiPanel.MojiData.Id}] {_mojiPanel.MojiData.ExampleText}";
            _mojiPanel.MojiData.FontSize = FontSizeTextBox.Value;
            _mojiPanel.MojiData.X = LocationXTextBox.Value;
            _mojiPanel.MojiData.Y = LocationYTextBox.Value;
            _mojiPanel.MojiData.TextDirection = (TextDirection)DirectionComboBox.SelectedIndex;
            _mojiPanel.MojiData.IsBold = (BoldCheckBox.IsChecked == true);
            _mojiPanel.MojiData.IsItalic = (ItalicCheckBox.IsChecked == true);
            _mojiPanel.MojiData.LineMargin = LineMarginTextBox.Value;
            _mojiPanel.MojiData.CharacterMargin = CharacterMarginTextBox.Value;
            _mojiPanel.MojiData.FontFamilyName = (string)FontFamilyComboBox.SelectedValue;
            _mojiPanel.MojiData.BorderThickness = BorderThicknessTextBox.Value;
            _mojiPanel.MojiData.BorderBlurrRadius = BorderBlurrRadiusTextBox.Value;
            _mojiPanel.MojiData.SecondBorderThickness = SecondBorderThicknessTextBox.Value;
            _mojiPanel.MojiData.SecondBorderBlurrRadius = SecondBorderBlurrRadiusTextBox.Value;
            _mojiPanel.MojiData.IsBackgroundBoxExists = (BackgroundBoxCheckBox.IsChecked == true);
            _mojiPanel.MojiData.BackgroundBoxPadding = BackgroundBoxPaddingTextBox.Value;
            _mojiPanel.MojiData.BackgroundBoxBorderThickness = BackgroundBoxBorderThicknessTextBox.Value;
            _mojiPanel.MojiData.BackgroundBoxCornerRadius = BackgroundBoxPaddingCornerRadiusTextBox.Value;
            _mojiPanel.MojiData.RotateAngle = RotateTextBox.Value;

            _mojiPanel.UpdateMojiView(isTextDecoraitonUpdated);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_runEvent == false) return;

            UpdateMojiView(false);
        }

        private void TextBox_ValueChanged(object sender, UpDownTextBoxEvent e)
        {
            if (_runEvent == false) return;

            UpdateMojiView(true);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_runEvent == false) return;

            UpdateMojiView(true);
        }

        private void CheckBox_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (_runEvent == false) return;

            UpdateMojiView(true);
        }

        private void ColorButton_Click(Color currentColor, Action<Color> action)
        {
            if (_runEvent == false) return;

            ColorSelector.ColorSelectorWindow colorSelectorWindow = new ColorSelector.ColorSelectorWindow(currentColor, action);
            colorSelectorWindow.Top = Top;
            colorSelectorWindow.Left = Left;
            colorSelectorWindow.Topmost = Topmost;
            var dialogResult = colorSelectorWindow.ShowDialog();

            if (dialogResult.HasValue == false || dialogResult.Value == false)
            {
                //  色を元に戻す
                action(currentColor);
            }
        }

        private void ForeColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorButton_Click(_mojiPanel.MojiData.ForeColor, (color) =>
            {
                _mojiPanel.MojiData.ForeColor = color;
                ((Button)sender).Background = new SolidColorBrush(color);
                _mojiPanel.UpdateMojiView(true);
            });
        }

        private void BorderColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorButton_Click(_mojiPanel.MojiData.BorderColor, (color) =>
            {
                _mojiPanel.MojiData.BorderColor = color;
                ((Button)sender).Background = new SolidColorBrush(color);
                _mojiPanel.UpdateMojiView(true);
            });
        }

        private void SecondBorderColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorButton_Click(_mojiPanel.MojiData.SecondBorderColor, (color) =>
            {
                _mojiPanel.MojiData.SecondBorderColor = color;
                ((Button)sender).Background = new SolidColorBrush(color);
                _mojiPanel.UpdateMojiView(true);
            });
        }

        private void BackgroundBoxColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorButton_Click(_mojiPanel.MojiData.BackgroundBoxColor, (color) =>
            {
                _mojiPanel.MojiData.BackgroundBoxColor = color;
                ((Button)sender).Background = new SolidColorBrush(color);
                _mojiPanel.UpdateMojiView(true);
            });
        }

        private void BackgroundBoxBorderColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorButton_Click(_mojiPanel.MojiData.BackgroundBoxBorderColor, (color) =>
            {
                _mojiPanel.MojiData.BackgroundBoxBorderColor = color;
                ((Button)sender).Background = new SolidColorBrush(color);
                _mojiPanel.UpdateMojiView(true);
            });
        }

        private void ShowTopMostCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (ShowTopMostCheckBox.IsChecked.HasValue == false) return;

            _mojiPanel.ShowTopmost = ShowTopMostCheckBox.IsChecked.Value;
            Topmost = _mojiPanel.ShowTopmost;
        }

        private void SaveFormatButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = DataIO.GetMojiFormatDirPath();
                saveFileDialog.Filter = "moji format files|*.xml";
                var dialogResult = saveFileDialog.ShowDialog();

                if (dialogResult.HasValue == false || dialogResult.Value == false) return;

                //  保存する文字データの本文は、ファイル名と同じにする
                MojiData formatMojiData = _mojiPanel.MojiData.Clone();
                formatMojiData.FullText = System.IO.Path.GetFileNameWithoutExtension(saveFileDialog.FileName);

                DataIO.WriteMojiFormat(formatMojiData, saveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                MainWindow.ShowError("文字フォーマット保存エラー", ex);
            }
        }

        private void LoadFormatButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = DataIO.GetMojiFormatDirPath();
                openFileDialog.Filter = "moji format files|*.xml";
                var dialogResult = openFileDialog.ShowDialog();

                if (dialogResult.HasValue == false || dialogResult.Value == false) return;

                var formatMojiData = DataIO.ReadMojiData(openFileDialog.FileName);

                //  IDと座標、テキストはそのままにしておく、他はコピーする
                formatMojiData.Id = _mojiPanel.MojiData.Id;
                formatMojiData.X = _mojiPanel.MojiData.X;
                formatMojiData.Y = _mojiPanel.MojiData.Y;
                formatMojiData.FullText = _mojiPanel.MojiData.FullText;   
                _mojiPanel.MojiData.Copy(formatMojiData);

                LoadMojiDataToWindow(_mojiPanel.MojiData);

                _mojiPanel.UpdateMojiView(true);
            }
            catch (Exception ex)
            {
                MainWindow.ShowError("文字フォーマット読み出しエラー", ex);
            }
        }
    }
}
