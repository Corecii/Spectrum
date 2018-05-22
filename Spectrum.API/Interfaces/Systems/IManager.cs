using Spectrum.API.IPC;

namespace Spectrum.API.Interfaces.Systems
{
    public interface IManager
    {
        IHotkeyManager Hotkeys { get; }

        void SendIPC(string ipcIdentifier, IPCData data);
        bool IsAvailableForIPC(string ipcIdentifier);
    }
}
