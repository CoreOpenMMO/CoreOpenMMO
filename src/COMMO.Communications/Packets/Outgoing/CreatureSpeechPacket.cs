// <copyright file="CreatureSpeechPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Communications.Packets.Outgoing
{
    using COMMO.Data.Contracts;
    using COMMO.Server.Data;
    using COMMO.Server.Data.Interfaces;
    using COMMO.Server.Data.Models.Structs;

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
            message.AddByte(PacketType);
            message.AddUInt32(0);
            message.AddString(SenderName);
            message.AddByte((byte)SpeechType);

            switch (SpeechType)
            {
                case SpeechType.Say:
                case SpeechType.Whisper:
                case SpeechType.Yell:
                case SpeechType.MonsterSay:
                // case SpeechType.MonsterYell:
                    message.AddLocation(Location);
                    break;
                // case SpeechType.ChannelRed:
                // case SpeechType.ChannelRedAnonymous:
                // case SpeechType.ChannelOrange:
                case SpeechType.ChannelYellow:
                // case SpeechType.ChannelWhite:
                    message.AddUInt16((ushort)ChannelId);
                    break;
                case SpeechType.RuleViolationReport:
                    message.AddUInt32(Time);
                    break;
                default:
                    break;

            }

            message.AddString(Text);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
