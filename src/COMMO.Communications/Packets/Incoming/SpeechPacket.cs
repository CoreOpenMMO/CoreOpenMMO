// <copyright file="SpeechPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using COMMO.Data.Contracts;
using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;
using COMMO.Server.Data.Models.Structs;

namespace COMMO.Communications.Packets.Incoming
{
    public class SpeechPacket : IPacketIncoming
    {
        public SpeechPacket(NetworkMessage message)
        {
            var type = message.GetByte();

            try
            {
                switch (Speech.Type)
                {
                    case SpeechType.Private:
                    // case SpeechType.PrivateRed:
                    case SpeechType.RuleViolationAnswer:
						Speech = new Speech
                        {
                            Type = (SpeechType)type,
                            Receiver = message.GetString(),
                            Message = message.GetString()
                        };
                        break;
                    case SpeechType.ChannelYellow:
						// case SpeechType.ChannelRed:
						// case SpeechType.ChannelRedAnonymous:
						// case SpeechType.ChannelWhite:
						Speech = new Speech
                        {
                            Type = (SpeechType)type,
                            ChannelId = (ChatChannel)message.GetUInt16(),
                            Message = message.GetString()
                        };
                        break;
                    default:
						Speech = new Speech
                        {
                            Type = (SpeechType)type,
                            Message = message.GetString()
                        };
                        break;
                }
            }
            catch
            {
                Console.WriteLine($"Unknown speech type {type}.");
            }
        }

        public Speech Speech { get; private set; }
    }
}
