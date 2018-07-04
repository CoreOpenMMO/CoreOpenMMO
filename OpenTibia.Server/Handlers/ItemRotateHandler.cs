﻿// <copyright file="ItemRotateHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers
{
    using OpenTibia.Communications;
    using OpenTibia.Server.Data;

    internal class ItemRotateHandler : IncomingPacketHandler
    {
        public override void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;
            player?.ClearPendingActions();

        }
    }
}