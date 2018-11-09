using Joystick.Interfaces;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Joystick.Configuration
{
    public class UserSettings : IUserSettings
    {
        private static ISettings AppSettings =>
            CrossSettings.Current;

        public string LastBluetoothAddr
        {
            get => AppSettings.GetValueOrDefault(nameof(LastBluetoothAddr), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(LastBluetoothAddr), value);
        }
    }
}
