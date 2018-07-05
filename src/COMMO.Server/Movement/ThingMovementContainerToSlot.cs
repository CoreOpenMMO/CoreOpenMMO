// <copyright file="ThingMovementContainerToSlot.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using COMMO.Scheduling.Contracts;
using COMMO.Server.Data.Interfaces;
using COMMO.Server.Data.Models.Structs;
using COMMO.Server.Movement.EventConditions;
using COMMO.Server.Notifications;

namespace COMMO.Server.Movement
{
    internal class ThingMovementContainerToSlot : MovementBase
    {
        public ThingMovementContainerToSlot(uint requestorId, IThing thingMoving, Location fromLocation, Location toLocation, byte count = 1)
            : base(requestorId, EvaluationTime.OnExecute)
        {
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(count));
            }

            if (Requestor == null)
            {
                throw new ArgumentException("Invalid requestor id.", nameof(requestorId));
            }

            Thing = thingMoving;
            Count = count;

            FromLocation = fromLocation;
            FromContainer = (Requestor as IPlayer)?.GetContainer(FromLocation.Container);
            FromIndex = (byte)FromLocation.Z;

            ToLocation = toLocation;
            ToSlot = (byte)ToLocation.Slot;

            var droppingItem = Requestor.Inventory?[ToSlot];

            if (FromContainer != null && FromContainer.HolderId != RequestorId)
            {
                Conditions.Add(new GrabberHasEnoughCarryStrengthEventCondition(RequestorId, Thing, droppingItem));
            }

            Conditions.Add(new SlotHasContainerAndContainerHasEnoughCapacityEventCondition(RequestorId, droppingItem));
            Conditions.Add(new GrabberHasContainerOpenEventCondition(RequestorId, FromContainer));
            Conditions.Add(new ContainerHasItemAndEnoughAmountEventCondition(Thing as IItem, FromContainer, FromIndex, Count));

            ActionsOnPass.Add(new GenericEventAction(MoveContainerToSlot));
        }

        public Location FromLocation { get; }

        public IContainer FromContainer { get; }

        public byte FromIndex { get; }

        public Location ToLocation { get; }

        public byte ToSlot { get; }

        public IThing Thing { get; }

        public byte Count { get; }

        private void MoveContainerToSlot()
        {
            IItem addedItem;
            var updateItem = Thing as IItem;

            if (FromContainer == null || updateItem == null || Requestor == null)
            {
                return;
            }

            // attempt to remove from the source container
            if (!FromContainer.RemoveContent(updateItem.Type.TypeId, FromIndex, Count, out addedItem))
            {
                return;
            }

            if (addedItem != null)
            {
                updateItem = addedItem;
            }

            IThing currentThing = null;

            // attempt to place the intended item at the slot.
            if (!Requestor.Inventory.Add(updateItem, out addedItem, ToSlot, Count))
            {
                // Something went wrong, add back to the source container...
                if (FromContainer.AddContent(updateItem, 0xFF))
                {
                    return;
                }

                // and we somehow failed to re-add it to the source container...
                // throw to the ground.
                currentThing = updateItem;
            }
            else
            {
                // added the new item to the slot
                if (addedItem == null)
                {
                    return;
                }

                // we exchanged or got some leftover item, place back in the source container at any index.
                if (FromContainer.AddContent(addedItem, 0xFF))
                {
                    return;
                }

                // and we somehow failed to re-add it to the source container...
                // throw to the ground.
                currentThing = addedItem;
            }

            Requestor.Tile.AddThing(ref currentThing, currentThing.Count);

            // notify all spectator players of that tile.
            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, Requestor.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, Requestor.Location)), Requestor.Location);
        }
    }
}