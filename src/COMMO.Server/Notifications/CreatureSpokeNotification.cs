// <copyright file="CreatureSpokeNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using COMMO.Communications;
using COMMO.Communications.Packets.Outgoing;
using COMMO.Data.Contracts;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Server.Notifications
{
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

            Creature = creature;
            SpeechType = speechType;
            Message = message;
            Channel = channel;
        }

        public override void Prepare()
        {
            ResponsePackets.Add(new CreatureSpeechPacket
            {
                ChannelId = Channel,
                SenderName = Creature.Name,
                Location = Creature.Location,
                SpeechType = SpeechType,
                Text = Message,
                Time = (uint)DateTime.Now.Ticks
            });
        }
    }
}