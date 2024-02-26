using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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
        /// Workingディレクトリにある画像のパスを返す
        /// </summary>
        /// <returns></returns>
        public static string GetWorkingDirImagePath(string extensionWithDot)
        {
            return Path.Combine(GetWorkingDirPath(), $"Image{extensionWithDot}");
        }

        /// <summary>
        /// 作業ディレクトリを初期化する
        /// </summary>
        public static void InitWorkingDirectory()
        {
            try
            {
                if (Directory.Exists(GetWorkingDirPath()))
                {
                    //  作業ディレクトリがある場合、中の画像を削除する
                    foreach (var extensionWithDot in new string[] { ".jpg", ".png" })
                    {
                        var imagePath = GetWorkingDirImagePath(extensionWithDot);
                        if (File.Exists(imagePath)) File.Delete(imagePath);
                    }
                }
                else
                {
                    //  作業ディレクトリがない場合、作業ディレクトリを作成する
                    Directory.CreateDirectory(GetWorkingDirPath());
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
                var extension = Path.GetExtension(sourceImageFilePath);

                InitWorkingDirectory();

                var destImageFilePath = GetWorkingDirImagePath(extension);

                File.Copy(sourceImageFilePath, destImageFilePath, true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("CopyImageToWorkingDirectory error.", ex);
            }
        }

        /// <summary>
        /// 文字データをファイルに出力する
        /// </summary>
        /// <param name="mojiData"></param>
        /// <param name="directoryPath"></param>
        public static void WriteMojiData(MojiData mojiData, string directoryPath)
        {
            try
            {
                var filePath = Path.Combine(directoryPath, $"MojiID{mojiData.Id}.xml");
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(MojiData));
                xmlSerializer.Serialize(File.OpenWrite(filePath), mojiData);
            }
            catch (Exception ex)
            {
                var ioex = new InvalidOperationException("WriteMojiData error.", ex);
                ioex.Data.Add("MojiData", mojiData);
                throw ioex;
            }
        }

        public static MojiData ReadMojiData(string filePath)
        {
            MojiData mojiData;

            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(MojiData));
                var mojiDataObj = xmlSerializer.Deserialize(File.OpenWrite(filePath));

                if (mojiDataObj == null) throw new InvalidOperationException("ReadMojiData xml convert null error.");

                mojiData = (MojiData)mojiDataObj;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"ReadMojiData {filePath} error.", ex);
            }

            return mojiData;
        }

        /// <summary>
        /// 作業データを出力する
        /// </summary>
        /// <param name="mcToolDirPath"></param>
        /// <param name="mojiDatas"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void WriteMCToolData(string mcToolDirPath, IEnumerable<MojiData> mojiDatas)
        {
            try
            {
                //  保存ディレクトリを作成する
                Directory.CreateDirectory(mcToolDirPath);

                //  画像をコピーする
                foreach (var extensionWithDot in new string[] { ".jpg", ".png" })
                {
                    var imagePath = GetWorkingDirImagePath(extensionWithDot);
                    if (File.Exists(imagePath))
                    {
                        File.Copy(imagePath, Path.Combine(mcToolDirPath, Path.GetFileName(imagePath)));
                    }
                }

                //  .mctoolファイルを作成、保存する
                var mcToolFilePath = Path.Combine(mcToolDirPath, Path.GetFileName(mcToolDirPath));

                StringBuilder mcToolFileText = new StringBuilder();

                mcToolFileText.AppendLine($"MojiCollaTool ver{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}");
                mcToolFileText.AppendLine($"SaveDateTime:{DateTime.Now}");

                File.WriteAllText(mcToolFilePath, mcToolFileText.ToString());

                //  文字データを保存する
                foreach (var mojiData in mojiDatas)
                {
                    WriteMojiData(mojiData, mcToolDirPath);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("WriteMCToolData error.", ex);
            }
        }

        public static List<MojiData> ReadMCToolData(string mcToolFilePath)
        {
            List<MojiData> mojiDatas = new List<MojiData>();

            try
            {
                //  読み出し対象のディレクトリパスを取得する
                var mcToolDirPath = Path.GetDirectoryName(mcToolFilePath);

                if (string.IsNullOrEmpty(mcToolDirPath)) throw new InvalidOperationException("mcToolDirPath error.");

                foreach (var filePath in Directory.GetFiles(mcToolDirPath, "*.xml", SearchOption.TopDirectoryOnly))
                {
                    mojiDatas.Add(ReadMojiData(filePath)); 
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("ReadMCToolData error.", ex);
            }

            return mojiDatas;
        }
    }
}
