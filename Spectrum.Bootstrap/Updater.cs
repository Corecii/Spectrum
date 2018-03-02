﻿using System;

namespace Spectrum.Bootstrap
{
    public static class Updater
    {
        private const int MaxUpdateRetryCount = 10;

        private static int _updateRetryCount = 0;

        public static object ManagerObject;

        public static void UpdateManager()
        {
            try
            {
                ManagerObject?.GetType().GetMethod("UpdateExtensions").Invoke(ManagerObject, null);
            }
            catch (Exception ex)
            {
                if (_updateRetryCount <= 10)
                {
                    Console.WriteLine($"[STAGE 1] Spectrum: Can't update extensions. Read below:\n{ex}");
                    _updateRetryCount++;
                }
            }
        }
    }
}
