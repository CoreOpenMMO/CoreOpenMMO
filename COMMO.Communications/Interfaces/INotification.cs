// <copyright file="INotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Communications.Interfaces
{
    using System.Collections.Generic;
    using COMMO.Scheduling.Contracts;
    using COMMO.Server.Data.Interfaces;

    public interface INotification : IEvent
    {
        /// <summary>
        /// Gets the connection reference to where this notification will be sent.
        /// </summary>
        Connection Connection { get; }

        IList<IPacketOutgoing> ResponsePackets { get; }

        void Prepare();

        void Send();
    }
}
