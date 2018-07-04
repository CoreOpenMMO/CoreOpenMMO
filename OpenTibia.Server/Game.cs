// <copyright file="Game.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Interfaces;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Data.Models;
    using OpenTibia.Scheduling;
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Server.Algorithms;
    using OpenTibia.Server.Combat;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;
    using OpenTibia.Server.Events;
    using OpenTibia.Server.Items;
    using OpenTibia.Server.Map;
    using OpenTibia.Server.Monsters;
    using OpenTibia.Server.Movement;
    using OpenTibia.Server.Notifications;
    using OpenTibia.Server.Scripting;
    using OpenTibia.Utilities;

    /// <summary>
    /// Main class.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Defines the <see cref="TimeSpan"/> to wait between checks of orphaned conections.
        /// </summary>
        private static readonly TimeSpan CheckOrphanConnectionsDelay = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Singleton instance of the <see cref="Game"/> class.
        /// </summary>
        private static readonly Lazy<Game> GameInstance = new Lazy<Game>(() => new Game());

        /// <summary>
        /// Gets the singleton instance of the <see cref="Game"/> class.
        /// </summary>
        public static Game Instance => Game.GameInstance.Value;

        private readonly object attackLock;
        private readonly object walkLock;
        private readonly object notifQueueLock;
        private readonly object combatQueueLock;

        private WorldState status;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        public Game()
        {
            this.attackLock = new object();
            this.walkLock = new object();
            this.notifQueueLock = new object();
            this.combatQueueLock = new object();

            this.Scheduler = new Scheduler(DateTime.Now); // TODO: chose another date here?

            this.NotificationQueue = new ConcurrentQueue<INotification>();
            this.CombatQueue = new ConcurrentQueue<ICombatOperation>();
            this.Connections = new ConcurrentDictionary<uint, Connection>();
            this.Creatures = new ConcurrentDictionary<uint, Creature>();

            // Initialize the map
            this.Map = new Map.Map(new SectorMapLoader(ServerConfiguration.LiveMapDirectory));

            // Initialize game vars.
            this.Status = WorldState.Creating;
            this.LightColor = (byte)LightColors.White;
            this.LightLevel = (byte)LightLevels.World;
        }

        public IItemEventLoader EventLoader { get; private set; }

        public IItemLoader ItemLoader { get; private set; }

        public IMonsterLoader MonsterLoader { get; private set; }

        public DateTime CurrentTime { get; private set; }

        public DateTime CombatSynchronizationTime { get; private set; }

        /// <summary>
        /// Gets the <see cref="ConcurrentDictionary{TKey,TValue}"/> of all <see cref="Creature"/>s in the game, in which the Key is the <see cref="Creature.CreatureId"/>.
        /// </summary>
        public ConcurrentDictionary<uint, Creature> Creatures { get; }

        /// <summary>
        /// Gets the <see cref="IDictionary{TKey,TValue}"/> containing the <see cref="IItemEvent"/>s of the game.
        /// </summary>
        public IDictionary<ItemEventType, HashSet<IItemEvent>> EventsCatalog { get; private set; }

        /// <summary>
        /// Gets the current world's light level <see cref="byte"/> value.
        /// </summary>
        public byte LightLevel { get; private set; }

        /// <summary>
        /// Gets the current world's light color <see cref="byte"/> value.
        /// </summary>
        public byte LightColor { get; private set; }

        /// <summary>
        /// Gets the current world's <see cref="WorldState"/>.
        /// </summary>
        public WorldState Status
        {
            get
            {
                return this.status;
            }

            private set
            {
                this.status = value;
                Console.WriteLine($"Game world is now {this.status}.");
            }
        }

        private IScheduler Scheduler { get; }

        /// <summary>
        /// Gets the <see cref="ConcurrentDictionary{TKey,TValue}"/> of all <see cref="Connection"/>s in the game, in which the Key is the <see cref="Creature.CreatureId"/>.
        /// </summary>
        private ConcurrentDictionary<uint, Connection> Connections { get; }

        /// <summary>
        /// Gets the current <see cref="ConcurrentQueue{T}"/> of <see cref="INotification"/>s, which the game processes on the <see cref="NotificationsProcessor"/> method.
        /// </summary>
        private ConcurrentQueue<INotification> NotificationQueue { get; }

        /// <summary>
        /// Gets the current <see cref="ConcurrentQueue{T}"/> of <see cref="ICombatOperation"/>s, which the game processes on the <see cref="CombatProcessor"/> method.
        /// </summary>
        private ConcurrentQueue<ICombatOperation> CombatQueue { get; }

        /// <summary>
        /// Gets or sets the master <see cref="CancellationToken"/> passed down to any new thread started by the <see cref="Game"/> instance.
        /// </summary>
        private CancellationToken CancelToken { get; set; }

        /// <summary>
        /// Gets the current <see cref="Map"/> instance of the game.
        /// </summary>
        private Map.Map Map { get; }

        public void Initialize(IItemEventLoader eventLoader, IItemLoader itemLoader, IMonsterLoader monsterLoader)
        {
            this.EventLoader = eventLoader ?? throw new ArgumentNullException(nameof(eventLoader));
            this.ItemLoader = itemLoader ?? throw new ArgumentNullException(nameof(itemLoader));
            this.MonsterLoader = monsterLoader ?? throw new ArgumentNullException(nameof(monsterLoader));
        }

        /// <summary>
        /// Attempts to find a path using the <see cref="AStar"/> implementation between two <see cref="Location"/>s.
        /// </summary>
        /// <param name="startLocation">The start location.</param>
        /// <param name="targetLocation">The target location to find a path to.</param>
        /// <param name="endLocation">The last searched location before returning.</param>
        /// <param name="maxStepsCount">Optional. The maximum number of search steps to perform before giving up on finding the target location. Default is 100.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Direction"/>s leading to the end location. The <paramref name="endLocation"/> and <paramref name="targetLocation"/> may or may not be the same.</returns>
        public IEnumerable<Direction> Pathfind(Location startLocation, Location targetLocation, out Location endLocation, int maxStepsCount = 100)
        {
            endLocation = startLocation;

            var fromTile = this.GetTileAt(startLocation);
            var toTile = this.GetTileAt(targetLocation);

            var searchId = Guid.NewGuid().ToString();
            var aSar = new AStar(TileNodeCache.Create(searchId, fromTile), TileNodeCache.Create(searchId, toTile), maxStepsCount);

            var result = aSar.Run();
            var dirList = new List<Direction>();

            try
            {
                if (result == SearchState.Failed)
                {
                    var lastTile = aSar.GetPath()?.LastOrDefault() as TileNode;

                    if (lastTile?.Tile != null)
                    {
                        endLocation = lastTile.Tile.Location;
                    }

                    return dirList;
                }

                var lastLoc = startLocation;

                foreach (var node in aSar.GetPath().Cast<TileNode>().Skip(1))
                {
                    var newDir = lastLoc.DirectionTo(node.Tile.Location, true);

                    dirList.Add(newDir);

                    lastLoc = node.Tile.Location;
                }

                endLocation = lastLoc;
            }
            finally
            {
                TileNodeCache.CleanUp(searchId);
            }

            return dirList;
        }

        public byte[] GetMapTileDescription(uint requestingPlayerId, Location location)
        {
            var tile = this.Map[location];
            var requestingPlayer = this.GetCreatureWithId(requestingPlayerId) as IPlayer;

            if (requestingPlayer == null)
            {
                return new byte[0];
            }

            return tile == null ? new byte[0] : this.Map.GetTileDescription(requestingPlayer, tile).ToArray();
        }

        public void SignalWalkAvailable()
        {
            lock (this.walkLock)
            {
                Monitor.Pulse(this.walkLock);
            }
        }

        public void SignalAttackReady()
        {
            lock (this.attackLock)
            {
                Monitor.Pulse(this.attackLock);
            }
        }

        public void Begin(CancellationToken masterCancellationToken)
        {
            if (masterCancellationToken == null || masterCancellationToken.IsCancellationRequested)
            {
                throw new ArgumentException("Invalid CancellationToken.");
            }

            this.CancelToken = masterCancellationToken;

            Task.Factory.StartNew(this.ConnectionSweeper, TaskCreationOptions.LongRunning);
            //Task.Factory.StartNew(this.DayCycle, TaskCreationOptions.LongRunning);

            //Task.Factory.StartNew(this.CheckCreatureWalk, TaskCreationOptions.LongRunning);
            //Task.Factory.StartNew(this.CheckCreatureAutoAttack, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(this.NotificationsProcessor, TaskCreationOptions.LongRunning);
            //Task.Factory.StartNew(this.CombatProcessor, TaskCreationOptions.LongRunning);

            this.EventsCatalog = this.EventLoader.Load(ServerConfiguration.MoveUseFileName);

            this.Scheduler.OnEventFired += this.ProcessFiredEvent;

            // Leave this at the very end, when everything is ready...
            this.Status = WorldState.Open;
        }

        public bool ScheduleEvent(IEvent newEvent, TimeSpan delay = default(TimeSpan))
        {
            newEvent.ThrowIfNull(nameof(newEvent));

            // pre check if can be executed only if not explicitly set to executeTime
            if (newEvent.EvaluateAt == EvaluationTime.OnExecute || newEvent.CanBeExecuted)
            {
                var noDelay = delay == default(TimeSpan) || delay < TimeSpan.Zero;

                if (noDelay)
                {
                    this.Scheduler.ImmediateEvent(newEvent);
                    return true;
                }

                this.Scheduler.ScheduleEvent(newEvent, DateTime.Now + delay);
                return true;
            }

            return false;
        }

        public void RequestCombatOp(ICombatOperation newOp)
        {
            lock (this.combatQueueLock)
            {
                this.CombatQueue.Enqueue(newOp);

                Monitor.Pulse(this.combatQueueLock);
            }
        }

        public void NotifySinglePlayer(IPlayer player, Func<Connection, Notification> notificationFunc)
        {
            if (player == null)
            {
                // TODO: proper logging
                Console.WriteLine($"WARN: null {nameof(player)} on NotifySinglePlayer.");
                return;
            }

            if (notificationFunc == null)
            {
                // TODO: proper logging
                Console.WriteLine($"WARN: null {nameof(notificationFunc)} on NotifySinglePlayer.");
                return;
            }

            try
            {
                var conn = this.Connections[player.CreatureId];

                this.InternalRequestNofitication(notificationFunc(conn));
            }
            catch (Exception ex)
            {
                // TODO: proper logging
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void NotifyAllPlayers(Func<Connection, Notification> notificationFunc)
        {
            if (notificationFunc == null)
            {
                // TODO: proper logging
                Console.WriteLine($"WARN: null {nameof(notificationFunc)} on NotifyAllPlayers.");
                return;
            }

            foreach (var conn in this.Connections.Values)
            {
                this.InternalRequestNofitication(notificationFunc(conn));
            }
        }

        public void NotifySpectatingPlayers(Func<Connection, Notification> notificationFunc, params Location[] locations)
        {
            if (notificationFunc == null)
            {
                // TODO: proper logging
                Console.WriteLine($"WARN: null {nameof(notificationFunc)} on NotifySpectatingPlayers.");
                return;
            }

            if (!locations.Any())
            {
                // TODO: proper logging
                Console.WriteLine($"WARN: no locations provided for notification.");
                return;
            }

            var allSpectating = this.GetSpectatingPlayers(locations.First());

            allSpectating = locations.Skip(1).Aggregate(allSpectating, (current, otherLocation) => current.Union(this.GetSpectatingPlayers(otherLocation), new CreatureEqualityComparer()).Select(c => c as Player));

            foreach (var spectator in allSpectating)
            {
                try
                {
                    var conn = this.Connections[spectator.CreatureId];

                    this.InternalRequestNofitication(notificationFunc(conn));
                }
                catch (Exception ex)
                {
                    // TODO: proper logging
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        private void CheckCreatureWalk()
        {
            while (!this.CancelToken.IsCancellationRequested)
            {
                try
                {
                    this.CurrentTime = DateTime.Now;
                    var minCoolDown = TimeSpan.MaxValue;

                    foreach (var creature in this.Creatures.Values.Where(c => c.WalkingQueue.Count > 0).ToList())
                    {
                        Tuple<byte, Direction> nextTuple;
                        var cooldownTime = creature.CalculateRemainingCooldownTime(CooldownType.Move, this.CurrentTime);

                        if (cooldownTime <= TimeSpan.Zero && creature.WalkingQueue.TryPeek(out nextTuple) && creature.NextStepId == nextTuple.Item1)
                        {
                            // Time to walk, let's process it.
                            if (!creature.WalkingQueue.TryDequeue(out nextTuple))
                            {
                                continue;
                            }

                            if (!this.RequestCreatureWalkToDirection(creature, nextTuple.Item2) && creature is IPlayer)
                            {
                                creature.StopWalking();

                                var player = creature as IPlayer;
                                var connection = this.Connections[player.CreatureId];

                                if (connection != null)
                                {
                                    this.InternalRequestNofitication(
                                        new GenericNotification(
                                            connection,
                                            new PlayerWalkCancelPacket { Direction = player.Direction },
                                            new TextMessagePacket { Message = "Sorry, not possible.", Type = MessageType.StatusSmall })
                                    );
                                }
                            }

                            // recalc cooldown for this creature
                            cooldownTime = creature.CalculateRemainingCooldownTime(CooldownType.Move, this.CurrentTime);

                            if (creature.WalkingQueue.Count > 0 && cooldownTime < minCoolDown)
                            {
                                minCoolDown = cooldownTime;
                            }
                        }
                        else if (cooldownTime < minCoolDown)
                        {
                            minCoolDown = cooldownTime;
                        }
                    }

                    lock (this.walkLock)
                    {
                        if (minCoolDown != TimeSpan.MaxValue)
                        {
                            var timeThatCheckTook = DateTime.Now - this.CurrentTime;
                            var timeDiff = minCoolDown - timeThatCheckTook; // factor in the time we took to check all queues.
                            var actualCooldown = timeDiff > TimeSpan.Zero ? timeDiff : TimeSpan.Zero; // and if that is positive.

                            Monitor.Wait(this.walkLock, actualCooldown); // there was work, but it's not time yet.
                        }
                        else
                        {
                            Monitor.Wait(this.walkLock); // there was no work, sleep until woken up.
                        }
                    }
                }
                catch (Exception ex)
                {
                    // TODO: proper logging
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private void CheckCreatureAutoAttack()
        {
            while (!this.CancelToken.IsCancellationRequested)
            {
                try
                {
                    this.CombatSynchronizationTime = DateTime.Now;
                    var minCoolDown = TimeSpan.MaxValue;

                    foreach (var creature in this.Creatures.Values.Where(c => c.AutoAttackTargetId > 0).ToList())
                    {
                        var cooldownTime = creature.CalculateRemainingCooldownTime(CooldownType.Combat, this.CombatSynchronizationTime);

                        if (cooldownTime <= TimeSpan.Zero)
                        {
                            // reset the cooldown time to max so that we don't count this
                            cooldownTime = TimeSpan.MaxValue;

                            // Time to attack, let's process it.
                            var target = this.GetCreatureWithId(creature.AutoAttackTargetId) as Creature;

                            if (target != null)
                            {
                                var standardAttackOp = new StandardAttackOperation(creature, target);

                                if (standardAttackOp.CanBeExecuted) // pre check
                                {
                                    this.RequestCombatOp(standardAttackOp);
                                    cooldownTime = standardAttackOp.ExhaustionCost;
                                }
                            }
                            else
                            {
                                // creature is not on the global creature's list, hence it's invalid now.
                                creature.SetAttackTarget(0);
                            }
                        }

                        if (cooldownTime < minCoolDown)
                        {
                            minCoolDown = cooldownTime;
                        }
                    }

                    lock (this.attackLock)
                    {
                        if (minCoolDown != TimeSpan.MaxValue)
                        {
                            var timeThatCheckTook = DateTime.Now - this.CombatSynchronizationTime;
                            var timeDiff = minCoolDown - timeThatCheckTook; // factor in the time we took to check all queues.
                            var actualCooldown = timeDiff > TimeSpan.Zero ? timeDiff : TimeSpan.Zero; // and if that is positive.

                            Monitor.Wait(this.attackLock, actualCooldown); // there was work, but it's not time yet.
                        }
                        else
                        {
                            Monitor.Wait(this.attackLock); // there was no work, sleep until woken up.
                        }
                    }
                }
                catch (Exception ex)
                {
                    // TODO: proper logging
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private void PlaceMonsters()
        {
            var monsterSpawns = this.MonsterLoader.LoadSpawns(ServerConfiguration.SpawnsFileName);
            var loadedCount = 0;
            var spawnsList = monsterSpawns as IList<Spawn> ?? monsterSpawns.ToList();
            var totalCount = spawnsList.Sum(s => s.Count);

            var percentageCompleteFunc = new Func<byte>(() => (byte)Math.Floor(Math.Min(100, Math.Max((decimal)0, loadedCount * 100 / (totalCount + 1)))));
            var cts = new CancellationTokenSource();

            Task.Factory.StartNew(
            () =>
            {
                while (!cts.IsCancellationRequested)
                {
                    // TODO: proper logging
                    Console.WriteLine($"Monster placed percentage: {percentageCompleteFunc()} %");
                    Thread.Sleep(TimeSpan.FromSeconds(7)); // TODO: this is arbitrary...
                }
            }, cts.Token);

            var rng = new Random();
            Parallel.ForEach(spawnsList, spawn =>
            {
                Parallel.For(0, spawn.Count, i =>
                {
                    // var halfRadius = spawn.Radius / 2;
                    var placed = false;

                    byte tries = 1;
                    do
                    {
                        // TODO: revisit this logic
                        var randomLoc = spawn.Location + new Location { X = (int)Math.Round((i * Math.Cos(rng.Next(360))) - i), Y = (int)Math.Round((i * Math.Cos(rng.Next(360))) - i), Z = 0 };
                        var randomTile = this.GetTileAt(randomLoc);

                        if (randomTile == null)
                        {
                            continue;
                        }

                        Location foundLocation;

                        // Need to actually pathfind to avoid placing a monster in unreachable places.
                        this.Pathfind(spawn.Location, randomTile.Location, out foundLocation, (i + 1) * 100);

                        var foundTile = this.GetTileAt(foundLocation);

                        if (foundTile != null && !foundTile.BlocksPass)
                        {
                            Functions.MonsterOnMap(foundLocation, spawn.Id);
                            placed = true;
                        }
                    }
                    while (++tries != 0 && !placed);

                    if (!placed)
                    {
                        // TODO: proper logging
                        Console.WriteLine($"Given up on placing monster with type {spawn.Id} around {spawn.Location}, no suitable tile found.");
                    }

                    Interlocked.Add(ref loadedCount, 1);
                });
            });

            // cancel the progress thread.
            cts.Cancel();
        }

        // Connection cleaning task
        private void ConnectionSweeper()
        {
            while (!this.CancelToken.IsCancellationRequested)
            {
                Thread.Sleep(Game.CheckOrphanConnectionsDelay);

                foreach (var pcKvp in this.Connections.Where(playerConn => !playerConn.Value.Socket.Connected).ToList())
                {
                    var player = this.GetCreatureWithId(pcKvp.Key) as Player;

                    if (player == null)
                    {
                        continue;
                    }

                    player.SetAttackTarget(0);

                    if (player.CanLogout)
                    {
                        this.AttemptLogout(player);
                    }
                }
            }
        }

        // Day -> Dusk -> Night -> Dawn cycle thread
        private void DayCycle()
        {
            // A day is an hour in real time...
            // So night in realtime lasts 1/3 of the day IRL
            // Dusk and Dawns last for... 30 minutes IRL, so les aproximate that to 2 minutes.
            var nightTime = TimeSpan.FromMinutes(18);
            var dawnTime = TimeSpan.FromMinutes(2);
            var dayTime = TimeSpan.FromMinutes(38);
            var duskTime = TimeSpan.FromMinutes(2);

            while (!this.CancelToken.IsCancellationRequested)
            {
                Thread.Sleep(dayTime);

                this.LightLevel = 130;
                this.LightColor = (byte)LightColors.Orange;
                this.NotifyAllPlayers(conn => new WorldLightChangedNotification(conn, this.LightLevel, this.LightColor));

                Thread.Sleep(duskTime);

                this.LightLevel = 30;
                this.LightColor = (byte)LightColors.White;
                this.NotifyAllPlayers(conn => new WorldLightChangedNotification(conn, this.LightLevel, this.LightColor));

                Thread.Sleep(nightTime);

                this.LightLevel = 130;
                this.LightColor = (byte)LightColors.Orange;
                this.NotifyAllPlayers(conn => new WorldLightChangedNotification(conn, this.LightLevel, this.LightColor));

                Thread.Sleep(dawnTime);

                this.LightLevel = 255;
                this.LightColor = (byte)LightColors.White;
                this.NotifyAllPlayers(conn => new WorldLightChangedNotification(conn, this.LightLevel, this.LightColor));
            }
        }

        // Notification thread
        private void NotificationsProcessor()
        {
            while (!this.CancelToken.IsCancellationRequested)
            {
                INotification notification;

                while (this.NotificationQueue.Count == 0 || !this.NotificationQueue.TryDequeue(out notification))
                {
                    lock (this.notifQueueLock)
                    {
                        Monitor.Wait(this.notifQueueLock);
                    }
                }

                try
                {
                    Console.WriteLine($"Sending {notification.GetType().Name} [{notification.EventId}] to {notification.Connection.PlayerId}");
                    notification.Send();
                }
                catch (Exception ex)
                {
                    // TODO: proper logging
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        // Movement thread
        private void ProcessFiredEvent(object sender, EventFiredEventArgs eventArgs)
        {
            if (sender != this.Scheduler || eventArgs?.Event == null)
            {
                return;
            }

            IEvent evt = eventArgs.Event;

            Console.WriteLine($"Processing event {evt.EventId}.");

            try
            {
                evt.Process();
                Console.WriteLine($"Processed event {evt.EventId}.");
            }
            catch (Exception ex)
            {
                // TODO: proper logging
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void CombatProcessor()
        {
            while (!this.CancelToken.IsCancellationRequested)
            {
                ICombatOperation combatOp;

                lock (this.combatQueueLock)
                {
                    while (this.CombatQueue.Count == 0 || !this.CombatQueue.TryDequeue(out combatOp))
                    {
                        Monitor.Wait(this.combatQueueLock);
                    }
                }

                try
                {
                    if (combatOp.CanBeExecuted)
                    {
                        var canAttack = true;
                        var attackerAsCreature = combatOp.Attacker as ICreature;
                        var targetAsCreature = combatOp.Target as ICreature;

                        if (targetAsCreature != null && attackerAsCreature != null)
                        {
                            canAttack &= attackerAsCreature.CanSee(targetAsCreature);
                            canAttack &= this.CanThrowBetween(attackerAsCreature.Location, targetAsCreature.Location);
                        }

                        if (canAttack)
                        {
                            combatOp.Execute();
                        }
                    }

                    // CombatOperations that cannot be performed are discarded nontheless.
                }
                catch (Exception ex)
                {
                    // TODO: proper logging
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        private void InternalRequestNofitication(INotification notification)
        {
            notification.Prepare();

            this.ScheduleEvent(notification);
        }

        public IEnumerable<uint> GetSpectatingCreatureIds(Location location)
        {
            return this.Map.GetCreatureIdsAt(location);
        }

        public ITile GetTileAt(Location location)
        {
            return this.Map[location];
        }

        private IEnumerable<IPlayer> GetSpectatingPlayers(Location location)
        {
            return this.Connections.Keys.Select(creatureId => this.Creatures[creatureId]).Where(c => c.CanSee(location)).Cast<IPlayer>().ToList();
        }

        public bool CanThrowBetween(Location fromLocation, Location toLocation, bool checkLineOfSight = true)
        {
            if (fromLocation == toLocation)
            {
                return true;
            }

            if ((fromLocation.Z >= 8 && toLocation.Z < 8) || (toLocation.Z >= 8 && fromLocation.Z < 8))
            {
                return false;
            }

            var deltaZ = Math.Abs(fromLocation.Z - toLocation.Z);

            if (deltaZ > 2)
            {
                return false;
            }

            var deltaX = Math.Abs(fromLocation.X - toLocation.X);
            var deltaY = Math.Abs(fromLocation.Y - toLocation.Y);

            // distance checks
            if (deltaX - deltaZ > 8 || deltaY - deltaZ > 6)
            {
                return false;
            }

            return !checkLineOfSight || this.InLineOfSight(fromLocation, toLocation);
        }

        public bool InLineOfSight(Location fromLocation, Location toLocation)
        {
            if (fromLocation == toLocation)
            {
                return true;
            }

            var start = fromLocation.Z > toLocation.Z ? toLocation : fromLocation;
            var destination = fromLocation.Z > toLocation.Z ? fromLocation : toLocation;

            var mx = (sbyte)(start.X < destination.X ? 1 : start.X == destination.X ? 0 : -1);
            var my = (sbyte)(start.Y < destination.Y ? 1 : start.Y == destination.Y ? 0 : -1);

            var a = destination.Y - start.Y;
            var b = start.X - destination.X;
            var c = -((a * destination.X) + (b * destination.Y));

            while ((start - destination).MaxValueIn2D != 0)
            {
                var moveHor = Math.Abs((a * (start.X + mx)) + (b * start.Y) + c);
                var moveVer = Math.Abs((a * start.X) + (b * (start.Y + my)) + c);
                var moveCross = Math.Abs((a * (start.X + mx)) + (b * (start.Y + my)) + c);

                if (start.Y != destination.Y && (start.X == destination.X || moveHor > moveVer || moveHor > moveCross))
                {
                    start.Y += my;
                }

                if (start.X != destination.X && (start.Y == destination.Y || moveVer > moveHor || moveVer > moveCross))
                {
                    start.X += mx;
                }

                var tile = this.GetTileAt(start);

                if (tile != null && tile.BlocksThrow)
                {
                    return false;
                }
            }

            while (start.Z != destination.Z)
            {
                // now we need to perform a jump between floors to see if everything is clear (literally)
                var tile = this.GetTileAt(start);

                if (tile?.Ground != null)
                {
                    return false;
                }

                start.Z++;
            }

            return true;
        }

        internal Player Login(PlayerModel playerRecord, Connection connection)
        {
            var rng = new Random();
            var player = new Player((uint)rng.Next(), playerRecord.Charname, 100, 100, 4240, 100, 100);

            // TODO: check if map.CanAddCreature(playerRecord.location);
            // playerRecord.location
            IThing playerThing = player;
            this.Map[Server.Map.Map.VeteranStart].AddThing(ref playerThing);

            this.NotifySpectatingPlayers(conn => new CreatureAddedNotification(conn, player, EffectT.BubbleBlue), player.Location);

            this.Connections.TryAdd(player.CreatureId, connection);

            if (!this.Creatures.TryAdd(player.CreatureId, player))
            {
                // TODO: proper logging
                Console.WriteLine($"WARNING: Failed to add {player.Name} to the global dictionary.");
            }

            return player;
        }

        internal bool AttemptLogout(IPlayer player)
        {
            if (player == null || !player.CanLogout)
            {
                return false;
            }

            // TODO: stuff missing?
            var oldStackpos = player.Tile.GetStackPosition(player);

            IThing playerThing = player;
            player.Tile.RemoveThing(ref playerThing);

            this.NotifySpectatingPlayers(conn => new CreatureRemovedNotification(conn, player, oldStackpos, EffectT.Puff), player.Location);

            Creature creature;
            this.Creatures.TryRemove(player.CreatureId, out creature);

            Connection connection;
            return this.Connections.TryRemove(player.CreatureId, out connection);
        }

        internal byte[] GetMapDescriptionAt(IPlayer forPlayer, Location location)
        {
            return this.Map.GetDescription(forPlayer, (ushort)(location.X - 8), (ushort)(location.Y - 6), location.Z, location.IsUnderground).ToArray(); // TODO: handle near top left of map edge case.
        }

        internal byte[] GetMapDescription(IPlayer player, ushort fromX, ushort fromY, sbyte currentZ, bool isUnderground, byte windowSizeX, byte windowSizeY)
        {
            return this.Map.GetDescription(player, fromX, fromY, currentZ, isUnderground, windowSizeX, windowSizeY).ToArray();
        }

        internal byte[] GetMapFloorsDescription(IPlayer forPlayer, ushort fromX, ushort fromY, short startZ, short endZ, byte windowSizeX, byte windowSizeY, int startingOffsetZ = 0)
        {
            var totalBytes = new List<byte>();

            var skip = -1;
            var stepZ = 1; // asume going down the ground

            if (startZ > endZ)
            {
                stepZ = -1; // we're going up!
            }

            for (int currentZ = startZ; currentZ != endZ + stepZ; currentZ += stepZ)
            {
                totalBytes.AddRange(this.Map.GetFloorDescription(forPlayer, fromX, fromY, (sbyte)currentZ, windowSizeX, windowSizeY, startZ - currentZ + startingOffsetZ, ref skip));
            }

            if (skip >= 0)
            {
                totalBytes.Add((byte)skip);
                totalBytes.Add(0xFF);
            }

            return totalBytes.ToArray();
        }

        internal bool RequestCreatureWalkToDirection(ICreature creature, Direction direction, TimeSpan delay = default(TimeSpan))
        {
            var fromLoc = creature.Location;
            var toLoc = fromLoc;

            switch (direction)
            {
                case Direction.North:
                    toLoc.Y -= 1;
                    break;
                case Direction.South:
                    toLoc.Y += 1;
                    break;
                case Direction.East:
                    toLoc.X += 1;
                    break;
                case Direction.West:
                    toLoc.X -= 1;
                    break;
                case Direction.NorthEast:
                    toLoc.X += 1;
                    toLoc.Y -= 1;
                    break;
                case Direction.NorthWest:
                    toLoc.X -= 1;
                    toLoc.Y -= 1;
                    break;
                case Direction.SouthEast:
                    toLoc.X += 1;
                    toLoc.Y += 1;
                    break;
                case Direction.SouthWest:
                    toLoc.X -= 1;
                    toLoc.Y += 1;
                    break;
            }

            var movement = new CreatureMovementOnMap(creature.CreatureId, creature, fromLoc, toLoc);

            return this.ScheduleEvent(movement, delay);
        }

        internal void TestingViaCreatureSpeech(IPlayer player, string msgStr)
        {
            var testStrObjs = msgStr.Replace("test.", string.Empty).Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (!testStrObjs.Any())
            {
                return;
            }

            switch (testStrObjs[0])
            {
                case "mon":
                    if (testStrObjs.Length > 1)
                    {
                        var monsterId = Convert.ToUInt16(testStrObjs[1]);

                        Task.Factory.StartNew(() => Functions.MonsterOnMap(player.LocationInFront, monsterId), this.CancelToken);
                    }

                    break;
            }
        }

        private void OnContainerContentEvent(sbyte operationType, IContainer container, byte index, IItem item)
        {
            var interestedCreatureIds = container.OpenedBy.Keys.ToList();

            foreach (var peekerId in interestedCreatureIds)
            {
                var peeker = this.GetCreatureWithId(peekerId) as IPlayer;

                if (peeker == null)
                {
                    continue;
                }

                switch (operationType)
                {
                    default:
                        // update
                        this.NotifySinglePlayer(peeker, conn => new GenericNotification(conn, new ContainerUpdateItemPacket { ContainerId = (byte)container.GetIdFor(peekerId), Index = index, Item = item }));
                        break;
                    case -1:
                        // remove
                        this.NotifySinglePlayer(peeker, conn => new GenericNotification(conn, new ContainerRemoveItemPacket { ContainerId = (byte)container.GetIdFor(peekerId), Index = index }));
                        break;
                    case 1:
                        // add
                        this.NotifySinglePlayer(peeker, conn => new GenericNotification(conn, new ContainerAddItemPacket { ContainerId = (byte)container.GetIdFor(peekerId), Item = item }));
                        break;
                }
            }
        }

        public void OnContainerContentUpdated(IContainer container, byte index, IItem item)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.OnContainerContentEvent(0, container, index, item);
        }

        public void OnContainerContentAdded(IContainer container, IItem item)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.OnContainerContentEvent(1, container, 0xFF, item);
        }

        public void OnContainerContentRemoved(IContainer container, byte index)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            this.OnContainerContentEvent(-1, container, index, null);
        }

        public ICreature GetCreatureWithId(uint creatureId)
        {
            try
            {
                return this.Creatures[creatureId];
            }
            catch
            {
                return null;
            }
        }
    }
}
