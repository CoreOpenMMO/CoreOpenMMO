// <copyright file="CreatureMovedNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using COMMO.Communications;
using COMMO.Communications.Packets.Outgoing;
using COMMO.Server.Data.Interfaces;
using COMMO.Server.Data.Models.Structs;

namespace COMMO.Server.Notifications
{
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

            CreatureId = creatureId;
            OldLocation = fromLocation;
            OldStackPosition = fromStackPos;
            NewLocation = toLocation;
            NewStackPosition = toStackPos;
            WasTeleport = wasTeleport || locationDiff.MaxValueIn3D > 1;
        }

        public override void Prepare()
        {
            var player = Game.Instance.GetCreatureWithId(Connection.PlayerId) as IPlayer;

            if (player == null)
            {
                return;
            }

            var creature = Game.Instance.GetCreatureWithId(CreatureId);

            if (CreatureId == Connection.PlayerId)
            {
                if (WasTeleport) // TODO: revise; this had a contradicting condition on the source (< 10 vs >= 10)
                {
                    if (OldStackPosition < 10)
                    {
                        ResponsePackets.Add(new RemoveAtStackposPacket
                        {
                            Location = OldLocation,
                            Stackpos = OldStackPosition
                        });
                    }

                    ResponsePackets.Add(new MapDescriptionPacket
                    {
                       Origin = NewLocation,
                       DescriptionBytes = Game.Instance.GetMapDescriptionAt(player, NewLocation)
                    });
                }
                else
                {
                    if (OldLocation.Z == 7 && NewLocation.Z > 7)
                    {
                        if (OldStackPosition < 10)
                        {
                            ResponsePackets.Add(new RemoveAtStackposPacket
                            {
                                Location = OldLocation,
                                Stackpos = OldStackPosition
                            });
                        }
                    }
                    else
                    {
                        ResponsePackets.Add(new CreatureMovedPacket
                        {
                            FromLocation = OldLocation,
                            FromStackpos = OldStackPosition,
                            ToLocation = NewLocation
                        });
                    }

                    // floor change down
                    if (NewLocation.Z > OldLocation.Z)
                    {
                        // going from surface to underground
                        if (NewLocation.Z == 8)
                        {
                            ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.FloorChangeDown)
                            {
                                DescriptionBytes = Game.Instance.GetMapFloorsDescription(player, (ushort)(OldLocation.X - 8), (ushort)(OldLocation.Y - 6), NewLocation.Z, (byte)(NewLocation.Z + 2), 18, 14, -1)
                            });
                        }

                        // going further down
                        else if (NewLocation.Z > OldLocation.Z && NewLocation.Z > 8 && NewLocation.Z < 14)
                        {
                            ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.FloorChangeDown)
                            {
                                DescriptionBytes = Game.Instance.GetMapFloorsDescription(player, (ushort)(OldLocation.X - 8), (ushort)(OldLocation.Y - 6), (byte)(NewLocation.Z + 2), (byte)(NewLocation.Z + 2), 18, 14, -3)
                            });
                        }
                        else
                        {
                            ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.FloorChangeDown)
                            {
                                DescriptionBytes = new byte[0] // no description needed.
                            });
                        }

                        // moving down a floor makes us out of sync, include east and south
                        ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.MapSliceEast)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(OldLocation.X + 9), (ushort)(OldLocation.Y - 7), NewLocation.Z, NewLocation.IsUnderground, 1, 14)
                        });

                        // south
                        ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.MapSliceSouth)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(OldLocation.X - 8), (ushort)(OldLocation.Y + 7), NewLocation.Z, NewLocation.IsUnderground, 18, 1)
                        });
                    }

                    // floor change up
                    else if (NewLocation.Z < OldLocation.Z)
                    {
                        // going to surface
                        if (NewLocation.Z == 7)
                        {
                            ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.FloorChangeUp)
                            {
                                DescriptionBytes = Game.Instance.GetMapFloorsDescription(player, (ushort)(OldLocation.X - 8), (ushort)(OldLocation.Y - 6), 5, 0, 18, 14, 3)
                            });
                        }

                        // underground, going one floor up (still underground)
                        else if (NewLocation.Z > 7)
                        {
                            ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.FloorChangeUp)
                            {
                                DescriptionBytes = Game.Instance.GetMapFloorsDescription(player, (ushort)(OldLocation.X - 8), (ushort)(OldLocation.Y - 6), (byte)(OldLocation.Z - 3), (byte)(OldLocation.Z - 3), 18, 14, 3)
                            });
                        }
                        else
                        {
                            ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.FloorChangeUp)
                            {
                                DescriptionBytes = new byte[0] // no description needed.
                            });
                        }

                        // moving up a floor up makes us out of sync, include west and north
                        ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.MapSliceWest)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(OldLocation.X - 8), (ushort)(OldLocation.Y - 5), NewLocation.Z, NewLocation.IsUnderground, 1, 14)
                        });

                        // north
                        ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.MapSliceNorth)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(OldLocation.X - 8), (ushort)(OldLocation.Y - 6), NewLocation.Z, NewLocation.IsUnderground, 18, 1)
                        });
                    }

                    if (OldLocation.Y > NewLocation.Y)
                    {
                        // north, for old x
                        ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.MapSliceNorth)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(OldLocation.X - 8), (ushort)(NewLocation.Y - 6), NewLocation.Z, NewLocation.IsUnderground, 18, 1)
                        });
                    }
                    else if (OldLocation.Y < NewLocation.Y)
                    {
                        // south, for old x
                        ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.MapSliceSouth)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(OldLocation.X - 8), (ushort)(NewLocation.Y + 7), NewLocation.Z, NewLocation.IsUnderground, 18, 1)
                        });
                    }

                    if (OldLocation.X < NewLocation.X)
                    {
                        // east, [with new y]
                        ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.MapSliceEast)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(NewLocation.X + 9), (ushort)(NewLocation.Y - 6), NewLocation.Z, NewLocation.IsUnderground, 1, 14)
                        });
                    }
                    else if (OldLocation.X > NewLocation.X)
                    {
                        // west, [with new y]
                        ResponsePackets.Add(new MapPartialDescriptionPacket(GameOutgoingPacketType.MapSliceWest)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(NewLocation.X - 8), (ushort)(NewLocation.Y - 6), NewLocation.Z, NewLocation.IsUnderground, 1, 14)
                        });
                    }
                }
            }
            else if (player.CanSee(OldLocation) && player.CanSee(NewLocation))
            {
                if (player.CanSee(creature))
                {
                    if (WasTeleport || OldLocation.Z == 7 && NewLocation.Z > 7 || OldStackPosition > 9)
                    {
                        if (OldStackPosition < 10)
                        {
                            ResponsePackets.Add(new RemoveAtStackposPacket
                            {
                                Location = OldLocation,
                                Stackpos = OldStackPosition
                            });
                        }

                        ResponsePackets.Add(new AddCreaturePacket
                        {
                            Location = NewLocation,
                            Creature = creature,
                            AsKnown = player.KnowsCreatureWithId(CreatureId),
                            RemoveThisCreatureId = player.ChooseToRemoveFromKnownSet() // chooses a victim if neeeded.
                        });
                    }
                    else
                    {
                        ResponsePackets.Add(new CreatureMovedPacket
                        {
                            FromLocation = OldLocation,
                            FromStackpos = OldStackPosition,
                            ToLocation = NewLocation
                        });
                    }
                }
            }
            else if (player.CanSee(OldLocation) && player.CanSee(creature))
            {
                if (OldStackPosition < 10)
                {
                    ResponsePackets.Add(new RemoveAtStackposPacket
                    {
                        Location = OldLocation,
                        Stackpos = OldStackPosition
                    });
                }
            }
            else if (player.CanSee(NewLocation) && player.CanSee(creature))
            {
                ResponsePackets.Add(new AddCreaturePacket
                {
                    Location = NewLocation,
                    Creature = creature,
                    AsKnown = player.KnowsCreatureWithId(CreatureId),
                    RemoveThisCreatureId = player.ChooseToRemoveFromKnownSet() // chooses a victim if neeeded.
                });
            }

            // if (WasTeleport)
            // {
            //    ResponsePackets.Add(new MagicEffectPacket()
            //    {
            //        Location = NewLocation,
            //        Effect = Effect_t.BubbleBlue
            //    });
            // }
        }
    }
}