// <copyright file="CharacterListPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Communications.Packets.Outgoing
{
    using System.Collections.Generic;
    using System.Linq;
    using COMMO.Server.Data;
    using COMMO.Server.Data.Interfaces;

    public class CharacterListPacket : PacketOutgoing
    {
        public IEnumerable<ICharacterListItem> Characters { get; set; }

        public ushort PremiumDaysLeft { get; set; }

        public override byte PacketType => (byte)ManagementOutgoingPacketType.CharacterList;

        public CharacterListPacket()
        {
            Characters = Enumerable.Empty<ICharacterListItem>();
            PremiumDaysLeft = 0;
        }

        public CharacterListPacket(IEnumerable<ICharacterListItem> characters, ushort premDays)
        {
            Characters = characters;
            PremiumDaysLeft = premDays;
        }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);
            message.AddByte((byte)Characters.Count());

            foreach (ICharacterListItem character in Characters)
            {
                message.AddString(character.Name);
                message.AddString(character.World);
                message.AddBytes(character.Ip);
                message.AddUInt16(character.Port);
            }

            message.AddUInt16(PremiumDaysLeft);
        }

        public override void CleanUp()
        {
            Characters = null;
        }
    }
}
