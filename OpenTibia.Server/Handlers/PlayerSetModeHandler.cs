// <copyright file="PlayerSetModeHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers
{
    using System;
    using OpenTibia.Communications;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data;

    internal class PlayerSetModeHandler : IncomingPacketHandler
    {
        public override void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            // No other content in message.
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            if (player == null)
            {
                return;
            }

            var rawFightMode = message.GetByte(); // 1 - offensive, 2 - balanced, 3 - defensive
            var rawChaseMode = message.GetByte(); // 0 - stand while fightning, 1 - chase opponent
            var rawSafeMode = message.GetByte();

            var fightMode = (FightMode)rawFightMode;

            // TODO: correctly implement.
            Console.WriteLine($"PlayerId {player.Name} changed modes to {fightMode}.");
        }
    }
}