// <copyright file="AutoMoveHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers
{
    using OpenTibia.Communications;
    using OpenTibia.Communications.Packets.Incoming;
    using OpenTibia.Server.Data;

    internal class AutoMoveHandler : IncomingPacketHandler
    {
        public override void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var packet = new AutoMovePacket(message);
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            player?.AutoWalk(packet.Directions);
        }
    }
}