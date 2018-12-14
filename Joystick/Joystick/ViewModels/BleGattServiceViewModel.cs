using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using Joystick.Interfaces;
using Joystick.Utils;
using nexus.core;
using nexus.protocols.ble;
using nexus.protocols.ble.gatt;

namespace Joystick.ViewModels
{
    public class BleGattServiceViewModel : ViewModelBase
    {
        #region Members

        private readonly IBleGattServerConnection _mGattServer;
        private readonly ICustomDisplayAlert _customDisplayAlert;
        private bool _mIsBusy;
        private Guid _mServiceGuid;

        #endregion

        #region Props

        public ObservableCollection<BleGattCharacteristicViewModel> Characteristic { get; } = new ObservableCollection<BleGattCharacteristicViewModel>();

        public Guid Guid => _mServiceGuid;

        public string Id => _mServiceGuid.ToString();

        public bool IsBusy
        {
            get => _mIsBusy;
            protected set => Set(ref _mIsBusy, value);
        }

        public string Name => GetServiceName(_mServiceGuid) ?? "Unknown Service";

        public string PageTitle => GetServiceName(_mServiceGuid) ?? Id;

        #endregion

        public BleGattServiceViewModel(Guid service, IBleGattServerConnection gattServer, ICustomDisplayAlert customDisplayAlert)
        {
            _mServiceGuid = service;
            _customDisplayAlert = customDisplayAlert;
            _mGattServer = gattServer;

            OnAppearing();
        }
        
        public  async void OnAppearing()
        {
            if (IsBusy || Characteristic.Count >= 1)
            {
                return;
            }
            IsBusy = true;
            try
            {
                var services = await _mGattServer.ListServiceCharacteristics(_mServiceGuid);
                var list = services?.ToList();
                if (list != null)
                {
                    foreach (var c in list)
                    {
                        var vm = new BleGattCharacteristicViewModel(_mServiceGuid, c, _mGattServer, _customDisplayAlert);
                        Characteristic.Add(vm);
                    }
                }
            }
            catch (GattException ex)
            {
                _customDisplayAlert.DisplayAlert("Error", ex.Message);
            }

            IsBusy = false;
        }

        private string GetServiceName(Guid guid)
        {
            var known = RegisteredAttributes.GetName(guid);
            return known.IsNullOrEmpty() ? null : (known.EndsWith("Service") ? known : known + " Service");
        }
    }
}
