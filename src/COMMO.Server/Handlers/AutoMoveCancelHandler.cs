// <copyright file="AutoMoveCancelHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Communications;
using COMMO.Communications.Packets.Outgoing;
using COMMO.Server.Data;

namespace COMMO.Server.Handlers
{
    internal class AutoMoveCancelHandler : IncomingPacketHandler
    {
        public override void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            // No content.
            player?.StopWalking();
            player?.ClearPendingActions();

            if (player != null)
            {
                ResponsePackets.Add(new PlayerWalkCancelPacket
                {
                    Direction = player.ClientSafeDirection
                });
            }
        }
    }
}