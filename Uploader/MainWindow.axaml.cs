using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace Uploader;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }


    private async Task VideoUploader(string FilePath, string VideoName)
    {
        var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
        new ClientSecrets
        {
            ClientId = "",
            ClientSecret = ""
        },
        new[] { YouTubeService.Scope.YoutubeUpload },
        "user",
        CancellationToken.None
    );


        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "YouTubeUploader"
        });

        var video = new Video
        {
            Snippet = new VideoSnippet
            {
                Title = VideoName,
                Description = "",
                Tags = new string[] { "gaming", "clip" },
                CategoryId = "22"
            },
            Status = new VideoStatus
            {
                PrivacyStatus = "unlisted"
            }
        };

        using (var fileStream = new FileStream(FilePath, FileMode.Open))
        {
            var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");


            videosInsertRequest.ProgressChanged += progress =>
            {
                switch (progress.Status)
                {
                    case UploadStatus.Uploading:
                        AppendToConsole($"Uploading... {progress.BytesSent} bytes sent.");
                        break;
                    case UploadStatus.Completed:
                        AppendToConsole("Upload completed!");
                        break;
                    case UploadStatus.Failed:
                        AppendToConsole("Upload failed: " + progress.Exception);
                        break;
                }
            };


            await videosInsertRequest.UploadAsync();
        }
    }
    private async Task ClipBoardPaste()
    {
        var clipboard = this.Clipboard;
        if (clipboard == null)
        {
            AppendToConsole("Clipboard is null");
            return;
        }
        var text = await clipboard.GetTextAsync();

        //Debug
        AppendToConsole("Clipboard text: " + (text ?? "null"));


        if (Uri.TryCreate(text, UriKind.Absolute, out var uri) && uri.IsFile)
        {

            string filePath = uri.LocalPath;


            string videoTitle = VideoNameBox.Text;
            if (string.IsNullOrWhiteSpace(videoTitle))
            {
                AppendToConsole("No video title specified. Please enter a name.");
                return;
            }
            await VideoUploader(filePath, videoTitle);

        }

        else
        {
            var fileData = await clipboard.GetDataAsync(DataFormats.Files);

            if (fileData is System.Collections.IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {

                    if (item is Avalonia.Platform.Storage.IStorageItem storageItem)
                    {
                        AppendToConsole("Clipboard file path: " + storageItem.Path.LocalPath);


                        string videoTitle = VideoNameBox.Text;
                        if (string.IsNullOrWhiteSpace(videoTitle))
                        {
                            AppendToConsole("No video title specified. Please enter a name.");
                            return;
                        }


                        await VideoUploader(storageItem.Path.LocalPath, videoTitle);
                    }
                }
            }
            else
            {
                AppendToConsole("Clipboard data is not a file or a valid URI.");
            }

        }
    }
    private async void PasteArea_KeyDown(object sender, KeyEventArgs Arg)
    {

        if ((Arg.KeyModifiers == KeyModifiers.Control && Arg.Key == Key.V) || (Arg.KeyModifiers == KeyModifiers.Meta && Arg.Key == Key.V))
        {
            await ClipBoardPaste();
        }
    }

    private void PasteArea_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        (sender as Control)?.Focus();
    }

    private void AppendToConsole(string message)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            ConsoleOutput.Text += message + Environment.NewLine;
        });
    }

}