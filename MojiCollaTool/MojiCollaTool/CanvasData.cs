using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MojiCollaTool
{
    [Serializable]
    public class CanvasData
    {
        public int Width { get; set; } = 0;

        public int Height { get; set; } = 0;

        public int ImageOffsetX { get; set; } = 0;

        public int ImageOffsetY { get; set; } = 0;

        public Color Background { get; set; } = Colors.White;

        /// <summary>
        /// 初期化する
        /// </summary>
        public void Init()
        {
            Width = 0;
            Height = 0;
            ImageOffsetX = 0;
            ImageOffsetY = 0;
            Background = Colors.White;
        }
    }
}
