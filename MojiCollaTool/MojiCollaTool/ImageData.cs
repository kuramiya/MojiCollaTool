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
        public int OriginalWidth { get; set; } = 0;

        public int OriginalHeight { get; set; } = 0;

        public int ModifiedWidth { get; set; } = 0;

        public int ModifiedHeight { get; set; } = 0;

        public ImageData()
        {
            //  何もしない
        }

        public ImageData(int originalWidth, int originalHeight)
        {
            OriginalWidth = originalWidth;
            OriginalHeight = originalHeight;
            ResetSize();
        }

        /// <summary>
        /// 初期化する
        /// </summary>
        public void Init()
        {
            OriginalWidth = 0;
            OriginalHeight = 0;
            ModifiedWidth = 0;
            ModifiedHeight = 0;
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
        /// 存在しないデータであるかを返す
        /// </summary>
        /// <returns></returns>
        public bool IsNullData()
        {
            return (OriginalWidth == 0 && OriginalHeight == 0);
        }

        /// <summary>
        /// サイズを更新する
        /// </summary>
        /// <param name="locatePosition"></param>
        /// <param name="imageData"></param>
        public void ModifySize(LocatePosition locatePosition, ImageData imageData)
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
                        ModifiedWidth = (int)((double)OriginalWidth * ((double)imageData.OriginalHeight / (double)OriginalHeight));
                    }
                    else
                    {
                        //  自分の方が小さい場合、相手を縮める
                        imageData.ModifiedHeight = OriginalHeight;
                        imageData.ModifiedWidth = (int)((double)imageData.OriginalWidth * ((double)OriginalHeight / (double)imageData.OriginalHeight));
                    }
                    break;
                case LocatePosition.Top:
                case LocatePosition.Bottom:
                    if (OriginalWidth > imageData.OriginalWidth)
                    {
                        //  自分の方が大きいので、自分を縮める
                        ModifiedWidth = imageData.OriginalWidth;
                        ModifiedHeight = (int)((double)OriginalHeight * ((double)imageData.OriginalWidth / (double)OriginalWidth));
                    }
                    else
                    {
                        //  自分の方が小さい場合、相手を縮める
                        imageData.ModifiedWidth = OriginalWidth;
                        imageData.ModifiedHeight = (int)((double)imageData.OriginalHeight * ((double)OriginalWidth / (double)imageData.OriginalWidth));
                    }
                    break;
                default:
                    break;
            }
        }

        public void Copy(ImageData copySource)
        {
            OriginalWidth = copySource.OriginalWidth;
            OriginalHeight = copySource.OriginalHeight;
            ModifiedWidth = copySource.ModifiedWidth;
            ModifiedHeight = copySource.ModifiedHeight;
        }

        public ImageData Clone()
        {
            ImageData clone = new ImageData();
            clone.Copy(this);
            return clone;
        }
    }
}
