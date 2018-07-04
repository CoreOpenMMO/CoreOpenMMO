﻿// <copyright file="IPacketOutgoing.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Data.Interfaces
{
    public enum ManagementOutgoingPacketType : byte
    {
        NoType = 0x00,
        Disconnect = 0x0A,
        MessageOfTheDay = 0x14,
        CharacterList = 0x64
    }

    public enum GameOutgoingPacketType : byte
    {
        NoType = 0x00,
        SelfAppear = 0x0A,
        Disconnect = 0x14,
        AddUnknownCreature = 0x61,
        AddKnownCreature = 0x62,
        MapDescription = 0x64,
        MapSliceNorth = 0x65,
        MapSliceEast = 0x66,
        MapSliceSouth = 0x67,
        MapSliceWest = 0x68,
        TileUpdate = 0x69,
        AddAtStackpos = 0x6A,
        TransformThing = 0x6B,
        RemoveAtStackpos = 0x6C,
        CreatureMoved = 0x6D,
        ContainerOpen = 0x6E,
        ContainerClose = 0x6F,
        ContainerAddItem = 0x70,
        ContainerUpdateItem = 0x71,
        ContainerRemoveItem = 0x72,
        InventoryItem = 0x78,
        InventoryEmpty = 0x79,
        WorldLight = 0x82,
        MagicEffect = 0x83,
        AnimatedText = 0x84,
        PlayerWalkCancel = 0xB5,
        CreatureLight = 0x8D,
        CreatureOutfit = 0x8E,
        PlayerStatus = 0xA0,
        PlayerSkills = 0xA1,
        PlayerConditions = 0xA2,
        CreatureSpeech = 0xAA,
        TextMessage = 0xB4,
        FloorChangeUp = 0xBE,
        FloorChangeDown = 0xBF,
        OutfitWindow = 0xC8
    }

    public interface IPacketOutgoing
    {
        byte PacketType { get; }

        void Add(NetworkMessage message);

        void CleanUp();
    }
}
