// <copyright file="Player.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models;
    using OpenTibia.Server.Data.Models.Structs;
    using OpenTibia.Server.Notifications;

    public class Player : Creature, IPlayer
    {
        const int KnownCreatureLimit = 100; // TODO: not sure of the number for this version... debugs will tell :|

        public override bool CanBeMoved => this.AccessLevel == 0;

        public override string InspectionText => this.Name;

        public override string CloseInspectionText => this.InspectionText;

        public ushort Level => this.Skills[SkillType.Level].Level;

        public byte LevelPercent => (byte)Math.Min(100, this.Skills[SkillType.Level].Count * 100 / (this.Skills[SkillType.Level].Target + 1));

        public uint Experience => (uint)this.Skills[SkillType.Level].Count;

        public byte AccessLevel { get; set; } // TODO: implement.

        public byte SoulPoints { get; } // TODO: nobody likes soulpoints... figure out what to do with them :)

        public IAction PendingAction { get; private set; }

        public bool CanLogout => this.AutoAttackTargetId == 0;

        private Dictionary<uint, long> KnownCreatures { get; }

        private Dictionary<string, bool> VipList { get; }

        public Location LocationInFront
        {
            get
            {
                switch (this.Direction)
                {
                    case Direction.North:
                        return new Location
                        {
                            X = this.Location.X,
                            Y = this.Location.Y - 1,
                            Z = this.Location.Z
                        };
                    case Direction.East:
                        return new Location
                        {
                            X = this.Location.X + 1,
                            Y = this.Location.Y,
                            Z = this.Location.Z
                        };
                    case Direction.West:
                        return new Location
                        {
                            X = this.Location.X - 1,
                            Y = this.Location.Y,
                            Z = this.Location.Z
                        };
                    case Direction.South:
                        return new Location
                        {
                            X = this.Location.X,
                            Y = this.Location.Y + 1,
                            Z = this.Location.Z
                        };
                    default:
                        return this.Location; // should not happen.
                }
            }
        }

        public override ushort AttackPower => this.Inventory.TotalAttack;

        public override ushort ArmorRating => this.Inventory.TotalArmor;

        public override ushort DefensePower => this.Inventory.TotalDefense;

        public override byte AutoAttackRange => Math.Max((byte)1, this.Inventory.AttackRange);

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
            this.Speed = 420;

            this.Outfit = new Outfit
            {
                Id = 130,
                Head = 114,
                Body = 114,
                Legs = 114,
                Feet = 114
            };

            this.LightBrightness = (byte)LightLevels.Torch;
            this.LightColor = (byte)LightColors.Orange;
            this.CarryStrength = 150;

            this.SoulPoints = 0;

            this.Skills[SkillType.Level] = new Skill(SkillType.Level, 1, 1.0, 10, 1, 150);
            this.Skills[SkillType.Magic] = new Skill(SkillType.Magic, 1, 1.0, 10, 1, 150);
            this.Skills[SkillType.Fist] = new Skill(SkillType.Fist, 10, 1.0, 10, 10, 150);
            this.Skills[SkillType.Axe] = new Skill(SkillType.Axe, 10, 1.0, 10, 10, 150);
            this.Skills[SkillType.Club] = new Skill(SkillType.Club, 10, 1.0, 10, 10, 150);
            this.Skills[SkillType.Sword] = new Skill(SkillType.Sword, 10, 1.0, 10, 10, 150);
            this.Skills[SkillType.Shield] = new Skill(SkillType.Shield, 10, 1.0, 10, 10, 150);
            this.Skills[SkillType.Ranged] = new Skill(SkillType.Ranged, 10, 1.0, 10, 10, 150);
            this.Skills[SkillType.Fishing] = new Skill(SkillType.Fishing, 10, 1.0, 10, 10, 150);

            this.OpenContainers = new IContainer[MaxContainers];
            this.Inventory = new PlayerInventory(this);
            this.KnownCreatures = new Dictionary<uint, long>();
            this.VipList = new Dictionary<string, bool>();

            this.OnThingChanged += this.CheckInventoryContainerProximity;
        }

        // ~PlayerId()
        // {
        //    OnLocationChanged -= CheckInventoryContainerProximity;
        // }
        public byte GetSkillInfo(SkillType skill)
        {
            return (byte)this.Skills[skill].Level;
        }

        public byte GetSkillPercent(SkillType skill)
        {
            return (byte)Math.Min(100, this.Skills[skill].Count * 100 / (this.Skills[skill].Target + 1));
        }

        public bool KnowsCreatureWithId(uint creatureId)
        {
            return this.KnownCreatures.ContainsKey(creatureId);
        }

        public void AddKnownCreature(uint creatureId)
        {
            try
            {
                this.KnownCreatures[creatureId] = DateTime.Now.Ticks;
            }
            catch
            {
                // happens when 2 try to add at the same time, which we don't care about.
            }
        }

        public uint ChooseToRemoveFromKnownSet()
        {
            // if the buffer is full we need to choose a vitim.
            while (this.KnownCreatures.Count == KnownCreatureLimit)
            {
                foreach (var candidate in this.KnownCreatures.OrderBy(kvp => kvp.Value).ToList()) // .ToList() prevents modifiying an enumerating collection in the rare case we hit an exception down there.
                {
                    try
                    {
                        if (this.KnownCreatures.Remove(candidate.Key))
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
            this.Outfit = outfit;
        }

        public void SetPendingAction(IAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            this.PendingAction = action;
        }

        public void ClearPendingActions()
        {
            this.PendingAction = null;
        }

        protected override void CheckPendingActions(IThing thingChanged, ThingStateChangedEventArgs eventArgs)
        {
            if (this.PendingAction == null || thingChanged != this || eventArgs.PropertyChanged != nameof(this.Location))
            {
                return;
            }

            if (this.Location == this.PendingAction.RetryLocation)
            {
                Task.Delay(this.CalculateRemainingCooldownTime(CooldownType.Action, DateTime.Now) + TimeSpan.FromMilliseconds(500))
                    .ContinueWith(previous =>
                    {
                        this.PendingAction.Perform();
                    });
            }
        }

        public sbyte GetContainerId(IContainer thingAsContainer)
        {
            for (sbyte i = 0; i < this.OpenContainers.Length; i++)
            {
                if (this.OpenContainers[i] == thingAsContainer)
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
                this.OpenContainers[openContainerId].Close(this.CreatureId);
                this.OpenContainers[openContainerId] = null;
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

            for (byte i = 0; i < this.OpenContainers.Length; i++)
            {
                if (this.OpenContainers[i] != null)
                {
                    continue;
                }

                this.OpenContainers[i] = container;
                this.OpenContainers[i].Open(this.CreatureId, i);

                return (sbyte)i;
            }

            var lastIdx = (sbyte)(this.OpenContainers.Length - 1);

            this.OpenContainers[lastIdx] = container;

            return lastIdx;
        }

        public void OpenContainerAt(IContainer thingAsContainer, byte index)
        {
            this.OpenContainers[index]?.Close(this.CreatureId);
            this.OpenContainers[index] = thingAsContainer;
            this.OpenContainers[index].Open(this.CreatureId, index);
        }

        public IContainer GetContainer(byte index)
        {
            try
            {
                var container = this.OpenContainers[index];

                container.Open(this.CreatureId, index);

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
            for (byte i = 0; i < this.OpenContainers.Length; i++)
            {
                if (this.OpenContainers[i] == null)
                {
                    continue;
                }

                var containerSourceLoc = this.OpenContainers[i].Location;

                switch (containerSourceLoc.Type)
                {
                    case LocationType.Ground:
                        var locDiff = this.Location - containerSourceLoc;

                        if (locDiff.MaxValueIn2D > 1)
                        {
                            var container = this.GetContainer(i);
                            this.CloseContainerWithId(i);

                            if (container != null)
                            {
                                container.OnThingChanged -= this.CheckInventoryContainerProximity;
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
