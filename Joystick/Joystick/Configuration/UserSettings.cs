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

        public int MaxX
        {
            get => AppSettings.GetValueOrDefault(nameof(MaxX), 30);
            set => AppSettings.AddOrUpdateValue(nameof(MaxX), value);
        }

        public int MinX
        {
            get => AppSettings.GetValueOrDefault(nameof(MinX), 30);
            set => AppSettings.AddOrUpdateValue(nameof(MinX), value);
        }

        public int MaxY
        {
            get => AppSettings.GetValueOrDefault(nameof(MaxY), 200);
            set => AppSettings.AddOrUpdateValue(nameof(MaxY), value);
        }

        public int MinY
        {
            get => AppSettings.GetValueOrDefault(nameof(MinY), 200);
            set => AppSettings.AddOrUpdateValue(nameof(MinY), value);
        }

        public int CenterX
        {
            get => AppSettings.GetValueOrDefault(nameof(CenterX), 0);
            set => AppSettings.AddOrUpdateValue(nameof(CenterX), value);
        }

        public int MinEngineStart
        {
            get => AppSettings.GetValueOrDefault(nameof(MinEngineStart), 40);
            set => AppSettings.AddOrUpdateValue(nameof(MinEngineStart), value);
        }

    }
}
