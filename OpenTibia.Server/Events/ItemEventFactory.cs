// <copyright file="ItemEventFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Events
{
    using System;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using static OpenTibia.Server.Parsing.Grammar.EventGrammar;

    public static class ItemEventFactory
    {
        public static IItemEvent Create(MoveUseEvent moveUseEvent)
        {
            if (moveUseEvent == null)
            {
                throw new ArgumentNullException(nameof(moveUseEvent));
            }

            if (moveUseEvent.Rule == null)
            {
                throw new ArgumentNullException(nameof(moveUseEvent.Rule));
            }

            switch (moveUseEvent.Type)
            {
                case ItemEventType.Collision:
                    return new CollisionItemEvent(moveUseEvent.Rule.ConditionSet, moveUseEvent.Rule.ActionSet);
                case ItemEventType.Use:
                    return new UseItemEvent(moveUseEvent.Rule.ConditionSet, moveUseEvent.Rule.ActionSet);
                case ItemEventType.MultiUse:
                    return new MultiUseItemEvent(moveUseEvent.Rule.ConditionSet, moveUseEvent.Rule.ActionSet);
                case ItemEventType.Separation:
                    return new SeparationItemEvent(moveUseEvent.Rule.ConditionSet, moveUseEvent.Rule.ActionSet);
                case ItemEventType.Movement:
                    return new MovementItemEvent(moveUseEvent.Rule.ConditionSet, moveUseEvent.Rule.ActionSet);
            }

            throw new InvalidCastException($"Unsuported type of event on EventFactory {moveUseEvent.Type}");
        }
    }
}
