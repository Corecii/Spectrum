using Spectrum.API.IPC;
using System.Collections.Generic;
using System.Linq;

namespace Spectrum.Manager.Runtime
{
    class PluginRegistry : List<PluginHost>
    {
        private List<PluginInfo> PluginDataCache { get; set; }

        public bool SetPluginState(string name, bool enabled)
        {
            var plugin = GetByName(name);
            if (plugin == null)
                return false;

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

        public List<PluginInfo> QueryLoadedPlugins()
        {
            if (PluginDataCache != null && PluginDataCache.Count == this.Count)
                return PluginDataCache;

            var list = new List<PluginInfo>();

            foreach (var loaded in this)
            {
                list.Add(new PluginInfo(
                    loaded.Manifest.FriendlyName,
                    loaded.Manifest.Author,
                    loaded.Manifest.AuthorContact,
                    loaded.Manifest.IPCIdentifier,
                    loaded.Manifest.Priority ?? 10
                ));
            }

            PluginDataCache = list;
            return list;
        }

        public PluginInfo GetPluginInfoByName(string name)
        {
            if (PluginDataCache == null || PluginDataCache.Count != this.Count)
                QueryLoadedPlugins(); 

            return PluginDataCache.FirstOrDefault(x => x.Name == name);
        }
    }
}
