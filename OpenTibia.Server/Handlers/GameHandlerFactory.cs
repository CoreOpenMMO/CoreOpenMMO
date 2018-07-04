// <copyright file="GameHandlerFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers
{
    using OpenTibia.Communications.Interfaces;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;

    public class GameHandlerFactory : IHandlerFactory
    {
        public IIncomingPacketHandler CreateIncommingForType(byte packeType)
        {
            switch ((GameIncomingPacketType)packeType)
            {
                case GameIncomingPacketType.PlayerLoginRequest:
                    return new PlayerLoginHandler();
                case GameIncomingPacketType.PlayerLogOut:
                    return new LogoutHandler();
                case GameIncomingPacketType.AutoMove:
                    return new AutoMoveHandler();
                case GameIncomingPacketType.WalkNorth:
                    return new PlayerWalkOnDemandHandler(Direction.North);
                case GameIncomingPacketType.WalkEast:
                    return new PlayerWalkOnDemandHandler(Direction.East);
                case GameIncomingPacketType.WalkSouth:
                    return new PlayerWalkOnDemandHandler(Direction.South);
                case GameIncomingPacketType.WalkWest:
                    return new PlayerWalkOnDemandHandler(Direction.West);
                case GameIncomingPacketType.CancelAutoWalk:
                    return new AutoMoveCancelHandler();
                case GameIncomingPacketType.WalkNorteast:
                    return new PlayerWalkOnDemandHandler(Direction.NorthEast);
                case GameIncomingPacketType.WalkNorthwest:
                    return new PlayerWalkOnDemandHandler(Direction.NorthWest);
                case GameIncomingPacketType.WalkSoutheast:
                    return new PlayerWalkOnDemandHandler(Direction.SouthEast);
                case GameIncomingPacketType.WalkSouthwest:
                    return new PlayerWalkOnDemandHandler(Direction.SouthWest);
                case GameIncomingPacketType.TurnEast:
                    return new PlayerTurnToDirectionHandler(Direction.East);
                case GameIncomingPacketType.TurnNorth:
                    return new PlayerTurnToDirectionHandler(Direction.North);
                case GameIncomingPacketType.TurnSouth:
                    return new PlayerTurnToDirectionHandler(Direction.South);
                case GameIncomingPacketType.TurnWest:
                    return new PlayerTurnToDirectionHandler(Direction.West);
                case GameIncomingPacketType.ItemThrow:
                    return new ItemMoveHandler();
                case GameIncomingPacketType.TradeRequest:
                    return new TradeRequestHandler();
                case GameIncomingPacketType.TradeLook:
                    return new TradeLookHandler();
                case GameIncomingPacketType.TradeAccept:
                    return new TradeAcceptHandler();
                case GameIncomingPacketType.TradeCancel:
                    return new TradeCancelHandler();
                case GameIncomingPacketType.ItemUse:
                    return new ItemUseHandler();
                case GameIncomingPacketType.ItemUseOn:
                    return new ItemUseOnHandler();
                case GameIncomingPacketType.ItemBattleWindow:
                    return new ItemUseBattleHandler();
                case GameIncomingPacketType.ItemRotate:
                    return new ItemRotateHandler();
                case GameIncomingPacketType.ContainerClose:
                    return new ContainerCloseHandler();
                case GameIncomingPacketType.ContainerUp:
                    return new ContainerUpHandler();
                case GameIncomingPacketType.WindowText:
                    return new TextWindowPostHandler();
                case GameIncomingPacketType.WindowHouse:
                    return new HouseWindowPostHandler();
                case GameIncomingPacketType.ItemLook:
                    return new LookAtHandler();
                case GameIncomingPacketType.Speech:
                    return new SpeechHandler();
                case GameIncomingPacketType.ChannelListRequest:
                    return new ChannelListRequestHandler();
                case GameIncomingPacketType.ChannelOpen:
                    return new ChannelOpenPublicHandler();
                case GameIncomingPacketType.ChannelClose:
                    return new ChannelClosePublicHandler();
                case GameIncomingPacketType.ChannelOpenPrivate:
                    return new ChannelOpenPrivateHandler();
                case GameIncomingPacketType.ReportProcess:
                    return new ReportProcessHandler();
                case GameIncomingPacketType.ReportClose:
                    return new ReportCloseHandler();
                case GameIncomingPacketType.ReportCancel:
                    return new ReportCancelHandler();
                case GameIncomingPacketType.ChangeModes:
                    return new PlayerSetModeHandler();
                case GameIncomingPacketType.Attack:
                    return new AttackHandler();
                case GameIncomingPacketType.Follow:
                    return new FollowHandler();
                case GameIncomingPacketType.PartyInvite:
                    return new PartyRequestHandler();
                case GameIncomingPacketType.PartyJoin:
                    return new PartyAcceptHandler();
                case GameIncomingPacketType.PartyRevoke:
                    return new PartyRejectHandler();
                case GameIncomingPacketType.PartyPassLeadership:
                    return new PartyPassLeadershipHandler();
                case GameIncomingPacketType.PartyLeave:
                    return new PartyLeaveHandler();
                case GameIncomingPacketType.ChannelCreatePrivate:
                    return new ChannelCreateOwnHandler();
                case GameIncomingPacketType.ChannelInvite:
                    return new ChannelInviteHandler();
                case GameIncomingPacketType.ChannelExclude:
                    return new ChannelKickHandler();
                case GameIncomingPacketType.StopAllActions:
                    return new StopAllActionsHandler();
                case GameIncomingPacketType.ResendTile:
                    return new ReSendTileRequestHandler();
                case GameIncomingPacketType.ResentContainer:
                    return new ReSendContainerRequestHandler();
                case GameIncomingPacketType.OutfitChangeRequest:
                    return new OutfitChangeRequestHandler();
                case GameIncomingPacketType.OutfitChangeCompleted:
                    return new OutfitChangedHandler();
                case GameIncomingPacketType.AddVip:
                    return new VipAddHandler();
                case GameIncomingPacketType.RemoveVip:
                    return new VipRemoveHandler();
                case GameIncomingPacketType.ReportBug:
                    return new ReportBugHandler();
                case GameIncomingPacketType.ReportViolation:
                    return new ReportViolationHandler();
                case GameIncomingPacketType.ReportDebugAssertion:
                    return new DebugAssertionHandler();
                default:
                    return new DefaultHandler(packeType);
            }
        }
    }
}
