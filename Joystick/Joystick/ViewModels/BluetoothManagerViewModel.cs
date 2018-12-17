using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using Joystick.Interfaces;
using Joystick.Utils;
using nexus.core;
using nexus.protocols.ble;
using nexus.protocols.ble.gatt;
using Xamarin.Forms;

namespace Joystick.ViewModels
{
    public class BluetoothManagerViewModel : ViewModelBase, IBluetoothManagerViewModel
    {
        #region Members

        private readonly IUserSettings _userSettings;
        private readonly INavigationService _navigationService;
        private readonly IBluetoothService _bluetoothService;
        private readonly ICustomDisplayAlert _customDisplayAlert;

        private bool _isScanning;

        protected CancellationTokenSource MScanCancel;
        private DateTime _mScanStopTime;

        private BlePeripheralViewModel _connectedDevice;
        private IBleGattServerConnection _mGattServer;
        private string _connection = string.Empty;

        private ConnectionProgress _connectionProgress = ConnectionProgress.Disconnected;

        private bool _mIsBusy;

        #endregion

        #region Props

        public ObservableCollection<BlePeripheralViewModel> DevicesList { get; } = new ObservableCollection<BlePeripheralViewModel>();

        public bool IsAdapterEnabled => _bluetoothService.Adapter.CurrentState.Value == EnabledDisabledState.Enabled ||
                                           _bluetoothService.Adapter.CurrentState.Value == EnabledDisabledState.Unknown;

        public bool IsScanning
        {
            get => _isScanning;
            protected set => Set(ref _isScanning, value);
        }

        public int ScanTimeRemaining => (int)AppUtils.ClampSeconds((_mScanStopTime - DateTime.UtcNow).TotalSeconds);

        public string ScanLabel => $"Поиск, осталось: {ScanTimeRemaining} сек.";

