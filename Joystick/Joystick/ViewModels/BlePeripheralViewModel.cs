using System;
using System.Linq;
using GalaSoft.MvvmLight;
using Joystick.Utils;
using nexus.core;
using nexus.core.text;
using nexus.protocols.ble.scan;

namespace Joystick.ViewModels
{
    public sealed class BlePeripheralViewModel: ViewModelBase, IEquatable<IBlePeripheral>
    {
        #region Members

        private IBlePeripheral _model;

        #endregion

        #region Props

        public IBlePeripheral Model => _model;

        public string Address => _model.Address != null && _model.Address.Length > 0
            ? _model.Address.Select(b => b.EncodeToBase16String()).Join(":")
            : Id;

        public string AddressAndName => Address + " / " + DeviceName;

        public string AdvertisedServices => _model.Advertisement?.Services.Select(
            x =>
            {
                var name = RegisteredAttributes.GetName(x);
                return name.IsNullOrEmpty()
                    ? x.ToString()
                    : x.ToString() + " (" + name + ")";
            }).Join(", ");

        public string Advertisement => _model.Advertisement.ToString();

        public string DeviceName => _model.Advertisement.DeviceName;

        public string Flags => _model.Advertisement?.Flags.ToString("G");

        public string Id => _model.DeviceId.ToString();

        public string Name => _model.Advertisement.DeviceName ?? Address;

        public int Rssi => _model.Rssi;

        public string ServiceData => _model.Advertisement?.ServiceData
            .Select(x => x.Key + "=0x" + x.Value?.ToArray()?.EncodeToBase16String())
            .Join(", ");

        public string Signal => _model.Rssi + " / " + _model.Advertisement.TxPowerLevel;

        public int TxPowerLevel => _model.Advertisement.TxPowerLevel;


        #endregion

        public override bool Equals(object other)
        {
            return _model.Equals(other);
        }

        public bool Equals(IBlePeripheral other)
        {
            return _model.Equals(other);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return _model.GetHashCode();
        }

        public BlePeripheralViewModel(IBlePeripheral model)
        {
            _model = model;
        }

        public void Update(IBlePeripheral model)
        {
            if (!Equals(_model, model))
            {
                _model = model;
            }
            RaisePropertyChanged(nameof(Address));
            RaisePropertyChanged(nameof(AddressAndName));
            RaisePropertyChanged(nameof(AdvertisedServices));
            RaisePropertyChanged(nameof(Advertisement));
            RaisePropertyChanged(nameof(DeviceName));
            RaisePropertyChanged(nameof(Flags));
            RaisePropertyChanged(nameof(Name));
            RaisePropertyChanged(nameof(Rssi));
            RaisePropertyChanged(nameof(ServiceData));
            RaisePropertyChanged(nameof(Signal));
            RaisePropertyChanged(nameof(TxPowerLevel));
        }
    }
}
