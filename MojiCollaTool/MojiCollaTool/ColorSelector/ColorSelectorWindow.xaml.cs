﻿using System;
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
        /// <summary>
        /// 色選択履歴の最大保持数
        /// </summary>
        public const int COLOR_HISTORY_MAX_COUNT = 20;

        /// <summary>
        /// 色選択履歴
        /// 静的オブジェクトとして保持している
        /// </summary>
        private static List<Color> colorHistory = new List<Color>();

        /// <summary>
        /// 色選択履歴のボタンリスト
        /// 最大20個まで
        /// </summary>
        private List<Button> colorHistoryButtons = new List<Button>();

        /// <summary>
        /// 前面or背面
        /// </summary>
        public ForeBack ForeBackSelection { get; set; } = ForeBack.Back;

        public SolidColorBrush CurrentForeBrush { get; set; } = new SolidColorBrush();

        public SolidColorBrush CurrentBackBrush { get; set; } = new SolidColorBrush();

        public SolidColorBrush NextForeBrush { get; set; } = new SolidColorBrush();

        public SolidColorBrush NextBackBrush { get; set; } = new SolidColorBrush();

        static ColorSelectorWindow()
        {
            //  色選択履歴を初期化する
            for (int i = 0; i < COLOR_HISTORY_MAX_COUNT; i++)
            {
                colorHistory.Add(Colors.White);
            }
        }

        public ColorSelectorWindow()
        {
            InitializeComponent();
        }

        public ColorSelectorWindow(ForeBack foreBackSelelction, Color currentForeColor, Color currentBackColor)
        {
            ForeBackSelection = foreBackSelelction;

            InitializeComponent();

            CurrentForeBrush = new SolidColorBrush(currentForeColor);
            NextForeBrush = new SolidColorBrush(currentForeColor);

            ManualColorPicker.BeforeBrush = CurrentForeBrush;
            ManualColorPicker.AfterBrush = NextForeBrush;

            colorHistoryButtons.Add(History1);
            colorHistoryButtons.Add(History2);
            colorHistoryButtons.Add(History3);
            colorHistoryButtons.Add(History4);
            colorHistoryButtons.Add(History5);
            colorHistoryButtons.Add(History6);
            colorHistoryButtons.Add(History7);
            colorHistoryButtons.Add(History8);
            colorHistoryButtons.Add(History9);
            colorHistoryButtons.Add(History10);
            colorHistoryButtons.Add(History11);
            colorHistoryButtons.Add(History12);
            colorHistoryButtons.Add(History13);
            colorHistoryButtons.Add(History14);
            colorHistoryButtons.Add(History15);
            colorHistoryButtons.Add(History16);
            colorHistoryButtons.Add(History17);
            colorHistoryButtons.Add(History18);
            colorHistoryButtons.Add(History19);
            colorHistoryButtons.Add(History20);
        }

        /// <summary>
        /// 色選択履歴を更新する
        /// </summary>
        /// <param name="color"></param>
        public void UpdateColorHistory(Color color)
        {
            colorHistory.Insert(0, color);
            colorHistory.RemoveAt(COLOR_HISTORY_MAX_COUNT - 1);

            for (int i = 0; i < COLOR_HISTORY_MAX_COUNT; i++)
            {
                colorHistoryButtons[i].Background = new SolidColorBrush(colorHistory[i]);
            }
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            var color = ((SolidColorBrush)((Button)sender).Background).Color;

            NextForeBrush = new SolidColorBrush(color);

            UpdateColorHistory(color);
        }
    }
}
