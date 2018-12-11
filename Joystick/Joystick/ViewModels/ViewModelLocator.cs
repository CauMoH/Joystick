using CommonServiceLocator;
using Joystick.Interfaces;
using Joystick.Ioc;

namespace Joystick.ViewModels
{
    public class ViewModelLocator
    {
        #region Members

        private readonly IoCConfig _iocConfig;

        #endregion

        #region Props

        public MainPageViewModel MainPageViewModel => ServiceLocator.Current.GetInstance<MainPageViewModel>();
        public IBluetoothManagerViewModel BluetoothManagerViewModel => ServiceLocator.Current.GetInstance<IBluetoothManagerViewModel>();
        public ISettingsViewModel SettingsViewModel => ServiceLocator.Current.GetInstance<ISettingsViewModel>();

        #endregion

        public ViewModelLocator()
        {
            _iocConfig = new IoCConfig();
            _iocConfig.RegisterServices();
            _iocConfig.RegisterViewModels();
        }
    }
}
