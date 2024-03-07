using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojiCollaTool
{
    [Serializable]
    public class ImageData
    {
        public string FileName { get; set; } = string.Empty;

        public double OriginalWidth { get; set; } = 0;

        public double OriginalHeight { get; set; } = 0;

        public double ModifiedWidth { get; set; } = 0;

        public double ModifiedHeight { get; set; } = 0;

        public ImageData()
        {
            //  何もしない
        }

        public ImageData(string fileName, double originalWidth, double originalHeight)
        {
            FileName = fileName;
            OriginalWidth = originalWidth;
            OriginalHeight = originalHeight;
            ResetSize();
        }

        /// <summary>
        /// サイズをリセットする
        /// </summary>
        public void ResetSize()
        {
            ModifiedWidth = OriginalWidth;
            ModifiedHeight = OriginalHeight;
        }

        /// <summary>
        /// サイズを更新する
        /// </summary>
        /// <param name="locatePosition"></param>
        /// <param name="imageData"></param>
        public void UpdateSize(LocatePosition locatePosition, ImageData imageData)
        {
            //  最初に初期化しておく
            ResetSize();
            imageData.ResetSize();

            switch (locatePosition)
            {
                case LocatePosition.Left:
                case LocatePosition.Right:
                    if(OriginalHeight > imageData.OriginalHeight)
                    {
                        //  自分の方が大きいので、自分を縮める
                        ModifiedHeight = imageData.OriginalHeight;
                        ModifiedWidth = OriginalWidth * (imageData.OriginalHeight / OriginalHeight);
                    }
                    else
                    {
                        //  自分の方が小さい場合、相手を縮める
                        imageData.ModifiedHeight = OriginalHeight;
                        imageData.ModifiedWidth = imageData.OriginalWidth * (OriginalHeight / imageData.OriginalHeight);
                    }
                    break;
                case LocatePosition.Top:
                case LocatePosition.Bottom:
                    if (OriginalWidth > imageData.OriginalWidth)
                    {
                        //  自分の方が大きいので、自分を縮める
                        ModifiedWidth = imageData.OriginalWidth;
                        ModifiedHeight = OriginalHeight * (imageData.OriginalWidth / OriginalWidth);
                    }
                    else
                    {
                        //  自分の方が小さい場合、相手を縮める
                        imageData.ModifiedWidth = OriginalWidth;
                        imageData.ModifiedHeight = imageData.OriginalHeight * (OriginalWidth / imageData.OriginalWidth);
                    }
                    break;
                default:
                    break;
            }
        }

    }
}
