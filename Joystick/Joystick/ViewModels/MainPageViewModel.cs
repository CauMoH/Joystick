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

        private string _title;
        private int _joystickXposition;
        private int _joystickYposition;

        private IBluetoothManagerViewModel _bluetoothManager;

        #endregion

        #region Props

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
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

        public MainPageViewModel(INavigationService navigationService, IBluetoothManagerViewModel bluetoothManager)
        {
            _navigationService = navigationService;
            _bluetoothManager = bluetoothManager;
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
        }

        #region Props

        /// <summary>
        /// Команда на выбор экрана bluetooth
        /// </summary>
        public ICommand GoToBluetoothCommand { get; private set; }

        #endregion

        private async void GoToBluetoothExecute()
        {
            await _navigationService.NavigateToBluetooth();
        }

        #endregion
    }
}
