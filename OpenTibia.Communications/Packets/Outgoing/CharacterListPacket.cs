// <copyright file="CharacterListPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class CharacterListPacket : PacketOutgoing
    {
        public IEnumerable<ICharacterListItem> Characters { get; set; }

        public ushort PremiumDaysLeft { get; set; }

        public override byte PacketType => (byte)ManagementOutgoingPacketType.CharacterList;

        public CharacterListPacket()
        {
            this.Characters = Enumerable.Empty<ICharacterListItem>();
            this.PremiumDaysLeft = 0;
        }

        public CharacterListPacket(IEnumerable<ICharacterListItem> characters, ushort premDays)
        {
            this.Characters = characters;
            this.PremiumDaysLeft = premDays;
        }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);
            message.AddByte((byte)this.Characters.Count());

            foreach (ICharacterListItem character in this.Characters)
            {
                message.AddString(character.Name);
                message.AddString(character.World);
                message.AddBytes(character.Ip);
                message.AddUInt16(character.Port);
            }

            message.AddUInt16(this.PremiumDaysLeft);
        }

        public override void CleanUp()
        {
            this.Characters = null;
        }
    }
}
