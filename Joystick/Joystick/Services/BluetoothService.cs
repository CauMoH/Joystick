using Joystick.Interfaces;
using nexus.protocols.ble;

namespace Joystick.Services
{
    public class BluetoothService : IBluetoothService
    {
        public IBluetoothLowEnergyAdapter Adapter { get; set; }
    }
}
