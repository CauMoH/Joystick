using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Joystick.ViewModels;
using nexus.protocols.ble;

namespace Joystick.Interfaces
{
    public interface IBluetoothManagerViewModel
    {
        bool IsAdapterEnabled { get; }

        bool IsScanning { get; }

        int ScanTimeRemaining { get; }

        string ScanLabel { get; }

        ObservableCollection<BlePeripheralViewModel> DevicesList { get; }

        string Connection { get; }

        ConnectionProgress ConnectionProgress { get; }

        bool IsBusy { get; }

        bool IsConnectedOrConnecting { get; }


        void ConnectToDevice(string deviceAddr);

        Task Disconnect();

        ICommand GoToHomeCommand { get; }

        ICommand SearchCommand { get; }

        ICommand ConnectCommand { get; }

        event EventHandler<EventArgs> PropChanged;
    }
}
