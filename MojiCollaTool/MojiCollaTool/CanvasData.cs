using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MojiCollaTool
{
    [Serializable]
    public class CanvasData
    {
        /// <summary>
        /// キャンバスの横幅
        /// </summary>
        public int Width { get; set; } = 0;

        /// <summary>
        /// キャンバスの縦幅
        /// </summary>
        public int Height { get; set; } = 0;

        public int ImageTopMargin { get; set; }

        public int ImageLeftMargin { get; set; }

        public int ImageBottomMargin { get; set; }

        public int ImageRightMargin { get; set; }

        /// <summary>
        /// キャンバスの色
        /// </summary>
        public Color Background { get; set; } = Colors.White;

        /// <summary>
        /// 設定値を更新する
        /// </summary>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <param name="updatedCanvasWidth"></param>
        /// <param name="updatedCanvasHeight"></param>
        public void Update(int imageWidth, int imageHeight, int updatedCanvasWidth, int updatedCanvasHeight)
        {
            //  まず最初に右から調整する
            ImageRightMargin = updatedCanvasWidth - (imageWidth + ImageLeftMargin);

            //  右側が0より小さくなった場合、左側を削る
            if(ImageRightMargin < 0 )
            {
                ImageLeftMargin -= ImageRightMargin;

                if (ImageLeftMargin < 0) ImageLeftMargin = 0;

                ImageRightMargin = 0;
            }

            //  まず最初に下から調整する
            ImageBottomMargin = updatedCanvasHeight - (imageHeight + ImageTopMargin);

            //  下側が0より小さくなった場合、左側を削る
            if (ImageBottomMargin < 0)
            {
                ImageTopMargin -= ImageBottomMargin;

                if (ImageTopMargin < 0) ImageTopMargin = 0;

                ImageBottomMargin = 0;
            }

            Width = updatedCanvasWidth;
            Height = updatedCanvasHeight;
        }

        public void UpdateImageTopMargin(int margin, int imageHeight)
        {
            if (margin < 0) return;
            ImageTopMargin = margin;
            ImageBottomMargin = Height - imageHeight - margin;
        }

        public void UpdateImageBottomMargin(int margin, int imageHeight)
        {
            if (margin < 0) return;
            ImageBottomMargin = margin;
            ImageTopMargin = Height - imageHeight - margin;
        }

        public void UpdateImageLeftMargin(int margin, int imageWidth)
        {
            if (margin < 0) return;
            ImageLeftMargin = margin;
            ImageRightMargin = Width - imageWidth - margin;
        }

        public void UpdateImageRightMargin(int margin, int imageWidth)
        {
            if (margin < 0) return;
            ImageRightMargin = margin;
            ImageLeftMargin = Width - imageWidth - margin;
        }

        public void UpdateCanvasWidthHeight(int imageWidth, int imageHeight)
        {
            Width = ImageLeftMargin + imageWidth + ImageRightMargin;
            Height = ImageTopMargin + imageHeight + ImageBottomMargin;
        }

        /// <summary>
        /// 初期化する
        /// </summary>
        public void Init()
        {
            Width = 0;
            Height = 0;
            ImageTopMargin = 0;
            ImageBottomMargin = 0;
            ImageRightMargin = 0;
            ImageLeftMargin = 0;
            Background = Colors.White;
        }

        public void Copy(CanvasData copySource)
        {
            Width = copySource.Width;
            Height = copySource.Height;
            ImageTopMargin = copySource.ImageTopMargin;
            ImageBottomMargin = copySource.ImageBottomMargin;
            ImageLeftMargin = copySource.ImageLeftMargin;
            ImageRightMargin = copySource.ImageRightMargin;
            Background = copySource.Background;
        }

        public CanvasData Clone()
        {
            CanvasData clone = new CanvasData();
            clone.Copy(this);
            return clone;
        }
    }
}
