using System;
using System.IO;
using Spectrum.API;
using Spectrum.API.Configuration;
using Spectrum.API.Experimental;
using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Interfaces.Systems;
using Spectrum.API.IPC;
using Spectrum.API.Logging;
using Spectrum.Interop.Game;
using Spectrum.Manager.Input;
using Spectrum.Manager.Managed;

namespace Spectrum.Manager
{
    public class Manager : IManager
    {
        private PluginContainer ManagedPluginContainer { get; set; }
        private PluginLoader ManagedPluginLoader { get; set; }
        private ExternalDependencyResolver ManagedDependencyResolver { get; set; }
        private Logger Log;

        public RuntimeAssetLoader Assets { get; private set; }
        public IHotkeyManager Hotkeys { get; set; }

        public bool IsEnabled { get; set; }
        public bool CanLoadPlugins => Directory.Exists(Defaults.PluginDirectory);

        public Manager()
        {
            Log = new Logger("Manager.log");
            Log.Info("Manager started");

            IsEnabled = true;

            CheckPaths();
            InitializeSettings();

            if(!Global.Settings.GetItem<Section>("Execution").GetItem<bool>("Enabled"))
            {
                Log.Error("Manager: Spectrum is disabled. Set 'Enabled' entry to 'true' in settings to restore extension framework functionality.");
                IsEnabled = false;
                return;
            }
            ManagedDependencyResolver = new ExternalDependencyResolver();
            Assets = new RuntimeAssetLoader();
            Hotkeys = new HotkeyManager();

            Scene.Loaded += (sender, args) =>
            {
                Game.ShowWatermark = Global.Settings.GetItem<Section>("Output").GetItem<bool>("ShowWatermark");

                if(Game.ShowWatermark)
                {
                    Game.WatermarkText = SystemVersion.VersionString;
                }
            };

            if(Global.Settings.GetItem<Section>("Execution").GetItem<bool>("LoadPlugins"))
            {
                LoadExtensions();
                StartExtensions();
            }
        }

        public void RegisterIPC(string ipcIdentifier)
        {

        }

        public void SendIPC(string ipcIdentifierTo, IPCData data)
        {

        }

        public void CheckPaths()
        {
            if(!Directory.Exists(Defaults.SettingsDirectory))
            {
                Log.Info("Settings directory does not exist. Creating...");
                Directory.CreateDirectory(Defaults.SettingsDirectory);
            }

            if(!Directory.Exists(Defaults.LogDirectory))
            {
                Log.Info("Log directory does not exist. Creating...");
                Directory.CreateDirectory(Defaults.LogDirectory);
            }

            if(!Directory.Exists(Defaults.PluginDirectory))
            {
                Log.Info("Plugin directory does not exist. Creating...");
                Directory.CreateDirectory(Defaults.PluginDirectory);
            }
        }

        public void UpdateExtensions()
        {
            if(!IsEnabled)
                return;

            ((HotkeyManager)Hotkeys).Update();

            if(ManagedPluginContainer != null)
            {
                foreach(var pluginInfo in ManagedPluginContainer)
                {
                    if(pluginInfo.Enabled && pluginInfo.UpdatesEveryFrame)
                    {
                        var plugin = pluginInfo.Instance as IUpdatable;
                        plugin?.Update();
                    }
                }
            }
        }

        private void InitializeSettings()
        {
            try
            {
                Global.Settings = new Settings("ManagerSettings");

                if(!Global.Settings.ContainsKey<Section>("Output"))
                {
                    RecreateSettings();
                }
                else
                {
                    if(!Global.Settings.GetItem<Section>("Output").ContainsKey("LogToConsole"))
                    {
                        Global.Settings.GetItem<Section>("Output")["LogToConsole"] = true;
                    }

                    if(!Global.Settings.GetItem<Section>("Output").ContainsKey("ShowWatermark"))
                    {
                        Global.Settings.GetItem<Section>("Output")["ShowWatermark"] = true;
                    }
                }

                if(!Global.Settings.ContainsKey<Section>("Execution"))
                {
                    RecreateSettings();
                }
                else
                {
                    if(!Global.Settings.GetItem<Section>("Execution").ContainsKey("FirstRun"))
                    {
                        Global.Settings.GetItem<Section>("Execution")["FirstRun"] = false;
                    }

                    if(!Global.Settings.GetItem<Section>("Execution").ContainsKey("LoadPlugins"))
                    {
                        Global.Settings.GetItem<Section>("Execution")["LoadPlugins"] = true;
                    }

                    if(!Global.Settings.GetItem<Section>("Execution").ContainsKey("Enabled"))
                    {
                        Global.Settings.GetItem<Section>("Execution")["Enabled"] = true;
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Error($"MANAGER: Couldn't load settings. Defaults loaded. Exception below.\n{ex}");
            }
        }

        private void RecreateSettings()
        {
            Global.Settings.Clear();

            Section sec = new Section
            {
                ["LogToConsole"] = true,
                ["ShowWatermark"] = true
            };
            Global.Settings["Output"] = sec;

            sec = new Section
            {
                ["FirstRun"] = false,
                ["LoadPlugins"] = true,
                ["Enabled"] = true
            };
            Global.Settings["Execution"] = sec;

            Global.Settings.Save();
        }

        private void LoadExtensions()
        {
            ManagedPluginContainer = new PluginContainer();
            ManagedPluginLoader = new PluginLoader(Defaults.PluginDirectory, ManagedPluginContainer);
            ManagedPluginLoader.LoadPlugins();
        }

        private void StartExtensions()
        {
            Log.Info("Initializing extensions");

            foreach(var pluginInfo in ManagedPluginContainer)
            {
                try
                {
                    pluginInfo.Instance.Initialize(this);

                    if(pluginInfo.Instance is IRequiresAssetLoad needsAsset)
                    {
                        Log.Info($"Loading assets for plugin {pluginInfo.Manifest.FriendlyName}");
                        needsAsset.LoadAssets(Assets);
                    }

                    Log.Info($"Plugin {pluginInfo.Manifest.FriendlyName} initialized");
                }
                catch(Exception ex)
                {
                    Log.Error($"Plugin {pluginInfo.Manifest.FriendlyName} failed to initialize. Exception below.\n{ex}");
                }
            }

            Log.Info("Extensions initialized");
        }
    }
}
