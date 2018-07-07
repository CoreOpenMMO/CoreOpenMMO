// <copyright file="AnimatedTextNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Communications;
using COMMO.Communications.Packets.Outgoing;
using COMMO.Data.Contracts;
using COMMO.Server.Data.Models.Structs;

namespace COMMO.Server.Notifications {
	internal class AnimatedTextNotification : Notification {
		public Location Location { get; }

		public TextColor TextColor { get; }

		public string Text { get; }

		public AnimatedTextNotification(Connection connection, Location location, string text, TextColor textColor = TextColor.White)
			: base(connection) {
			if (string.IsNullOrWhiteSpace(text))
				throw new System.ArgumentException(nameof(text) + " can't be null or whitespace.");

			Location = location;
			Text = text;
			TextColor = textColor;
		}

		public override void Prepare() {
			ResponsePackets.Add(new AnimatedTextPacket {
				Location = Location,
				Text = Text,
				Color = TextColor
			});
		}
	}
}