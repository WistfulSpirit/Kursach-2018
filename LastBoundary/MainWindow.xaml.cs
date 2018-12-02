using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using Vlc.DotNet.Wpf;
using System.Windows.Controls;

namespace LastBoundary
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int VOLUME_STEP = 5;
        private VlcVideoSourceProvider sourceProvider;
        DispatcherTimer timer;
        /* TODO:
         *  реализовать все остальные кнопки на интерфейсе которые сами себя и описывают
         */

        string[] options = new string[] //VLCPlayer options
                {
                    "--file-logging", "vvv", "--extraintf=logger",  "--verbose=2", "--logfile=Logs.log", "--video-filter=adjust", "--canvas-aspect=16:9", "--aspect-ratio=16:9"
                };

        public MainWindow()
        {
            InitializeComponent();//auto-created
            TVControl.GetChannelsFromDataBase();//Recieve channel list from database and save it in class field
            InitializeTimer();
            TVControl.BrightnessChanged += SetBrightness;
            TVControl.ContrastChanged += SetContrast;
            slVolume.Maximum = TVControl.MaxVolume;
            slVolume.Minimum = TVControl.MinVolume;
            InitializeVLCplayer();
            BindImage();
            ApplySettings();
        }

        private void ApplySettings()
        {
            slVolume.Value = Properties.Settings.Default.Volume;

            TVControl.IsMute = Properties.Settings.Default.IsMute;
            sourceProvider.MediaPlayer.Audio.IsMute = TVControl.IsMute;
            imgSoundToolBar.Source = new BitmapImage(new Uri((TVControl.IsMute ? "/img/mute.png" : "/img/sound.png"), UriKind.Relative));


            int num = Properties.Settings.Default.LastViewdChannelNumber - 1;
            if (num < 0)
                TVControl.CurrentChannel = TVControl.ChannelList[0];
            else if (num > TVControl.ChannelList.Count())
                TVControl.CurrentChannel = TVControl.ChannelList[TVControl.ChannelList.Count() - 1];
            else
                TVControl.CurrentChannel = TVControl.ChannelList[num];

            TVControl.Brightness = Properties.Settings.Default.Brightness;
            TVControl.Contrast = Properties.Settings.Default.Contrast;
            SetBrightness(TVControl.Brightness);
            SetContrast(TVControl.Contrast);
        }

        private void InitializeVLCplayer()//Initialize VLC player using sourceprovider
        {
            this.sourceProvider = new VlcVideoSourceProvider(this.Dispatcher);
            var libDirectory = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
            sourceProvider.CreatePlayer(libDirectory, options);
        }

        private void BindImage()//Bind Image.Source to VLC sourceprovider
        {
            this.Video.SetBinding(System.Windows.Controls.Image.SourceProperty,
                new Binding(nameof(VlcVideoSourceProvider.VideoSource)) { Source = sourceProvider });

            this.BackgroundVideo.SetBinding(System.Windows.Controls.Image.SourceProperty,
                new Binding(nameof(VlcVideoSourceProvider.VideoSource)) { Source = sourceProvider });
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 3);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            int NChan = Int32.Parse(lblToChannel.Content.ToString().Replace("-", string.Empty));
            ChangeChannel(NChan);
        }

        private void ChangeChannel(int Num)
        {
            if (Num - 1 >= TVControl.ChannelList.Count())//Check if number in range of channel numbers
            {
                lblToChannel.Visibility = Visibility.Hidden;
                return;
            }
            TVControl.PreviousChannel = TVControl.CurrentChannel;
            TVControl.CurrentChannel = TVControl.ChannelList[Num - 1];
            TVControl.Log(Action.ChannelChange, null);
            lblConnection.Content = "Идет подключение";
            Task.Run(() =>
            {
                sourceProvider.MediaPlayer.Stop();
                sourceProvider.MediaPlayer.Play(TVControl.CurrentChannel.Link, options);
                sourceProvider.MediaPlayer.Audio.IsMute = TVControl.IsMute;
                lblToChannel.Dispatcher.Invoke(() => lblToChannel.Visibility = Visibility.Hidden);
                lblConnection.Dispatcher.Invoke(() => lblConnection.Content = "Подключено");
            });
        }

        private void SetBrightness(int Value)
        {
            sourceProvider.MediaPlayer.Video.Adjustments.Brightness = (Value * 2) / 100.0f;
            TVControl.Brightness = Value;
        }

        private void SetContrast(int Value)
        {
            sourceProvider.MediaPlayer.Video.Adjustments.Contrast = (Value * 2) / 100.0f;
            TVControl.Contrast = Value;
        }

        private void btnImgSettings_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow();
            settings.Owner = this;
            settings.Show();
        }

        private void btnOn_Click(object sender, RoutedEventArgs e)
        {
            if (!TVControl.IsOn)
            {
                toHide.Fill = new SolidColorBrush(Colors.Transparent);
                TVControl.IsOn = true;
                lblConnection.Content = "Идет подключение";
                Task.Run(() =>
                {
                    sourceProvider.MediaPlayer.Play(TVControl.CurrentChannel.Link);
                    lblConnection.Dispatcher.Invoke(() => lblConnection.Content = "Подключено");
                });
            }
            else
            {
                Task.Run(() => sourceProvider.MediaPlayer.Stop());
                toHide.Fill = new SolidColorBrush(Colors.DarkGray);
                TVControl.IsOn = false;
                lblConnection.Content = "Отключено";
            }
        }

        private void NumBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TVControl.IsOn)
            {
                Button btn = sender as Button;
                if (TVControl.IsOn)
                {
                    if (timer.IsEnabled)
                    {
                        if (lblToChannel.Content.ToString().Contains("--"))
                        {
                            lblToChannel.Content = lblToChannel.Content.ToString()[0] + btn.Content.ToString() + "-";
                            return;
                        }
                        if (lblToChannel.Content.ToString().Contains("-"))
                        {
                            lblToChannel.Content = lblToChannel.Content.ToString().Replace('-', btn.Content.ToString()[0]);
                            return;
                        }
                        ChangeChannel(Int32.Parse(lblToChannel.Content.ToString()));
                        timer.Stop();
                    }
                    else
                    {
                        lblToChannel.Content = btn.Content + "--";
                        timer.Start();
                        lblToChannel.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.LastViewdChannelNumber = TVControl.CurrentChannel.Number;
            Properties.Settings.Default.Volume = sourceProvider.MediaPlayer.Audio.Volume;

            Properties.Settings.Default.IsMute = TVControl.IsMute;
            Properties.Settings.Default.Brightness = TVControl.Brightness;
            Properties.Settings.Default.Contrast = TVControl.Contrast;

            Properties.Settings.Default.Save();
            Task.Run(() => sourceProvider.MediaPlayer.Stop());
        }

        private void btnSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (TVControl.IsOn)
            {
                if (TVControl.PreviousChannel != null)
                {
                    ChangeChannel(TVControl.PreviousChannel.Number);
                }
            }
        }

        private void slVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TVControl.IsMute = false;
            sourceProvider.MediaPlayer.Audio.IsMute = TVControl.IsMute;
            imgSoundToolBar.Source = new BitmapImage(new Uri("/img/sound.png", UriKind.Relative));
            sourceProvider.MediaPlayer.Audio.Volume = (int)slVolume.Value;
        }

        private void imgSoundToolBar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TVControl.IsMute = !sourceProvider.MediaPlayer.Audio.IsMute;
            sourceProvider.MediaPlayer.Audio.IsMute = TVControl.IsMute;
            imgSoundToolBar.Source = new BitmapImage(new Uri((TVControl.IsMute ? "/img/mute.png" : "/img/sound.png"), UriKind.Relative));
            TVControl.Log(Action.Mute, "Mute = " + TVControl.IsMute);
        }

        private void btnVolumeUp_Click(object sender, RoutedEventArgs e)
        {
            if (TVControl.IsOn)
            {
                int volume = sourceProvider.MediaPlayer.Audio.Volume;
                if (volume < TVControl.MaxVolume)
                {
                    volume += VOLUME_STEP;
                    if (volume > TVControl.MaxVolume)
                        volume = TVControl.MaxVolume;
                    sourceProvider.MediaPlayer.Audio.Volume = volume;
                    slVolume.Value = volume;
                }
            }
        }

        private void btnChannelUp_Click(object sender, RoutedEventArgs e)
        {
            if (TVControl.IsOn)
            {
                int next;
                if (TVControl.CurrentChannel.Number + 1 > TVControl.ChannelList.Count)
                    next = 1;
                else
                    next = TVControl.CurrentChannel.Number + 1;
                TVControl.Log(Action.ChannelUp, String.Format($"From channel {TVControl.CurrentChannel.Number} to {next}"));
                ChangeChannel(next);
            }
        }

        private void btnVolumeDown_Click(object sender, RoutedEventArgs e)
        {
            if (TVControl.IsOn)
            {
                int volume = sourceProvider.MediaPlayer.Audio.Volume;
                if (volume > TVControl.MinVolume)
                {
                    volume -= VOLUME_STEP;
                    if (volume < TVControl.MinVolume)
                        volume = 0;
                    sourceProvider.MediaPlayer.Audio.Volume = volume;
                    slVolume.Value = volume;
                }
            }
        }

        private void btnChannelDown_Click(object sender, RoutedEventArgs e)
        {
            if (TVControl.IsOn)
            {
                int next;
                if (TVControl.CurrentChannel.Number - 1 <= 1)
                    next = TVControl.ChannelList.Count();
                else
                    next = TVControl.CurrentChannel.Number - 1;
                TVControl.Log(Action.ChannelDown, String.Format($"From channel {TVControl.CurrentChannel.Number} to {next}"));
                ChangeChannel(next);
            }
        }

        private void Image_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            sourceProvider.MediaPlayer.Audio.Volume = 800;
        }
    }
}
