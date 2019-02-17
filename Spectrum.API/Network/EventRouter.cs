using Events;
using Spectrum.API.Interfaces.Systems;
using Spectrum.API.Network.Events;
using System;
using System.Collections.Generic;
using Spectrum.API.Extensions;
using Spectrum.API.Reflection;
using UnityEngine;

namespace Spectrum.API.Network
{
    internal class EventRouter : IEventRouter
    {
        internal Dictionary<string, List<Action<NetworkPlayer, string>>> ClientToServerCallbacks { get; }
        internal Dictionary<string, List<Action<NetworkPlayer, string>>> ServerToClientCallbacks { get; }
        internal Dictionary<string, List<Action<NetworkPlayer, string>>> BroadcastAllCallbacks { get; }

        public NetworkTarget ClientToServerNetworkTarget;
        public NetworkTarget ServerToClientNetworkTarget;
        public NetworkTarget BroadcastAllNetworkTarget;

        internal int ClientToServerIndex;
        internal int ServerToClientIndex;
        internal int BroadcastAllIndex;

        internal bool HasInit = false;

        internal EventRouter()
        {
            ClientToServerCallbacks = new Dictionary<string, List<Action<NetworkPlayer, string>>>();
            ServerToClientCallbacks = new Dictionary<string, List<Action<NetworkPlayer, string>>>();
            BroadcastAllCallbacks = new Dictionary<string, List<Action<NetworkPlayer, string>>>();

            ClientToServerNetworkTarget = new NetworkTarget(RPCMode.Server);
            ServerToClientNetworkTarget = new NetworkTarget(RPCMode.Others);
            BroadcastAllNetworkTarget = new NetworkTarget(RPCMode.Others);

            ClientToServer.Subscribe(ClientToServerEventReceived);
            ServerToClient.Subscribe(ServerToClientEventReceived);
            BroadcastAll.Subscribe(BroadcastAllEventReceived);
        }

        public void Init()
        {
            if (HasInit)
                return;
            HasInit = true;
            ClientToServerIndex = NetworkOverrides.RegisterClientToServerEvent<ClientToServer.Data>();
            ServerToClientIndex = NetworkOverrides.RegisterServerToClientEvent<ServerToClient.Data>();
            BroadcastAllIndex = NetworkOverrides.RegisterBroadcastAllEvent<BroadcastAll.Data>();
        }

        public void RegisterClientToServerCallback(string eventName, Action<NetworkPlayer, string> callback)
        {
            if (!ClientToServerCallbacks.ContainsKey(eventName))
                ClientToServerCallbacks.Add(eventName, new List<Action<NetworkPlayer, string>>());

            ClientToServerCallbacks[eventName].Add(callback);
        }
        public void RegisterClientToServerCallback(string eventName, Action<BitStreamAbstract> callback)
        {
            NetworkProtected.RegisterClientToServerCallback(eventName, callback);
        }

        public void RegisterServerToClientCallback(string eventName, Action<NetworkPlayer, string> callback)
        {
            if (!ServerToClientCallbacks.ContainsKey(eventName))
                ServerToClientCallbacks.Add(eventName, new List<Action<NetworkPlayer, string>>());

            ServerToClientCallbacks[eventName].Add(callback);
        }
        public void RegisterServerToClientCallback(string eventName, Action<BitStreamAbstract> callback)
        {
            NetworkProtected.RegisterServerToClientCallback(eventName, callback);
        }

        public void RegisterBroadcastAllCallback(string eventName, Action<NetworkPlayer, string> callback)
        {
            if (!BroadcastAllCallbacks.ContainsKey(eventName))
                BroadcastAllCallbacks.Add(eventName, new List<Action<NetworkPlayer, string>>());

            BroadcastAllCallbacks[eventName].Add(callback);
        }
        public void RegisterBroadcastAllCallback(string eventName, Action<BitStreamAbstract> callback)
        {
            NetworkProtected.RegisterBroadcastAllCallback(eventName, callback);
        }

        // We can't use `NetworkTarget target = ...` because it won't be a compile-time constant
        // So instead we'll just have two methods

        //FireServerToClient
        public void FireServerToClient(string name, string data, NetworkTarget target, NetworkGroup group = NetworkGroup.GlobalGroup)
        {
            var eventData = new ServerToClient.Data(name, data, group);
            if (target.SendToSelf_)
            {
                ServerToClientEventReceived(eventData);
            }
            NetworkProtected.ServerToClientTransceiver.CallPrivateMethod(new MethodMetadata()
            {
                IsStatic = false,
                Name = "BroadcastEventRPC"
            }, group, "ReceiveServerToClientEvent", target, ServerToClientIndex, eventData);
        }
        public void FireServerToClient(string name, string data, NetworkGroup group = NetworkGroup.GlobalGroup)
        {
            FireServerToClient(name, data, ServerToClientNetworkTarget, group);
        }

