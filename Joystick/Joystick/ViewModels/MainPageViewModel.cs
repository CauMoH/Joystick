using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using Joystick.Interfaces;
using Xamarin.Forms;

namespace Joystick.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region Members

        private readonly INavigationService _navigationService;
        private readonly IBluetoothManagerViewModel _bluetoothManager;
        private readonly ISettingsViewModel _settingsViewModel;

        private string _title;
        private Tuple<int, int> _joystickRawXposition;
        private Tuple<int, int> _joystickRawYposition;

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
                RaisePropertyChanged(nameof(JoystickXposition));
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
                RaisePropertyChanged(nameof(JoystickYposition));
            }
        }

        public int JoystickXposition
        {
            get
            {
                if (_joystickRawXposition == null || _joystickRawXposition.Item2 == 0)
                {
                    return 0;
                }

                if (_joystickRawXposition.Item2 / 2 == _joystickRawXposition.Item1)
                {
                    return  _settingsViewModel.CenterX;
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

                return result;
            }
        }

        public int JoystickYposition
        {
            get
            {
                if (_joystickRawYposition == null || _joystickRawYposition.Item2 == 0)
                {
                    return 0;
                }

                if (_joystickRawYposition.Item2 / 2 == _joystickRawYposition.Item1)
                {
                    return 0;
                }

                int resolution;
                int result;

                if (_joystickRawYposition.Item2 / 2 > _joystickRawYposition.Item1)
                {
                    resolution = _settingsViewModel.MaxY - _settingsViewModel.MinEngineStart;
                    result = -2 * (_joystickRawYposition.Item1 * resolution / _joystickRawYposition.Item2 - resolution / 2 ) + _settingsViewModel.MinEngineStart;
                }
                else
                {
                    resolution = _settingsViewModel.MinY - _settingsViewModel.MinEngineStart;
                    result = ((_joystickRawYposition.Item1 * resolution / _joystickRawYposition.Item2 - resolution / 2 ) * 2 + _settingsViewModel.MinEngineStart) * -1;
                }

                return result;
            }
        }

        public string Connection => _bluetoothManager.Connection;

        #endregion

        public MainPageViewModel(INavigationService navigationService, IBluetoothManagerViewModel bluetoothManager, ISettingsViewModel settingsViewModel)
        {
            _navigationService = navigationService;
            _bluetoothManager = bluetoothManager;
            _settingsViewModel = settingsViewModel;

            _bluetoothManager.PropChanged += BluetoothManager_OnPropChanged;

            InitCommands();
        }

        #region Event Handlers

        private void BluetoothManager_OnPropChanged(object sender, EventArgs eventArgs)
        {
            RaisePropertyChanged(nameof(Connection));
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
