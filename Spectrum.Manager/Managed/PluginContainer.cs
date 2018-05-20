using System.Collections.Generic;

namespace Spectrum.Manager.Managed
{
    class PluginContainer : List<PluginHost>
    {
        public bool SetPluginState(string name, bool enabled)
        {
            var plugin = GetPluginByName(name);
            if (plugin == null)
            {
                return false;
            }
            plugin.Enabled = enabled;
            return true;
        }

        public PluginHost GetPluginByName(string name)
        {
            foreach (var pluginInfo in this)
            {
                if (pluginInfo.Manifest.FriendlyName == name)
                    return pluginInfo;
            }
            return null;
        }
    }
}
