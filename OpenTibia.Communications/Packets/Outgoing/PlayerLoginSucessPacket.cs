// <copyright file="PlayerLoginSucessPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Data.Models;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class PlayerLoginSucessPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)ManagementOutgoingPacketType.NoType;

        public int AccountId { get; set; }

        public string CharacterName { get; set; }

        public byte Gender { get; set; }

        public string Guild { get; set; }

        public string GuildTitle { get; set; }

        public string PlayerTitle { get; set; }

        public IList<Buddy> VipContacts { get; set; }

        public int PremiumDays { get; set; }

        public bool RecentlyActivatedPremmium { get; set; }

        public HashSet<string> Privileges { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(0x00); // Should always be 0 for this packet; means there was no error.
            message.AddUInt32((uint)this.AccountId);
            message.AddString(this.CharacterName);
            message.AddByte(this.Gender);

            message.AddString(this.Guild ?? string.Empty);
            message.AddString(string.IsNullOrWhiteSpace(this.Guild) || string.IsNullOrWhiteSpace(this.GuildTitle) ? "0" : this.GuildTitle);
            message.AddString(string.IsNullOrWhiteSpace(this.Guild) || string.IsNullOrWhiteSpace(this.PlayerTitle) ? "0" : this.PlayerTitle);

            message.AddByte((byte)(this.VipContacts == null ? 0 : Math.Min(this.VipContacts.Count, 100)));

            foreach (var contact in this.VipContacts.Take(100))
            {
                message.AddUInt32((uint)contact.BuddyId);
                message.AddString(contact.BuddyString);
            }

            message.AddByte((byte)(this.Privileges == null ? 0 : Math.Min(this.Privileges.Count, 255)));

            foreach (var privString in this.Privileges.Take(255))
            {
                message.AddString(privString);
            }

            message.AddByte((byte)(this.RecentlyActivatedPremmium ? 0x01 : 0x00));
        }

        public override void CleanUp()
        {
            this.VipContacts = null;
            this.Privileges = null;
        }
    }
}
