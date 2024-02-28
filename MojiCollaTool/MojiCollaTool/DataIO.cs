﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
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
        /// 作業フォルダ内の画像を削除する
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
                //  作業フォルダ内の画像を先に削除する
                DeleteWorkingDirImage();

                var destImageFilePath = Path.Combine(GetWorkingDirPath(), Path.GetFileName(sourceImageFilePath));

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
                var filePath = Path.Combine(directoryPath, $"MojiData{mojiData.Id}.xml");
                using(var stream = File.OpenWrite(filePath))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(MojiData));
                    xmlSerializer.Serialize(stream, mojiData);
                }
            }
            catch (Exception ex)
            {
                var ioex = new InvalidOperationException("WriteMojiData error.", ex);
                ioex.Data.Add("MojiData", mojiData);
                throw ioex;
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
                using(var stream = File.OpenRead(filePath))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(MojiData));
                    var mojiDataObj = xmlSerializer.Deserialize(File.OpenRead(filePath));

                    if (mojiDataObj == null) throw new InvalidOperationException("ReadMojiData xml convert null error.");

                    mojiData = (MojiData)mojiDataObj;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"ReadMojiData {filePath} error.", ex);
            }

            return mojiData;
        }

        /// <summary>
        /// プロジェクトデータを出力する
        /// </summary>
        /// <param name="projectDirPath"></param>
        /// <param name="mojiDatas"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void WriteProjectData(string projectDirPath, IEnumerable<MojiData> mojiDatas)
        {
            try
            {
                //  保存ディレクトリを作成する
                Directory.CreateDirectory(projectDirPath);

                //  画像をコピーする
                var workingDirImagePath = GetWorkingDirImagePath();
                if (File.Exists(workingDirImagePath))
                {
                    File.Copy(workingDirImagePath, Path.Combine(projectDirPath, Path.GetFileName(workingDirImagePath)));
                }

                //  .mctoolファイルを作成、出力する
                var mcToolFilePath = Path.Combine(projectDirPath, Path.GetFileName(projectDirPath));

                StringBuilder mcToolFileText = new StringBuilder();

                mcToolFileText.AppendLine($"MojiCollaTool ver{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}");
                mcToolFileText.AppendLine($"SaveDateTime:{DateTime.Now}");

                File.WriteAllText(mcToolFilePath, mcToolFileText.ToString());

                //  文字データを出力する
                foreach (var mojiData in mojiDatas)
                {
                    WriteMojiData(mojiData, projectDirPath);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("WriteProjectData error.", ex);
            }
        }

        /// <summary>
        /// mctoolデータを読み出す
        /// </summary>
        /// <param name="mcToolFilePath"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static List<MojiData> ReadProjectData(string mcToolFilePath)
        {
            List<MojiData> mojiDatas = new List<MojiData>();

            try
            {
                //  読み出し対象のディレクトリパスを取得する
                var projectDirPath = Path.GetDirectoryName(mcToolFilePath);

                if (string.IsNullOrEmpty(projectDirPath)) throw new InvalidOperationException("projectDirPath error.");

                //  画像を作業フォルダにコピーする
                var imagePath = GetImagePath(projectDirPath);
                if(string.IsNullOrEmpty(imagePath))
                {
                    //  画像がない場合、作業フォルダの画像を削除する
                    DeleteWorkingDirImage();
                }
                else
                {
                    CopyImageToWorkingDirectory(imagePath);
                }

                //  文字データを読み出す
                foreach (var filePath in Directory.GetFiles(projectDirPath, "*.xml", SearchOption.TopDirectoryOnly))
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
                throw new InvalidOperationException("ReadProjectData error.", ex);
            }

            return mojiDatas;
        }
    }
}
