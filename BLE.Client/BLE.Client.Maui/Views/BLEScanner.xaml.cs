using BLE.Client.Maui.ViewModels;

namespace BLE.Client.Maui.Views;

public partial class BLEScanner : ContentPage
{
    private readonly BLEScannerViewModel _viewModel;

    public BLEScanner()
    {
        InitializeComponent();
        _viewModel = new();
        BindingContext = _viewModel;
    }

    private void Device_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ConnectButton.IsEnabled = true;
        _viewModel.Device_SelectionChanged(sender, e);
    }

    private void Connect_Clicked(object sender, EventArgs e)
    {
        _viewModel.Connect_Clicked(sender, e);
    }
}