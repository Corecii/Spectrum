using Events;
using Events.Network;
using Events.Scene;
using Newtonsoft.Json;
using Spectrum.API.Events.EventArgs;
using Spectrum.API.Extensions;
using Spectrum.API.Interfaces.Systems;
using Spectrum.API.Network;
using Spectrum.API.Network.Events;
using Spectrum.API.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Spectrum.API.Security
{
    internal class CheatSystem : ICheatSystem
    {
        private readonly Dictionary<Assembly, bool> _cheatStates;
        private readonly SubscriberList _subscriberList;

        private Logging.Logger Logger { get; }
        private IManager Manager { get; }

        public bool AnyCheatsEnabled => _cheatStates.Values.Contains(true);

        public event EventHandler<CheatStateInfoEventArgs> CheatStateInfoReceived;
        public event EventHandler<CheatStateFailureEventArgs> CheatStateInfoFailure;

        internal CheatSystem(IManager manager)
        {
            _cheatStates = new Dictionary<Assembly, bool>();

            _subscriberList = new SubscriberList
            {
                new StaticEvent<LoadFinish.Data>.Subscriber(OnSceneLoadFinished),
                new StaticEvent<ClientConnected.Data>.Subscriber(OnClientConnected)
            };
            _subscriberList.Subscribe();

            Logger = new Logging.Logger("network.log");

            Manager = manager;
            manager.EventRouter.RegisterServerToClientCallback(EventNames.CheatStateInfoRequest, OnCheatStateInfoRequested);
            manager.EventRouter.RegisterClientToServerCallback(EventNames.CheatStateInfoResponse, OnCheatStateInfoReceived);
        }

        public void Enable()
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (!_cheatStates.ContainsKey(callingAssembly))
                _cheatStates.Add(callingAssembly, true);
            else
                _cheatStates[callingAssembly] = true;
        }

        public void Disable()
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (_cheatStates.ContainsKey(callingAssembly))
                _cheatStates[callingAssembly] = false;
        }

        private void OnSceneLoadFinished(LoadFinish.Data data)
        {
            G.Sys.CheatsManager_.SetPrivateMember(new MemberMetadata
            {
                IsProperty = false,
                IsStatic = false,
                MemberName = "anyGameplayCheatsUsedThisLevel_"
            }, _cheatStates.Values.Contains(true));
        }

        private void OnClientConnected(ClientConnected.Data data)
        {
            Manager.EventRouter.FireServerToClient(EventNames.CheatStateInfoRequest, string.Empty, new NetworkTarget(data.player_));
        }

        private void OnCheatStateInfoRequested(NetworkPlayer sender, string json)
        {
            Manager.EventRouter.FireClientToServer(EventNames.CheatStateInfoResponse, JsonConvert.SerializeObject(new CheatStateInfo(AnyCheatsEnabled)));
        }

        private void OnCheatStateInfoReceived(NetworkPlayer sender, string json)
        {
            if (sender == UnityEngine.Network.player) return;

            try
            {
                var info = JsonConvert.DeserializeObject<CheatStateInfo>(json);
                CheatStateInfoReceived?.Invoke(this, new CheatStateInfoEventArgs(info, sender));
            }
            catch (Exception e)
            {
                CheatStateInfoFailure?.Invoke(this, new CheatStateFailureEventArgs(sender, e, json));
            }
        }
    }
}
