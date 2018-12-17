namespace Joystick.Interfaces
{
    public interface IUserSettings
    {
        string LastBluetoothAddr { get; set; }
        short MaxX { get; set; }
        short MinX { get; set; }
        short MaxY { get; set; }
        short MinY { get; set; }
        short CenterX { get; set; }
        short MinEngineStart { get; set; }
        bool IsEnableLights { get; set; }
        short UpdateInMs { get; set; }
    }
}
