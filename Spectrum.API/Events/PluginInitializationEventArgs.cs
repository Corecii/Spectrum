using Spectrum.API.IPC;
using System;

namespace Spectrum.API.Events
{
    public class PluginInitializationEventArgs : EventArgs
    {
        public PluginInfo Plugin { get; }
        public bool Last { get; }

        public PluginInitializationEventArgs(PluginInfo plugin, bool last)
        {
            Plugin = plugin;
            Last = last;
        }
    }
}
