// <copyright file="UseItemPlayerAction.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Linq;
using COMMO.Communications.Packets.Incoming;
using COMMO.Communications.Packets.Outgoing;
using COMMO.Data.Contracts;
using COMMO.Server.Data.Interfaces;
using COMMO.Server.Data.Models.Structs;
using COMMO.Server.Events;
using COMMO.Server.Scripting;

namespace COMMO.Server.Actions
{
    internal class UseItemPlayerAction : BasePlayerAction
    {
        public UseItemPlayerAction(IPlayer player, ItemUsePacket itemUsePacket, Location retryLocation)
            : base(player, itemUsePacket, retryLocation)
        {
        }

        protected override void InternalPerform()
        {
            var itemUsePacket = Packet as ItemUsePacket;

            IThing thingToUse = null;

            if (itemUsePacket == null)
            {
                return;
            }

            switch (itemUsePacket.FromLocation.Type)
            {
                case LocationType.Ground:
                    thingToUse = Game.Instance.GetTileAt(itemUsePacket.FromLocation)?.GetThingAtStackPosition(itemUsePacket.FromStackPos);
                    break;
                case LocationType.Container:
                    var fromContainer = Player.GetContainer(itemUsePacket.FromLocation.Container);
                    try
                    {
                        thingToUse = fromContainer.Content[fromContainer.Content.Count - itemUsePacket.FromStackPos - 1];
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                    } // Happens when the content list does not contain the thing.
                    break;
                case LocationType.Slot:
                    try
                    {
                        thingToUse = Player.Inventory[Convert.ToByte(itemUsePacket.FromLocation.Slot)];
                    }
                    catch
                    {
                        // ignored
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (thingToUse == null)
            {
                // No thing to use found, exit here.
                return;
            }

            var thingAsItem = thingToUse as IItem;
            var thingAsContainer = thingAsItem as IContainer;

            if (thingAsItem != null && thingAsItem.ChangesOnUse)
            {
                Functions.Change(ref thingToUse, thingAsItem.ChangeOnUseTo, 0);
            }
            else if (thingAsItem != null && thingAsItem.IsContainer && thingAsContainer != null)
            {
                var openContainerId = Player.GetContainerId(thingAsContainer);

                if (openContainerId < 0)
                {
                    // PlayerId doesn't have this container open.
                    switch (itemUsePacket.FromLocation.Type)
                    {
                        case LocationType.Ground:
                            Player.OpenContainer(thingAsContainer);
                            break;
                        case LocationType.Container:
                            Player.OpenContainerAt(thingAsContainer, itemUsePacket.Index);
                            break;
                        case LocationType.Slot:
                            Player.OpenContainerAt(thingAsContainer, itemUsePacket.Index);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    thingAsContainer.OnThingChanged += Player.CheckInventoryContainerProximity;

                    ResponsePackets.Add(new ContainerOpenPacket
                    {
                        ContainerId = (byte)thingAsContainer.GetIdFor(Player.CreatureId),
                        ClientItemId = thingAsItem.ThingId,
                        HasParent = thingAsContainer.Parent != null,
                        Name = thingAsItem.Type.Name,
                        Volume = thingAsContainer.Volume,
                        Contents = thingAsContainer.Content
                    });
                }
                else
                {
                    // Close it.
                    Player.CloseContainerWithId((byte)openContainerId);
                    thingAsContainer.OnThingChanged -= Player.CheckInventoryContainerProximity;

                    ResponsePackets.Add(new ContainerClosePacket
                    {
                        ContainerId = (byte)openContainerId
                    });
                }
            }
            else
            {
                var useEvents = Game.Instance.EventsCatalog[ItemEventType.Use].Cast<UseItemEvent>();

                var candidate = useEvents.FirstOrDefault(e => e.ItemToUseId == itemUsePacket.ClientId && e.Setup(thingToUse, null, Player) && e.CanBeExecuted);

                if (candidate != null)
                {
                    // Execute all actions.
                    candidate.Execute();
                }
            }
        }
    }
}