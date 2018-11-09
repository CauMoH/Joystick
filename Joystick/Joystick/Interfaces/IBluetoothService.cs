using nexus.protocols.ble;

namespace Joystick.Interfaces
{
    public interface IBluetoothService
    {
        IBluetoothLowEnergyAdapter Adapter { get; set; }
    }
}
