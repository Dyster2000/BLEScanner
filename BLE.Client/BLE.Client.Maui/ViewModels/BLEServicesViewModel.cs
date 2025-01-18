using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace BLE.Client.Maui.ViewModels
{
    public class BLEServicesViewModel : BaseViewModel
    {
        private readonly IBluetoothLE _bluetoothManager;
        protected IAdapter Adapter;

        public ObservableCollection<BLEServiceViewModel> BLEServices { get; private init; } = [];
        public ObservableCollection<BLECharacteristicViewModel> BLECharacteristics { get; private init; } = [];


        private string _selectedDeviceName = "Selected BLE Device: NONE";
        public string SelectedDeviceName
        {
            get => _selectedDeviceName;
            set
            {
                if (_selectedDeviceName != value)
                {
                    _selectedDeviceName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public BLEServicesViewModel()
        {
            _bluetoothManager = CrossBluetoothLE.Current;
            Adapter = _bluetoothManager?.Adapter;

            if (_bluetoothManager is null)
            {
                DebugMessage("BluetoothManager is null");
            }
            else if (Adapter is null)
            {
                DebugMessage("Adapter is null");
            }
        }

        public async Task<bool> OnAppearing()
        {
            if (BLEScannerViewModel.CurrentDevice == null)
                return false;

            BLEServices.Clear();

            bool connectionLost = false;
            Adapter.DeviceConnectionLost += (o, e) =>
            {
                if (e.Device.Id == BLEScannerViewModel.CurrentDevice.DeviceId)
                    connectionLost = true;
            };

            do
            {
                connectionLost = false;
                BLEServices.Clear();

                var connectedDevice = await Adapter.ConnectToKnownDeviceAsync(BLEScannerViewModel.CurrentDevice.DeviceId);

                if (connectedDevice == null)
                {
                    DebugMessage($"Failed to connect to device {BLEScannerViewModel.CurrentDevice.Name}:{BLEScannerViewModel.CurrentDevice.DeviceId}");
                    SelectedDeviceName = $"Selected BLE Device: {BLEScannerViewModel.CurrentDevice.DeviceId} NOT FOUND";
                    return false;
                }
                SelectedDeviceName = "Selected BLE Device: " + BLEScannerViewModel.CurrentDevice.Name;

                try
                {
                    var servicesListReadOnly = await connectedDevice.GetServicesAsync(); // Read in the Services available

                    for (int i = 0; i < servicesListReadOnly.Count; i++)
                    {
                        AddService(servicesListReadOnly[i]);
                    }
                }
                catch (Exception ex)
                {
                    DebugMessage($"Error in GetServicesAsync() for selected device {BLEScannerViewModel.CurrentDevice.DeviceId} - Error: {ex}");
                }
                // If we made it through the end without loosing connection we are good. Otherwise try again
            } while (connectionLost);

            return true;
        }

        public async void ServiceSelectionChanged(BLEServiceViewModel selected)
        {
            if (BLEScannerViewModel.CurrentDevice == null)
                return;

            BLECharacteristics.Clear();

            bool connectionLost = false;
            Adapter.DeviceConnectionLost += (o, e) =>
            {
                if (e.Device.Id == BLEScannerViewModel.CurrentDevice.DeviceId)
                    connectionLost = true;
            };

            do
            {
                connectionLost = false;
                BLECharacteristics.Clear();

                var connectedDevice = await Adapter.ConnectToKnownDeviceAsync(BLEScannerViewModel.CurrentDevice.DeviceId);

                if (connectedDevice == null)
                {
                    DebugMessage($"[ServiceSelectionChanged] Failed to connect to device {BLEScannerViewModel.CurrentDevice.Name}:{BLEScannerViewModel.CurrentDevice.DeviceId}");
                    return;
                }

                try
                {
                    var service = await connectedDevice.GetServiceAsync(selected.ServiceId);

                    if (service == null)
                    {
                        DebugMessage($"[ServiceSelectionChanged] Failed to find service {selected.Name}:{selected.ServiceId}");
                        break;
                    }

                    var characteristicListReadOnly = await service.GetCharacteristicsAsync(); // Read in the Characteristics available

                    for (int i = 0; i < characteristicListReadOnly.Count; i++)
                    {
                        AddCharacteristic(characteristicListReadOnly[i]);
                    }
                }
                catch (Exception ex)
                {
                    DebugMessage($"Error getting characteristics for {selected.Name}:{selected.ServiceId} - Error: {ex}");
                }
                // If we made it through the end without loosing connection we are good. Otherwise try again
            } while (connectionLost);
        }

        private void AddService(IService service)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var vm = new BLEServiceViewModel(service);
                DebugMessage($"Add Service: {vm}");
                BLEServices.Add(vm);
            });
        }

        private void AddCharacteristic(ICharacteristic characteristic)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var vm = new BLECharacteristicViewModel(characteristic);
                DebugMessage($"Add Characteristic: {vm}");
                BLECharacteristics.Add(vm);
            });
        }

        private void DebugMessage(string message)
        {
            Debug.WriteLine(message);
            App.Logger.AddMessage(message);
        }
    }
}
