using Spectrum.API.Experimental;
using Spectrum.API.IPC;

namespace Spectrum.API.Interfaces.Systems
{
    public interface IManager
    {
        IHotkeyManager Hotkeys { get; }

        void RegisterIPC(string pluginIdentifier);
        void SendIPC(string pluginRecipient, IPCData data);
    }
}
