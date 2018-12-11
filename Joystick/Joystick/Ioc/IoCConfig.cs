using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using Joystick.Configuration;
using Joystick.Interfaces;
using Joystick.Navigation;
using Joystick.Services;
using Joystick.ViewModels;

namespace Joystick.Ioc
{
    public class IoCConfig
    {
        public IoCConfig()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
        }

        public void RegisterServices()
        {
            SimpleIoc.Default.Register<ICustomDisplayAlert, CustomDisplayAlert>();
            SimpleIoc.Default.Register<INavigationService, UberNavigation>();
            SimpleIoc.Default.Register<IUserSettings, UserSettings>();
            SimpleIoc.Default.Register<IBluetoothService, BluetoothService>();
        }

        public void RegisterViewModels()
        {
            SimpleIoc.Default.Register<IBluetoothManagerViewModel, BluetoothManagerViewModel>();
            SimpleIoc.Default.Register<ISettingsViewModel, SettingsViewModel>();
            SimpleIoc.Default.Register<MainPageViewModel>();
        }
    }
}
