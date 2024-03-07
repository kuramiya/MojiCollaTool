using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MojiCollaTool
{
    /// <summary>
    /// 配置場所を示す列挙
    /// </summary>
    public enum LocatePosition
    {
        Left,
        Right,
        Top,
        Bottom,
    }

    [Serializable]
    public class CanvasData
    {
        /// <summary>
        /// キャンバスの横幅
        /// </summary>
        public double CanvasWidth { get; set; } = 0;

        /// <summary>
        /// キャンバスの縦幅
        /// </summary>
        public double CanvasHeight { get; set; } = 0;

        public ImageData FirstImageData { get; set; }

        public ImageData SecondImageData { get; set; }

        public LocatePosition SecondImageLocatePosition { get; set; } = LocatePosition.Left;

        public double ImageWidth
        {
            get => FirstImageData.ModifiedWidth + SecondImageData.ModifiedWidth;
        }

        public double ImageHeight
        {
            get => FirstImageData.ModifiedHeight + SecondImageData.ModifiedHeight;
        }

        public double ImageTopMargin { get; set; }

        public double ImageLeftMargin { get; set; }

        public double ImageBottomMargin { get; set; }

        public double ImageRightMargin { get; set; }

        /// <summary>
        /// キャンバスの色
        /// </summary>
        public Color CanvasColor { get; set; } = Colors.White;

        /// <summary>
        /// キャンバスサイズを更新する
        /// </summary>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        public void UpdateCanvasSize(int imageWidth, int imageHeight)
        {
            CanvasWidth = ImageLeftMargin + imageWidth + ImageRightMargin;
            CanvasHeight = ImageTopMargin + imageHeight + ImageBottomMargin;
        }

        /// <summary>
        /// 初期化する
        /// </summary>
        public void Init()
        {
            CanvasWidth = 0;
            CanvasHeight = 0;
            InitMargin();
            CanvasColor = Colors.White;
        }

        /// <summary>
        /// マージン地を初期化する
        /// </summary>
        public void InitMargin()
        {
            ImageTopMargin = 0;
            ImageBottomMargin = 0;
            ImageRightMargin = 0;
            ImageLeftMargin = 0;
        }

        public void Copy(CanvasData copySource)
        {
            CanvasWidth = copySource.CanvasWidth;
            CanvasHeight = copySource.CanvasHeight;
            ImageTopMargin = copySource.ImageTopMargin;
            ImageBottomMargin = copySource.ImageBottomMargin;
            ImageLeftMargin = copySource.ImageLeftMargin;
            ImageRightMargin = copySource.ImageRightMargin;
            CanvasColor = copySource.CanvasColor;
        }

        public CanvasData Clone()
        {
            CanvasData clone = new CanvasData();
            clone.Copy(this);
            return clone;
        }
    }
}
