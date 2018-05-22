using System;
using System.IO;
using Spectrum.API;
using Spectrum.API.Configuration;
using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Interfaces.Systems;
using Spectrum.API.IPC;
using Spectrum.API.Logging;
using Spectrum.Interop.Game;
using Spectrum.Manager.Input;
using Spectrum.Manager.Runtime;

namespace Spectrum.Manager
{
    public class Manager : IManager
    {
        private PluginRegistry PluginRegistry { get; set; }
        private PluginLoader PluginLoader { get; set; }
        private Logger Log;

        public IHotkeyManager Hotkeys { get; set; }

        public bool IsEnabled { get; set; }
        public bool CanLoadPlugins => Directory.Exists(Defaults.ManagerPluginDirectory);

        public Manager()
        {
            Log = new Logger("Manager.log");
            Log.Info("Manager started");

            IsEnabled = true;

            CheckPaths();
            InitializeSettings();

            if (!Global.Settings.GetItem<Section>("Execution").GetItem<bool>("Enabled"))
            {
                Log.Error("Manager: Spectrum is disabled. Set 'Enabled' entry to 'true' in settings to restore extension framework functionality.");
                IsEnabled = false;
                return;
            }

            Hotkeys = new HotkeyManager();
            Scene.Loaded += (sender, args) =>
            {
                Game.ShowWatermark = Global.Settings.GetItem<Section>("Output").GetItem<bool>("ShowWatermark");

                if (Game.ShowWatermark)
                {
                    Game.WatermarkText = SystemVersion.VersionString;
                }
            };

            if (Global.Settings.GetItem<Section>("Execution").GetItem<bool>("LoadPlugins"))
            {
                LoadExtensions();
                StartExtensions();
            }
        }

        public void SendIPC(string ipcIdentifierTo, IPCData data)
        {
            var pluginHost = PluginRegistry.GetByIPCIdentifier(ipcIdentifierTo);
            if (pluginHost != null)
            {
                if (!pluginHost.IsIPCEnabled)
                {
                    Log.Error($"Plugin with IPC ID {data.SourceIdentifier} tried to send IPCData to {ipcIdentifierTo}, but target is not IPC enabled.");
                    return;
                }

                (pluginHost.Instance as IIPCEnabled).HandleIPCData(data);
            }
        }

        public bool IsAvailableForIPC(string ipcIdentifier)
        {
            return PluginRegistry.GetByIPCIdentifier(ipcIdentifier) != null;
        }

        public void CheckPaths()
        {
            if (!Directory.Exists(Defaults.ManagerSettingsDirectory))
            {
                Log.Info("Settings directory does not exist. Creating...");
                Directory.CreateDirectory(Defaults.ManagerSettingsDirectory);
            }

            if (!Directory.Exists(Defaults.ManagerLogDirectory))
            {
                Log.Info("Log directory does not exist. Creating...");
                Directory.CreateDirectory(Defaults.ManagerLogDirectory);
            }

            if (!Directory.Exists(Defaults.ManagerPluginDirectory))
            {
                Log.Info("Plugin directory does not exist. Creating...");
                Directory.CreateDirectory(Defaults.ManagerPluginDirectory);
            }
        }

        public void UpdateExtensions()
        {
            if (!IsEnabled)
                return;

            ((HotkeyManager)Hotkeys).Update();

            if (PluginRegistry != null)
            {
                foreach (var pluginInfo in PluginRegistry)
                {
                    if (pluginInfo.Enabled && pluginInfo.UpdatesEveryFrame)
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

                if (!Global.Settings.ContainsKey<Section>("Output"))
                {
                    Global.Settings["Output"] = new Section
                    {
                        ["LogToConsole"] = true,
                        ["ShowWatermark"] = true
                    };
                }
                else
                {
                    if (!Global.Settings.GetItem<Section>("Output").ContainsKey("LogToConsole"))
                        Global.Settings.GetItem<Section>("Output")["LogToConsole"] = true;

                    if (!Global.Settings.GetItem<Section>("Output").ContainsKey("ShowWatermark"))
                        Global.Settings.GetItem<Section>("Output")["ShowWatermark"] = true;
                }

                if (!Global.Settings.ContainsKey<Section>("Execution"))
                {
                    Global.Settings["Execution"] = new Section
                    {
                        ["FirstRun"] = false,
                        ["LoadPlugins"] = true,
                        ["Enabled"] = true
                    };
                }
                else
                {
                    if (!Global.Settings.GetItem<Section>("Execution").ContainsKey("FirstRun"))
                        Global.Settings.GetItem<Section>("Execution")["FirstRun"] = false;

                    if (!Global.Settings.GetItem<Section>("Execution").ContainsKey("LoadPlugins"))
                        Global.Settings.GetItem<Section>("Execution")["LoadPlugins"] = true;

                    if (!Global.Settings.GetItem<Section>("Execution").ContainsKey("Enabled"))
                        Global.Settings.GetItem<Section>("Execution")["Enabled"] = true;
                }

                if (Global.Settings.Dirty)
                    Global.Settings.Save();
            }
            catch (Exception ex)
            {
                Log.Error($"Couldn't load settings. Defaults loaded. Exception has been caught, see the log for details.");
                Log.ExceptionSilent(ex);
            }
        }

        private void LoadExtensions()
        {
            PluginRegistry = new PluginRegistry();
            PluginLoader = new PluginLoader(Defaults.ManagerPluginDirectory, PluginRegistry);
            PluginLoader.LoadPlugins();
        }

        private void StartExtensions()
        {
            Log.Info("Initializing extensions");

            foreach (var pluginInfo in PluginRegistry)
            {
                try
                {
                    pluginInfo.Instance.Initialize(this);
                    Log.Info($"Plugin {pluginInfo.Manifest.FriendlyName} initialized");
                }
                catch (Exception ex)
                {
                    Log.Error($"Plugin {pluginInfo.Manifest.FriendlyName} failed to initialize. Exception below.\n{ex}");
                }
            }

            Log.Info("Extensions initialized");
        }
    }
}
