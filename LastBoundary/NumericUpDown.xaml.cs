using System;
using System.Windows;
using System.Windows.Controls;

namespace LastBoundary
{
    /// <summary>
    /// Логика взаимодействия для NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        public event EventHandler ValueChanged;

        private int minValue=0, maxValue=100, startValue = 50;
        private int curVal;
        //private EventHandler ValueChanged;
        //make an event ValueChanged
        public NumericUpDown()
        {
            InitializeComponent();
        }


        public int Maximum
        {
            get { return maxValue; }
            set { maxValue = value; }
        }
       
        public int Minimum
        {
            get { return minValue; }
            set { minValue = value; }
        }
        
        public int Value
        {
            get { return curVal; }
            set { curVal = value; tbX.Text = curVal.ToString(); }
        }
        /*public static readonly DependencyProperty ValueProperty =
    DependencyProperty.Register("Value", typeof(int), typeof(NumericUpDown));*/
        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            if (tbX.Text != "") curVal = Convert.ToInt32(tbX.Text);
            else curVal = 0;
            if (curVal < maxValue)
                tbX.Text = Convert.ToString(++curVal);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            if (tbX.Text != "") curVal = Convert.ToInt32(tbX.Text);
            else curVal = 0;
            if (curVal > minValue)
                tbX.Text = Convert.ToString(--curVal);
        }

        private void tbX_TextChanged(object sender, TextChangedEventArgs e)
        {
            int number = 0;
            if (tbX.Text != "")
            {
                if (!int.TryParse(tbX.Text, out number)) tbX.Text = startValue.ToString();
            }
            else
                tbX.Text = minValue.ToString();
            if (number > maxValue) tbX.Text = maxValue.ToString();
            if (number < minValue) tbX.Text = minValue.ToString();
            curVal = int.Parse(tbX.Text);
            tbX.SelectionStart = tbX.Text.Length;
            OnValueChanged();
        }

        /*
        /// <summary>
        /// raising my own event
        /// </summary>
        /// <param name="Name">Ы</param>
        /// */
        public void OnValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

    }

}
