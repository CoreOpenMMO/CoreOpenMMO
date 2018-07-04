﻿// <copyright file="FinishAuctionsPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using System;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class FinishAuctionsPacket : IPacketIncoming, IFinishAuctionsInfo
    {
        public FinishAuctionsPacket(NetworkMessage message)
        {
            throw new NotImplementedException();
        }
    }
}