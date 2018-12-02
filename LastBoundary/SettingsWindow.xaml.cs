using Microsoft.Win32;
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

namespace LastBoundary
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            NUDBrightness.ValueChanged += NUDBrightness_ValueChanged;
            NUDContrast.ValueChanged += NUDContrast_ValueChanged;
            slBrightness.Value = TVControl.Brightness;
            slContrast.Value = TVControl.Contrast;
            FillListBox(TVControl.ChannelList);
        }

        private void FillListBox(List<Channel> channels)
        {
            lbChannels.Items.Clear();
            foreach (var chan in channels)
                lbChannels.Items.Add(string.Format($"{chan.Number} {chan.Name}"));
        }

        private void slContrast_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            NUDContrast.Value = (int)slContrast.Value;
        }

        private void slBrightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            NUDBrightness.Value = (int)slBrightness.Value;
        }

        private void NUDBrightness_ValueChanged(object sender, EventArgs e)
        {
            slBrightness.Value = NUDBrightness.Value;
            TVControl.BrightnessChanged((int)slBrightness.Value);
        }

        private void NUDContrast_ValueChanged(object sender, EventArgs e)
        {
            slContrast.Value = NUDContrast.Value;
            TVControl.ContrastChanged((int)slContrast.Value);
        }

        private void btnStartSearch_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Это перезапишет уже существующие каналы, продолжить?", "Предупреждение!",
                                                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                lbChannels.Items.Clear();
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    foreach (var chan in TVControl.GetChannelsFromFile(openFileDialog.FileName))
                        lbChannels.Items.Add(string.Format($"{chan.Number} {chan.Name}"));
                }
            }

        }

        private void btnGetLogs_Click(object sender, RoutedEventArgs e)
        {
            string FileName = "ActionLog.log";
            TVControl.SaveLogToFile(FileName);
            MessageBox.Show("Записано в файл " + FileName);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Owner.Focus();
        }
    }
}
