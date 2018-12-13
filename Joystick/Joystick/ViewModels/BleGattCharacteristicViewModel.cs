using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using Joystick.Interfaces;
using Joystick.Utils;
using nexus.core;
using nexus.core.text;
using nexus.protocols.ble;
using nexus.protocols.ble.gatt;
using Xamarin.Forms;

namespace Joystick.ViewModels
{
    public class BleGattCharacteristicViewModel : ViewModelBase
    {
        #region Members

        private readonly IBleGattServerConnection _mGattServer;
        private readonly ICustomDisplayAlert _customDisplayAlert;
        private readonly Guid _mServiceGuid;
        private Guid _mCharacteristicGuid;
        private string _mDescriptorValues;
        private bool _mIsBusy;
        private IDisposable _mNotificationSubscription;
        private CharacteristicProperty _mProps;
        private string _mValueAsHex;
        private string _mValueAsString;
        private byte[] _mWriteValue;

        #endregion

        #region Props

        public bool CanNotify => _mProps.CanNotify() && !IsBusy;

        public bool CanRead => _mProps.CanRead() && !IsBusy;

        public bool CanWrite => _mProps.CanWrite() && !IsBusy;

        public string DescriptorValues
        {
            get => _mDescriptorValues;
            private set => Set(ref _mDescriptorValues, value);
        }

        public string Id => _mCharacteristicGuid.ToString();

        public bool IsBusy
        {
            get => _mIsBusy;
            protected set
            {
                if (value != _mIsBusy)
                {
                    _mIsBusy = value;
                    RaisePropertyChanged(nameof(CanRead));
                    RaisePropertyChanged(nameof(CanNotify));
                    RaisePropertyChanged(nameof(CanWrite));
                }
            }
        }

        public string Name => RegisteredAttributes.GetName(_mCharacteristicGuid) ?? "Unknown Characteristic";

        public bool NotifyEnabled
        {
            get => _mNotificationSubscription != null;
            set
            {
                if (value != (_mNotificationSubscription != null))
                {
                    if (value)
                    {
                        EnableNotificationsExecute();
                    }
                    else
                    {
                        DisableNotificationsExecute();
                    }
                }
            }
        }

        public string ValueAsHex
        {
            get => _mValueAsHex;
            private set => Set(ref _mValueAsHex, value);
        }

        public string ValueAsString
        {
            get => _mValueAsString;
            private set => Set(ref _mValueAsString, value);
        }

        public byte[] WriteValue
        {
            get => _mWriteValue;
            set => Set(ref _mWriteValue, value);
        }

        #endregion

        public BleGattCharacteristicViewModel(Guid serviceGuid, Guid characteristicGuid,
                                               IBleGattServerConnection gattServer,
                                               ICustomDisplayAlert customDisplayAlert)
        {
            _mGattServer = gattServer;
            _mServiceGuid = serviceGuid;
            _mCharacteristicGuid = characteristicGuid;
            _customDisplayAlert = customDisplayAlert;

            ValueAsHex = string.Empty;
            ValueAsString = string.Empty;

            _mGattServer.ReadCharacteristicProperties(_mServiceGuid, _mCharacteristicGuid).ContinueWith(
               x =>
               {
                   Device.BeginInvokeOnMainThread(
                   () =>
                     {
                         if (x.IsFaulted)
                         {
                             _customDisplayAlert.DisplayAlert("Error", x.Exception?.GetBaseException().Message);
                         }
                         else
                         {
                             _mProps = x.Result;
                             RaisePropertyChanged(nameof(CanNotify));
                             RaisePropertyChanged(nameof(CanRead));
                             RaisePropertyChanged(nameof(CanWrite));
                         }
                     });
               });

            InitCommands();
        }
        
        public async Task UpdateDescriptors()
        {
            try
            {
                var descriptors = (await _mGattServer.ListCharacteristicDescriptors(_mServiceGuid, _mCharacteristicGuid))
                   .ToList();
                var vals = "";
                foreach (var desc in descriptors)
                {
                    vals += desc + ": " +
                            await _mGattServer.ReadDescriptorValue(_mServiceGuid, _mCharacteristicGuid, desc) + "\n";
                }
                DescriptorValues = vals;
            }
            catch (GattException ex)
            {
                _customDisplayAlert.DisplayAlert("Error", ex.Message);
            }
        }
        
        private void UpdateDisplayedValue(byte[] bytes)
        {
            ValueAsHex = bytes.EncodeToBase16String();
            try
            {
                ValueAsString = bytes.AsUtf8String();
            }
            catch
            {
                ValueAsString = string.Empty;
            }
        }

        #region Commands

        private void InitCommands()
        {
            RefreshValueCommand = new Command(async () => { await ReadCharacteristicValueExecute(); });
            EnableNotificationsCommand = new Command(EnableNotificationsExecute);
            DisableNotificationsCommand = new Command(DisableNotificationsExecute);
            ToggleNotificationsCommand = new Command(() => { NotifyEnabled = !NotifyEnabled; });
            WriteBytesCommand = new Command(async () => { await WriteCurrentBytesExecute(); });
        }

        #region Props

        public ICommand DisableNotificationsCommand { get; private set; }
        public ICommand EnableNotificationsCommand { get; private set; }
        public ICommand WriteBytesCommand { get; private set; }
        public ICommand RefreshValueCommand { get; private set; }
        public ICommand ToggleNotificationsCommand { get; private set; }

        #endregion

        private async Task ReadCharacteristicValueExecute()
        {
            IsBusy = true;
            try
            {
                UpdateDisplayedValue(await _mGattServer.ReadCharacteristicValue(_mServiceGuid, _mCharacteristicGuid));
            }
            catch (GattException ex)
            {
                _customDisplayAlert.DisplayAlert("Error", ex.Message);
            }
            IsBusy = false;
        }

        private void EnableNotificationsExecute()
        {
            if (_mNotificationSubscription == null)
            {
                try
                {
                    _mNotificationSubscription = _mGattServer.NotifyCharacteristicValue(
                        _mServiceGuid,
                        _mCharacteristicGuid,
                        UpdateDisplayedValue);
                }
                catch (GattException ex)
                {
                    _customDisplayAlert.DisplayAlert("Error", ex.Message);
                }
            }
            RaisePropertyChanged(nameof(NotifyEnabled));
        }

        private void DisableNotificationsExecute()
        {
            _mNotificationSubscription?.Dispose();
            _mNotificationSubscription = null;
            RaisePropertyChanged(nameof(NotifyEnabled));
        }

        private async Task WriteCurrentBytesExecute()
        {
            if (_mWriteValue.Length != 0)
            {
                try
                {
                    IsBusy = true;
                    var writeTask = _mGattServer.WriteCharacteristicValue(_mServiceGuid, _mCharacteristicGuid, _mWriteValue);
                    // update the characteristic value with the awaited results of the write
                    UpdateDisplayedValue(await writeTask);
                }
                catch (GattException ex)
                {
                    _customDisplayAlert.DisplayAlert("Error", ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        #endregion
    }
}
