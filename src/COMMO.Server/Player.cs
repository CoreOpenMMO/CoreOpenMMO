// <copyright file="Player.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COMMO.Communications.Packets.Outgoing;
using COMMO.Data.Contracts;
using COMMO.Server.Data.Interfaces;
using COMMO.Server.Data.Models;
using COMMO.Server.Data.Models.Structs;
using COMMO.Server.Notifications;

namespace COMMO.Server
{
    public class Player : Creature, IPlayer
    {
        const int KnownCreatureLimit = 100; // TODO: not sure of the number for this version... debugs will tell :|

        public override bool CanBeMoved => AccessLevel == 0;

        public override string InspectionText => Name;

        public override string CloseInspectionText => InspectionText;

        public ushort Level => Skills[SkillType.Level].Level;

        public byte LevelPercent => (byte)Math.Min(100, Skills[SkillType.Level].Count * 100 / (Skills[SkillType.Level].Target + 1));

        public uint Experience => (uint)Skills[SkillType.Level].Count;

        public byte AccessLevel { get; set; } // TODO: implement.

        public byte SoulPoints { get; } // TODO: nobody likes soulpoints... figure out what to do with them :)

        public IAction PendingAction { get; private set; }

        public bool CanLogout => AutoAttackTargetId == 0;

        private Dictionary<uint, long> KnownCreatures { get; }

        private Dictionary<string, bool> VipList { get; }

        public Location LocationInFront
        {
            get
            {
                switch (Direction)
                {
                    case Direction.North:
                        return new Location
                        {
                            X = Location.X,
                            Y = Location.Y - 1,
                            Z = Location.Z
                        };
                    case Direction.East:
                        return new Location
                        {
                            X = Location.X + 1,
                            Y = Location.Y,
                            Z = Location.Z
                        };
                    case Direction.West:
                        return new Location
                        {
                            X = Location.X - 1,
                            Y = Location.Y,
                            Z = Location.Z
                        };
                    case Direction.South:
                        return new Location
                        {
                            X = Location.X,
                            Y = Location.Y + 1,
                            Z = Location.Z
                        };
                    default:
                        return Location; // should not happen.
                }
            }
        }

        public override ushort AttackPower => Inventory.TotalAttack;

        public override ushort ArmorRating => Inventory.TotalArmor;

        public override ushort DefensePower => Inventory.TotalDefense;

        public override byte AutoAttackRange => Math.Max((byte)1, Inventory.AttackRange);

        public sealed override IInventory Inventory { get; protected set; }

        private IContainer[] OpenContainers { get; }

        private const sbyte MaxContainers = 16;

        public Player(
            uint id,
            string name,
            ushort maxHitpoints,
            ushort maxManapoints,
            ushort corpse,
            ushort hitpoints = 0,
            ushort manapoints = 0)
            : base(id, name, string.Empty, maxHitpoints, maxManapoints, corpse, hitpoints, manapoints)
        {
            // TODO: implement, Temp values
            Speed = 420;

            Outfit = new Outfit
            {
                Id = 130,
                Head = 114,
                Body = 114,
                Legs = 114,
                Feet = 114
            };

            LightBrightness = (byte)LightLevels.Torch;
            LightColor = (byte)LightColors.Orange;
            CarryStrength = 150;

            SoulPoints = 0;

            Skills[SkillType.Level] = new Skill(SkillType.Level, 1, 1.0, 10, 1, 150);
            Skills[SkillType.Magic] = new Skill(SkillType.Magic, 1, 1.0, 10, 1, 150);
            Skills[SkillType.Fist] = new Skill(SkillType.Fist, 10, 1.0, 10, 10, 150);
            Skills[SkillType.Axe] = new Skill(SkillType.Axe, 10, 1.0, 10, 10, 150);
            Skills[SkillType.Club] = new Skill(SkillType.Club, 10, 1.0, 10, 10, 150);
            Skills[SkillType.Sword] = new Skill(SkillType.Sword, 10, 1.0, 10, 10, 150);
            Skills[SkillType.Shield] = new Skill(SkillType.Shield, 10, 1.0, 10, 10, 150);
            Skills[SkillType.Ranged] = new Skill(SkillType.Ranged, 10, 1.0, 10, 10, 150);
            Skills[SkillType.Fishing] = new Skill(SkillType.Fishing, 10, 1.0, 10, 10, 150);

            OpenContainers = new IContainer[MaxContainers];
            Inventory = new PlayerInventory(this);
            KnownCreatures = new Dictionary<uint, long>();
            VipList = new Dictionary<string, bool>();

            OnThingChanged += CheckInventoryContainerProximity;
        }

