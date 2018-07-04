// <copyright file="Functions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Scripting
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;
    using OpenTibia.Server.Items;
    using OpenTibia.Server.Monsters;
    using OpenTibia.Server.Movement;
    using OpenTibia.Server.Notifications;

    public static class Functions
    {
        public const string ThingOneIdentifier = "Obj1";
        public const string ThingTwoIdentifier = "Obj2";
        public const string CurrentUserIdentifier = "User";

        public const string NameShorthand = "%N";

        private static TimeSpan DelayForFunctions = TimeSpan.FromMilliseconds(1);

        private static object ConvertSingleItem(string value, Type newType)
        {
            if (typeof(IConvertible).IsAssignableFrom(newType))
            {
                return Convert.ChangeType(value, newType);
            }

            var converter = CustomConvertersFactory.GetConverter(newType);

            if (converter == null)
            {
                throw new InvalidOperationException($"No suitable Converter found for type {newType}.");
            }

            return converter.Convert(value);
        }

        private static object ConvertStringToNewNonNullableType(string value, Type newType)
        {
            // Do conversion form string to array - not sure how array will be stored in string
            if (newType.IsArray)
            {
                // For comma separated list
                var singleItemType = newType.GetElementType();

                var elements = new ArrayList();
                foreach (var element in value.Split(','))
                {
                    var convertedSingleItem = ConvertSingleItem(element, singleItemType);
                    elements.Add(convertedSingleItem);
                }

                return elements.ToArray(singleItemType);
            }

            return ConvertSingleItem(value, newType);
        }

        private static object ConvertStringToNewType(string value, Type newType)
        {
            // If it's not a nullable type, just pass through the parameters to Convert.ChangeType
            if (newType.IsGenericType && newType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (value == null)
                {
                    return null;
                }

                return ConvertStringToNewNonNullableType(value, new NullableConverter(newType).UnderlyingType);
            }

            return ConvertStringToNewNonNullableType(value, newType);
        }

        public static bool InvokeCondition(IThing obj1, IThing obj2, IPlayer user, string methodName, params object[] parameters)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new ArgumentNullException(nameof(methodName));
            }

            var negateCondition = methodName.StartsWith("!");

            methodName = methodName.TrimStart('!');

            var type = typeof(Functions);

            var methodInfo = type.GetMethod(methodName);

            try
            {
                if (methodInfo == null)
                {
                    throw new MissingMethodException(type.Name, methodName);
                }

                var methodParameters = methodInfo.GetParameters();

                var parametersForInvocation = new List<object>();

                for (var i = 0; i < methodParameters.Length; i++)
                {
                    if (ThingOneIdentifier.Equals(parameters[i] as string))
                    {
                        parametersForInvocation.Add(obj1);
                    }
                    else if (ThingTwoIdentifier.Equals(parameters[i] as string))
                    {
                        parametersForInvocation.Add(obj2);
                    }
                    else if (CurrentUserIdentifier.Equals(parameters[i] as string))
                    {
                        parametersForInvocation.Add(user);
                    }
                    else
                    {
                        parametersForInvocation.Add(ConvertStringToNewType(parameters[i] as string, methodParameters[i].ParameterType));
                    }
                }

                var result = (bool)methodInfo.Invoke(null, parametersForInvocation.ToArray());

                return negateCondition ? !result : result;
            }
            catch (Exception ex)
            {
                // TODO: proper logging
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            return false;
        }

        public static void InvokeAction(ref IThing obj1, ref IThing obj2, ref IPlayer user, string methodName, params object[] parameters)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new ArgumentNullException(nameof(methodName));
            }

            var type = typeof(Functions);

            var methodInfo = type.GetMethod(methodName);

            try
            {
                if (methodInfo == null)
                {
                    throw new MissingMethodException(type.Name, methodName);
                }

                var methodParameters = methodInfo.GetParameters();

                var parametersForInvocation = new List<object>();

                for (var i = 0; i < methodParameters.Length; i++)
                {
                    if (ThingOneIdentifier.Equals(parameters[i] as string))
                    {
                        parametersForInvocation.Add(obj1);
                    }
                    else if (ThingTwoIdentifier.Equals(parameters[i] as string))
                    {
                        parametersForInvocation.Add(obj2);
                    }
                    else if (CurrentUserIdentifier.Equals(parameters[i] as string))
                    {
                        parametersForInvocation.Add(user);
                    }
                    else
                    {
                        var convertedValue = ConvertStringToNewType(parameters[i] as string, methodParameters[i].ParameterType);

                        parametersForInvocation.Add(convertedValue);
                    }
                }

                var paramsArray = parametersForInvocation.ToArray();

                methodInfo.Invoke(null, paramsArray);

                // update references to special parameters.
                for (var i = 0; i < methodParameters.Length; i++)
                {
                    if (ThingOneIdentifier.Equals(parameters[i] as string))
                    {
                        obj1 = paramsArray[i] as IThing;
                    }
                    else if (ThingTwoIdentifier.Equals(parameters[i] as string))
                    {
                        obj2 = paramsArray[i] as IThing;
                    }
                    else if (CurrentUserIdentifier.Equals(parameters[i] as string))
                    {
                        user = paramsArray[i] as IPlayer;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static bool CountObjects(IThing thingAt, string comparer, ushort value)
        {
            if (thingAt?.Tile == null || string.IsNullOrWhiteSpace(comparer))
            {
                return false;
            }

            var count = thingAt.Tile.Ground == null ? 0 : 1;
                count += thingAt.Tile.DownItems.Count();

            switch (comparer.Trim())
            {
                case "=":
                case "==":
                    return count == value;
                case ">=":
                    return count >= value;
                case "<=":
                    return count <= value;
                case ">":
                    return count > value;
                case "<":
                    return count < value;
            }

            return false;
        }

        public static bool IsCreature(IThing thing)
        {
            return thing is ICreature;
        }

        public static bool IsType(IThing thing, ushort typeId)
        {
            var item = thing as IItem;

            return item != null && item.Type.TypeId == typeId;
        }

        public static bool IsPosition(IThing thing, Location location)
        {
            return thing != null && thing.Location == location;
        }

        public static bool IsPlayer(IThing thing)
        {
            return thing is IPlayer;
        }

        public static bool IsObjectThere(Location location, ushort typeId)
        {
            var targetTile = Game.Instance.GetTileAt(location);

            return targetTile?.BruteFindItemWithId(typeId) != null;
        }

        public static bool HasRight(IPlayer user, string rightStr)
        {
            return true; // TODO: implement.
        }

        public static bool MayLogout(IPlayer user)
        {
            return user.CanLogout;
        }

        public static bool HasFlag(IThing itemThing, string flagStr)
        {
            if (!(itemThing is IItem))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(flagStr))
            {
                return true;
            }

            ItemFlag parsedFlag;

            return Enum.TryParse(flagStr, out parsedFlag) && ((IItem)itemThing).Type.Flags.Contains(parsedFlag);
        }

        public static bool HasProfession(IThing thing, byte profesionId)
        {
            return thing != null && thing is IPlayer && false; // TODO: implement professions.
        }

        public static bool HasInstanceAttribute(IThing thing, string attributeStr, string comparer, ushort value)
        {
            var thingAsItem = thing as IItem;

            if (thing == null || string.IsNullOrWhiteSpace(attributeStr) || string.IsNullOrWhiteSpace(comparer) || thingAsItem == null)
            {
                return false;
            }

            ItemAttribute actualAttribute;

            if (!Enum.TryParse(attributeStr, out actualAttribute))
            {
                return false;
            }

            if (!thingAsItem.Attributes.ContainsKey(actualAttribute))
            {
                return false;
            }

            switch (comparer.Trim())
            {
                case "=":
                case "==":
                    return Convert.ToUInt16(thingAsItem.Attributes[actualAttribute]) == value;
                case ">=":
                    return Convert.ToUInt16(thingAsItem.Attributes[actualAttribute]) >= value;
                case "<=":
                    return Convert.ToUInt16(thingAsItem.Attributes[actualAttribute]) <= value;
                case ">":
                    return Convert.ToUInt16(thingAsItem.Attributes[actualAttribute]) > value;
                case "<":
                    return Convert.ToUInt16(thingAsItem.Attributes[actualAttribute]) < value;
            }

            return false;
        }

        public static bool IsHouse(IThing thing)
        {
            return thing?.Tile != null && thing.Tile.IsHouse;
        }

        public static bool IsHouseOwner(IThing thing, IPlayer user)
        {
            return IsHouse(thing); // && thing.Tile.House.Owner == user.Name;
        }

        public static bool Random(byte value)
        {
            return new Random().Next(100) <= value;
        }

        public static void Create(IThing atThing, ushort itemId, byte unknown)
        {
            IThing item = ItemFactory.Create(itemId);
            var targetTile = atThing.Tile;

            if (item == null || targetTile == null)
            {
                return;
            }

            targetTile.AddThing(ref item);

            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, targetTile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, targetTile.Location)), targetTile.Location);
        }

        public static void CreateOnMap(Location location, ushort itemId, byte unknown)
        {
            IThing item = ItemFactory.Create(itemId);

            if (item == null)
            {
                return;
            }

            var targetTile = Game.Instance.GetTileAt(location);

            if (targetTile == null)
            {
                return;
            }

            targetTile.AddThing(ref item);

            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, location, Game.Instance.GetMapTileDescription(conn.PlayerId, location)), location);
        }

        public static void ChangeOnMap(Location location, ushort fromItemId, ushort toItemId, byte unknown)
        {
            var targetTile = Game.Instance.GetTileAt(location);
            IThing newThing = ItemFactory.Create(toItemId);

            if (targetTile == null || newThing == null)
            {
                return;
            }

            targetTile.BruteRemoveItemWithId(fromItemId);
            targetTile.AddThing(ref newThing);

            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, location, Game.Instance.GetMapTileDescription(conn.PlayerId, location)), targetTile.Location);
        }

        public static void Effect(IThing thing, byte effectByte)
        {
            if (thing == null)
            {
                return;
            }

            if (effectByte == 0 || effectByte > (byte)EffectT.SoundWhite)
            {
                Console.WriteLine($"Invalid effect {effectByte} called, ignored.");
                return;
            }

            Game.Instance.NotifySpectatingPlayers(
                conn => new GenericNotification(
                    conn,
                    new MagicEffectPacket { Location = thing.Location, Effect = (EffectT)effectByte }),
                thing.Location);
        }

        public static void EffectOnMap(Location location, byte effectByte)
        {
            if (effectByte == 0 || effectByte > (byte)EffectT.SoundWhite)
            {
                Console.WriteLine($"Invalid effect {effectByte} called on map, ignored.");
                return;
            }

            Game.Instance.NotifySpectatingPlayers(
                conn => new GenericNotification(
                    conn, 
                    new MagicEffectPacket { Location = location, Effect = (EffectT)effectByte }),
                location);
        }

        public static void Delete(IThing thing)
        {
            var targetTile = thing?.Tile;

            if (thing == null || targetTile == null)
            {
                return;
            }

            var toRemove = thing;
            targetTile.RemoveThing(ref toRemove, thing.Count);

            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, targetTile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, targetTile.Location)), targetTile.Location);
        }

        public static void DeleteOnMap(Location location, ushort itemId)
        {
            var targetTile = Game.Instance.GetTileAt(location);

            if (targetTile == null)
            {
                return;
            }

            targetTile.BruteRemoveItemWithId(itemId);

            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, location, Game.Instance.GetMapTileDescription(conn.PlayerId, location)), targetTile.Location);
        }

        public static void MonsterOnMap(Location location, ushort monsterId)
        {
            var monster = MonsterFactory.Create(monsterId);

            if (monster != null)
            {
                IThing monsterAsThing = monster;
                var tile = Game.Instance.GetTileAt(location);

                if (tile == null)
                {
                    Console.WriteLine($"Unable to place monster at {location}, no tile there.");
                    return;
                }

                // place the monster.
                tile.AddThing(ref monsterAsThing);

                if (!Game.Instance.Creatures.TryAdd(monster.CreatureId, monster))
                {
                    Console.WriteLine($"WARNING: Failed to add {monster.Article} {monster.Name} to the global dictionary.");
                }

                Game.Instance.NotifySpectatingPlayers(conn => new CreatureAddedNotification(conn, monster, EffectT.BubbleBlue), monster.Location);
            }
        }

        public static void Change(ref IThing thing, ushort toItemId, byte unknown)
        {
            if (thing == null)
            {
                return;
            }

            var targetTile = thing.Tile;
            IThing newThing = ItemFactory.Create(toItemId);

            if (targetTile == null || newThing == null)
            {
                return;
            }

            var toRemove = thing;
            targetTile.RemoveThing(ref toRemove);
            targetTile.AddThing(ref newThing);

            thing = newThing;

            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, targetTile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, targetTile.Location)), targetTile.Location);
        }

        public static void ChangeRel(IThing fromThing, Location locationOffset, ushort fromItemId, ushort toItemId, byte unknown)
        {
            if (fromThing == null)
            {
                return;
            }

            var targetTile = Game.Instance.GetTileAt(fromThing.Location + locationOffset);
            IThing newThing = ItemFactory.Create(toItemId);

            if (targetTile == null || newThing == null)
            {
                return;
            }

            targetTile.BruteRemoveItemWithId(fromItemId);
            targetTile.AddThing(ref newThing);

            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, targetTile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, targetTile.Location)), targetTile.Location);
        }

        public static void Damage(IThing damagingThing, IThing damagedThing, byte damageSourceType, ushort damageValue)
        {
            // TODO: implement correctly when combat is...
            var damagedCreature = damagedThing as ICreature;

            if (damagedCreature == null)
            {
                return;
            }

            switch (damageSourceType)
            {
                default: // physical
                    break;
                case 2: // magic? or mana?
                    break;
                case 4: // fire instant
                    Effect(damagedThing, (byte)EffectT.Flame);
                    break;
                case 8: // energy instant
                    Effect(damagedThing, (byte)EffectT.DamageEnergy);
                    break;
                case 16: // poison instant?
                    Effect(damagedThing, (byte)EffectT.RingsGreen);
                    break;
                case 32: // poison over time (poisoned condition)
                    break;
                case 64: // fire over time (burned condition)
                    break;
                case 128: // energy over time (electrified condition)
                    break;
            }
        }

        public static void Logout(IPlayer user)
        {
            Game.Instance.AttemptLogout(user); // TODO: force?
        }

        public static void Move(IThing thingToMove, Location targetLocation)
        {
            if (thingToMove == null)
            {
                return;
            }

            var thingAsCreature = thingToMove as ICreature;
            var thingAsItem = thingToMove as IItem;

            if (thingAsCreature != null)
            {
                Game.Instance.ScheduleEvent(new CreatureMovementOnMap(0, thingAsCreature, thingToMove.Location, targetLocation), DelayForFunctions);
            }
            else if (thingAsItem != null)
            {
                Game.Instance.ScheduleEvent(new ThingMovementOnMap(0, thingAsItem, thingToMove.Location, thingToMove.Tile.GetStackPosition(thingToMove), targetLocation, thingAsItem.Count), DelayForFunctions);
            }
        }

        public static void MoveRel(ICreature user, IThing objectUsed, Location locationOffset)
        {
            Game.Instance.ScheduleEvent(new ThingMovementOnMap(0, user, user.Location, user.Tile.GetStackPosition(user), objectUsed.Location + locationOffset, 1, true), DelayForFunctions);
        }

        public static void MoveTop(IThing fromThing, Location targetLocation)
        {
            if (fromThing == null)
            {
                return;
            }

            // Move all down items and creatures on tile.
            foreach (var item in fromThing.Tile.DownItems.ToList())
            {
                Game.Instance.ScheduleEvent(new ThingMovementOnMap(0, item, fromThing.Location, fromThing.Tile.GetStackPosition(item), targetLocation), DelayForFunctions);
            }

            foreach (var creatureId in fromThing.Tile.CreatureIds.ToList())
            {
                Game.Instance.ScheduleEvent(new CreatureMovementOnMap(0, Game.Instance.GetCreatureWithId(creatureId), fromThing.Location, targetLocation, true), DelayForFunctions);
            }
        }

        /// <summary>
        /// Moves the top thing in the stack of the <paramref name="fromThing"/>'s <see cref="Thing.Tile"/> to the relative location off of it.
        /// </summary>
        /// <param name="fromThing">The reference <see cref="Thing"/> to move from.</param>
        /// <param name="locationOffset">The <see cref="Location"/> offset to move to.</param>
        public static void MoveTopRel(IThing fromThing, Location locationOffset)
        {
            var targetLocation = fromThing.Location + locationOffset;

            // Move all down items and creatures on tile.
            foreach (var item in fromThing.Tile.DownItems.ToList())
            {
                Game.Instance.ScheduleEvent(new ThingMovementOnMap(0, item, fromThing.Location, fromThing.Tile.GetStackPosition(fromThing), targetLocation, item.Count), DelayForFunctions);
            }

            foreach (var creatureId in fromThing.Tile.CreatureIds.ToList())
            {
                Game.Instance.ScheduleEvent(new CreatureMovementOnMap(0, Game.Instance.GetCreatureWithId(creatureId), fromThing.Location, targetLocation, true), DelayForFunctions);
            }
        }

        public static void MoveTopOnMap(Location fromLocation, ushort itemId, Location toLocation)
        {
            var tile = Game.Instance.GetTileAt(fromLocation);

            if (tile == null)
            {
                return;
            }

            MoveTop(tile.BruteFindItemWithId(itemId), toLocation);
        }

        public static void Text(IThing fromThing, string text, byte textType)
        {
        }

        public static void WriteName(IPlayer user, string format, IThing targetThing)
        {
            // TODO: implement.
        }

        public static void SetStart(IThing thing, Location location)
        {
            if (!(thing is IPlayer))
            {
            }

            // TODO: implement.
        }
    }
}
