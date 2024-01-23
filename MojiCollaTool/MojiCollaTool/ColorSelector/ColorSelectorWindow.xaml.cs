using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfColorPicker;

namespace MojiCollaTool.ColorSelector
{
    /// <summary>
    /// ColorSelectorWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ColorSelectorWindow : Window
    {
        public SolidColorBrush BeforeBrush { get; set; } = new SolidColorBrush();

        public SolidColorBrush AfterBrush { get; set; } = new SolidColorBrush();

        public ColorSelectorWindow()
        {
            InitializeComponent();
        }

        public ColorSelectorWindow(Color currentColor)
        {
            InitializeComponent();

            BeforeBrush = new SolidColorBrush(currentColor);
            AfterBrush = new SolidColorBrush(currentColor);

            ManualColorPicker.BeforeBrush = BeforeBrush;
            ManualColorPicker.AfterBrush = AfterBrush;
        }
    }
}
