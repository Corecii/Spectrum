using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Spectrum.API;
using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Logging;
using Spectrum.API.Configuration;
using Spectrum.API.Exceptions;
using Spectrum.Manager.Runtime.Metadata;

namespace Spectrum.Manager.Runtime
{
    class PluginLoader
    {
        private string PluginDirectory { get; }
        private PluginRegistry PluginRegistry { get; }

        private Logger Log { get; }

        public PluginLoader(string pluginDirectory, PluginRegistry pluginRegistry)
        {
            PluginDirectory = pluginDirectory;
            PluginRegistry = pluginRegistry;

            Log = new Logger(Defaults.PluginLoaderLogFileName)
            {
                WriteToConsole = Global.Settings.GetItem<Section>("Output").GetItem<bool>("LogToConsole")
            };
            Log.Info("Plugin loader starting up...");
        }

        public void LoadPlugins()
        {
            Log.Info("Starting load procedure.");
            var pluginDirectories = Directory.GetDirectories(PluginDirectory);

            foreach (var path in pluginDirectories)
            {
                var manifestPath = Path.Combine(path, "plugin.json");
                Console.WriteLine(manifestPath);

                if (!File.Exists(manifestPath))
                {
                    Log.Error($"No plugin manifest in {path}. Skipping.");
                    continue;
                }

                PluginManifest manifest;
                try
                {
                    manifest = PluginManifest.FromFile(manifestPath);
                }
                catch (MetadataReadException mre)
                {
                    Log.Error($"Error reading manifest:  {mre.Message}");
                    continue;
                }

                if (manifest == null)
                {
                    Console.WriteLine("Dicks");
                }

                if (PluginRegistry.GetByName(manifest.FriendlyName) != null)
                {
                    Log.Error($"Plugin conflict detected. A plugin with name {manifest.FriendlyName} already exists.");
                    continue;
                }

                Log.Info($"Trying to load the plugin according to the following manifest:\n{manifest}");

                var targetModulePath = Path.Combine(path, manifest.ModuleFileName);
                if (!File.Exists(targetModulePath))
                {
                    Log.Error("The manifest-declared module file does not exist.");
                    continue;
                }

                bool depsLoaded = true;
                foreach (var depFileName in manifest.Dependencies)
                {
                    var depPath = Path.Combine(Path.Combine(path, Defaults.PrivateDependencyDirectory), depFileName);
                    if (!File.Exists(depPath))
                    {
                        Log.Error($"Couldn't load declared private dependency assembly {depFileName} for plugin {manifest.ModuleFileName}. Verify it exists and try again.");

                        depsLoaded = false;
                        break;
                    }

                    try
                    {
                        Assembly.LoadFrom(depPath);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Couldn't load declared private dependency assembly {depFileName} for plugin {manifest.ModuleFileName}. Is the assembly corrupted?");
                        Log.ExceptionSilent(ex);

                        depsLoaded = false;
                        break;
                    }
                }

                if (!depsLoaded)
                    continue;

                Assembly assembly;
                try
                {
                    assembly = Assembly.LoadFrom(targetModulePath);
                }
                catch (Exception e)
                {
                    Log.Error("Couldn't load target module assembly.");
                    Log.ExceptionSilent(e);

                    continue;
                }

                Type[] exportedTypes;
                try
                {
                    exportedTypes = assembly.GetExportedTypes();
                }
                catch (ReflectionTypeLoadException rtle)
                {
                    Log.Error($"Couldn't import types of assembly {manifest.ModuleFileName}. Possible outdated dependencies?");
                    Log.ExceptionSilent(rtle);

                    continue;
                }
                catch (Exception e)
                {
                    Log.Error($"An exception occured while importing types from assembly {manifest.ModuleFileName}.");
                    Log.ExceptionSilent(e);

                    continue;
                }

                Type entryClassType = exportedTypes.FirstOrDefault(x => x.Name == manifest.EntryClassName);
                if (entryClassType == null)
                {
                    Log.Error($"The assembly {manifest.ModuleFileName} does not define an entry point class {manifest.EntryClassName}.");
                    continue;
                }


                if (!typeof(IPlugin).IsAssignableFrom(entryClassType))
                {
                    Log.Error($"The exported entry class {manifest.EntryClassName} does not implement IPlugin interface.");
                    continue;
                }

                IPlugin instance;
                try
                {
                    instance = (IPlugin)Activator.CreateInstance(entryClassType);
                }
                catch (TypeLoadException tle)
                {
                    Log.Error($"Couldn't create an instance from exported entry class type {manifest.EntryClassName} of assembly {manifest.ModuleFileName}.");
                    Log.ExceptionSilent(tle);

                    continue;
                }
                catch (Exception ex)
                {
                    Log.Error("An unexpected exception occured. The plugin has not been activated. Check the exception log file for details.");
                    Log.ExceptionSilent(ex);

                    continue;
                }

                var pluginHost = new PluginHost()
                {
                    Manifest = manifest,
                    RootDirectory = path,
                    Enabled = true,
                    Instance = instance
                };

                if (manifest.CompatibleAPILevel != SystemVersion.APILevel)
                    Log.Warning($"Plugin assembly {manifest.ModuleFileName} declares that it was compiled for an earlier Spectrum version. Expect unexpected.");

                if (typeof(IUpdatable).IsAssignableFrom(entryClassType))
                {
                    Log.Info($"Plugin exports IUpdatable interface. This means it will run Update() every frame.");
                    pluginHost.UpdatesEveryFrame = true;
                }

                if (typeof(IIPCEnabled).IsAssignableFrom(entryClassType))
                {
                    Log.Info($"Plugin exports IIPCEnabled interface. This means it can respond and/or communicate with other plugins.");
                    pluginHost.IsIPCEnabled = true;
                }

                PluginRegistry.Add(pluginHost);
                Log.Info($"Plugin assembly {manifest.ModuleFileName} has been loaded.");
            }
        }
    }
}
