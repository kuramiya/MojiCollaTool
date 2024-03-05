using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Serialization;
using System.Xml.Serialization;

namespace MojiCollaTool
{
    public class DataIO
    {
        /// <summary>
        /// エラーログを書き出す
        /// </summary>
        /// <param name="message"></param>
        public static void WriteErrorLog(string message)
        {
            try
            {
                var errorLogDirPath = Path.Combine(GetExeDirPath(), "ErrorLog");

                if (Directory.Exists(errorLogDirPath) == false)
                {
                    Directory.CreateDirectory(errorLogDirPath);
                }

                var errorLogFilePath = Path.Combine(errorLogDirPath, $"ErrorLog {DateTime.Now:yyyyMMdd-HHmmss}.txt");

                File.WriteAllText(errorLogFilePath, message);
            }
            catch
            {
                //  何もしない
            }
        }

        /// <summary>
        /// exeのあるディレクトリパスを返す
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static string GetExeDirPath()
        {
            var assembly = System.Reflection.Assembly.GetEntryAssembly();

            if (assembly == null) throw new InvalidOperationException("GetExeDirPath error.");

            var dirPath = Path.GetDirectoryName(assembly.Location);

            if (dirPath == null) throw new InvalidOperationException("GetExeDirPath error.");

            return dirPath;
        }

        /// <summary>
        /// Workingディレクトリパスを返す
        /// </summary>
        /// <returns></returns>
        public static string GetWorkingDirPath()
        {
            return Path.Combine(GetExeDirPath(), "Working");
        }

