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

        private int _maxX;
        private int _minX;
        private int _maxY;
        private int _minY;
        private int _centerX;
        private int _minEngineStart;

        #endregion

        #region Props

        public int MaxX
        {
            get => _maxX;
            set => Set(ref _maxX, value);
        }
        
        public int MinX
        {
            get => _minX;
            set => Set(ref _minX, value);
        }

        public int MaxY
        {
            get => _maxY;
            set => Set(ref _maxY, value);
        }

        public int MinY
        {
            get => _minY;
            set => Set(ref _minY, value);
        }

        public int CenterX
        {
            get => _centerX;
            set => Set(ref _centerX, value);
        }

        public int MinEngineStart
        {
            get => _minEngineStart;
            set => Set(ref _minEngineStart, value);
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

            await _navigationService.NavigateToHome();
        }

        #endregion

    }
}
