using System;
using System.IO;
using System.Reflection;

namespace Spectrum.Bootstrap
{
    public static class Loader
    {
        private static string ManagerDllPath
        {
            get
            {
                var bootstrapLocation = Assembly.GetExecutingAssembly().Location;
                return Path.Combine(Path.GetDirectoryName(bootstrapLocation), "..#Spectrum#Spectrum.Manager.dll".Replace('#', Path.DirectorySeparatorChar));
            }
        }

        public static void StartManager()
        {
            foreach (var arg in Environment.GetCommandLineArgs())
            {
                if (arg == StartupArguments.AllocateConsole)
                {
                    if (IsMonoPlatform() && IsUnix())
                    {
                        ConsoleAllocator.CreateUnix();
                        Log.Info("Running on non-Windows platform. Skipping AllocConsole()...");
                    }
                    else
                    {
                        ConsoleAllocator.CreateWin32();
                    }

                    var version = Assembly.GetAssembly(typeof(Loader)).GetName().Version;

                    Console.WriteLine($"Spectrum Extension System for Distance. Version {version.Major}.{version.Minor}.{version.Build}.{version.Revision}.");
                    Console.WriteLine("Verbose mode enabled. Remove '-console' command line switch to disable.");
                    Console.WriteLine("--------------------------------------------");
                }
            }

            if (!File.Exists(ManagerDllPath))
            {
                Log.Error($"Could not find Spectrum Plugin Manager assembly at {ManagerDllPath}. Terminating.");
                return;
            }

            Log.Info($"Located Spectrum Plugin Manager assembly at {ManagerDllPath}");

            try
            {
                var managerAssembly = Assembly.LoadFrom(ManagerDllPath);
                var managerType = managerAssembly.GetType("Spectrum.Manager.Manager", false);

                if (managerType == null)
                {
                    Log.Error("Could not find the correct type 'Spectrum.Manager.Manager' in the loaded Spectrum Plugin Manager assembly. Terminating.");
                    return;
                }

                Log.Info("Bootstrap process finished, passing control to Spectrum Plugin Manager.");
                Updater.ManagerObject = Activator.CreateInstance(managerType);
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected initalization failure occured. Exception follows.");
                Log.Exception(ex);
            }
        }

        private static bool IsMonoPlatform()
        {
            var platformID = (int)Environment.OSVersion.Platform;
            return platformID == 4 || platformID == 6 || platformID == 128;
        }

        private static bool IsUnix()
        {
            var platformID = Environment.OSVersion.Platform;
            switch (platformID)
            {
                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    return true;
                default:
                    return false;
            }
        }
    }
}
