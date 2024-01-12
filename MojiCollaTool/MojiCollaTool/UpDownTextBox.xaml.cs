using System;
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

        public bool IsMinusAllowed { get; set; } = false;

        public bool IsZeroAllowed { get; set; } = true;

        public event EventHandler<UpDownTextBoxEvent>? ValueChanged;

        private DispatcherTimer longPressEventTimer = new DispatcherTimer();

        private bool isLongPress = false;

        private bool isLongPressEventUp;

        public UpDownTextBox()
        {
            InitializeComponent();

            longPressEventTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            longPressEventTimer.Tick += LongPressEventTimer_Tick;
        }

        public void SetValue(int value, bool runEvent = true)
        {
            Value = value;

            RunEvent = runEvent;
            ValueTextBox.Text = value.ToString();
            RunEvent = true;
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            Value--;

            LimitValueMinimum();

            ValueTextBox.Text = Value.ToString();
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            Value++;
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

                if (ValueChanged != null) ValueChanged(this, new UpDownTextBoxEvent(Value));
            }
        }

        private async void DownButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            await StartLongPressAsync(false);
        }

        private async void UpButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            await StartLongPressAsync(true);
        }

        private async Task StartLongPressAsync(bool isUp)
        {
            isLongPress = true;

            await Task.Delay(LONG_PRESS_START_WAIT_TIME_MSEC);

            if (isLongPress == false) return;

            isLongPressEventUp = isUp;

            longPressEventTimer.Start();
        }

        private void UpDownButton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            isLongPress = false;
            longPressEventTimer.Stop();
        }

        private void LongPressEventTimer_Tick(object? sender, EventArgs e)
        {
            if(isLongPressEventUp)
            {
                Value++;
            }
            else
            {
                Value--;
                LimitValueMinimum();
            }
            ValueTextBox.Text = Value.ToString();
        }

        private void LimitValueMinimum()
        {
            if (Value <= 0 && IsZeroAllowed == false)
            {
                Value = 1;
            }
            else if (Value < 0 && IsMinusAllowed == false)
            {
                Value = 0;
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
