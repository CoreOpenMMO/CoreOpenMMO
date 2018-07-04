// <copyright file="ThingMovementContainerToSlot.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement
{
    using System;
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;
    using OpenTibia.Server.Movement.EventConditions;
    using OpenTibia.Server.Notifications;

    internal class ThingMovementContainerToSlot : MovementBase
    {
        public ThingMovementContainerToSlot(uint requestorId, IThing thingMoving, Location fromLocation, Location toLocation, byte count = 1)
            : base(requestorId, EvaluationTime.OnExecute)
        {
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(count));
            }

            if (this.Requestor == null)
            {
                throw new ArgumentException("Invalid requestor id.", nameof(requestorId));
            }

            this.Thing = thingMoving;
            this.Count = count;

            this.FromLocation = fromLocation;
            this.FromContainer = (this.Requestor as IPlayer)?.GetContainer(this.FromLocation.Container);
            this.FromIndex = (byte)this.FromLocation.Z;

            this.ToLocation = toLocation;
            this.ToSlot = (byte)this.ToLocation.Slot;

            var droppingItem = this.Requestor.Inventory?[this.ToSlot];

            if (this.FromContainer != null && this.FromContainer.HolderId != this.RequestorId)
            {
                this.Conditions.Add(new GrabberHasEnoughCarryStrengthEventCondition(this.RequestorId, this.Thing, droppingItem));
            }

            this.Conditions.Add(new SlotHasContainerAndContainerHasEnoughCapacityEventCondition(this.RequestorId, droppingItem));
            this.Conditions.Add(new GrabberHasContainerOpenEventCondition(this.RequestorId, this.FromContainer));
            this.Conditions.Add(new ContainerHasItemAndEnoughAmountEventCondition(this.Thing as IItem, this.FromContainer, this.FromIndex, this.Count));

            this.ActionsOnPass.Add(new GenericEventAction(this.MoveContainerToSlot));
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
            var updateItem = this.Thing as IItem;

            if (this.FromContainer == null || updateItem == null || this.Requestor == null)
            {
                return;
            }

            // attempt to remove from the source container
            if (!this.FromContainer.RemoveContent(updateItem.Type.TypeId, this.FromIndex, this.Count, out addedItem))
            {
                return;
            }

            if (addedItem != null)
            {
                updateItem = addedItem;
            }

            IThing currentThing = null;

            // attempt to place the intended item at the slot.
            if (!this.Requestor.Inventory.Add(updateItem, out addedItem, this.ToSlot, this.Count))
            {
                // Something went wrong, add back to the source container...
                if (this.FromContainer.AddContent(updateItem, 0xFF))
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
                if (this.FromContainer.AddContent(addedItem, 0xFF))
                {
                    return;
                }

                // and we somehow failed to re-add it to the source container...
                // throw to the ground.
                currentThing = addedItem;
            }

            this.Requestor.Tile.AddThing(ref currentThing, currentThing.Count);

            // notify all spectator players of that tile.
            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, this.Requestor.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, this.Requestor.Location)), this.Requestor.Location);
        }
    }
}