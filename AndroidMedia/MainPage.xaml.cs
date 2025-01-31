using System.Windows.Input;

namespace AndroidMedia
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            BindingContext = this;
        }

        public bool ShowPlayer { get; set; }
        public string VideoFilePath { get; set; }

        public ICommand PickVideoCommand => new Command(async () =>
        {
            var video = await MediaPicker.Default.PickVideoAsync();
            await HandleVideoResult(video);
        });

        public ICommand CaptureVideoCommand => new Command(async () =>
        {
            var video = await MediaPicker.Default.CaptureVideoAsync();

            await HandleVideoResult(video);
        });

        private async Task HandleVideoResult(FileResult? fileResult)
        {
            if (fileResult == null)
                return;

            var tempFileName = $"temp_meps_video{Path.GetExtension(fileResult.FileName)}";

            var stream = await fileResult.OpenReadAsync();
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);

            var videosPath = FileSystem.AppDataDirectory;
            var localPath = Path.Combine(videosPath, tempFileName);
            File.WriteAllBytes(localPath, memoryStream.ToArray());

            stream.Close();
            memoryStream.Close();

            VideoFilePath = $"@{localPath}";
            ShowPlayer = true;

            OnPropertyChanged(nameof(VideoFilePath));
            OnPropertyChanged(nameof(ShowPlayer));

        }
    }

}
