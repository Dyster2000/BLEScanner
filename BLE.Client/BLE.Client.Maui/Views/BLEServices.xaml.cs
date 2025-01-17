using BLE.Client.Maui.ViewModels;

namespace BLE.Client.Maui.Views;

public partial class BLEServices : ContentPage
{
    private readonly BLEServicesViewModel _viewModel;

    public BLEServices()
    {
        InitializeComponent();

        _viewModel = new();
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.OnAppearing();
    }

    private void Service_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _viewModel.ServiceSelectionChanged(e.CurrentSelection[0] as BLEServiceViewModel);
    }
}