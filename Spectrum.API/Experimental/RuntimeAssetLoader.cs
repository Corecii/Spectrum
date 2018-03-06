using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Spectrum.API.Experimental
{
    public class RuntimeAssetLoader
    {
        private Logging.Logger Logger { get; }

        public bool CanLoadAssets { get; private set; } = true;
        public Dictionary<string, AssetBundle> LoadedBundles { get; }

        public RuntimeAssetLoader()
        {
            Logger = new Logging.Logger(Path.Combine(Defaults.LogDirectory, Defaults.RuntimeAssetLoaderLogFileName))
            {
                WriteToConsole = true
            };

            LoadedBundles = new Dictionary<string, AssetBundle>();

            if(!Directory.Exists(Defaults.AssetDirectory))
            {
                Logger.Info("Creating missing Assets/ directory.");
                try
                {
                    Directory.CreateDirectory(Defaults.AssetDirectory);
                }
                catch(Exception ex)
                {
                    Logger.Exception(ex);
                    CanLoadAssets = false;
                }
            }

            if(CanLoadAssets)
            {
                Logger.Info("Runtime Asset Loader intialized.");
            }
            else
            {
                Logger.Error("There were errors during initialization. Runtime Asset Loader has been disabled.");
            }
        }

        public AssetBundle LoadFromFile(string fileName)
        {
            if(!CanLoadAssets)
            {
                Logger.Info("RuntimeAssetLoader.LoadFromFile: The custom asset framework is disabled (check the exception log for details).");
                return null;
            }

            try
            {
                if(!LoadedBundles.ContainsKey(fileName))
                {
                    var targetPath = Path.Combine(Defaults.AssetDirectory, fileName);

                    var assetBundle = AssetBundle.LoadFromFile(targetPath);
                    LoadedBundles.Add(fileName, assetBundle);

                    Logger.Info($"Loaded asset bundle {targetPath}");

                    return assetBundle;
                }
                else
                {
                    Logger.Warning("Tried to load the same bundle twice.");
                    return null;
                }
            }
            catch(Exception ex)
            {
                Logger.Exception(ex);
                return null;
            }
        }
    }
}
