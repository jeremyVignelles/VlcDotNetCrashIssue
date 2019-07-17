using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VlcCrashRepro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int NbRows = 3;
        private const int NbColumns = 3;
        private const int NbCallers = 10;

        private static readonly Random Rnd = new Random(42);

        private static readonly IReadOnlyList<Uri> StreamUris = new[]
            {
                "rtsp://192.168.5.87/VideoInput/1/H264/1",
                "rtsp://192.168.5.87/VideoInput/2/H264/1",
                "rtsp://192.168.3.12/axis-media/media.amp",
                "rtsp://192.168.3.13/axis-media/media.amp",
                "rtsp://192.168.3.17/axis-media/media.amp",
                "rtsp://192.168.3.31/axis-media/media.amp",
                "rtsp://192.168.3.37/axis-media/media.amp",
                "rtsp://192.168.3.38/axis-media/media.amp",
                "rtsp://192.168.3.44/axis-media/media.amp",
                "rtsp://192.168.3.47/axis-media/media.amp",
                "rtsp://192.168.3.50/axis-media/media.amp",
                "rtsp://192.168.3.51/axis-media/media.amp",
                "rtsp://192.168.3.53/axis-media/media.amp",
                "rtsp://192.168.3.55/axis-media/media.amp",
                "rtsp://192.168.3.59/axis-media/media.amp",
                "rtsp://192.168.3.61/axis-media/media.amp",
                "rtsp://192.168.3.62/axis-media/media.amp",
                "rtsp://192.168.3.73/axis-media/media.amp",
                "rtsp://192.168.3.76/axis-media/media.amp",
                "rtsp://192.168.3.80/axis-media/media.amp",
                "rtsp://192.168.3.81/axis-media/media.amp",
                "rtsp://192.168.3.82/axis-media/media.amp",
            }.Select(val => new Uri(val))
            .ToArray();

        public MainWindow()
        {
            InitializeComponent();

            for (var col = 0; col < NbColumns; col++)
            {
                PlayerGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (var row = 0; row < NbRows; row++)
            {
                PlayerGrid.RowDefinitions.Add(new RowDefinition());
            }

            var viewModels = new List<VideoPlayerViewModel>();
            var playerIndex = 1;
            for (var row = 0; row < NbRows; row++)
            {
                for (var col = 0; col < NbColumns; col++)
                {
                    var viewModel = new VideoPlayerViewModel(playerIndex++);
                    viewModels.Add(viewModel);

                    var player = new VideoPlayer(viewModel);
                    PlayerGrid.Children.Add(player);
                    Grid.SetColumn(player, col);
                    Grid.SetRow(player, row);
                }
            }

            async Task AffectRandomStreams()
            {
                while (true)
                {
                    var viewModel = viewModels[Rnd.Next(viewModels.Count)];
                    var uri = StreamUris[Rnd.Next(StreamUris.Count)];
                    viewModel.AffectMedia(uri);
                    await Task.Delay(Rnd.Next(5000)+500);
                }
            }

            for (var i = 0; i < NbCallers; i++)
            {
                Task.Run(AffectRandomStreams);
            }
        }
    }
}