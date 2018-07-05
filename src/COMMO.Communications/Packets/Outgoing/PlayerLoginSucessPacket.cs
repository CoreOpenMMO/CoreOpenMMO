// <copyright file="PlayerLoginSucessPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Communications.Packets.Outgoing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using COMMO.Data.Models;
    using COMMO.Server.Data;
    using COMMO.Server.Data.Interfaces;

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
            message.AddUInt32((uint)AccountId);
            message.AddString(CharacterName);
            message.AddByte(Gender);

            message.AddString(Guild ?? string.Empty);
            message.AddString(string.IsNullOrWhiteSpace(Guild) || string.IsNullOrWhiteSpace(GuildTitle) ? "0" : GuildTitle);
            message.AddString(string.IsNullOrWhiteSpace(Guild) || string.IsNullOrWhiteSpace(PlayerTitle) ? "0" : PlayerTitle);

            message.AddByte((byte)(VipContacts == null ? 0 : Math.Min(VipContacts.Count, 100)));

            foreach (var contact in VipContacts.Take(100))
            {
                message.AddUInt32((uint)contact.BuddyId);
                message.AddString(contact.BuddyString);
            }

            message.AddByte((byte)(Privileges == null ? 0 : Math.Min(Privileges.Count, 255)));

            foreach (var privString in Privileges.Take(255))
            {
                message.AddString(privString);
            }

            message.AddByte((byte)(RecentlyActivatedPremmium ? 0x01 : 0x00));
        }

        public override void CleanUp()
        {
            VipContacts = null;
            Privileges = null;
        }
    }
}
