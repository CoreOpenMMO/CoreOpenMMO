// <copyright file="AutoMoveCancelHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers
{
    using OpenTibia.Communications;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Data;

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
                this.ResponsePackets.Add(new PlayerWalkCancelPacket
                {
                    Direction = player.ClientSafeDirection
                });
            }
        }
    }
}