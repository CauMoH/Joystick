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

        public short MaxX
        {
            get => (short)AppSettings.GetValueOrDefault(nameof(MaxX), 30);
            set => AppSettings.AddOrUpdateValue(nameof(MaxX), value);
        }

        public short MinX
        {
            get => (short)AppSettings.GetValueOrDefault(nameof(MinX), 30);
            set => AppSettings.AddOrUpdateValue(nameof(MinX), value);
        }

        public short MaxY
        {
            get => (short)AppSettings.GetValueOrDefault(nameof(MaxY), 200);
            set => AppSettings.AddOrUpdateValue(nameof(MaxY), value);
        }

        public short MinY
        {
            get => (short)AppSettings.GetValueOrDefault(nameof(MinY), 200);
            set => AppSettings.AddOrUpdateValue(nameof(MinY), value);
        }

        public short CenterX
        {
            get => (short)AppSettings.GetValueOrDefault(nameof(CenterX), 0);
            set => AppSettings.AddOrUpdateValue(nameof(CenterX), value);
        }

        public short MinEngineStart
        {
            get => (short)AppSettings.GetValueOrDefault(nameof(MinEngineStart), 40);
            set => AppSettings.AddOrUpdateValue(nameof(MinEngineStart), value);
        }

        public bool IsEnableLights
        {
            get => AppSettings.GetValueOrDefault(nameof(IsEnableLights), false);
            set => AppSettings.AddOrUpdateValue(nameof(IsEnableLights), value);
        }

        public short UpdateInMs
        {
            get => (short)AppSettings.GetValueOrDefault(nameof(UpdateInMs), 200);
            set => AppSettings.AddOrUpdateValue(nameof(UpdateInMs), value);
        }
    }
}
