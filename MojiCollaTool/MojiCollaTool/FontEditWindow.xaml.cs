using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// FontEditWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class FontEditWindow : Window
    {
        ObservableCollection<TextBlock> allFontTextBlocks = new ObservableCollection<TextBlock>();

        public FontEditWindow()
        {
            InitializeComponent();

            foreach (var tb in FontUtil.GetFontTextBlocks())
            {
                allFontTextBlocks.Add(tb);
            }

            AllFontListView.ItemsSource = allFontTextBlocks;
        }
    }
}
