// <copyright file="CreatureSpeechPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    public class CreatureSpeechPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.CreatureSpeech;

        public string SenderName { get; set; }

        public SpeechType SpeechType { get; set; }

        public string Text { get; set; }

        public Location Location { get; set; }

        public ChatChannel ChannelId { get; set; }

        public uint Time { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);
            message.AddUInt32(0);
            message.AddString(this.SenderName);
            message.AddByte((byte)this.SpeechType);

            switch (this.SpeechType)
            {
                case SpeechType.Say:
                case SpeechType.Whisper:
                case SpeechType.Yell:
                case SpeechType.MonsterSay:
                // case SpeechType.MonsterYell:
                    message.AddLocation(this.Location);
                    break;
                // case SpeechType.ChannelRed:
                // case SpeechType.ChannelRedAnonymous:
                // case SpeechType.ChannelOrange:
                case SpeechType.ChannelYellow:
                // case SpeechType.ChannelWhite:
                    message.AddUInt16((ushort)this.ChannelId);
                    break;
                case SpeechType.RuleViolationReport:
                    message.AddUInt32(this.Time);
                    break;
                default:
                    break;

            }

            message.AddString(this.Text);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
