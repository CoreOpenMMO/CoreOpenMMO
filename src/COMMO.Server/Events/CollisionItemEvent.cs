// <copyright file="CollisionItemEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Server.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using COMMO.Data.Contracts;

    internal class CollisionItemEvent : BaseItemEvent
    {
        public ushort ThingIdOfCollision { get; }

        public CollisionItemEvent(IList<string> conditionSet, IList<string> actionSet)
            : base(conditionSet, actionSet)
        {
            var isTypeCondition = Conditions.FirstOrDefault(func => IsTypeFunctionName.Equals(func.FunctionName));

            if (isTypeCondition == null)
            {
                throw new ArgumentNullException($"Unable to find {IsTypeFunctionName} function.");
            }

            ThingIdOfCollision = Convert.ToUInt16(isTypeCondition.Parameters[1]);
        }

        public override ItemEventType Type => ItemEventType.Collision;
    }
}