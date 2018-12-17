using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using Joystick.Interfaces;
using Xamarin.Forms;

namespace Joystick.ViewModels
{
    public class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        #region Members

        private readonly IUserSettings _userSettings;
        private readonly INavigationService _navigationService;

        private short _maxX;
        private short _minX;
        private short _maxY;
        private short _minY;
        private short _centerX;
        private short _minEngineStart;
        private short _updateTime;

        #endregion

        #region Props

        public short MaxX
        {
            get => _maxX;
            set => Set(ref _maxX, value);
        }
        
        public short MinX
        {
            get => _minX;
            set => Set(ref _minX, value);
        }

        public short MaxY
        {
            get => _maxY;
            set => Set(ref _maxY, value);
        }

        public short MinY
        {
            get => _minY;
            set => Set(ref _minY, value);
        }

        public short CenterX
        {
            get => _centerX;
            set => Set(ref _centerX, value);
        }

        public short MinEngineStart
        {
            get => _minEngineStart;
            set => Set(ref _minEngineStart, value);
        }

        public short UpdateTime
        {
            get => _updateTime;
            set => Set(ref _updateTime, value);
        }

        #endregion

        public SettingsViewModel(IUserSettings settings, INavigationService navigationService)
        {
            _userSettings = settings;
            _navigationService = navigationService;

            InitCommands();
            LoadSettings();
        }

        private void LoadSettings()
        {
            MaxX = _userSettings.MaxX;
            MinX = _userSettings.MinX;
            MaxY = _userSettings.MaxY;
            MinY = _userSettings.MinY;
            CenterX = _userSettings.CenterX;
            MinEngineStart = _userSettings.MinEngineStart;
            UpdateTime = _userSettings.UpdateInMs;
        }

        #region Commands

        private void InitCommands()
        {
            SaveCommand = new Command(SaveExecute);
        }

        public ICommand SaveCommand { get; private set; }

        private async void SaveExecute()
        {
            _userSettings.MaxX = MaxX;
            _userSettings.MinX = MinX;
            _userSettings.MaxY = MaxY;
            _userSettings.MinY = MinY;
            _userSettings.CenterX = CenterX;
            _userSettings.MinEngineStart = MinEngineStart;
            _userSettings.UpdateInMs = UpdateTime;

            PropChanged?.Invoke(this, EventArgs.Empty);

            await _navigationService.NavigateToHome();
        }

        #endregion

        public event EventHandler<EventArgs> PropChanged;
    }
}
