using System;
using UnityEngine;

namespace Spectrum.API.Interfaces.Systems
{
    public interface IEventRouter
    {
        void Init();
        void RegisterClientToServerCallback(string eventName, Action<NetworkPlayer, string> callback);
        void RegisterServerToClientCallback(string eventName, Action<NetworkPlayer, string> callback);
        void RegisterBroadcastAllCallback(string eventName, Action<NetworkPlayer, string> callback);
        void RegisterClientToServerCallback(string eventName, Action<BitStreamAbstract> callback);
        void RegisterServerToClientCallback(string eventName, Action<BitStreamAbstract> callback);
        void RegisterBroadcastAllCallback(string eventName, Action<BitStreamAbstract> callback);
        void FireClientToServer(string name, string data, NetworkTarget target, NetworkGroup group = NetworkGroup.GlobalGroup);
        void FireClientToServer(string name, string data, NetworkGroup group = NetworkGroup.GlobalGroup);
        void FireClientToServer(string name, Action<BitStreamAbstract> serializeData, NetworkTarget target, NetworkGroup group = NetworkGroup.GlobalGroup);
        void FireClientToServer(string name, Action<BitStreamAbstract> serializeData, NetworkGroup group = NetworkGroup.GlobalGroup);
        void FireServerToClient(string name, string data, NetworkTarget target, NetworkGroup group = NetworkGroup.GlobalGroup);
        void FireServerToClient(string name, string data, NetworkGroup group = NetworkGroup.GlobalGroup);
        void FireServerToClient(string name, Action<BitStreamAbstract> serializeData, NetworkTarget target, NetworkGroup group = NetworkGroup.GlobalGroup);
        void FireServerToClient(string name, Action<BitStreamAbstract> serializeData, NetworkGroup group = NetworkGroup.GlobalGroup);
        void FireBroadcastAll(string name, string data, NetworkTarget target, NetworkGroup group = NetworkGroup.GlobalGroup);
        void FireBroadcastAll(string name, string data, NetworkGroup group = NetworkGroup.GlobalGroup);
        void FireBroadcastAll(string name, Action<BitStreamAbstract> serializeData, NetworkTarget target, NetworkGroup group = NetworkGroup.GlobalGroup);
        void FireBroadcastAll(string name, Action<BitStreamAbstract> serializeData, NetworkGroup group = NetworkGroup.GlobalGroup);
    }
}
