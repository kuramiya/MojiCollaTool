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

        /// <summary>
        /// キャンバス設定画面
        /// </summary>
        private CanvasEditWindow _canvasEditWindow = null!;

        /// <summary>
        /// 最後にダイアログで使用したディレクトリ
        /// </summary>
        private string? lastUsedDirectory = string.Empty;

        private bool _runEvent = false;

        public MainWindow()
        {
            InitializeComponent();

            lastUsedDirectory = DataIO.GetExeDirPath();

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

        /// <summary>
        /// 初期化ボタンの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mojiPanels.Count > 0)
            {
                var checkDialogResult = ShowOKCancelDialog("文字データが存在しています。削除しても問題ありませんか？");
                if (checkDialogResult == false) return;

                RemoveAllMojiPanel();
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (string.IsNullOrEmpty(lastUsedDirectory) == false) openFileDialog.InitialDirectory = lastUsedDirectory;
            openFileDialog.Filter = "image files|*.jpg;*.png;";
            var dialogResult = openFileDialog.ShowDialog();

            if (dialogResult.HasValue == false || dialogResult.Value == false) return;

            try
            {
                //  キャンバス操作画面を閉じる
                _canvasEditWindow?.Close();

                //  キャンバスデータを初期化する
                CanvasData.Init();

                //  作業ディレクトリを初期化する
                DataIO.InitWorkingDirectory();

                //  今の画像の表示を削除する
                UnloadImage(ImageControl1);
                UnloadImage(ImageControl2);

                //  画像1を画面に表示する
                CanvasData.ImageData1 =  LoadImage(ImageControl1, openFileDialog.FileName);

                //  キャンバスサイズを更新する
                CanvasData.UpdateCanvasSize();

                //  キャンバス表示を更新する
                UpdateCanvas();

                //  画像を作業ディレクトリにコピーする
                DataIO.CopyImageToWorkingDirectory(1, openFileDialog.FileName);

                lastUsedDirectory = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
            }
            catch (Exception ex)
            {
                ShowError("初期化エラー", ex);
                return;
            }
        }

        /// <summary>
        /// 画像入れ替えボタンの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwapImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (string.IsNullOrEmpty(lastUsedDirectory) == false) openFileDialog.InitialDirectory = lastUsedDirectory;
            openFileDialog.Filter = "image files|*.jpg;*.png;";
            var dialogResult = openFileDialog.ShowDialog();

            if (dialogResult.HasValue == false || dialogResult.Value == false) return;

            SwapImage(openFileDialog.FileName);
        }

        /// <summary>
        /// 画像のみの入れ替えを行う
        /// </summary>
        /// <param name="filePath"></param>
        private void SwapImage(string filePath)
        {
            try
            {
                //  キャンバス操作画面を閉じる
                _canvasEditWindow?.Close();

                //  キャンバスデータを初期化する
                CanvasData.Init();

                //  作業ディレクトリ内の画像を削除する
                DataIO.DeleteAllWorkingDirImage(filePath);

                //  今の画像の表示を削除する
                UnloadImage(ImageControl1);
                UnloadImage(ImageControl2);

                //  画像1を画面に表示する
                CanvasData.ImageData1 = LoadImage(ImageControl1, filePath);

                //  キャンバスサイズを更新する
                CanvasData.UpdateCanvasSize();

                //  キャンバス表示を更新する
                UpdateCanvas();

                //  画像を作業ディレクトリにコピーする
                DataIO.CopyImageToWorkingDirectory(1, filePath);

                lastUsedDirectory = System.IO.Path.GetDirectoryName(filePath);
            }
            catch (Exception ex)
            {
                ShowError("画像入れ替えエラー", ex);
            }
        }

        /// <summary>
        /// 画像を並べるボタンの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MultiImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (string.IsNullOrEmpty(lastUsedDirectory) == false) openFileDialog.InitialDirectory = lastUsedDirectory;
            openFileDialog.Filter = "image files|*.jpg;*.png;";
            var dialogResult = openFileDialog.ShowDialog();

            if (dialogResult.HasValue == false || dialogResult.Value == false) return;

            //  画像1が存在していない場合、画像1から設定する
            //  画像入れ替えと同じ処理を行なう
            if (CanvasData.ImageData1.IsNullData())
            {
                SwapImage(openFileDialog.FileName);
                return;
            }

            try
            {
                //  キャンバス操作画面を閉じる
                _canvasEditWindow?.Close();

                //  画像2の画像データを初期化する
                CanvasData.ImageData2.Init();

                //  作業ディレクトリ内の画像2データを削除する
                DataIO.DeleteWorkingDirImage(2, openFileDialog.FileName);

                //  今の画像2の表示を削除する
                UnloadImage(ImageControl2);

                //  画像2を画面に表示する
                CanvasData.ImageData2 = LoadImage(ImageControl2, openFileDialog.FileName);

                //  画像の連結に合わせて、サイズを調整する
                CanvasData.ModifyImageSize();

                //  キャンバスサイズを更新する
                CanvasData.UpdateCanvasSize();

                //  キャンバス表示を更新する
                UpdateCanvas();

                //  画像を作業ディレクトリにコピーする
                DataIO.CopyImageToWorkingDirectory(2, openFileDialog.FileName);

                lastUsedDirectory = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
            }
            catch (Exception ex)
            {
                ShowError("画像追加エラー", ex);
            }
        }

        /// <summary>
        /// 画像を読み出し、表示する
        /// </summary>
        /// <param name="image"></param>
        /// <param name="filePath"></param>
        /// <returns>画像データ</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private ImageData LoadImage(Image image, string filePath)
        {
            try
            {
                var imageSource = ImageUtil.LoadImageSource2(filePath);

                image.Source = imageSource;

                return new ImageData((int)imageSource.Width, (int)imageSource.Height);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("画像読み出し、表示エラー", ex);
            }
        }

        /// <summary>
        /// 画像を非表示にする
        /// </summary>
        /// <param name="image"></param>
        private void UnloadImage(Image image)
        {
            image.Source = null;
        }

        /// <summary>
        /// プロジェクトロードボタンの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mojiPanels.Count > 0)
            {
                var checkDialogResult = ShowOKCancelDialog("文字データが存在しています。置き換えても問題ありませんか？");
                if (checkDialogResult == false) return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (string.IsNullOrEmpty(lastUsedDirectory) == false) openFileDialog.InitialDirectory = lastUsedDirectory;
            openFileDialog.Filter = "mctzip project file|*.mctzip";
            var dialogResult = openFileDialog.ShowDialog();

            if (dialogResult.HasValue == false || dialogResult.Value == false) return;

            LoadProject(openFileDialog.FileName);
        }

        /// <summary>
        /// プロジェクトファイルをロードする
        /// </summary>
        /// <param name="filePath"></param>
        private void LoadProject(string filePath)
        {
            try
            {
                //  キャンバス操作画面を閉じる
                _canvasEditWindow?.Close();

                //  今ある文字を削除する
                RemoveAllMojiPanel();

                //  今の画像の表示を削除する
                UnloadImage(ImageControl1);
                UnloadImage(ImageControl2);

                //  作業ディレクトリを初期化する
                DataIO.InitWorkingDirectory();

                //  プロジェクトファイルを作業ディレクトリに展開する
                DataIO.ReadProjectDataToWorkingDir(filePath);

                //  画像1を画面に表示する
                var workingDirImagePath = DataIO.GetWorkingDirImagePath(1);
                if (string.IsNullOrEmpty(workingDirImagePath) == false)
                {
                    LoadImage(ImageControl1, workingDirImagePath);
                }

                //  画像2を画面に表示する
                workingDirImagePath = DataIO.GetWorkingDirImagePath(2);
                if (string.IsNullOrEmpty(workingDirImagePath) == false)
                {
                    LoadImage(ImageControl2, workingDirImagePath);
                }

                //  作業ディレクトリからキャンバスデータを読み出す
                CanvasData = DataIO.ReadCanvasDataFromWorkingDir();

                //  キャンバスデータを画面に反映する
                UpdateCanvas();

                //  作業ディレクトリから文字データを読み出す
                var mojiDatas = DataIO.ReadMojiDatasFromWorkingDir();

                //  文字データを表示する
                foreach (var mojiData in mojiDatas)
                {
                    AddMojiPanel(new MojiPanel(mojiData, this));
                }

                lastUsedDirectory = System.IO.Path.GetDirectoryName(filePath);
            }
            catch (Exception ex)
            {
                ShowError($"{filePath} プロジェクト読み出しエラー", ex);
            }
        }

        private void SaveProjectButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (string.IsNullOrEmpty(lastUsedDirectory) == false) saveFileDialog.InitialDirectory = lastUsedDirectory;
            saveFileDialog.Filter = "mctzip project file|*.mctzip";
            saveFileDialog.FileName = $"MCToolProject{DateTime.Now:yyyyMMdd-HHmmss}.mctzip";

            var dialogResult = saveFileDialog.ShowDialog();

            if (dialogResult.HasValue == false || dialogResult.Value == false) return;

            try
            {
                DataIO.WriteWorkingDirToProjectDataFile(saveFileDialog.FileName, _mojiPanels.Select(x => x.MojiData), CanvasData);

                lastUsedDirectory = System.IO.Path.GetDirectoryName(saveFileDialog.FileName);

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
        public static void ShowError(string message, Exception? ex = null)
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

        private void OutputImageButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (string.IsNullOrEmpty(lastUsedDirectory) == false) saveFileDialog.InitialDirectory = lastUsedDirectory;
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

                lastUsedDirectory = System.IO.Path.GetDirectoryName(saveFileDialog.FileName);

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
                _canvasEditWindow = new CanvasEditWindow(CanvasData, this);
                _canvasEditWindow.Show();
            }
        }

        /// <summary>
        /// キャンバスを更新する
        /// </summary>
        /// <param name="canvasData"></param>
        public void UpdateCanvas()
        {
            if(CanvasData.ImageData1.IsNullData() == false)
            {
                ImageControl1.Width = CanvasData.ImageData1.ModifiedWidth;
                ImageControl1.Height = CanvasData.ImageData1.ModifiedHeight;
            }

            if(CanvasData.ImageData2.IsNullData() == false)
            {
                ImageControl2.Width = CanvasData.ImageData2.ModifiedWidth;
                ImageControl2.Height = CanvasData.ImageData2.ModifiedHeight;
            }

            MainCanvas.Width = CanvasData.CanvasWidth;
            MainCanvas.Height = CanvasData.CanvasHeight;

            CanvasBackgroundRect.Fill = new SolidColorBrush(CanvasData.CanvasColor);

            CanvasBackgroundRect.Width = CanvasData.CanvasWidth;
            CanvasBackgroundRect.Height = CanvasData.CanvasHeight;

            ImageControl1.Margin = CanvasData.GetImage1Margin();
            ImageControl2.Margin = CanvasData.GetImage2Margin();
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
