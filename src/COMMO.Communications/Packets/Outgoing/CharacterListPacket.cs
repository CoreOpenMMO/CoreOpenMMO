// <copyright file="CharacterListPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using COMMO.Configuration;
using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Outgoing
{
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
            //message.AddByte((byte)Characters.Count());

			//byte charmax = 0xff;
			//var size = (byte) Math.Min(charmax, account.Characters.Count);

			var serverName = "COTS";
			var serverIp = "127.0.0.1";

			ushort gamePort = 7172;

			if (ServiceConfiguration.GetConfiguration().ReceivedClientVersionInt > 1010) {
				message.AddByte(1); // number of worlds
				message.AddByte(0); // world id
				message.AddString(serverName);
				message.AddString(serverIp);
				message.AddUInt16(gamePort);
				message.AddByte(0);

				message.AddByte((byte) Characters.Count());

				foreach (var character in Characters) {
					message.AddString(character.Name);
					message.AddString(character.World);
					message.AddBytes(character.Ip);
					message.AddUInt16(character.Port);
				}

				var frepremium = true;

				//Add premium days
				message.AddByte(0);
				if (frepremium) {
					message.AddByte(1);
					message.AddUInt32(0);
				}
				else {
					message.AddByte(PremiumDaysLeft > 0 ? (byte) 1 : (byte) 0);
					message.AddUInt32((uint) (DateTime.Now.Ticks + (PremiumDaysLeft * 86400)));
				}
			}
			else {

				foreach (var character in Characters) {
					message.AddString(character.Name);
					message.AddString(character.World);
					message.AddBytes(character.Ip);
					message.AddUInt16(character.Port);
				}

				message.AddUInt16(PremiumDaysLeft);
			}

        }

		public override void CleanUp() => Characters = null;
	}
}
