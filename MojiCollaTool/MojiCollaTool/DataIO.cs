using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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
        public static string GetWorkingDirImagePath()
        {
            return Path.Combine(GetWorkingDirPath(), "Image");
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

                if(Directory.Exists(GetWorkingDirPath()) == false)
                {
                    Directory.CreateDirectory(GetWorkingDirPath());
                }

                var destImageFilePath = $"{GetWorkingDirImagePath()}{extension}";

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
                Directory.CreateDirectory(mcToolDirPath);

                var mcToolFilePath = Path.Combine(mcToolDirPath, Path.GetFileName(mcToolDirPath));

                StringBuilder mcToolFileText = new StringBuilder();

                mcToolFileText.AppendLine($"MojiCollaTool ver{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}");
                mcToolFileText.AppendLine($"SaveDateTime:{DateTime.Now}");

                File.WriteAllText(mcToolFilePath, mcToolFileText.ToString());

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
    }
}
