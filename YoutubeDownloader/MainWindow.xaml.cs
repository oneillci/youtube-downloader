using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using YoutubeExtractor;

namespace YoutubeDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            txtLocation.Text = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads\");
        }

        private async void BtnDownload_OnClick(object sender, RoutedEventArgs e)
        {
            await DoDownload();
        }

        private async void TxtLocation_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await DoDownload();
            }
        }

        private async Task DoDownload()
        {
            var videoInfos = DownloadUrlResolver.GetDownloadUrls(txtUrl.Text);
            var video = videoInfos.Where(info => info.VideoType == VideoType.Mp4 && info.AudioBitrate > 0).OrderByDescending(v => v.Resolution).FirstOrDefault();
            if (video == null)
            {
                MessageBox.Show("Could not find a video with audio to download");
            }

            string path = txtLocation.Text;

            if (!path.EndsWith(".mp4"))
            {
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads",
                    video.Title.Replace(':', '-') + video.VideoExtension);
                txtLocation.Text = path;
            }

            var videoDownloader = new VideoDownloader(video, path);
            videoDownloader.DownloadProgressChanged +=
                (s, args) => Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => progress.Value = args.ProgressPercentage));
            btnDownload.IsEnabled = false;
            await Task.Run(() => videoDownloader.Execute());
            btnDownload.IsEnabled = true;
            progress.Value = 0;
        }

    }
}
