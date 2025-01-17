using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Text;

namespace BLE.Client.Maui.ViewModels;

public class LogViewModel
{
    public ICommand ClearLogMessages { get; init; } = new Command(ClearMessages);
    public ICommand CopyToClipboard { get; init; } = new Command(CopyLogToClipboard);

    public static ReadOnlyObservableCollection<string> Messages => App.Logger.Messages;

    private static void ClearMessages()
    {
        App.Logger.ClearMessages();
    }

    private static void CopyLogToClipboard()
    {
        var sb = new StringBuilder();

        sb.AppendJoin("\r\n", Messages);
        Clipboard.Default.SetTextAsync(sb.ToString());
    }
}
