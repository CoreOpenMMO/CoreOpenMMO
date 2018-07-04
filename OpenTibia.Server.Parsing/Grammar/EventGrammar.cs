// <copyright file="EventGrammar.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Parsing.Grammar
{
    using System;
    using System.Linq;
    using OpenTibia.Data.Contracts;
    using Sprache;

    public class EventGrammar
    {
        public static readonly Parser<MoveUseEvent> Event =
            from rule in CipGrammar.ConditionalActionRule
            select new MoveUseEvent(rule);

        public class MoveUseEvent
        {
            public ItemEventType Type { get; }

            public ConditionalActionRule Rule { get; }

            public MoveUseEvent(ConditionalActionRule rule)
            {
                if (rule == null)
                {
                    throw new ArgumentNullException(nameof(rule));
                }

                var firstCondition = rule.ConditionSet.FirstOrDefault();

                ItemEventType eventType;

                if (!Enum.TryParse(firstCondition, out eventType))
                {
                    throw new ArgumentException("Invalid rule supplied.");
                }

                this.Type = eventType;
                this.Rule = rule;

                rule.ConditionSet.RemoveAt(0); // remove first.
            }
        }
    }
}
