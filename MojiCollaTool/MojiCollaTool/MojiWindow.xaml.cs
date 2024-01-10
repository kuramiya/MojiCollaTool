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

namespace MojiCollaTool
{
    /// <summary>
    /// MojiWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MojiWindow : Window
    {
        public Moji Moji { get; set; }

        public int Id => Moji.Id;

        public string ExampleText => Moji.ExampleText;

        private MainWindow mainWindow;

        public MojiWindow(int id, MainWindow mainWindow)
        {
            Moji = new Moji(id);
            this.mainWindow = mainWindow;

            InitializeComponent();
        }

        public MojiWindow(Moji moji, MainWindow mainWindow)
        {
            Moji = moji;
            this.mainWindow = mainWindow;

            InitializeComponent();
        }

    }
}
