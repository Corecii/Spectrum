using System.IO;
using System.Reflection;

namespace Spectrum.API
{
    public class Defaults
    {
        private static string BasePath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string PrivateDependencyDirectory => "Dependencies";
        public static string PrivateDataDirectory => "Data";
        public static string PrivateLogDirectory => "Logs";
        public static string PrivateSettingsDirectory => "Settings";
        public static string PrivateAssetsDirectory => "Assets";

        public static string ManagerPluginDirectory => Path.Combine(BasePath, "Plugins");
        public static string ManagerLogDirectory => Path.Combine(BasePath, "Logs");
        public static string ManagerSettingsDirectory => Path.Combine(BasePath, "Settings");

        public const string HotkeyManagerLogFileName = "HotkeyManager.log";
        public const string PluginLoaderLogFileName = "PluginLoader.log";
        public const string DependencyResolverLogFileName = "DependencyResolver.log";
        public const string FileSystemLogFileName = "FileSystem.log";
        public const string RuntimeAssetLoaderLogFileName = "RuntimeAssetLoader.log";
    }
}
