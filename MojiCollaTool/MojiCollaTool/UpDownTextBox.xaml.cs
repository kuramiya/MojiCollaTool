using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
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

namespace MojiCollaTool
{
    /// <summary>
    /// UpDownTextBox.xaml の相互作用ロジック
    /// </summary>
    public partial class UpDownTextBox : UserControl
    {
        public int Value { get; set; } = 0;

        public bool RunEvent { get; set; } = true;

        public bool IsMinusAllowed { get; set; } = false;

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

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            Value--;
            if (Value < 0 && IsMinusAllowed == false)
            {
                Value = 0;
            }
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
                if (ValueChanged != null) ValueChanged(this, new UpDownTextBoxEvent(Value));
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
