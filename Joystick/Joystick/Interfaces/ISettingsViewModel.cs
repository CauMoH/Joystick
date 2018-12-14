using System;

namespace Joystick.Interfaces
{
    public interface ISettingsViewModel
    {
        int MaxX { get; }
        int MinX { get; }
        int MaxY { get; }
        int MinY { get; }
        int CenterX { get; }
        int MinEngineStart { get; }

        event EventHandler<EventArgs> PropChanged;
    }
}
