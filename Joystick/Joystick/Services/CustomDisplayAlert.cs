using Joystick.Interfaces;
using Xamarin.Forms;

namespace Joystick.Services
{
    public class CustomDisplayAlert : ICustomDisplayAlert
    {
        public void DisplayAlert(string title, string message)
        {
            string[] values = { title, message };
            MessagingCenter.Send(this, "DisplayAlert", values);
        }
    }
}
