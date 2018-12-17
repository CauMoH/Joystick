using System;
using System.Windows.Input;

namespace Joystick.Interfaces
{
    public interface ISettingsViewModel
    {
        short MaxX { get; }
        short MinX { get; }
        short MaxY { get; }
        short MinY { get; }
        short CenterX { get; }
        short MinEngineStart { get; }
        short UpdateTime { get; }

        ICommand SaveCommand { get; }

        event EventHandler<EventArgs> PropChanged;
    }
}
