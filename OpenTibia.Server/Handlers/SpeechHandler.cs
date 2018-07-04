// <copyright file="SpeechHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers
{
    using System;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Packets.Incoming;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Notifications;

    internal class SpeechHandler : IncomingPacketHandler
    {
        public override void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var speechPacket = new SpeechPacket(message);
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            if (player == null)
            {
                return;
            }

            // TODO: proper implementation.
            var msgStr = speechPacket.Speech.Message;

            if (msgStr.ToLower().StartsWith("test"))
            {
                Game.Instance.TestingViaCreatureSpeech(player, msgStr);
            }

            // TODO: implement all spells and speech related hooks.
            Game.Instance.NotifySpectatingPlayers(conn => new CreatureSpokeNotification(connection, player, speechPacket.Speech.Type, speechPacket.Speech.Message, speechPacket.Speech.ChannelId), player.Location);

            Console.WriteLine($"{player.Name}: {speechPacket.Speech.Message}");
        }
    }
}