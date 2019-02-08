using Events;
using Events.Scene;

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
        }

        public static void Disable()
        {
            _cheatsEnabled = false;
        }

        private static void OnSceneLoadFinished(LoadFinish.Data data)
        {

        }
    }
}
