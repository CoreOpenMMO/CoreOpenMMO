// <copyright file="ConditionalActionRule.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Parsing
{
    using System.Collections.Generic;
    using System.Linq;

    public class ConditionalActionRule
    {
        public IList<string> ConditionSet { get; }

        public IList<string> ActionSet { get; }

        public ConditionalActionRule(IEnumerable<string> conditions, IEnumerable<string> actions)
        {
            this.ConditionSet = conditions.ToList();
            this.ActionSet = actions.ToList();
        }
    }
}
