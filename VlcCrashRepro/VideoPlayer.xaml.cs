using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using Vlc.DotNet.Core;
using Vlc.DotNet.Forms;

namespace VlcCrashRepro
{
    /// <summary>
    /// Interaction logic for VideoPlayer.xaml
    /// </summary>
    public partial class VideoPlayer : UserControl
    {
        //private static readonly TimeSpan StreamWatchDogInterval = TimeSpan.FromSeconds(10);

        // ReSharper disable once AssignNullToNotNullAttribute
        // ReSharper disable once PossibleNullReferenceException
        private static readonly DirectoryInfo LibDirectory = new DirectoryInfo(Path.Combine(
            new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName,
            "libvlc",
            IntPtr.Size == 4 ? "win-x86" : "win-x64"));

        private VideoPlayerViewModel ViewModel { get; }

        //private readonly DispatcherTimer _streamWatchdogTimer = new DispatcherTimer();
        private readonly VlcControl _vlcControl;

        private readonly object _vlcControlLock = new object();

        public VideoPlayer(VideoPlayerViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = viewModel;

            InitializeComponent();

            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;

            _vlcControl = new VlcControl();
            PlayerContainer.Child = _vlcControl;
            _vlcControl.BeginInit();
            _vlcControl.VlcLibDirectory = LibDirectory;
            _vlcControl.EndInit();

            _vlcControl.VlcMediaPlayer.Log += MediaPlayerOnLog;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VideoPlayerViewModel.MediaUri))
            {
                Play(ViewModel.MediaUri);
            }
        }

        private void Play(Uri mediaUri)
        {
            lock (_vlcControlLock)
            {
                Console.WriteLine($"{ViewModel.Index} {DateTime.Now.ToString(DateTimeFormatInfo.CurrentInfo.FullDateTimePattern)} Playing");
                _vlcControl.Stop();
                if (mediaUri == null)
                {
                    return;
                }

                _vlcControl.Play(mediaUri);
                Console.WriteLine($"{ViewModel.Index} {DateTime.Now.ToString(DateTimeFormatInfo.CurrentInfo.FullDateTimePattern)} Play done");
            }
        }

        private void MediaPlayerOnLog(object sender, VlcMediaPlayerLogEventArgs e)
        {
            Console.WriteLine($"mediaPlayer {ViewModel.Index} {e.Module}: {e.Message}");
        }
    }
}
