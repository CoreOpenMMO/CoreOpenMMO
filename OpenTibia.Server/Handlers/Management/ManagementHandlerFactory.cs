// <copyright file="ManagementHandlerFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers.Management
{
    using OpenTibia.Communications;
    using OpenTibia.Communications.Interfaces;
    using OpenTibia.Server.Data.Interfaces;

    public class ManagementHandlerFactory : IHandlerFactory
    {
        public IIncomingPacketHandler CreateIncommingForType(byte packeType)
        {
            switch ((LoginOrManagementIncomingPacketType)packeType)
            {
                case LoginOrManagementIncomingPacketType.AuthenticationRequest:
                    return new AuthenticationHandler();
                case LoginOrManagementIncomingPacketType.ServerStatusRequest:
                    return new ServerStatusHandler();
                case LoginOrManagementIncomingPacketType.PlayerLogIn:
                    return new Handlers.PlayerLoginHandler();
                case LoginOrManagementIncomingPacketType.PlayerLogOut:
                    return new PlayerLogoutHandler();
                case LoginOrManagementIncomingPacketType.NameLock:
                    // DebugPacket();
                    // TODO
                    // writeMsg.addByte(0x00); // ErrorCode
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.Banishment:
                    return new BanismentHandler();
                case LoginOrManagementIncomingPacketType.Notation:
                    return new NotationHandler();
                case LoginOrManagementIncomingPacketType.ReportStatement:
                    return new ReportStatementHandler();
                case LoginOrManagementIncomingPacketType.CharacterDeath:
                    return new CharacterDeathHandler();
                case LoginOrManagementIncomingPacketType.CreatePlayerUnsure:
                    // DebugPacket();
                    // unsigned int ACCID = msg.getU32();
                    // unsigned int unknown = msg.getByte();

                    // writeMsg.addByte(0x00); // ErrorCode
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.FinishAuctions:
                    return new FinishAuctionsHandler();
                case LoginOrManagementIncomingPacketType.TransferHouses:
                    // return new TransferHousesHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.EvictFreeAccounts:
                    // return new EvictFreeAccountsHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.EvictDeleted:
                    // return new EvictDeletedHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.EvictDelinquentGuildhouse:
                    // return new EvictDelinquentGuildhouseHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.GetHouseOwners:
                    // return new GetHouseOwnersHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.InsertHouseOwner:
                    // return new InsertHouseOwnerHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.UpdateHouseOwner:
                    // return new UpdateHouseOwnerHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.DeleteHouseOwner:
                    // return new DeleteHouseOwnerHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.BanishIpAddress:
                    // return new BanishIpAddressHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.AddVip:
                    // return new AddVIPHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.GetAuctions:
                    // return new GetAuctionsHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.StartAuction:
                    // return new StartAuctionHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.InsertHouses:
                    return new InsertHousesHandler();
                case LoginOrManagementIncomingPacketType.ClearIsOnline:
                    return new ClearIsOnlineHandler();
                case LoginOrManagementIncomingPacketType.CreatePlayerList:
                    return new CreatePlayerListHandler();
                case LoginOrManagementIncomingPacketType.LogKilledCreatures:
                    // return new LogKilledCreaturesHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.LoadPlayers:
                    return new LoadPlayersHandler();
                case LoginOrManagementIncomingPacketType.ExcludeFromAuctions:
                    // return new ExcludeFromAuctionsHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.LoadWorld:
                    return new LoadWorldHandler();
                case LoginOrManagementIncomingPacketType.HighscoreUnsure:
                    // return new HighscoreHandler();
                    return new DefaultHandler(packeType);
                case LoginOrManagementIncomingPacketType.CreateHighscores:
                    // return new CreateHighscoresHandler();
                    return new DefaultHandler(packeType);
                default:
                    return new DefaultHandler(packeType);
            }
        }
    }
}
