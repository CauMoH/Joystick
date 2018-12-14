using System.Linq;
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
            await Application.Current.MainPage.Navigation.PopToRootAsync();
        }

        public async Task NavigateToBluetooth()
        {
            RemoveLastPage();
            await Application.Current.MainPage.Navigation.PushAsync(new BluetoothSheetPage(), true);
        }

        public async Task NavigateToSettings()
        {
            RemoveLastPage();
            await Application.Current.MainPage.Navigation.PushAsync(new SettingsSheetPage(), true);
        }

        private async void RemoveLastPage()
        {
            var lastPage = Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();
            if (!(lastPage is MainPage))
            {
                await Application.Current.MainPage.Navigation.PopAsync();
            }
        }
    }
}
