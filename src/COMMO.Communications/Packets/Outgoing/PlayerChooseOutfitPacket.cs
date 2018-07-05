// <copyright file="PlayerChooseOutfitPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Communications.Packets.Outgoing
{
    using COMMO.Server.Data;
    using COMMO.Server.Data.Interfaces;
    using COMMO.Server.Data.Models.Structs;

    public class PlayerChooseOutfitPacket : PacketOutgoing
    {
        public Outfit CurrentOutfit { get; set; }

        public ushort ChooseFromId { get; set; }

        public ushort ChooseToId { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.OutfitWindow;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);
            message.AddUInt16(CurrentOutfit.Id);

            if (CurrentOutfit.Id != 0)
            {
                message.AddByte(CurrentOutfit.Head);
                message.AddByte(CurrentOutfit.Body);
                message.AddByte(CurrentOutfit.Legs);
                message.AddByte(CurrentOutfit.Feet);
            }
            else
            {
                message.AddUInt16(CurrentOutfit.LikeType);
            }

            message.AddUInt16(ChooseFromId);
            message.AddUInt16(ChooseToId);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
