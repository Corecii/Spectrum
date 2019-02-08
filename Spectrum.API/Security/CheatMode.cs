using Events;
using Events.Scene;
using Spectrum.API.Extensions;
using Spectrum.API.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Spectrum.API.Security
{
    public class CheatMode
    {
        private static readonly Dictionary<Assembly, bool> _cheatStates;

        static CheatMode()
        {
            _cheatStates = new Dictionary<Assembly, bool>();
            LoadFinish.Subscribe((data) => OnSceneLoadFinished(data));
        }

        public static void Enable()
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (!_cheatStates.ContainsKey(callingAssembly))
                _cheatStates.Add(callingAssembly, true);
            else
                _cheatStates[callingAssembly] = true;
        }

        public static void Disable()
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (_cheatStates.ContainsKey(callingAssembly))
                _cheatStates[callingAssembly] = false;
        }

        private static void OnSceneLoadFinished(LoadFinish.Data data)
        {
            G.Sys.CheatsManager_.SetPrivateMember(new MemberMetadata
            {
                IsProperty = false,
                IsStatic = false,
                MemberName = "anyGameplayCheatsUsedThisLevel_"
            }, _cheatStates.Values.Contains(true));
        }
    }
}
