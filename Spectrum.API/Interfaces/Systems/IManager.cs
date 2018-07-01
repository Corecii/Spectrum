using Spectrum.API.IPC;
using System.Collections.Generic;

namespace Spectrum.API.Interfaces.Systems
{
    public interface IManager
    {
        IHotkeyManager Hotkeys { get; }

        void SendIPC(string ipcIdentifier, IPCData data);
        bool IsAvailableForIPC(string ipcIdentifier);

        bool SetConfig<T>(string key, T value);
        T GetConfig<T>(string key);

        List<PluginInfo> QueryLoadedPlugins();
    }
}
