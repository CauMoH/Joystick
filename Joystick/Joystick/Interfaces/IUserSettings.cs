namespace Joystick.Interfaces
{
    public interface IUserSettings
    {
        string LastBluetoothAddr { get; set; }
        int MaxX { get; set; }
        int MinX { get; set; }
        int MaxY { get; set; }
        int MinY { get; set; }
        int CenterX { get; set; }
        int MinEngineStart { get; set; }
    }
}
