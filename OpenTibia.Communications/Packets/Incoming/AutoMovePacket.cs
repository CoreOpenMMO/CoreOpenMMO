// <copyright file="AutoMovePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using System;
    using System.IO;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    /// <summary>
    /// Class that represents an auto movement packet.
    /// </summary>
    public class AutoMovePacket : IPacketIncoming, IAutoMoveInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMovePacket"/> class.
        /// </summary>
        /// <param name="message">The message to parse the packet from.</param>
        public AutoMovePacket(NetworkMessage message)
        {
            try
            {
                var numberOfMovements = message.GetByte();

                this.Directions = new Direction[numberOfMovements];

                for (var i = 0; i < numberOfMovements; i++)
                {
                    var dir = message.GetByte();
                    switch (dir)
                    {
                        case 1:
                            this.Directions[i] = Direction.East;
                            break;
                        case 2:
                            this.Directions[i] = Direction.NorthEast;
                            break;
                        case 3:
                            this.Directions[i] = Direction.North;
                            break;
                        case 4:
                            this.Directions[i] = Direction.NorthWest;
                            break;
                        case 5:
                            this.Directions[i] = Direction.West;
                            break;
                        case 6:
                            this.Directions[i] = Direction.SouthWest;
                            break;
                        case 7:
                            this.Directions[i] = Direction.South;
                            break;
                        case 8:
                            this.Directions[i] = Direction.SouthEast;
                            break;
                        default:
                            throw new InvalidDataException($"Invalid direction value {dir} on {nameof(AutoMovePacket)}.");
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: proper logging
                Console.WriteLine(ex.ToString());
            }
        }

        /// <inheritdoc/>
        public Direction[] Directions { get; }
    }
}
