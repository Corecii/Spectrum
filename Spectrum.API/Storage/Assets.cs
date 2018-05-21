using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Spectrum.API.Experimental
{
    public class Assets
    {
        public AssetBundle Bundle { get; private set; }

        private string RootDirectory { get; }
        private string FileName { get; }
        private string FilePath => Path.Combine(Path.Combine(RootDirectory, Defaults.PrivateAssetsDirectory), FileName);

        private static Logging.Logger Log { get; }
        
        static Assets()
        {
            Log = new Logging.Logger(Defaults.RuntimeAssetLoaderLogFileName);
        }

        public Assets(string fileName)
        {
            RootDirectory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            FileName = fileName;

            if(!File.Exists(FilePath))
            {
                Log.Error($"Couldn't find requested asset bundle at {FilePath}");
                return;
            }

            Bundle = Load();
        }

        private AssetBundle Load()
        {
            try
            {
                var assetBundle = AssetBundle.LoadFromFile(FilePath);
                Log.Info($"Loaded asset bundle {FilePath}");

                return assetBundle;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return null;
            }
        }
    }
}
