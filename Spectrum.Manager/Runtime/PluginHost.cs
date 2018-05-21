using Spectrum.API.Interfaces.Plugins;
using Spectrum.Manager.Runtime.Metadata;

namespace Spectrum.Manager.Runtime
{
    class PluginHost
    {
        public PluginManifest Manifest { get; internal set; }
        public IPlugin Instance { get; internal set; }

        public string RootDirectory { get; internal set; }
        public bool Enabled { get; set; }
        public bool UpdatesEveryFrame { get; internal set; }
        public bool IsIPCEnabled { get; internal set; }

        public PluginHost() { }

        public PluginHost(PluginManifest manifest, IPlugin instance, string rootDirectory, bool enabledByDefault, bool updatesEveryFrame, bool isIpcEnabled)
        {
            Manifest = manifest;
            Instance = instance;

            RootDirectory = rootDirectory;
            Enabled = enabledByDefault;
            UpdatesEveryFrame = updatesEveryFrame;
            IsIPCEnabled = isIpcEnabled;
        }
    }
}
