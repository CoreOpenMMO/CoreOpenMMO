// <copyright file="LoginOrManagementIncomingPacketType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Data.Interfaces
{
    public enum LoginOrManagementIncomingPacketType : byte
    {
        AuthenticationRequest = 0x00,
        LoginServerRequest = 0x01,
        ServerStatusRequest = 0xFF,
        PlayerLogIn = 0x14,
        PlayerLogOut = 0x15,
        NameLock = 0x17,
        Banishment = 0x19,
        Notation = 0x1A,
        ReportStatement = 0x1B,
        CharacterDeath = 0x1D,
        CreatePlayerUnsure = 0x20,
        FinishAuctions = 0x21,
        TransferHouses = 0x23,
        EvictFreeAccounts = 0x24,
        EvictDeleted = 0x25,
        EvictDelinquentGuildhouse = 0x26,
        GetHouseOwners = 0x2A,
        InsertHouseOwner = 0x27,
        UpdateHouseOwner = 0x28,
        DeleteHouseOwner = 0x29,
        BanishIpAddress = 0x1C,
        AddVip = 0x1E,
        RemoveVip = 0x1F,
        GetAuctions = 0x2B,
        StartAuction = 0x2C,
        InsertHouses = 0x2D,
        ClearIsOnline = 0x2E,
        CreatePlayerList = 0x2F,
        LogKilledCreatures = 0x30,
        LoadPlayers = 0x32,
        ExcludeFromAuctions = 0x33,
        LoadWorld = 0x35,
        HighscoreUnsure = 0xCB,
        CreateHighscores = 0xCC,
        Any = 0xFF // Do not send
    }
}