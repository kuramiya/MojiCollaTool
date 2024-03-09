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
        public int CanvasWidth { get; set; } = 0;

        /// <summary>
        /// キャンバスの縦幅
        /// </summary>
        public int CanvasHeight { get; set; } = 0;

        /// <summary>
        /// 画像1のデータ
        /// </summary>
        public ImageData ImageData1 { get; set; } = new ImageData();

        /// <summary>
        /// 画像2のデータ
        /// </summary>
        public ImageData ImageData2 { get; set; } = new ImageData();

        /// <summary>
        /// 画像2の配置場所
        /// </summary>
        public LocatePosition Image2LocatePosition { get; set; } = LocatePosition.Left;

        /// <summary>
        /// 画像縦サイズ
        /// </summary>
        public int ImageWidth
        {
            get
            {
                switch (Image2LocatePosition)
                {
                    case LocatePosition.Left:
                    case LocatePosition.Right:
                        return ImageData1.ModifiedWidth + ImageData2.ModifiedWidth;
                    case LocatePosition.Top:
                    case LocatePosition.Bottom:
                    default:
                        return ImageData1.ModifiedWidth;
                }
            }
        }

        /// <summary>
        /// 画像横サイズ
        /// </summary>
        public int ImageHeight
        {
            get
            {
                switch (Image2LocatePosition)
                {
                    case LocatePosition.Left:
                    case LocatePosition.Right:
                        return ImageData1.ModifiedHeight;
                    case LocatePosition.Top:
                    case LocatePosition.Bottom:
                    default:
                        return ImageData1.ModifiedHeight + ImageData2.ModifiedHeight;
                }
            }
        }

        public int ImageMarginTop { get; set; }

        public int ImageMarginLeft { get; set; }

        public int ImageMarginBottom { get; set; }

        public int ImageMarginRight { get; set; }

        /// <summary>
        /// キャンバスの色
        /// </summary>
        public Color CanvasColor { get; set; } = Colors.White;

        public CanvasData()
        {
            Init();
        }

        /// <summary>
        /// キャンバスサイズを更新する
        /// </summary>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        public void UpdateCanvasSize()
        {
            CanvasWidth = ImageMarginLeft + ImageWidth + ImageMarginRight;
            CanvasHeight = ImageMarginTop + ImageHeight + ImageMarginBottom;
        }

        /// <summary>
        /// 画像サイズを調整する
        /// </summary>
        public void ModifyImageSize()
        {
            ImageData1.ModifySize(Image2LocatePosition, ImageData2);
        }

        /// <summary>
        /// 初期化する
        /// </summary>
        public void Init()
        {
            CanvasWidth = 0;
            CanvasHeight = 0;
            ImageData1.Init();
            ImageData2.Init();
            Image2LocatePosition = LocatePosition.Left;
            InitMargin();
            CanvasColor = Colors.White;
        }

        /// <summary>
        /// マージン地を初期化する
        /// </summary>
        public void InitMargin()
        {
            ImageMarginTop = 0;
            ImageMarginBottom = 0;
            ImageMarginRight = 0;
            ImageMarginLeft = 0;
        }

        /// <summary>
        /// 画像1のマージンを返す
        /// </summary>
        /// <returns></returns>
        public Thickness GetImage1Margin()
        {
            double left = ImageMarginLeft;
            double right = ImageMarginRight;
            double top = ImageMarginTop;
            double bottom = ImageMarginBottom;

            switch (Image2LocatePosition)
            {
                case LocatePosition.Left:
                    left += ImageData2.ModifiedWidth;
                    break;
                case LocatePosition.Right:
                    right += ImageData2.ModifiedWidth;
                    break;
                case LocatePosition.Top:
                    top += ImageData2.ModifiedHeight;
                    break;
                case LocatePosition.Bottom:
                    bottom += ImageData2.ModifiedHeight;
                    break;
                default:
                    break;
            }

            return new Thickness(left, top, right, bottom);
        }

        /// <summary>
        /// 画像2のマージンを返す
        /// </summary>
        /// <returns></returns>
        public Thickness GetImage2Margin()
        {
            double left = ImageMarginLeft;
            double right = ImageMarginRight;
            double top = ImageMarginTop;
            double bottom = ImageMarginBottom;

            switch (Image2LocatePosition)
            {
                case LocatePosition.Left:
                    right += ImageData1.ModifiedWidth;
                    break;
                case LocatePosition.Right:
                    left += ImageData1.ModifiedWidth;
                    break;
                case LocatePosition.Top:
                    bottom += ImageData1.ModifiedHeight;
                    break;
                case LocatePosition.Bottom:
                    top += ImageData1.ModifiedHeight;
                    break;
                default:
                    break;
            }

            return new Thickness(left, top, right, bottom);
        }

        public void Copy(CanvasData copySource)
        {
            CanvasWidth = copySource.CanvasWidth;
            CanvasHeight = copySource.CanvasHeight;
            ImageData1.Copy(copySource.ImageData1);
            ImageData2.Copy(copySource.ImageData2);
            ImageMarginTop = copySource.ImageMarginTop;
            ImageMarginBottom = copySource.ImageMarginBottom;
            ImageMarginLeft = copySource.ImageMarginLeft;
            ImageMarginRight = copySource.ImageMarginRight;
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
