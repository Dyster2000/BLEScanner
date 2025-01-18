using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Extensions;

namespace BLE.Client.Maui.ViewModels
{
    public class BLECharacteristicViewModel : BaseViewModel
    {
        private readonly Guid DominoStatusCharacteristic = new Guid("b43a1a69-5dc4-4573-b47c-53e31ca661f2");
        private readonly Guid DominoManualControlCharacteristic = new Guid("874b19c2-4bfa-4453-83b4-e0d3a28317fd");
        private readonly Guid DominoDrawControlCharacteristic = new Guid("56d0d406-5ae9-4e66-8ff7-bd43c12e6263");

        private Guid _characteristicId = new();
        public Guid CharacteristicId
        {
            get => _characteristicId;
            set
            {
                if (_characteristicId != value)
                {
                    _characteristicId = value;
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

        public BLECharacteristicViewModel(ICharacteristic characteristic)
        {
            CharacteristicId = characteristic.Id;
            if (CharacteristicId == DominoStatusCharacteristic)
                Name = "** HackPack Domino Status Characteristic **";
            else if (CharacteristicId == DominoManualControlCharacteristic)
                Name = "** HackPack Domino Manual Control Characteristic **";
            else if (CharacteristicId == DominoDrawControlCharacteristic)
                Name = "** HackPack Domino Draw Control Characteristic **";
            else
                Name = characteristic.Name;
        }

        public override string ToString()
        {
            return $"{Name}:{CharacteristicId}";
        }
    }
}
