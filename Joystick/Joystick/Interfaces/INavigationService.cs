using System.Threading.Tasks;

namespace Joystick.Interfaces
{
    public interface INavigationService
    {
        Task NavigateToHome();

        Task NavigateToBluetooth();
    }
}
