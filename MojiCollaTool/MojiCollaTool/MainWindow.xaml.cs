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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Security.AccessControl;
using System.Security.RightsManagement;
using System.IO.Enumeration;

namespace MojiCollaTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 文字リスト
        /// </summary>
        private List<MojiPanel> _mojiPanels = new List<MojiPanel>();

        /// <summary>
        /// 文字リストに表示するためのリスト
        /// </summary>
        private ObservableCollection<MojiPanel> _viewMojiPanels = new ObservableCollection<MojiPanel>();

        /// <summary>
        /// キャンバスに関する設定データ
        /// </summary>
        public CanvasData CanvasData { get; set; } = new CanvasData();

        private CanvasEditWindow _canvasEditWindow = null!;

        private bool _runEvent = false;

        public MainWindow()
        {
            InitializeComponent();

            Title = $"MojiCollaTool ver{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";

            MojiListView.ItemsSource = _viewMojiPanels;

            ResetScale();
        }

        /// <summary>
        /// 文字リストを更新する
        /// </summary>
        private void UpdateMojiList()
        {
            _viewMojiPanels.Clear();

            foreach (var mojiPanel in _mojiPanels)
            {
                _viewMojiPanels.Add(mojiPanel);
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            //  文字リスト更新処理の都合の良いタイミングが思いつかなかったので、画面がアクティブになるたびに更新するようにしている
            UpdateMojiList();
        }

        private void InitButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mojiPanels.Count > 0)
            {
                var checkDialogResult = ShowOKCancelDialog("文字データが存在しています。削除しても問題ありませんか？");
                if (checkDialogResult == false) return;

                RemoveAllMojiPanel();
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "image files|*.jpg;*.png;";
            var dialogResult = openFileDialog.ShowDialog();

            if (dialogResult.HasValue == false || dialogResult.Value == false) return;

            try
            {
                //  作業ディレクトリを初期化する
                DataIO.InitWorkingDirectory();

                //  画像を画面に表示する
                LoadImageToView(openFileDialog.FileName);

                //  画像を作業ディレクトリにコピーする
                DataIO.CopyImageToWorkingDirectory(openFileDialog.FileName);
            }
            catch (Exception ex)
            {
                ShowError("初期化エラー", ex);
                return;
            }
        }

        private void SwapImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "image files|*.jpg;*.png;";
            var dialogResult = openFileDialog.ShowDialog();

            if (dialogResult.HasValue == false || dialogResult.Value == false) return;

            SwapImage(openFileDialog.FileName);
        }

        private void SwapImage(string filePath)
        {
            try
            {
                //  作業ディレクトリ内の画像パスを取得する
                var workingDirImagePath = DataIO.GetWorkingDirImagePath();
                if(string.IsNullOrEmpty(workingDirImagePath) == false)
                {
                    if (filePath != workingDirImagePath)
                    {
                        //  パスが違うときのみ
                        //  作業ディレクトリ内の画像を削除する
                        DataIO.DeleteWorkingDirImage();
                    }
                }

                //  画像を画面に表示する
                LoadImageToView(filePath);

                //  画像を作業ディレクトリにコピーする
                DataIO.CopyImageToWorkingDirectory(filePath);

                ShowInfoDialog($"{filePath} 画像入れ替え完了");
            }
            catch (Exception ex)
            {
                ShowError("画像入れ替えエラー", ex);
            }
        }

        /// <summary>
        /// 画像を読み出し登録する
        /// </summary>
        /// <param name="filePath"></param>
        private void LoadImageToView(string filePath)
        {
            try
            {
                var imageSource = ImageUtil.LoadImageSource(filePath);

                MainImage.Source = imageSource;

                MainCanvas.Width = imageSource.Width;
                MainCanvas.Height = imageSource.Height;

                CanvasData.Init();
                CanvasData.Width = (int)imageSource.Width;
                CanvasData.Height = (int)imageSource.Height;
                UpdateCanvas(CanvasData);

                ResetScale();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("画像読み出し、表示エラー", ex);
            }
        }

        private void LoadProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mojiPanels.Count > 0)
            {
                var checkDialogResult = ShowOKCancelDialog("文字データが存在しています。置き換えても問題ありませんか？");
                if (checkDialogResult == false) return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "mctzip project file|*.mctzip";
            var dialogResult = openFileDialog.ShowDialog();

            if (dialogResult.HasValue == false || dialogResult.Value == false) return;

            LoadProject(openFileDialog.FileName);
        }

        private void LoadProject(string filePath)
        {
            try
            {
                //  今ある文字を削除する
                RemoveAllMojiPanel();

                //  作業ディレクトリを初期化する
                DataIO.InitWorkingDirectory();

                //  プロジェクトファイルを作業ディレクトリに展開する
                DataIO.ReadProjectDataToWorkingDir(filePath);

                //  作業ディレクトリから画像を読み出す
                var workingDirImagePath = DataIO.GetWorkingDirImagePath();
                if (string.IsNullOrEmpty(workingDirImagePath))
                {
                    //  画像がない場合、画像のソースを削除する
                    MainImage.Source = null;
                }
                else
                {
                    //  画像がある場合、表示する
                    LoadImageToView(workingDirImagePath);
                }

                //  作業ディレクトリからキャンバスデータを読み出す
                var canvasData = DataIO.ReadCanvasDataFromWorkingDir();

                //  キャンバスデータを画面に反映する
                UpdateCanvas(canvasData);

                //  作業ディレクトリから文字データを読み出す
                var mojiDatas = DataIO.ReadMojiDatasFromWorkingDir();

                //  文字データを表示する
                foreach (var mojiData in mojiDatas)
                {
                    AddMojiPanel(new MojiPanel(mojiData, this));
                }

                ShowInfoDialog($"{filePath} プロジェクト読み出し完了");
            }
            catch (Exception ex)
            {
                ShowError($"{filePath} プロジェクト読み出しエラー", ex);
            }
        }

        private void SaveProjectButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "mctzip project file|*.mctzip";
            saveFileDialog.FileName = $"MCToolProject{DateTime.Now:yyyyMMdd-HHmmss}.mctzip";

            var dialogResult = saveFileDialog.ShowDialog();

            if (dialogResult.HasValue == false || dialogResult.Value == false) return;

            try
            {
                DataIO.WriteWorkingDirToProjectDataFile(saveFileDialog.FileName, _mojiPanels.Select(x => x.MojiData), CanvasData);

                ShowInfoDialog($"{saveFileDialog.FileName} プロジェクト保存完了");
            }
            catch (Exception ex)
            {
                ShowError("プロジェクト保存エラー", ex);
            }
        }

        /// <summary>
        /// エラーメッセージを表示する
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
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
            MessageBox.Show(dialogMessage.ToString(), "エラー", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        /// <summary>
        /// 情報通知ダイアログを表示する
        /// </summary>
        /// <param name="message"></param>
        public static void ShowInfoDialog(string message)
        {
            MessageBox.Show(message, "インフォメーション", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// 確認ダイアログを表示する
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool ShowOKCancelDialog(string message)
        {
            var dialogResult = MessageBox.Show(message, "確認", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            switch (dialogResult)
            {
                case MessageBoxResult.OK:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 次に使用する文字のIDを返す
        /// </summary>
        /// <returns></returns>
        private int GetNextMojiId()
        {
            if (_mojiPanels.Count <= 0)
            {
                return 1;
            }
            else
            {
                return _mojiPanels.Max(x => x.Id + 1);
            }
        }

        private void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            var mojiPanel = new MojiPanel(GetNextMojiId(), this);

            AddMojiPanel(mojiPanel);
        }

        /// <summary>
        /// 文字を複製する
        /// </summary>
        /// <param name="mojiPanel"></param>
        public void ReproductionMoji(MojiPanel mojiPanel)
        {
            var reproductedMojiData = mojiPanel.MojiData.Reproduct(GetNextMojiId());

            var reproductedMojiPanel = new MojiPanel(reproductedMojiData, this);

            AddMojiPanel(reproductedMojiPanel);
        }

        /// <summary>
        /// 文字パネルを追加する
        /// </summary>
        /// <param name="mojiPanel"></param>
        private void AddMojiPanel(MojiPanel mojiPanel)
        {
            _mojiPanels.Add(mojiPanel);

            UpdateMojiList();

            MainCanvas.Children.Add(mojiPanel);
        }

        /// <summary>
        /// 文字パネルを削除する
        /// </summary>
        /// <param name="mojiPanel"></param>
        public void RemoveMojiPanel(MojiPanel mojiPanel)
        {
            _mojiPanels.Remove(mojiPanel);

            UpdateMojiList();

            MainCanvas.Children.Remove(mojiPanel);
        }

        /// <summary>
        /// すべての文字パネルを削除する
        /// </summary>
        public void RemoveAllMojiPanel()
        {
            while (_mojiPanels.Count > 0)
            {
                RemoveMojiPanel(_mojiPanels.First());
            }
        }

        private void MainImage_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this);
        }

        private void OutputImageButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "png file|*.png|jpg file|*.jpg";
            saveFileDialog.FileName = $"MojiColla{DateTime.Now:yyyyMMdd-HHmmss}.png";
            var dialogResult = saveFileDialog.ShowDialog();

            if (dialogResult.HasValue == false || dialogResult.Value == false) return;

            //  画像出力前の拡大縮小率を保管しておく
            double preScale_Perect = ScalingTextBox.Value;

            try
            {
                //  画像出力前に、100%サイズに戻す（あとで戻す）                
                UpdateScale(100);

                if(saveFileDialog.FileName.EndsWith("jpg"))
                {
                    MainCanvas.ToImage(saveFileDialog.FileName, new JpegBitmapEncoder());
                }
                else
                {
                    MainCanvas.ToImage(saveFileDialog.FileName, new PngBitmapEncoder());
                }

                ShowInfoDialog($"{saveFileDialog.FileName} 画像出力完了");
            }
            catch (Exception ex)
            {
                ShowError("画像出力エラー", ex);
            }
            finally
            {
                UpdateScale(preScale_Perect);
            }
        }

        private void ScalingTextBox_ValueChanged(object sender, UpDownTextBoxEvent e)
        {
            if (_runEvent == false) return;

            UpdateScale(e.Value);
        }

        private void ResetScale()
        {
            _runEvent = false;

            UpdateScale(100);

            ScalingTextBox.SetValue(100, false);

            _runEvent = true;
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

        private void MojiListView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender == null) return;

            var mojiPanel = ((ListView)sender).SelectedItem;

            ((MojiPanel)mojiPanel)?.ShowMojiWindow();
        }

        private void CanvasEditButton_Click(object sender, RoutedEventArgs e)
        {
            if(_canvasEditWindow == null || _canvasEditWindow.IsVisible == false)
            {
                CanvasEditWindow canvasEditWindow = new CanvasEditWindow((int)MainImage.ActualWidth, (int)MainImage.ActualHeight, CanvasData.Clone(), this);
                canvasEditWindow.Show();
            }
        }

        /// <summary>
        /// キャンバスを更新する
        /// </summary>
        /// <param name="canvasData"></param>
        public void UpdateCanvas(CanvasData canvasData)
        {
            CanvasData = canvasData;

            MainCanvas.Width = canvasData.Width;
            MainCanvas.Height = canvasData.Height;

            CanvasBackgroundRect.Fill = new SolidColorBrush(canvasData.Background);

            CanvasBackgroundRect.Width = canvasData.Width;
            CanvasBackgroundRect.Height = canvasData.Height;

            MainImage.Margin = new Thickness(canvasData.ImageLeftMargin, canvasData.ImageTopMargin, canvasData.ImageRightMargin, canvasData.ImageBottomMargin);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(_mojiPanels.Count > 0)
            {
                var dialogResult = ShowOKCancelDialog("文字データが存在しています。終了しても問題ありませんか？");
                if(dialogResult == false) e.Cancel = true;
            }
        }

        private void CanvasGrid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (filePaths.Length <= 0) return;

                //  最初のファイルのみ処理する
                var filePath = filePaths[0];

                switch(System.IO.Path.GetExtension(filePath))
                {
                    case ".jpg":
                    case ".png":
                        SwapImage(filePath);
                        break;
                    case ".mctzip":
                        if (_mojiPanels.Count > 0)
                        {
                            var checkDialogResult = ShowOKCancelDialog("文字データが存在しています。置き換えても問題ありませんか？");
                            if (checkDialogResult == false) return;
                        }
                        LoadProject(filePath);
                        break;
                    default:
                        return;
                }
            }
        }

        private void CanvasGrid_DragOver(object sender, DragEventArgs e)
        {
            //  ファイルの場合、マウスのエフェクトを変更する
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (filePaths.Length <= 0) return;

                //  最初のファイルのみ処理する
                var filePath = filePaths[0];

                switch (System.IO.Path.GetExtension(filePath))
                {
                    case ".jpg":
                    case ".png":
                    case ".mctzip":
                        e.Effects = DragDropEffects.All;
                        break;
                    default:
                        e.Effects = DragDropEffects.None;
                        return;
                }
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }
    }
}
