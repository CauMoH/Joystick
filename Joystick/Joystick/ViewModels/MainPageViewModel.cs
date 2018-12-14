using System;
using System.Timers;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using Joystick.Data;
using Joystick.Interfaces;
using Joystick.Utils;
using nexus.protocols.ble;
using Xamarin.Forms;

namespace Joystick.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region Members

        private readonly INavigationService _navigationService;
        private readonly IBluetoothManagerViewModel _bluetoothManager;
        private readonly ISettingsViewModel _settingsViewModel;
        private readonly IUserSettings _userSettings;

        private string _title;
        private Tuple<int, int> _joystickRawXposition;
        private Tuple<int, int> _joystickRawYposition;
        private int _joystickXposition;
        private int _joystickYposition;
        private bool _isEnableLights;

        private const int UpdateValuesInMs = 200;

        private readonly Timer _timer = new Timer(UpdateValuesInMs);

        private bool _isFirstConnectToLastDevice;


        #endregion

        #region Props

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public Tuple<int, int> JoystickRawXposition
        {
            get => _joystickRawXposition;
            set
            {
                if (value.Item2 == 0)
                    return;

                Set(ref _joystickRawXposition, value);
                
                UpdateXPosition();
            }
        }

        public Tuple<int, int> JoystickRawYposition
        {
            get => _joystickRawYposition;
            set
            {
                if(value.Item2 == 0)
                    return;

                Set(ref _joystickRawYposition, value);
                
                UpdateYPosition();
            }
        }

        public bool IsEnableLights
        {
            get => _isEnableLights;
            set => Set(ref _isEnableLights, value);
        }

        public int JoystickXposition
        {
            get => _joystickXposition;
            set => Set(ref _joystickXposition, value);
        }

        public int JoystickYposition
        {
            get => _joystickYposition;
            set => Set(ref _joystickYposition, value);
        }

        public string Connection => _bluetoothManager.Connection;

        #endregion

        public MainPageViewModel(INavigationService navigationService,
                                 IBluetoothManagerViewModel bluetoothManager,
                                 ISettingsViewModel settingsViewModel,
                                 IUserSettings userSettings)
        {
            _navigationService = navigationService;
            _bluetoothManager = bluetoothManager;
            _settingsViewModel = settingsViewModel;
            _userSettings = userSettings;

            _bluetoothManager.PropChanged += BluetoothManager_OnPropChanged;
            _settingsViewModel.PropChanged += SettingsViewModel_OnPropChanged;

            InitCommands();

            _timer.Elapsed += UpdaterTimer_OnElapsed;
            _timer.Start();

            Init();
        }
        
        private void Init()
        {
            JoystickXposition = _settingsViewModel.CenterX;
            JoystickYposition = 0;
        }

        private void UpdateXPosition()
        {
            if (_joystickRawXposition == null || _joystickRawXposition.Item2 == 0)
            {
                JoystickXposition = 0;
                return;
            }

            if (_joystickRawXposition.Item2 / 2 == _joystickRawXposition.Item1)
            {
                JoystickXposition = _settingsViewModel.CenterX;
                return;
            }

            int resolution;
            int result;

            if (_joystickRawXposition.Item2 / 2 > _joystickRawXposition.Item1)
            {
                resolution = _settingsViewModel.MinX - _settingsViewModel.CenterX;
                result = (_joystickRawXposition.Item1 * resolution / _joystickRawXposition.Item2 - resolution / 2) * 2 - _settingsViewModel.CenterX;
            }
            else
            {
                resolution = _settingsViewModel.MaxX - _settingsViewModel.CenterX;
                result = (_joystickRawXposition.Item1 * resolution / _joystickRawXposition.Item2 - resolution / 2) * 2 + _settingsViewModel.CenterX;
            }

            JoystickXposition = result;
        }

        private void UpdateYPosition()
        {
            if (_joystickRawYposition == null || _joystickRawYposition.Item2 == 0)
            {
                JoystickYposition = 0;
                return;
            }

            if (_joystickRawYposition.Item2 / 2 == _joystickRawYposition.Item1)
            {
                JoystickYposition = 0;
                return;
            }

            int resolution;
            int result;

            if (_joystickRawYposition.Item2 / 2 > _joystickRawYposition.Item1)
            {
                resolution = _settingsViewModel.MaxY - _settingsViewModel.MinEngineStart;
                result = -2 * (_joystickRawYposition.Item1 * resolution / _joystickRawYposition.Item2 - resolution / 2) + _settingsViewModel.MinEngineStart;
            }
            else
            {
                resolution = _settingsViewModel.MinY - _settingsViewModel.MinEngineStart;
                result = ((_joystickRawYposition.Item1 * resolution / _joystickRawYposition.Item2 - resolution / 2) * 2 + _settingsViewModel.MinEngineStart) * -1;
            }

            JoystickYposition = result;
        }
        
        #region Event Handlers

        private void BluetoothManager_OnPropChanged(object sender, EventArgs eventArgs)
        {
            RaisePropertyChanged(nameof(Connection));
        }

        private void SettingsViewModel_OnPropChanged(object sender, EventArgs e)
        {
            Init();
        }

        private void UpdaterTimer_OnElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (_bluetoothManager.IsAdapterEnabled)
                {
                    if (!_isFirstConnectToLastDevice &&
                        !_bluetoothManager.IsConnectedOrConnecting &&
                        !_bluetoothManager.IsScanning &&
                        !string.IsNullOrWhiteSpace(_userSettings.LastBluetoothAddr))
                    {
                        _isFirstConnectToLastDevice = true;
                        _bluetoothManager.ConnectToLastDevice();
                        return;
                    }
                }
                
                if (_bluetoothManager.IsBusy || _bluetoothManager.ConnectionProgress != ConnectionProgress.Connected)
                {
                    return;
                }

                if (_bluetoothManager.Services.Count == 0 || _bluetoothManager.Services[0].Characteristic.Count == 0)
                {
                    return;
                }

                var characteristic = _bluetoothManager.Services[0].Characteristic[0];

                if (!characteristic.CanWrite)
                {
                    return;
                }

                var data = new DataResponse
                {
                    XPosition = JoystickXposition,
                    YPosition = JoystickYposition,
                    IsEnableLights = IsEnableLights
                };

                var bytes = Serializer.SerializeValuesResponse(data);

                characteristic.WriteValue = bytes;
                characteristic.WriteBytesCommand.Execute(null);
            }
            catch
            {
                
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Инициализация команд
        /// </summary>
        private void InitCommands()
        {
            GoToBluetoothCommand = new Command(GoToBluetoothExecute);
            GoToSettingsCommand = new Command(GoToSettingsExecute);
        }

        #region Props

        /// <summary>
        /// Команда на переход в настройкам bluetooth
        /// </summary>
        public ICommand GoToBluetoothCommand { get; private set; }

        /// <summary>
        /// Команда на переход к настройкам
        /// </summary>
        public ICommand GoToSettingsCommand { get; private set; }

        #endregion

        private async void GoToBluetoothExecute()
        {
            await _navigationService.NavigateToBluetooth();
        }

        private async void GoToSettingsExecute()
        {
            await _navigationService.NavigateToSettings();
        }

        #endregion
    }
}
