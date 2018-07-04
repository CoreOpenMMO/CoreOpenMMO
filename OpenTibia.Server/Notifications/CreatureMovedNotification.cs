// <copyright file="CreatureMovedNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using System;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    internal class CreatureMovedNotification : Notification
    {
        public bool WasTeleport { get; }

        public byte OldStackPosition { get; }

        public byte NewStackPosition { get; }

        public Location OldLocation { get; }

        public Location NewLocation { get; }

        public uint CreatureId { get; }

        public CreatureMovedNotification(Connection connection, uint creatureId, Location fromLocation, byte fromStackPos, Location toLocation, byte toStackPos, bool wasTeleport)
            : base(connection)
        {
            var locationDiff = fromLocation - toLocation;

            this.CreatureId = creatureId;
            this.OldLocation = fromLocation;
            this.OldStackPosition = fromStackPos;
            this.NewLocation = toLocation;
            this.NewStackPosition = toStackPos;
            this.WasTeleport = wasTeleport || locationDiff.MaxValueIn3D > 1;
        }

        public override void Prepare()
        {
            var player = Game.Instance.GetCreatureWithId(this.Connection.PlayerId) as IPlayer;

            if (player == null)
            {
                return;
            }

            var creature = Game.Instance.GetCreatureWithId(this.CreatureId);

            if (this.CreatureId == this.Connection.PlayerId)
            {
                if (this.WasTeleport) // TODO: revise; this had a contradicting condition on the source (< 10 vs >= 10)
                {
                    if (this.OldStackPosition < 10)
                    {
                        this.ResponsePackets.Add(new RemoveAtStackposPacket
                        {
                            Location = this.OldLocation,
                            Stackpos = this.OldStackPosition
                        });
                    }

                    this.ResponsePackets.Add(new MapDescriptionPacket
                    {
                       Origin = this.NewLocation,
                       DescriptionBytes = Game.Instance.GetMapDescriptionAt(player, this.NewLocation)
                    });
                }
                else
                {
                    if (this.OldLocation.Z == 7 && this.NewLocation.Z > 7)
                    {
                        if (this.OldStackPosition < 10)
                        {
                            this.ResponsePackets.Add(new RemoveAtStackposPacket
                            {
                                Location = this.OldLocation,
                                Stackpos = this.OldStackPosition
                            });
                        }
                    }
                    else
                    {
                        this.ResponsePackets.Add(new CreatureMovedPacket
                        {
                            FromLocation = this.OldLocation,
                            FromStackpos = this.OldStackPosition,
                            ToLocation = this.NewLocation
                        });
                    }

                    // floor change down
                    if (this.NewLocation.Z > this.OldLocation.Z)
                    {
                        // going from surface to underground
                        if (this.NewLocation.Z == 8)
                        {
                            this.ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.FloorChangeDown)
                            {
                                DescriptionBytes = Game.Instance.GetMapFloorsDescription(player, (ushort)(this.OldLocation.X - 8), (ushort)(this.OldLocation.Y - 6), this.NewLocation.Z, (byte)(this.NewLocation.Z + 2), 18, 14, -1)
                            });
                        }

                        // going further down
                        else if (this.NewLocation.Z > this.OldLocation.Z && this.NewLocation.Z > 8 && this.NewLocation.Z < 14)
                        {
                            this.ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.FloorChangeDown)
                            {
                                DescriptionBytes = Game.Instance.GetMapFloorsDescription(player, (ushort)(this.OldLocation.X - 8), (ushort)(this.OldLocation.Y - 6), (byte)(this.NewLocation.Z + 2), (byte)(this.NewLocation.Z + 2), 18, 14, -3)
                            });
                        }
                        else
                        {
                            this.ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.FloorChangeDown)
                            {
                                DescriptionBytes = new byte[0] // no description needed.
                            });
                        }

                        // moving down a floor makes us out of sync, include east and south
                        this.ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.MapSliceEast)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(this.OldLocation.X + 9), (ushort)(this.OldLocation.Y - 7), this.NewLocation.Z, this.NewLocation.IsUnderground, 1, 14)
                        });

                        // south
                        this.ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.MapSliceSouth)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(this.OldLocation.X - 8), (ushort)(this.OldLocation.Y + 7), this.NewLocation.Z, this.NewLocation.IsUnderground, 18, 1)
                        });
                    }

                    // floor change up
                    else if (this.NewLocation.Z < this.OldLocation.Z)
                    {
                        // going to surface
                        if (this.NewLocation.Z == 7)
                        {
                            this.ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.FloorChangeUp)
                            {
                                DescriptionBytes = Game.Instance.GetMapFloorsDescription(player, (ushort)(this.OldLocation.X - 8), (ushort)(this.OldLocation.Y - 6), 5, 0, 18, 14, 3)
                            });
                        }

                        // underground, going one floor up (still underground)
                        else if (this.NewLocation.Z > 7)
                        {
                            this.ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.FloorChangeUp)
                            {
                                DescriptionBytes = Game.Instance.GetMapFloorsDescription(player, (ushort)(this.OldLocation.X - 8), (ushort)(this.OldLocation.Y - 6), (byte)(this.OldLocation.Z - 3), (byte)(this.OldLocation.Z - 3), 18, 14, 3)
                            });
                        }
                        else
                        {
                            this.ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.FloorChangeUp)
                            {
                                DescriptionBytes = new byte[0] // no description needed.
                            });
                        }

                        // moving up a floor up makes us out of sync, include west and north
                        this.ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.MapSliceWest)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(this.OldLocation.X - 8), (ushort)(this.OldLocation.Y - 5), this.NewLocation.Z, this.NewLocation.IsUnderground, 1, 14)
                        });

                        // north
                        this.ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.MapSliceNorth)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(this.OldLocation.X - 8), (ushort)(this.OldLocation.Y - 6), this.NewLocation.Z, this.NewLocation.IsUnderground, 18, 1)
                        });
                    }

                    if (this.OldLocation.Y > this.NewLocation.Y)
                    {
                        // north, for old x
                        this.ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.MapSliceNorth)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(this.OldLocation.X - 8), (ushort)(this.NewLocation.Y - 6), this.NewLocation.Z, this.NewLocation.IsUnderground, 18, 1)
                        });
                    }
                    else if (this.OldLocation.Y < this.NewLocation.Y)
                    {
                        // south, for old x
                        this.ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.MapSliceSouth)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(this.OldLocation.X - 8), (ushort)(this.NewLocation.Y + 7), this.NewLocation.Z, this.NewLocation.IsUnderground, 18, 1)
                        });
                    }

                    if (this.OldLocation.X < this.NewLocation.X)
                    {
                        // east, [with new y]
                        this.ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.MapSliceEast)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(this.NewLocation.X + 9), (ushort)(this.NewLocation.Y - 6), this.NewLocation.Z, this.NewLocation.IsUnderground, 1, 14)
                        });
                    }
                    else if (this.OldLocation.X > this.NewLocation.X)
                    {
                        // west, [with new y]
                        this.ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.MapSliceWest)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(this.NewLocation.X - 8), (ushort)(this.NewLocation.Y - 6), this.NewLocation.Z, this.NewLocation.IsUnderground, 1, 14)
                        });
                    }
                }
            }
            else if (player.CanSee(this.OldLocation) && player.CanSee(this.NewLocation))
            {
                if (player.CanSee(creature))
                {
                    if (this.WasTeleport || this.OldLocation.Z == 7 && this.NewLocation.Z > 7 || this.OldStackPosition > 9)
                    {
                        if (this.OldStackPosition < 10)
                        {
                            this.ResponsePackets.Add(new RemoveAtStackposPacket
                            {
                                Location = this.OldLocation,
                                Stackpos = this.OldStackPosition
                            });
                        }

                        this.ResponsePackets.Add(new AddCreaturePacket
                        {
                            Location = this.NewLocation,
                            Creature = creature,
                            AsKnown = player.KnowsCreatureWithId(this.CreatureId),
                            RemoveThisCreatureId = player.ChooseToRemoveFromKnownSet() // chooses a victim if neeeded.
                        });
                    }
                    else
                    {
                        this.ResponsePackets.Add(new CreatureMovedPacket
                        {
                            FromLocation = this.OldLocation,
                            FromStackpos = this.OldStackPosition,
                            ToLocation = this.NewLocation
                        });
                    }
                }
            }
            else if (player.CanSee(this.OldLocation) && player.CanSee(creature))
            {
                if (this.OldStackPosition < 10)
                {
                    this.ResponsePackets.Add(new RemoveAtStackposPacket
                    {
                        Location = this.OldLocation,
                        Stackpos = this.OldStackPosition
                    });
                }
            }
            else if (player.CanSee(this.NewLocation) && player.CanSee(creature))
            {
                this.ResponsePackets.Add(new AddCreaturePacket
                {
                    Location = this.NewLocation,
                    Creature = creature,
                    AsKnown = player.KnowsCreatureWithId(this.CreatureId),
                    RemoveThisCreatureId = player.ChooseToRemoveFromKnownSet() // chooses a victim if neeeded.
                });
            }

            // if (this.WasTeleport)
            // {
            //    this.ResponsePackets.Add(new MagicEffectPacket()
            //    {
            //        Location = this.NewLocation,
            //        Effect = Effect_t.BubbleBlue
            //    });
            // }
        }
    }
}