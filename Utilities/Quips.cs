﻿using Il2CppInterop.Runtime;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;
using static RetroCamera.Configuration.QuipManager;

namespace RetroCamera.Utilities;
internal static class Quips
{
    static EntityManager EntityManager => Core.EntityManager;
    static Entity LocalCharacter => Core.LocalCharacter;
    static Entity LocalUser => Core.LocalUser;
    static NetworkId NetworkId => LocalUser.GetNetworkId();
    static FromCharacter? _fromCharacter;
    public static FromCharacter FromCharacter =>
        _fromCharacter ??= new FromCharacter
        {
            Character = LocalCharacter,
            User = LocalUser
        };

    static readonly ComponentType[] _componentTypes =
    [
        ComponentType.ReadOnly(Il2CppType.Of<FromCharacter>()),
        ComponentType.ReadOnly(Il2CppType.Of<NetworkEventType>()),
        ComponentType.ReadOnly(Il2CppType.Of<SendNetworkEventTag>()),
        ComponentType.ReadOnly(Il2CppType.Of<ChatMessageEvent>())
    ];

    static readonly NetworkEventType _networkEventType = new()
    {
        IsAdminEvent = false,
        EventId = NetworkEvents.EventId_ChatMessageEvent,
        IsDebugEvent = false,
    };
    public static void SendCommandQuip(CommandQuip commandQuip)
    {
        string name = commandQuip.Name;
        string command = commandQuip.Command;

        ChatMessageEvent chatMessage = new()
        {
            MessageText = new FixedString512Bytes(command),
            MessageType = ChatMessageType.Local,
            ReceiverEntity = NetworkId
        };

        Entity networkEvent = EntityManager.CreateEntity(_componentTypes);

        networkEvent.Write(FromCharacter);
        networkEvent.Write(_networkEventType);
        networkEvent.Write(chatMessage);

        Core.Log.LogInfo($"CommandQuip - {name}({command})");
    }
}
