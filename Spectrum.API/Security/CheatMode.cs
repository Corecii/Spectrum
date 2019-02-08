using Events;
using Events.Scene;
using Spectrum.API.Extensions;
using Spectrum.API.Reflection;

namespace Spectrum.API.Security
{
    public class CheatMode
    {
        private static SubscriberList _subscriberList;
        private static bool _cheatsEnabled;

        static CheatMode()
        {
            _subscriberList = new SubscriberList
            {
                new StaticEvent<LoadFinish.Data>.Subscriber((data) => OnSceneLoadFinished(data))
            };
        }

        public static void Enable()
        {
            _cheatsEnabled = true;
            _subscriberList.Subscribe();
        }

        public static void Disable()
        {
            _cheatsEnabled = false;
            _subscriberList.Unsubscribe();
        }

        private static void OnSceneLoadFinished(LoadFinish.Data data)
        {
            G.Sys.CheatsManager_.SetPrivateMember(new MemberMetadata
            {
                IsProperty = false,
                IsStatic = false,
                MemberName = "anyGameplayCheatsUsedThisLevel_"
            }, _cheatsEnabled);
        }
    }
}
