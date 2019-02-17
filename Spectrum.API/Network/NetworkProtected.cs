using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spectrum.API.Extensions;
using Spectrum.API.Network.Events;
using Spectrum.API.Reflection;
using UnityEngine;

namespace Spectrum.API.Network
{
    internal abstract class NetworkProtected : NetworkStaticEventTransceiver
    {
        // This class must inherit NetworkStaticEventTransceiver in order to create a subclass of the protected SubscriberAbstract
        // This class remains abstract to prevent the complexity associated with creating a "fake" Unity Component and having to implement the class

        // This class is used solely for BitStreamAbstract event compatibility
        // Any methods or properties in this class that are named the same as those in EventRouter are their equivalents but for BitStreamAbstract events.
        // This class should be used solely internally and be exposed through EventRouter

        static internal Dictionary<string, Action<BitStreamAbstract>> ClientToServerCallbacks { get; } = new Dictionary<string, Action<BitStreamAbstract>>();
        static internal Dictionary<string, Action<BitStreamAbstract>> ServerToClientCallbacks { get; } = new Dictionary<string, Action<BitStreamAbstract>>();
        static internal Dictionary<string, Action<BitStreamAbstract>> BroadcastAllCallbacks { get; } = new Dictionary<string, Action<BitStreamAbstract>>();

        static protected CallbackSubscriber ClientToServerSubscriber;
        static protected CallbackSubscriber ServerToClientSubscriber;
        static protected CallbackSubscriber BroadcastAllSubscriber;

        static internal int ClientToServerIndex { get => ClientToServerSubscriber.Index; }
        static internal int ServerToClientIndex { get => ServerToClientSubscriber.Index; }
        static internal int BroadcastAllIndex { get => BroadcastAllSubscriber.Index; }

        static internal ClientToServerNetworkTransceiver ClientToServerTransceiver;
        static internal ServerToClientNetworkTransceiver ServerToClientTransceiver;
        static internal ClientToClientNetworkTransceiver ClientToClientTransceiver;

        public static void Init()
        {
            ClientToServerSubscriber = new CallbackSubscriber(ClientToServerCallbacks);
            ServerToClientSubscriber = new CallbackSubscriber(ServerToClientCallbacks);
            BroadcastAllSubscriber = new CallbackSubscriber(BroadcastAllCallbacks);

            ClientToServerTransceiver = GameObject.FindObjectOfType<ClientToServerNetworkTransceiver>();
            ServerToClientTransceiver = GameObject.FindObjectOfType<ServerToClientNetworkTransceiver>();
            ClientToClientTransceiver = GameObject.FindObjectOfType<ClientToClientNetworkTransceiver>();
        }

        static internal int GetListCount(NetworkStaticEventTransceiver transceiver)
        {
            return (transceiver as NetworkStaticEventTransceiver).GetPrivateMember<List<SubscriberAbstract>>(new MemberMetadata()
            {
                MemberName = "list_",
                IsProperty = false,
                IsStatic = false,
                Type = typeof(NetworkStaticEventTransceiver)
            }).Count;
        }

        static internal void RegisterClientToServerCallback(string name, Action<BitStreamAbstract> action)
        {
            ClientToServerCallbacks[name] = action;
        }
        static internal void RegisterServerToClientCallback(string name, Action<BitStreamAbstract> action)
        {
            ServerToClientCallbacks[name] = action;
        }
        static internal void RegisterBroadcastAllCallback(string name, Action<BitStreamAbstract> action)
        {
            BroadcastAllCallbacks[name] = action;
        }

        static internal void ClientToServerEventReceived(string eventName, BitStreamAbstract stream)
        {
            if (ClientToServerCallbacks.ContainsKey(eventName))
                ClientToServerCallbacks[eventName](stream);
        }

        static internal void ServerToClientEventReceived(string eventName, BitStreamAbstract stream)
        {
            if (ServerToClientCallbacks.ContainsKey(eventName))
                ServerToClientCallbacks[eventName](stream);
        }

        static internal void BroadcastAllEventReceived(string eventName, BitStreamAbstract stream)
        {
            if (BroadcastAllCallbacks.ContainsKey(eventName))
                BroadcastAllCallbacks[eventName](stream);
        }

        protected class CallbackSubscriber : SubscriberAbstract
        {
            public override Type EventDataType_ => null;

            public int Index { get => index_; }

            protected internal Dictionary<string, Action<BitStreamAbstract>> Callbacks;

            public CallbackSubscriber(Dictionary<string, Action<BitStreamAbstract>> Callbacks)
            {
                this.Callbacks = Callbacks;
            }

            public override void TransceiverSubscribe() {}

            public override void TransceiverUnsubscribe() {}

            public override void ReceiveRPC(BitStreamAbstract stream)
            {
                string EventName = "";
                stream.Serialize(ref EventName);
                if (Callbacks.ContainsKey(EventName))
                {
                    Callbacks[EventName](stream);
                }
            }

            public void Register(NetworkStaticEventTransceiver transceiver)
            {
                // Adds this subscriber to the events list
                transceiver.CallPrivateMethod(new MethodMetadata() {
                    IsStatic = false,
                    Name = "Register"
                }, this);
            }
        }
    }
}
