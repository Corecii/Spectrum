namespace Spectrum.Manager.Runtime.Metadata
{
    public class PluginLoadData
    {
        public string DirectoryPath { get; }
        public PluginManifest Manifest { get; }

        public PluginLoadData(string directoryPath, PluginManifest manifest)
        {
            DirectoryPath = directoryPath;
            Manifest = manifest;
        }
    }
}
