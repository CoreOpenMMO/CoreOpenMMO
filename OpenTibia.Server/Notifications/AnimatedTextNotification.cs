// <copyright file="AnimatedTextNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Models.Structs;

    internal class AnimatedTextNotification : Notification
    {
        public Location Location { get; }

        public TextColor TextColor { get; }

        public string Text { get; }

        public AnimatedTextNotification(Connection connection, Location location, string text, TextColor textColor = TextColor.White)
            : base(connection)
        {
            text.ThrowIfNullOrWhiteSpace(nameof(text));

            this.Location = location;
            this.Text = text;
            this.TextColor = textColor;
        }

        public override void Prepare()
        {
            this.ResponsePackets.Add(new AnimatedTextPacket
            {
                Location = this.Location,
                Text = this.Text,
                Color = this.TextColor
            });
        }
    }
}