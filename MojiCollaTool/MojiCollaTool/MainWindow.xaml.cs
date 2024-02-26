﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Security.AccessControl;

namespace MojiCollaTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<MojiPanel> mojiPanels = new ObservableCollection<MojiPanel>();

        private int nextMojiId = 1;

        private bool runEvent = false;

        public MainWindow()
        {
            InitializeComponent();

            Title = $"MojiCollaTool ver{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";

            ResetScale();
        }

        private void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.png;";
            var dialogResult = openFileDialog.ShowDialog();

            if (dialogResult.HasValue == false || dialogResult.Value == false) return;

            try
            {
                var imageSource = ImageUtil.LoadImageSource(openFileDialog.FileName);

                MainImage.Source = imageSource;

                MainCanvas.Width = imageSource.Width;
                MainCanvas.Height = imageSource.Height;

                ResetScale();

                DataIO.CopyImageToWorkingDirectory(openFileDialog.FileName);
            }
            catch (Exception ex)
            {
                ShowError("Image load error.", ex);
            }
        }

        private void ShowError(string message, Exception? ex = null)
        {
            StringBuilder logMessage = new StringBuilder();
            logMessage.AppendLine(message);
            if(ex != null)
            {
                logMessage.AppendLine(ex.ToString());
            }
            DataIO.WriteErrorLog(logMessage.ToString());

            StringBuilder dialogMessage = new StringBuilder();
            dialogMessage.AppendLine(message);
            if(ex != null)
            {
                dialogMessage.Append(ex.Message);
            }
            MessageBox.Show(dialogMessage.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void ShowInfoDialog(string message)
        {
            MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            var mojiPanel = new MojiPanel(nextMojiId, this);
            ++nextMojiId;

            mojiPanels.Add(mojiPanel);

            MainCanvas.Children.Add(mojiPanel);
        }

        public void ReproductionMoji(MojiPanel mojiPanel)
        {
            var reproductedMojiData = mojiPanel.MojiData.Reproduct(nextMojiId);
            ++nextMojiId;

            var reproductedMojiPanel = new MojiPanel(reproductedMojiData, this);

            mojiPanels.Add(reproductedMojiPanel);

            MainCanvas.Children.Add(reproductedMojiPanel);
        }

        public void RemoveMoji(MojiPanel mojiPanel)
        {
            mojiPanels.Remove(mojiPanel);

            MainCanvas.Children.Remove(mojiPanel);
        }

        private void MainImage_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this);
        }

        private void OutputImageButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG File|*.png|JPG File|*.jpg";
            saveFileDialog.FileName = $"MojiColla{DateTime.Now:yyyyMMdd-HHmmss}.png";
            var dialogResult = saveFileDialog.ShowDialog();

            if (dialogResult.HasValue == false || dialogResult.Value == false) return;

            double preScale_Perect = ScalingTextBox.Value;

            try
            {
                UpdateScale(100);

                if(saveFileDialog.FileName.EndsWith("jpg"))
                {
                    MainCanvas.ToImage(saveFileDialog.FileName, new JpegBitmapEncoder());
                }
                else
                {
                    MainCanvas.ToImage(saveFileDialog.FileName, new PngBitmapEncoder());
                }

                ShowInfoDialog($"{saveFileDialog.FileName} image exported.");
            }
            catch (Exception ex)
            {
                ShowError("Image create, output error.", ex);
            }
            finally
            {
                UpdateScale(preScale_Perect);
            }
        }

        private void ScalingTextBox_ValueChanged(object sender, UpDownTextBoxEvent e)
        {
            if (runEvent == false) return;

            UpdateScale(e.Value);
        }

        private void ResetScale()
        {
            runEvent = false;

            UpdateScale(100);

            ScalingTextBox.SetValue(100, false);

            runEvent = true;
        }

        private void UpdateScale(double scale_Percent)
        {
            CanvasScaleTransform.ScaleX = scale_Percent / 100.0;
            CanvasScaleTransform.ScaleY = scale_Percent / 100.0;
        }

        private void CanvasGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(e.Delta > 0)
            {
                ScalingTextBox.RunUpButton();
            }

            if(e.Delta < 0)
            {
                ScalingTextBox.RunDownButton();
            }
        }

        private void SaveProjectButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "MCTool File|*.mctool";

            var dialogResult = saveFileDialog.ShowDialog();

            if (dialogResult.HasValue == false || dialogResult.Value == false) return;

            try
            {
                DataIO.WriteMCToolData(saveFileDialog.FileName, mojiPanels.Select(x => x.MojiData));

                ShowInfoDialog($"{saveFileDialog.FileName} project saved.");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex);
            }
        }

        private void LoadProjectButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "mctool File|*.mctool";
            var dialogResult = openFileDialog.ShowDialog();

            if (dialogResult.HasValue == false || dialogResult.Value == false) return;

            try
            {

            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
