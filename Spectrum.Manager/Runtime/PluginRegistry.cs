using System.Collections.Generic;
using System.Linq;

namespace Spectrum.Manager.Runtime
{
    class PluginRegistry : List<PluginHost>
    {
        public bool SetPluginState(string name, bool enabled)
        {
            var plugin = GetByName(name);
            if (plugin == null)
            {
                return false;
            }
            plugin.Enabled = enabled;
            return true;
        }

        public PluginHost GetByName(string name)
        {
            return this.FirstOrDefault(x => x.Manifest.FriendlyName == name);
        }

        public PluginHost GetByIPCIdentifier(string ipcIdentifier)
        {
            return this.FirstOrDefault(x => x.Manifest.IPCIdentifier == ipcIdentifier);
        }
    }
}
