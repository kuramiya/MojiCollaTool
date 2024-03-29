﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MojiCollaTool
{
    /// <summary>
    /// UpDownTextBox.xaml の相互作用ロジック
    /// </summary>
    public partial class UpDownTextBox : UserControl
    {
        private const int LONG_PRESS_START_WAIT_TIME_MSEC = 300;

        public int Value { get; set; } = 0;

        public bool RunEvent { get; set; } = true;

        /// <summary>
        /// 入力可能な最小値、この値を含める
        /// </summary>
        public int ValueMinLimit { get; set; } = 0;

        /// <summary>
        /// 入力可能な最大値、この値を含める
        /// </summary>
        public int ValueMaxLimit { get; set; } = int.MaxValue;

        public int Step { get; set; } = 1;

        public event EventHandler<UpDownTextBoxEvent>? ValueChanged;

        public UpDownTextBox()
        {
            InitializeComponent();
        }

        public void SetValue(int value, bool runEvent = true)
        {
            Value = value;

            RunEvent = runEvent;
            ValueTextBox.Text = value.ToString();
            RunEvent = true;
        }

        public void RunDownButton()
        {
            DownButton_Click(this, new RoutedEventArgs());
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            Value -= Step;

            LimitValueMinimum();

            ValueTextBox.Text = Value.ToString();
        }

        public void RunUpButton()
        {
            UpButton_Click(this, new RoutedEventArgs());
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            Value += Step;

            LimitValueMaximum();

            ValueTextBox.Text = Value.ToString();
        }

        private void ValueTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (RunEvent == false) return;

            int tempValue;
            bool parseOk = int.TryParse(ValueTextBox.Text, out tempValue);

            if(parseOk)
            {
                Value = tempValue;

                LimitValueMinimum();
                LimitValueMaximum();

                if (ValueChanged != null) ValueChanged(this, new UpDownTextBoxEvent(Value));
            }
        }

        private void LimitValueMinimum()
        {
            if(Value <= ValueMinLimit)
            {
                Value = ValueMinLimit;
            }
        }

        private void LimitValueMaximum()
        {
            if(Value >= ValueMaxLimit)
            {
                Value = ValueMaxLimit;
            }
        }

        private void ValueTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (RunEvent == false) return;

            if(e.Key == Key.Up)
            {
                UpButton_Click(this, new RoutedEventArgs());
            }
            else if(e.Key == Key.Down)
            {
                DownButton_Click(this, new RoutedEventArgs());
            }
        }

        private void Common_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                UpButton_Click(this, new RoutedEventArgs());
            }

            if (e.Delta < 0)
            {
                DownButton_Click(this, new RoutedEventArgs());
            }

        }
    }

    public class UpDownTextBoxEvent : EventArgs
    {
        public int Value { get; set; } = 0;

        public UpDownTextBoxEvent(int value)
        {
            Value = value;
        }
    }
}