        // ~PlayerId()
        // {
        //    OnLocationChanged -= CheckInventoryContainerProximity;
        // }
        public byte GetSkillInfo(SkillType skill)
        {
            return (byte)Skills[skill].Level;
        }

        public byte GetSkillPercent(SkillType skill)
        {
            return (byte)Math.Min(100, Skills[skill].Count * 100 / (Skills[skill].Target + 1));
        }

        public bool KnowsCreatureWithId(uint creatureId)
        {
            return KnownCreatures.ContainsKey(creatureId);
        }

        public void AddKnownCreature(uint creatureId)
        {
            try
            {
                KnownCreatures[creatureId] = DateTime.Now.Ticks;
            }
            catch
            {
                // happens when 2 try to add at the same time, which we don't care about.
            }
        }

        public uint ChooseToRemoveFromKnownSet()
        {
            // if the buffer is full we need to choose a vitim.
            while (KnownCreatures.Count == KnownCreatureLimit)
            {
                foreach (var candidate in KnownCreatures.OrderBy(kvp => kvp.Value).ToList()) // .ToList() prevents modifiying an enumerating collection in the rare case we hit an exception down there.
                {
                    try
                    {
                        if (KnownCreatures.Remove(candidate.Key))
                        {
                            return candidate.Key;
                        }
                    }
                    catch
                    {
                        // happens when 2 try to remove time, which we don't care too much.
                    }
                }
            }

            return uint.MinValue; // 0
        }

        public void SetOutfit(Outfit outfit)
        {
            Outfit = outfit;
        }

        public void SetPendingAction(IAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            PendingAction = action;
        }

        public void ClearPendingActions()
        {
            PendingAction = null;
        }

        protected override void CheckPendingActions(IThing thingChanged, ThingStateChangedEventArgs eventArgs)
        {
            if (PendingAction == null || thingChanged != this || eventArgs.PropertyChanged != nameof(Location))
            {
                return;
            }

            if (Location == PendingAction.RetryLocation)
            {
                Task.Delay(CalculateRemainingCooldownTime(CooldownType.Action, DateTime.Now) + TimeSpan.FromMilliseconds(500))
                    .ContinueWith(previous =>
                    {
                        PendingAction.Perform();
                    });
            }
        }

        public sbyte GetContainerId(IContainer thingAsContainer)
        {
            for (sbyte i = 0; i < OpenContainers.Length; i++)
            {
                if (OpenContainers[i] == thingAsContainer)
                {
                    return i;
                }
            }

            return -1;
        }

        public void CloseContainerWithId(byte openContainerId)
        {
            try
            {
                OpenContainers[openContainerId].Close(CreatureId);
                OpenContainers[openContainerId] = null;
            }
            catch
            {
                // ignored
            }
        }

        public sbyte OpenContainer(IContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            for (byte i = 0; i < OpenContainers.Length; i++)
            {
                if (OpenContainers[i] != null)
                {
                    continue;
                }

                OpenContainers[i] = container;
                OpenContainers[i].Open(CreatureId, i);

                return (sbyte)i;
            }

            var lastIdx = (sbyte)(OpenContainers.Length - 1);

            OpenContainers[lastIdx] = container;

            return lastIdx;
        }

        public void OpenContainerAt(IContainer thingAsContainer, byte index)
        {
            OpenContainers[index]?.Close(CreatureId);
            OpenContainers[index] = thingAsContainer;
            OpenContainers[index].Open(CreatureId, index);
        }

        public IContainer GetContainer(byte index)
        {
            try
            {
                var container = OpenContainers[index];

                container.Open(CreatureId, index);

                return container;
            }
            catch
            {
                // ignored
            }

            return null;
        }

        public void CheckInventoryContainerProximity(IThing thingChanging, ThingStateChangedEventArgs eventArgs)
        {
            for (byte i = 0; i < OpenContainers.Length; i++)
            {
                if (OpenContainers[i] == null)
                {
                    continue;
                }

                var containerSourceLoc = OpenContainers[i].Location;

                switch (containerSourceLoc.Type)
                {
                    case LocationType.Ground:
                        var locDiff = Location - containerSourceLoc;

                        if (locDiff.MaxValueIn2D > 1)
                        {
                            var container = GetContainer(i);
                            CloseContainerWithId(i);

                            if (container != null)
                            {
                                container.OnThingChanged -= CheckInventoryContainerProximity;
                            }

                            var containerId = i;

                            Game.Instance.NotifySinglePlayer(this, conn => new GenericNotification(conn, new ContainerClosePacket { ContainerId = containerId }));
                        }

                        break;
                    case LocationType.Container:
                        break;
                    case LocationType.Slot:
                        break;
                }
            }
        }
    }
}
