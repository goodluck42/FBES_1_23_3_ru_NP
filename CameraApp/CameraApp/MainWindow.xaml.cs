using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
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
using Emgu.CV;

namespace CameraApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private readonly VideoCapture _videoCapture;
	private readonly DispatcherTimer _dispatcherTimer;
	private const int FrameRate = 60;

	private CameraClient _cameraClient;
	
	public MainWindow()
	{
		InitializeComponent();

		_dispatcherTimer = new DispatcherTimer
		{
			Interval = TimeSpan.FromMilliseconds(1000.0 / FrameRate),
		};
		_dispatcherTimer.Tick += UpdateFrame;
		_videoCapture = new VideoCapture();

		_cameraClient = new();
	}

	private void CameraStart_Click(object sender, RoutedEventArgs e)
	{
		_videoCapture.Start();
		_dispatcherTimer.Start();
	}

	private void CameraStop_Click(object sender, RoutedEventArgs e)
	{
	}

	private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
	{
	}

	private void UpdateFrame(object? sender, EventArgs e)
	{
		using var mat = _videoCapture.QueryFrame();
		using var bitmap = mat.ToBitmap();
		using var memoryStream = new MemoryStream();
		
		bitmap.Save(memoryStream, ImageFormat.Jpeg);

		memoryStream.Position = 0;
		
		_cameraClient.SendFrame(memoryStream.ToArray());
		
		CameraImage.Source = ToBitmapSource(bitmap);
	}

	private BitmapSource ToBitmapSource(Bitmap bitmap)
	{
		var hBitmap = bitmap.GetHbitmap();

		return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
			hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
	}
}