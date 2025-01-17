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

        private IDevice _connectedDevice; // Pointer for the connected BLE Device

        public BLEServicesViewModel()
        {
            _connectedDevice = null;
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
            _connectedDevice = await Adapter.ConnectToKnownDeviceAsync(BLEScannerViewModel.CurrentDevice.DeviceId);

            if (_connectedDevice == null)
                return false;
            SelectedDeviceName = "Selected BLE Device: " + BLEScannerViewModel.CurrentDevice.Name;

            try
            {
                var servicesListReadOnly = await _connectedDevice.GetServicesAsync(); // Read in the Services available

                BLEServices.Clear();
                for (int i = 0; i < servicesListReadOnly.Count; i++)
                {
                    AddService(servicesListReadOnly[i]);
                }
            }
            catch
            {
                DebugMessage($"Error in GetServicesAsync() for selected service {_connectedDevice.Id}");
            }

            return true;
        }

        public async void ServiceSelectionChanged(BLEServiceViewModel selected)
        {
            try
            {
                var characteristicListReadOnly = await selected.Service.GetCharacteristicsAsync(); // Read in the Characteristics available

                BLECharacteristics.Clear();
                for (int i = 0; i < characteristicListReadOnly.Count; i++)
                {
                    AddCharacteristic(characteristicListReadOnly[i]);
                }
            }
            catch
            {
                DebugMessage($"Error in GetCharacteristicsAsync() for selected service {selected.Service.Id}");
            }
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
