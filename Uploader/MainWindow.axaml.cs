using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform;

namespace Uploader;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }


    private async Task ClipBoardPaste()
    {
        var clipboard = this.Clipboard;
        var text = await clipboard.GetTextAsync();

        //TODO: FINISH THIS FUNCTION

    }
    private async void PasteArea_KeyDown(object sender, KeyEventArgs Arg)
    {
        if ((Arg.KeyModifiers == KeyModifiers.Control && Arg.Key == Key.V) || (Arg.KeyModifiers == KeyModifiers.Meta && Arg.Key == Key.V))
        {
            await ClipBoardPaste();
        }
    }


}