        public void FireServerToClient(string name, Action<BitStreamAbstract> serializeData, NetworkTarget target, NetworkGroup group = NetworkGroup.GlobalGroup)
        {
            var index = NetworkProtected.ServerToClientIndex;
            var streamWriter = new BitStreamWriter();
            streamWriter.Serialize(ref index);
            serializeData(streamWriter);
            if (target.SendToSelf_)
            {
                NetworkProtected.ServerToClientEventReceived(name, new BitStreamReader(streamWriter.ToBytes()));
            }
            NetworkProtected.ServerToClientTransceiver.CallPrivateMethod(new MethodMetadata()
            {
                IsStatic = false,
                Name = "SendRPC",
                Types = new Type[] {typeof(NetworkGroup), typeof(string), typeof(NetworkTarget), typeof(object[])}
            }, group, "ReceiveServerToClientEvent", target, streamWriter.ToBytes());
        }
        public void FireServerToClient(string name, Action<BitStreamAbstract> serializeData, NetworkGroup group = NetworkGroup.GlobalGroup)
        {
            FireServerToClient(name, serializeData, ClientToServerNetworkTarget, group);
        }

        //FireBroadcastAll
        public void FireBroadcastAll(string name, string data, NetworkTarget target, NetworkGroup group = NetworkGroup.GlobalGroup)
        {
            var eventData = new BroadcastAll.Data(name, data, group);
            if (target.SendToSelf_)
            {
                BroadcastAllEventReceived(eventData);
            }
            NetworkProtected.ClientToClientTransceiver.CallPrivateMethod(new MethodMetadata()
            {
                IsStatic = false,
                Name = "BroadcastEventRPC"
            }, group, "ReceiveBroadcastAllEvent", target, BroadcastAllIndex, eventData);
        }
        public void FireBroadcastAll(string name, string data, NetworkGroup group = NetworkGroup.GlobalGroup)
        {
            FireBroadcastAll(name, data, BroadcastAllNetworkTarget, group);
        }

        public void FireBroadcastAll(string name, Action<BitStreamAbstract> serializeData, NetworkTarget target, NetworkGroup group = NetworkGroup.GlobalGroup)
        {
            var index = NetworkProtected.BroadcastAllIndex;
            var streamWriter = new BitStreamWriter();
            streamWriter.Serialize(ref index);
            serializeData(streamWriter);
            if (target.SendToSelf_)
            {
                NetworkProtected.BroadcastAllEventReceived(name, new BitStreamReader(streamWriter.ToBytes()));
            }
            NetworkProtected.ClientToClientTransceiver.CallPrivateMethod(new MethodMetadata()
            {
                IsStatic = false,
                Name = "SendRPC",
                Types = new Type[] { typeof(NetworkGroup), typeof(string), typeof(NetworkTarget), typeof(object[]) }
            }, group, "ReceiveBroadcastAllEvent", target, streamWriter.ToBytes());
        }
        public void FireBroadcastAll(string name, Action<BitStreamAbstract> serializeData, NetworkGroup group = NetworkGroup.GlobalGroup)
        {
            FireBroadcastAll(name, serializeData, ClientToServerNetworkTarget, group);
        }

        //FireClientToServer
        public void FireClientToServer(string name, string data, NetworkTarget target, NetworkGroup group = NetworkGroup.GlobalGroup)
        {
            var eventData = new ClientToServer.Data(name, data, group);
            if (target.SendToSelf_)
            {
                ClientToServerEventReceived(eventData);
            }
            NetworkProtected.ClientToServerTransceiver.CallPrivateMethod(new MethodMetadata()
            {
                IsStatic = false,
                Name = "BroadcastEventRPC"
            }, group, "ReceiveClientToServerEvent", target, ClientToServerIndex, eventData);
        }
        public void FireClientToServer(string name, string data, NetworkGroup group = NetworkGroup.GlobalGroup)
        {
            FireClientToServer(name, data, ClientToServerNetworkTarget, group);
        }

        public void FireClientToServer(string name, Action<BitStreamAbstract> serializeData, NetworkTarget target, NetworkGroup group = NetworkGroup.GlobalGroup)
        {
            var index = NetworkProtected.ClientToServerIndex;
            var streamWriter = new BitStreamWriter();
            streamWriter.Serialize(ref index);
            serializeData(streamWriter);
            if (target.SendToSelf_)
            {
                NetworkProtected.ClientToServerEventReceived(name, new BitStreamReader(streamWriter.ToBytes()));
            }
            NetworkProtected.ClientToServerTransceiver.CallPrivateMethod(new MethodMetadata()
            {
                IsStatic = false,
                Name = "SendRPC",
                Types = new Type[] { typeof(NetworkGroup), typeof(string), typeof(NetworkTarget), typeof(object[]) }
            }, group, "ReceiveClientToServerEvent", target, streamWriter.ToBytes());
        }
        public void FireClientToServer(string name, Action<BitStreamAbstract> serializeData, NetworkGroup group = NetworkGroup.GlobalGroup)
        {
            FireClientToServer(name, serializeData, ClientToServerNetworkTarget, group);
        }
        // End Fire Methods

        private void ClientToServerEventReceived(ClientToServer.Data data)
        {
            if (ClientToServerCallbacks.ContainsKey(data.EventName))
                ClientToServerCallbacks[data.EventName].ForEach((action) => action?.Invoke(data.Sender, data.EventData));
        }

        private void ServerToClientEventReceived(ServerToClient.Data data)
        {
            if (ServerToClientCallbacks.ContainsKey(data.EventName))
                ServerToClientCallbacks[data.EventName].ForEach((action) => action?.Invoke(data.Sender, data.EventData));
        }

        private void BroadcastAllEventReceived(BroadcastAll.Data data)
        {
            if (BroadcastAllCallbacks.ContainsKey(data.EventName))
                BroadcastAllCallbacks[data.EventName].ForEach((action) => action?.Invoke(data.Sender, data.EventData));
        }
    }
}
