using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Extensions;

namespace BLE.Client.Maui.ViewModels
{
    public class BLEServiceViewModel : BaseViewModel
    {
        public IService Service { get; private set; }

        private readonly Guid DominoService = new Guid("faa94de0-cd7c-43fa-b71d-40324ff9ab2b");

        private Guid _serviceId = new();
        public Guid ServiceId
        {
            get => _serviceId;
            set
            {
                if (_serviceId != value)
                {
                    _serviceId = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public BLEServiceViewModel(IService service)
        {
            Service = service;
            ServiceId = service.Id;
            if (ServiceId == DominoService)
                Name = "** HackPack Domino Service **";
            else
                Name = service.Name;
        }

        public override string ToString()
        {
            return $"{Name}:{ServiceId}";
        }
    }
}
