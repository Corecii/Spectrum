using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Spectrum.API;
using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Logging;
using Spectrum.API.Exceptions;
using Spectrum.Manager.Runtime.Metadata;
using System.Collections.Generic;

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
                WriteToConsole = Global.Settings.GetItem<bool>("LogToConsole")
            };
            Log.Info("Plugin loader starting up...");
        }

        public void LoadPlugins()
        {
            Log.Info("Starting load procedure.");
            var pluginDirectories = Directory.GetDirectories(PluginDirectory);
            var pluginLoadDataList = new List<PluginLoadData>();

            foreach (var path in pluginDirectories)
            {
                var manifestPath = Path.Combine(path, "plugin.json");

                if (!File.Exists(manifestPath))
                {
                    Log.Error($"No plugin manifest in {path}. Skipping.");
                    continue;
                }

                PluginManifest manifest;
                try
                {
                    manifest = PluginManifest.FromFile(manifestPath);

                    if (!manifest.IsValid())
                    {
                        Log.Error($"Skipping plugin with invalid manifest. Path: {path}.");
                        continue;
                    }

                    if (manifest.SkipLoad)
                    {
                        Log.Warning($"Manifest declares skip flag, not loading plugin {path}.");
                        continue;
                    }

                    pluginLoadDataList.Add(new PluginLoadData(path, manifest));
                }
                catch (MetadataReadException mre)
                {
                    Log.Error($"Error reading manifest: {mre.Message}\nException: {mre.InnerException}");
                    continue;
                }
            }

            pluginLoadDataList = pluginLoadDataList.OrderByDescending(x => x.Manifest.Priority).ToList();
            foreach (var pluginLoadData in pluginLoadDataList)
            {
                LoadPlugin(pluginLoadData);
            }
            Log.Info($"Plugin load finished, loaded {PluginRegistry.Count} plugins.");
        }

        public void LoadPlugin(PluginLoadData loadData)
        {
            string path = loadData.DirectoryPath;
            PluginManifest manifest = loadData.Manifest;

            Log.Info($"Trying to load the plugin according to the following manifest:\n{manifest}", true);

            if (PluginRegistry.GetByName(manifest.FriendlyName) != null)
            {
                Log.Error($"Plugin conflict detected. A plugin with name {manifest.FriendlyName} already exists.");
                return;
            }

            var targetModulePath = Path.Combine(path, manifest.ModuleFileName);
            if (!File.Exists(targetModulePath))
            {
                Log.Error("The manifest-declared module file does not exist.");
                return;
            }

            if (manifest.Dependencies != null)
            {
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
                    return;
            }

            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFrom(targetModulePath);
            }
            catch (Exception e)
            {
                Log.Error("Couldn't load target module assembly. Exception has been silently logged.");
                Log.ExceptionSilent(e);

                return;
            }

            Type[] exportedTypes;
            try
            {
                exportedTypes = assembly.GetExportedTypes();
            }
            catch (ReflectionTypeLoadException rtle)
            {
                Log.Error($"Couldn't import types of assembly {manifest.ModuleFileName}. The plugin was built for an older Spectrum.API module.");
                Log.ExceptionSilent(rtle);

                return;
            }
            catch (Exception e)
            {
                Log.Error($"An exception occured while importing types from assembly {manifest.ModuleFileName}.");
                Log.ExceptionSilent(e);

                return;
            }

            Type entryClassType = exportedTypes.FirstOrDefault(x => x.Name == manifest.EntryClassName);
            if (entryClassType == null)
            {
                Log.Error($"The assembly {manifest.ModuleFileName} does not define an entry point class {manifest.EntryClassName}.");
                return;
            }


            if (!typeof(IPlugin).IsAssignableFrom(entryClassType))
            {
                Log.Error($"The exported entry class {manifest.EntryClassName} does not implement IPlugin interface.");
                return;
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

                return;
            }
            catch (Exception ex)
            {
                Log.Error("An unexpected exception occured. The plugin has not been activated. Check the exception log file for details.");
                Log.ExceptionSilent(ex);

                return;
            }

            var pluginHost = new PluginHost
            {
                Manifest = manifest,
                RootDirectory = path,
                Enabled = true,
                Instance = instance
            };

            if (typeof(IUpdatable).IsAssignableFrom(entryClassType))
            {
                Log.Info($"Plugin exports IUpdatable interface. This means it will run Update() every frame.");
                pluginHost.UpdatesEveryFrame = true;
            }

            if (typeof(IIPCEnabled).IsAssignableFrom(entryClassType))
            {
                if (!string.IsNullOrEmpty(manifest.IPCIdentifier))
                {
                    Log.Info($"Plugin exports IIPCEnabled interface. This means it can communicate with other plugins.");
                    pluginHost.IsIPCEnabled = true;
                }
                else
                {
                    Log.Error($"Plugin exports IIPCEnabled interface, but does not provide an IPC identifier in manifest. IPC for this plugin will be disabled.");
                    pluginHost.IsIPCEnabled = false;
                }
            }

            PluginRegistry.Add(pluginHost);
            Log.Success($"Plugin assembly {manifest.ModuleFileName} has been loaded.\n");
        }
    }
}
