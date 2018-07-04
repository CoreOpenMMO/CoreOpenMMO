// <copyright file="Creature.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    public abstract class Creature : Thing, ICreature, ICombatActor
    {
        private static readonly object IdLock = new object();
        private static uint idCounter = 1;

        private readonly object enqueueWalkLock;

        protected Creature(
            uint id,
            string name,
            string article,
            uint maxHitpoints,
            uint maxManapoints,
            ushort corpse = 0,
            ushort hitpoints = 0,
            ushort manapoints = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (maxHitpoints == 0)
            {
                throw new ArgumentException($"{nameof(maxHitpoints)} must be positive.");
            }

            this.enqueueWalkLock = new object();

            this.CreatureId = id;
            this.Name = name;
            this.Article = article;
            this.MaxHitpoints = maxHitpoints;
            this.Hitpoints = Math.Min(this.MaxHitpoints, hitpoints == 0 ? this.MaxHitpoints : hitpoints);
            this.MaxManapoints = maxManapoints;
            this.Manapoints = Math.Min(this.MaxManapoints, manapoints);
            this.Corpse = corpse;

            this.Cooldowns = new Dictionary<CooldownType, Tuple<DateTime, TimeSpan>>
            {
                { CooldownType.Move, new Tuple<DateTime, TimeSpan>(DateTime.Now, TimeSpan.Zero) },
                { CooldownType.Action, new Tuple<DateTime, TimeSpan>(DateTime.Now, TimeSpan.Zero) },
                { CooldownType.Combat, new Tuple<DateTime, TimeSpan>(DateTime.Now, TimeSpan.Zero) },
                { CooldownType.Talk, new Tuple<DateTime, TimeSpan>(DateTime.Now, TimeSpan.Zero) }
            };

            this.Outfit = new Outfit
            {
                Id = 0,
                LikeType = 0
            };

            this.Speed = 220;

            this.WalkingQueue = new ConcurrentQueue<Tuple<byte, Direction>>();

            // Subscribe any attack-impacting conditions here
            this.OnThingChanged += this.CheckAutoAttack;         // Are we in range with our target now/still?
            this.OnThingChanged += this.CheckPendingActions;                    // Are we in range with our pending action?
            // OnTargetChanged += CheckAutoAttack;          // Are we attacking someone new / not attacking anymore?
            // OnInventoryChanged += Mind.AttackConditionsChanged;        // Equipped / DeEquiped something?
            this.Skills = new Dictionary<SkillType, ISkill>();
            this.Hostiles = new HashSet<uint>();
            this.Friendly = new HashSet<uint>();
        }

        public event OnAttackTargetChange OnTargetChanged;

        public override ushort ThingId => CreatureThingId;

        public override byte Count => 0x01;

        public override string InspectionText => $"{this.Article} {this.Name}";

        public override string CloseInspectionText => this.InspectionText;

        public uint ActorId => this.CreatureId;

        public uint CreatureId { get; }

        public string Article { get; }

        public string Name { get; }

        public ushort Corpse { get; }

        public uint Hitpoints { get; }

        public uint MaxHitpoints { get; }

        public uint Manapoints { get; }

        public uint MaxManapoints { get; }

        public decimal CarryStrength { get; protected set; }

        public Outfit Outfit { get; protected set; }

        public Direction Direction { get; protected set; }

        public Direction ClientSafeDirection
        {
            get
            {
                switch (this.Direction)
                {
                    case Direction.North:
                    case Direction.East:
                    case Direction.South:
                    case Direction.West:
                        return this.Direction;
                    case Direction.NorthEast:
                    case Direction.SouthEast:
                        return Direction.East;
                    case Direction.NorthWest:
                    case Direction.SouthWest:
                        return Direction.West;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public byte LightBrightness { get; protected set; }

        public byte LightColor { get; protected set; }

        public ushort Speed { get; protected set; }

        public uint Flags { get; private set; }

        public BloodType Blood { get; protected set; } // TODO: implement.

        public abstract ushort AttackPower { get; }

        public abstract ushort ArmorRating { get; }

        public abstract ushort DefensePower { get; }

        public uint AutoAttackTargetId { get; protected set; }

        public abstract byte AutoAttackRange { get; }

        public byte AutoAttackCredits { get; }

        public byte AutoDefenseCredits { get; }

        public decimal BaseAttackSpeed { get; }

        public decimal BaseDefenseSpeed { get; }

        public TimeSpan CombatCooldownTimeRemaining => this.CalculateRemainingCooldownTime(CooldownType.Combat, Game.Instance.CombatSynchronizationTime);

        public DateTime LastAttackTime => this.Cooldowns[CooldownType.Combat].Item1;

        public TimeSpan LastAttackCost => this.Cooldowns[CooldownType.Combat].Item2;

        public IDictionary<SkillType, ISkill> Skills { get; }

        public IDictionary<CooldownType, Tuple<DateTime, TimeSpan>> Cooldowns { get; }

        // public IList<Condition> Conditions { get; protected set; } // TODO: implement.
        public bool IsInvisible { get; protected set; } // TODO: implement.

        public bool CanSeeInvisible { get; } // TODO: implement.

        public byte Skull { get; protected set; } // TODO: implement.

        public byte Shield { get; protected set; } // TODO: implement.

        public ConcurrentQueue<Tuple<byte, Direction>> WalkingQueue { get; }

        public byte NextStepId { get; set; }

        public HashSet<uint> Hostiles { get; }

        public HashSet<uint> Friendly { get; }

        public abstract IInventory Inventory { get; protected set; }

        public static uint GetNewId()
        {
            lock (IdLock)
            {
                return idCounter++; // we got 2^32 ids to give per game run... enough!
            }
        }

        protected virtual void CheckPendingActions(IThing thingChanged, ThingStateChangedEventArgs eventAgrs) { }

        // ~CreatureId()
        // {
        //    OnLocationChanged -= CheckAutoAttack;         // Are we in range with our target now/still?
        //    OnLocationChanged -= CheckPendingActions;                  // Are we in range with any of our pending actions?
        //    //OnTargetChanged -= CheckAutoAttack;           // Are we attacking someone new / not attacking anymore?
        //    //OnInventoryChanged -= Mind.AttackConditionsChanged;      // Equipped / DeEquiped something?
        // }
        public void IncreaseSkillCounter(SkillType skill, ushort value)
        {
            if (!this.Skills.ContainsKey(skill))
            {
                // TODO: proper logging.
                Console.WriteLine($"CreatureId {this.Name} does not have the skill {skill} in it's skill set.");
            }

            this.Skills[skill].IncreaseCounter(value);
        }

        public bool HasFlag(CreatureFlag flag)
        {
            var flagValue = (uint)flag;

            return (this.Flags & flagValue) == flagValue;
        }

        public void SetFlag(CreatureFlag flag)
        {
            this.Flags |= (uint)flag;
        }

        public void UnsetFlag(CreatureFlag flag)
        {
            this.Flags &= ~(uint)flag;
        }

        public bool CanSee(ICreature otherCreature)
        {
            return !otherCreature.IsInvisible || this.CanSeeInvisible;
        }

        public bool CanSee(Location pos)
        {
            if (this.Location.Z <= 7)
            {
                // we are on ground level or above (7 -> 0)
                // view is from 7 -> 0
                if (pos.Z > 7)
                {
                    return false;
                }
            }
            else if (this.Location.Z >= 8)
            {
                // we are underground (8 -> 15)
                // view is +/- 2 from the floor we stand on
                if (Math.Abs(this.Location.Z - pos.Z) > 2)
                {
                    return false;
                }
            }

            var offsetZ = this.Location.Z - pos.Z;

            if (pos.X >= this.Location.X - 8 + offsetZ && pos.X <= this.Location.X + 9 + offsetZ &&
                pos.Y >= this.Location.Y - 6 + offsetZ && pos.Y <= this.Location.Y + 7 + offsetZ)
            {
                return true;
            }

            return false;
        }

        public byte GetStackPosition()
        {
            return this.Tile.GetStackPosition(this);
        }

        public void TurnToDirection(Direction direction)
        {
            this.Direction = direction;
        }

        public void SetAttackTarget(uint targetId)
        {
            if (targetId == this.CreatureId || this.AutoAttackTargetId == targetId)
            {
                // if we want to attack ourselves or if the current target is already the one we want... no change needed.
                return;
            }

            // save the previus target to report
            var oldTargetId = this.AutoAttackTargetId;

            if (targetId == 0)
            {
                // clearing our target.
                if (this.AutoAttackTargetId != 0)
                {
                    var attackTarget = Game.Instance.GetCreatureWithId(this.AutoAttackTargetId);

                    if (attackTarget != null)
                    {
                        attackTarget.OnThingChanged -= this.CheckAutoAttack;
                    }

                    this.AutoAttackTargetId = 0;
                }
            }
            else
            {
                // TODO: verify against this.Hostiles.Union(this.Friendly).
                // if (creature != null)
                // {
                this.AutoAttackTargetId = targetId;

                var attackTarget = Game.Instance.GetCreatureWithId(this.AutoAttackTargetId);

                if (attackTarget != null)
                {
                    attackTarget.OnThingChanged += this.CheckAutoAttack;
                }

                // }
                // else
                // {
                //    Console.WriteLine("Taget creature not found in attacker\'s view.");
                // }
            }

            // report the change to our subscribers.
            this.OnTargetChanged?.Invoke(oldTargetId, targetId);
            this.CheckAutoAttack(this, new ThingStateChangedEventArgs() { PropertyChanged = nameof(this.location) });
        }

        public void UpdateLastAttack(TimeSpan exahust)
        {
            this.Cooldowns[CooldownType.Combat] = new Tuple<DateTime, TimeSpan>(Game.Instance.CombatSynchronizationTime, exahust);
        }

        public void CheckAutoAttack(IThing thingChanged, ThingStateChangedEventArgs eventAgrs)
        {
            if (this.AutoAttackTargetId == 0)
            {
                return;
            }

            var attackTarget = Game.Instance.GetCreatureWithId(this.AutoAttackTargetId);

            if (attackTarget == null || (thingChanged != this && thingChanged != attackTarget) || eventAgrs.PropertyChanged != nameof(Thing.Location))
            {
                return;
            }

            var locationDiff = this.Location - attackTarget.Location;
            var inRange = this.CanSee(attackTarget) && locationDiff.Z == 0 && locationDiff.MaxValueIn2D <= this.AutoAttackRange;

            if (inRange)
            {
                Game.Instance.SignalAttackReady();
            }
        }

        public void StopWalking()
        {
            lock (this.enqueueWalkLock)
            {
                this.WalkingQueue.Clear(); // reset the actual queue
                this.UpdateLastStepInfo(0);
            }
        }

        public void AutoWalk(params Direction[] directions)
        {
            lock (this.enqueueWalkLock)
            {
                if (this.WalkingQueue.Count > 0)
                {
                    this.StopWalking();
                }

                var nextStepId = this.NextStepId;

                foreach (var direction in directions)
                {
                    this.WalkingQueue.Enqueue(new Tuple<byte, Direction>((byte)(nextStepId++ % byte.MaxValue), direction));
                }

                Game.Instance.SignalWalkAvailable();
            }
        }

        public TimeSpan CalculateRemainingCooldownTime(CooldownType type, DateTime currentTime)
        {
            TimeSpan timeDiff = TimeSpan.Zero;

            try
            {
                timeDiff = this.Cooldowns[type].Item1 + this.Cooldowns[type].Item2 - currentTime;
            }
            catch
            {
                Console.WriteLine($"WARN: cooldown type {type} not found in {this.Name}'s cooldowns.");
            }

            return timeDiff > TimeSpan.Zero ? timeDiff : TimeSpan.Zero;
        }

        public void UpdateLastStepInfo(byte lastStepId, bool wasDiagonal = true)
        {
            var tilePenalty = this.Tile?.Ground?.MovementPenalty;
            var totalPenalty = (tilePenalty ?? 200) * (wasDiagonal ? 2 : 1);

            this.Cooldowns[CooldownType.Move] = new Tuple<DateTime, TimeSpan>(DateTime.Now, TimeSpan.FromMilliseconds(1000 * totalPenalty / (double)Math.Max(1, (int)this.Speed)));

            this.NextStepId = (byte)(lastStepId + 1);
        }
    }
}
