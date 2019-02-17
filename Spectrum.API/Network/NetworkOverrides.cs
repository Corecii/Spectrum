using Spectrum.API.Extensions;
using Spectrum.API.Reflection;
using UnityEngine;

namespace Spectrum.API.Network
{
    public static class NetworkOverrides
    {
        internal static int RegisterServerToClientEvent<T>() where T : struct, IBitSerializable, INetworkGrouped
        {
            var transceiverInstance = GameObject.FindObjectOfType<ServerToClientNetworkTransceiver>();

            (transceiverInstance as NetworkStaticEventTransceiver)
                .CallPrivateGenericMethod<T>(new MethodMetadata
                {
                    IsStatic = false,
                    Name = "RegisterServerToClientEvent"
                });
            return NetworkProtected.GetListCount(transceiverInstance) - 1;
        }

        internal static int RegisterTargetedEvent<T>() where T : struct, IBitSerializable, INetworkGrouped
        {
            var transceiverInstance = GameObject.FindObjectOfType<ServerToClientNetworkTransceiver>();

            (transceiverInstance as NetworkStaticEventTransceiver)
                .CallPrivateGenericMethod<T>(new MethodMetadata
                {
                    IsStatic = false,
                    Name = "RegisterTargetedEvent"
                });
            return NetworkProtected.GetListCount(transceiverInstance) - 1;
        }

        internal static int RegisterClientToServerEvent<T>() where T : struct, IBitSerializable, INetworkGrouped
        {
            var transceiverInstance = GameObject.FindObjectOfType<ClientToServerNetworkTransceiver>();

            (transceiverInstance as NetworkStaticEventTransceiver)
                .CallPrivateGenericMethod<T>(new MethodMetadata
                {
                    IsStatic = false,
                    Name = "RegisterClientToServerEvent"
                });
            return NetworkProtected.GetListCount(transceiverInstance) - 1;
        }

        internal static int RegisterBroadcastAllEvent<T>() where T : struct, IBitSerializable, INetworkGrouped
        {
            var transceiverInstance = GameObject.FindObjectOfType<ClientToClientNetworkTransceiver>();

            (transceiverInstance as NetworkStaticEventTransceiver)
                .CallPrivateGenericMethod<T>(new MethodMetadata
                {
                    IsStatic = false,
                    Name = "RegisterBroadcastAllEvent"
                });
            return NetworkProtected.GetListCount(transceiverInstance) - 1;
        }
    }
}
