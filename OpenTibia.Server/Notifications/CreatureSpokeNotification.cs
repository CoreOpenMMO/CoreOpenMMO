// <copyright file="CreatureSpokeNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using System;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;

    internal class CreatureSpokeNotification : Notification
    {
        public ICreature Creature { get; }

        public SpeechType SpeechType { get; }

        public string Message { get; }

        public ChatChannel Channel { get; }

        public CreatureSpokeNotification(Connection connection, ICreature creature, SpeechType speechType, string message, ChatChannel channel = ChatChannel.None)
            : base(connection)
        {
            if (creature == null)
            {
                throw new ArgumentNullException(nameof(creature));
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.Creature = creature;
            this.SpeechType = speechType;
            this.Message = message;
            this.Channel = channel;
        }

        public override void Prepare()
        {
            this.ResponsePackets.Add(new CreatureSpeechPacket
            {
                ChannelId = this.Channel,
                SenderName = this.Creature.Name,
                Location = this.Creature.Location,
                SpeechType = this.SpeechType,
                Text = this.Message,
                Time = (uint)DateTime.Now.Ticks
            });
        }
    }
}