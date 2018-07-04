// <copyright file="WorldConfigPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class WorldConfigPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)ManagementOutgoingPacketType.NoType;

        public byte DailyResetHour { get; set; }

        public byte[] IpAddressBytes { get; set; }

        public ushort MaximumRookgardians { get; set; }

        public ushort MaximumTotalPlayers { get; set; }

        public ushort Port { get; set; }

        public ushort PremiumMainlandBuffer { get; set; }

        public ushort PremiumRookgardiansBuffer { get; set; }

        public byte WorldType { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(0x00);

            message.AddByte(this.WorldType);
            message.AddByte(this.DailyResetHour);

            message.AddBytes(this.IpAddressBytes);
            message.AddUInt16(this.Port);

            message.AddUInt16(this.MaximumTotalPlayers);
            message.AddUInt16(this.PremiumMainlandBuffer);
            message.AddUInt16(this.MaximumRookgardians);
            message.AddUInt16(this.PremiumRookgardiansBuffer);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}