// <copyright file="LogoutHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers
{
    using OpenTibia.Communications;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data;

    internal class LogoutHandler : IncomingPacketHandler
    {
        public override void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            // no further content
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            if (player == null)
            {
                return;
            }

            if (Game.Instance.AttemptLogout(player))
            {
                connection.Close();
            }
            else
            {
                this.ResponsePackets.Add(new TextMessagePacket
                {
                    Type = MessageType.StatusSmall,
                    Message = "You may not logout (test message)"
                });
            }
        }
    }
}