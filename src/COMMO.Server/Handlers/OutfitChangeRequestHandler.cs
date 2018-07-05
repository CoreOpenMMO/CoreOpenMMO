// <copyright file="OutfitChangeRequestHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Communications;
using COMMO.Communications.Packets.Outgoing;
using COMMO.Server.Data;

namespace COMMO.Server.Handlers
{
    internal class OutfitChangeRequestHandler : IncomingPacketHandler
    {
        public override void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            // No further content on message.
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            if (player == null)
            {
                return;
            }

            // TODO: if player actually has permissions to change outfit.

            // TODO: get these based on sex and premium
            ushort chooseFromId = 128;
            ushort chooseToId = 134;

            ResponsePackets.Add(new PlayerChooseOutfitPacket
            {
                CurrentOutfit = player.Outfit,
                ChooseFromId = chooseFromId,
                ChooseToId = chooseToId
            });
        }
    }
}