        public string Connection
        {
            get => _connection;
            set
            {
                Set(ref _connection, value);
                PropChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public ConnectionProgress ConnectionProgress
        {
            get => _connectionProgress;
            set
            {
                Set(ref _connectionProgress, value);
                PropChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool IsBusy
        {
            get => _mIsBusy;
            protected set
            {
                if (value != _mIsBusy)
                {
                    _mIsBusy = value;
                    RaisePropertyChanged(nameof(IsConnectedOrConnecting));
                    PropChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool IsConnectedOrConnecting => _mIsBusy || _connectionProgress != ConnectionProgress.Disconnected;

        public ObservableCollection<BleGattServiceViewModel> Services { get; } = new ObservableCollection<BleGattServiceViewModel>();

        #endregion

        public BluetoothManagerViewModel(IUserSettings userSettings,
            INavigationService navigationService,
            IBluetoothService bluetoothService,
            ICustomDisplayAlert customDisplayAlert)
        {
            _userSettings = userSettings;
            _navigationService = navigationService;
            _bluetoothService = bluetoothService;
            _customDisplayAlert = customDisplayAlert;

            InitCommands();
            InitBluetooth();
        }

        private async void InitBluetooth()
        {
            _bluetoothService.Adapter.CurrentState.Subscribe(state =>
            {
                RaisePropertyChanged(nameof(IsAdapterEnabled));
                PropChanged?.Invoke(this ,EventArgs.Empty);
            });

            if (!IsAdapterEnabled)
            {
                await ToggleAdapter(true);
            }
        }

        private async Task ToggleAdapter(bool enable)
        {
            StopScan();
            try
            {
                await (enable ? _bluetoothService.Adapter.EnableAdapter() : _bluetoothService.Adapter.DisableAdapter());
            }
            catch (SecurityException ex)
            {
                _customDisplayAlert.DisplayAlert("Ошибка", ex.Message);
            }

            RaisePropertyChanged(nameof(IsAdapterEnabled));
            PropChanged?.Invoke(this, EventArgs.Empty);
        }

        private void StopScan()
        {
            MScanCancel?.Cancel();
        }

        public void ConnectToLastDevice()
        {
            Connect(Guid.Parse(_userSettings.LastBluetoothAddr));
        }

        private async void Connect(object device)
        {
            try
            {
                StopScan();

                if (IsBusy || _mGattServer != null && _mGattServer.State != ConnectionState.Disconnected)
                {
                    return;
                }

                await Disconnect();
                IsBusy = true;

                BlePeripheralConnectionRequest connection;

                if (device is BlePeripheralViewModel model)
                {
                    _connectedDevice = model;

                    connection = await _bluetoothService.Adapter.ConnectToDevice(
                        _connectedDevice.Model,
                        TimeSpan.FromSeconds(AppUtils.CONNECTION_TIMEOUT_SECONDS),
                        progress => { ConnectionProgress = progress; });
                }
                else
                {
                    var deviceGuid = device is Guid guid ? guid : new Guid();

                    connection = await _bluetoothService.Adapter.ConnectToDevice(
                        deviceGuid,
                        TimeSpan.FromSeconds(AppUtils.CONNECTION_TIMEOUT_SECONDS),
                        progress => { ConnectionProgress = progress; });
                }
               
                if (connection.IsSuccessful())
                {
                    _mGattServer = connection.GattServer;

                    _userSettings.LastBluetoothAddr = _mGattServer.DeviceId.ToString();

                    _mGattServer.Subscribe(
                        async c =>
                        {
                            if (c == ConnectionState.Disconnected)
                            {
                                _customDisplayAlert.DisplayAlert("Info", "Device disconnected");
                                await Disconnect();
                            }

                            Connection = c.ToString();
                        });

                    Connection = "Reading Services";

                    try
                    {
                        var services = (await _mGattServer.ListAllServices()).ToList();
                        foreach (var serviceId in services)
                        {
                            if (Services.Any(viewModel => viewModel.Guid.Equals(serviceId)))
                            {
                                continue;
                            }

                            Services.Add(new BleGattServiceViewModel(serviceId, _mGattServer, _customDisplayAlert));
                        }

                        if (Services.Count == 0)
                        {
                            _customDisplayAlert.DisplayAlert("Info", "No services found");
                        }

                        Connection = _mGattServer.ToString();
                    }
                    catch (GattException ex)
                    {
                        _customDisplayAlert.DisplayAlert("Error", ex.Message);
                    }
                }
                else
                {
                    ConnectionProgress = ConnectionProgress.Disconnected;

                    string errorMsg;
                    if (connection.ConnectionResult == ConnectionResult.ConnectionAttemptCancelled)
                    {
                        errorMsg = "Connection attempt cancelled after {0} seconds (see {1})".F(
                           AppUtils.CONNECTION_TIMEOUT_SECONDS,
                           GetType().Name + ".cs");
                    }
                    else
                    {
                        errorMsg = "Error connecting to device: {0}".F(connection.ConnectionResult);
                    }

                    _customDisplayAlert.DisplayAlert("Error", errorMsg);
                }

                IsBusy = false;

            }
            catch (Exception ex)
            {
                ConnectionProgress = ConnectionProgress.Disconnected;
                IsBusy = false;
                _customDisplayAlert.DisplayAlert("Ошибка", ex.Message);
            }
        }

        public async Task Disconnect()
        {
            IsBusy = true;
            if (_mGattServer != null)
            {
                await _mGattServer.Disconnect();
                _mGattServer = null;
            }

            Services.Clear();
            IsBusy = false;
        }

        #region Commands

        private void InitCommands()
        {
            GoToHomeCommand = new Command(GoToHomeExecute);
            SearchCommand = new Command(SearchExecute);
            ConnectCommand = new Command<BlePeripheralViewModel>(ConnectExecute);
        }

        #region Props

        public ICommand GoToHomeCommand { get; private set; }

        public ICommand SearchCommand { get; private set; }

        public ICommand ConnectCommand { get; private set; }
        
        #endregion

        private async void GoToHomeExecute()
        {
            DevicesList.Clear();
            StopScan();
            IsBusy = false;
            await _navigationService.NavigateToHome();
        }

        private async void SearchExecute()
        {
            if (IsScanning)
            {
                return;
            }

            if (!IsAdapterEnabled)
            {
                _customDisplayAlert.DisplayAlert("Ошибка", "Cannot start scan, Bluetooth is turned off");
                return;
            }

            StopScan();
            IsScanning = true;
            var seconds = AppUtils.ClampSeconds(AppUtils.SCAN_SECONDS_MAX);
            MScanCancel = new CancellationTokenSource(TimeSpan.FromSeconds(seconds));
            _mScanStopTime = DateTime.UtcNow.AddSeconds(seconds);

            RaisePropertyChanged(nameof(ScanTimeRemaining));
            RaisePropertyChanged(nameof(ScanLabel));
            PropChanged?.Invoke(this, EventArgs.Empty);
            Device.StartTimer(
               TimeSpan.FromSeconds(1),
               () =>
               {
                   RaisePropertyChanged(nameof(ScanTimeRemaining));
                   RaisePropertyChanged(nameof(ScanLabel));
                   PropChanged?.Invoke(this, EventArgs.Empty);
                   return IsScanning;
               });
           
            await _bluetoothService.Adapter.ScanForBroadcasts(
                peripheral =>
                {
                    Device.BeginInvokeOnMainThread(
                        () =>
                        {
                            var existing = DevicesList.FirstOrDefault(d => d.Equals(peripheral));
                            if (existing != null)
                            {
                                existing.Update(peripheral);
                            }
                            else
                            {
                                DevicesList.Add(new BlePeripheralViewModel(peripheral));
                            }
                        });
                },
               MScanCancel.Token);

            IsScanning = false;
        }

        private void ConnectExecute(BlePeripheralViewModel device)
        {
            Connect(device);
        }

        private async Task Update(BlePeripheralViewModel peripheral)
        {
            if (_connectedDevice != null && !_connectedDevice.Model.Equals(peripheral.Model))
            {
                await Disconnect();
            }

            _connectedDevice = peripheral;
        }

        #endregion

        public event EventHandler<EventArgs> PropChanged;
    }
}
