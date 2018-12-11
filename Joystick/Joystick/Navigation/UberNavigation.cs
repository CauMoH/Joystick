using System.Threading.Tasks;
using Joystick.Interfaces;
using Joystick.Views;
using Xamarin.Forms;

namespace Joystick.Navigation
{
    public class UberNavigation : INavigationService
    {
        public async Task NavigateToHome()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new MainPage(), true);
        }

        public async Task NavigateToBluetooth()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new BluetoothSheetPage(), true);
        }

        public async Task NavigateToSettings()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new SettingsSheetPage(), true);
        }
    }
}