        /// <summary>
        /// ディレクトリ内の最初の画像を返す
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static string GetImagePath(string dirPath)
        {
            try
            {
                foreach (var filePath in Directory.GetFiles(dirPath))
                {
                    var extension = Path.GetExtension(filePath);
                    if (extension == ".jpg" || extension == ".png") return filePath;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("GetWorkingDirImagePath error.", ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// Workingディレクトリにある画像のパスを返す
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static string GetWorkingDirImagePath()
        {
            return GetImagePath(GetWorkingDirPath());
        }

        /// <summary>
        /// 作業ディレクトリ内の画像を削除する
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static void DeleteWorkingDirImage()
        {
            try
            {
                foreach (var filePath in Directory.GetFiles(GetWorkingDirPath()))
                {
                    var extension = Path.GetExtension(filePath);
                    if (extension == ".jpg" || extension == ".png") File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("DeleteWorkingDirImage error.", ex);
            }
        }

        /// <summary>
        /// 作業ディレクトリを初期化する
        /// </summary>
        public static void InitWorkingDirectory()
        {
            try
            {
                var workingDirPath = GetWorkingDirPath();

                if (Directory.Exists(workingDirPath))
                {
                    //  作業ディレクトリがある場合、中のファイルを削除する
                    foreach (var filePath in Directory.GetFiles(workingDirPath))
                    {
                        File.Delete(filePath);
                    }
                }
                else
                {
                    //  作業ディレクトリがない場合、作業ディレクトリを作成する
                    Directory.CreateDirectory(workingDirPath);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("InitWorkingDirectory error.", ex);
            }
        }

        /// <summary>
        /// 読み込んだ画像をWorkingディレクトリにコピーする
        /// </summary>
        /// <param name="sourceImageFilePath"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void CopyImageToWorkingDirectory(string sourceImageFilePath)
        {
            try
            {
                var destImageFilePath = Path.Combine(GetWorkingDirPath(), Path.GetFileName(sourceImageFilePath));

                //  同じファイルの場合は何もしない
                if(sourceImageFilePath == destImageFilePath) return;

                File.Copy(sourceImageFilePath, destImageFilePath, true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("CopyImageToWorkingDirectory error.", ex);
            }
        }

        /// <summary>
        /// XML形式でファイルを書き出す
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        public static void WriteXMLData<T>(T data, string filePath)
        {
            using (var stream = new StreamWriter(filePath, false, new UTF8Encoding(false)))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(stream, data);
            }
        }

        /// <summary>
        /// XML形式のファイルを読み出す
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static T ReadXMLData<T>(string filePath)
        {
            using (var stream = new StreamReader(filePath, new UTF8Encoding(false)))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                var obj = xmlSerializer.Deserialize(stream);

                if (obj == null) throw new InvalidOperationException("XML convert null error.");

                return (T)obj;
            }
        }

        /// <summary>
        /// 文字データをファイルに出力する
        /// </summary>
        /// <param name="mojiData"></param>
        /// <param name="dirPath"></param>
        public static void WriteMojiData(MojiData mojiData, string dirPath)
        {
            try
            {
                var filePath = Path.Combine(dirPath, $"MojiData{mojiData.Id}.xml");
                WriteXMLData(mojiData, filePath);
            }
            catch (Exception ex)
            {
                var ioex = new InvalidOperationException("WriteMojiData error.", ex);
                ioex.Data.Add("MojiData", mojiData);
                throw ioex;
            }
        }

        /// <summary>
        /// 文字データをファイルに出力する
        /// </summary>
        /// <param name="mojiDatas"></param>
        /// <param name="dirPath"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void WriteMojiDatas(IEnumerable<MojiData> mojiDatas, string dirPath)
        {
            try
            {
                foreach (var mojiData in mojiDatas)
                {
                    WriteMojiData(mojiData, dirPath);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("WriteMojiDatasToWorkingDir error.", ex);
            }
        }

        /// <summary>
        /// 文字データを読み出す
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static MojiData ReadMojiData(string filePath)
        {
            MojiData mojiData;

            try
            {
                mojiData = ReadXMLData<MojiData>(filePath);

                mojiData.RestoreFullTextNewLine();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"ReadMojiData {filePath} error.", ex);
            }

            return mojiData;
        }

        /// <summary>
        /// 文字データをディレクトリから読み出す
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static List<MojiData> ReadMojiDatas(string dirPath)
        {
            List<MojiData> mojiDatas = new List<MojiData>();

            try
            {
                //  文字データを読み出す
                foreach (var filePath in Directory.GetFiles(dirPath, "MojiData*.xml", SearchOption.TopDirectoryOnly))
                {
                    var mojiData = ReadMojiData(filePath);

                    //  ID重複チェック
                    //  重複している場合、存在する最大のID+1の値に設定する
                    if (mojiDatas.Exists(x => x.Id == mojiData.Id))
                    {
                        mojiData.Id = mojiDatas.Max(x => x.Id) + 1;
                    }

                    mojiDatas.Add(mojiData);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("ReadMojiDatas error.", ex);
            }

            return mojiDatas;
        }

        /// <summary>
        /// 文字データを作業ディレクトリから読み出す
        /// </summary>
        /// <returns></returns>
        public static List<MojiData> ReadMojiDatasFromWorkingDir()
        {
            return ReadMojiDatas(GetWorkingDirPath());
        }

        /// <summary>
        /// 情報テキストファイルを書き込む
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static void WriteInfoTextToWorkingDir(string filePath)
        {
            try
            {
                StringBuilder infoText = new StringBuilder();

                infoText.AppendLine($"SoftwareName:MojiCollaTool");
                infoText.AppendLine($"SoftwareVersion:{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}");
                infoText.AppendLine($"TimeStamp:{DateTime.Now}");

                File.WriteAllText(filePath, infoText.ToString());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("WriteInfoTextToWorkingDir error.", ex);
            }
        }

        /// <summary>
        /// 作業ディレクトリをプロジェクトファイルとして書き込む
        /// </summary>
        /// <param name="projectFilePath"></param>
        /// <param name="mojiDatas"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void WriteWorkingDirToProjectDataFile(string projectFilePath, IEnumerable<MojiData> mojiDatas, CanvasData canvasData)
        {
            try
            {
                var workingDirPath = GetWorkingDirPath();

                WriteInfoTextToWorkingDir(Path.Combine(workingDirPath, "Info.txt"));

                WriteMojiDatas(mojiDatas, workingDirPath);

                WriteCanvasData(canvasData, workingDirPath);

                //  zipファイル処理は上書きオプションがないため、同じファイルがある場合は削除してから書き込む
                if(File.Exists(projectFilePath))
                {
                    File.Delete(projectFilePath);
                }

                ZipFile.CreateFromDirectory(workingDirPath, projectFilePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("CompressWorkingDirToProjectDataFile error.", ex);
            }
        }

        /// <summary>
        /// プロジェクトファイルを読み出し作業ファイルに展開する
        /// </summary>
        /// <param name="projectFilePath"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void ReadProjectDataToWorkingDir(string projectFilePath)
        {
            try
            {
                ZipFile.ExtractToDirectory(projectFilePath, GetWorkingDirPath(), true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("ReadProjectData error.", ex);
            }
        }

        /// <summary>
        /// キャンバスデータを書き込む
        /// </summary>
        /// <param name="canvasData"></param>
        /// <param name="dirPath"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void WriteCanvasData(CanvasData canvasData, string dirPath)
        {
            try
            {
                var filePath = Path.Combine(dirPath, $"CanvasData.xml");
                WriteXMLData(canvasData, filePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("WriteCanvasData error.", ex);
            }
        }

        /// <summary>
        /// キャンバスデータを読み出す
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static CanvasData ReadCanvasData(string dirPath)
        {
            try
            {
                var filePath = Path.Combine(dirPath, $"CanvasData.xml");
                return ReadXMLData<CanvasData>(filePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("ReadCanvasData error.", ex);
            }
        }

        /// <summary>
        /// 作業ディレクトリからキャンバスデータを読み出す
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static CanvasData ReadCanvasDataFromWorkingDir()
        {
            try
            {
                return ReadCanvasData(GetWorkingDirPath());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("ReadCanvasDataFromWorkingDir error.", ex);
            }
        }
    }
}
