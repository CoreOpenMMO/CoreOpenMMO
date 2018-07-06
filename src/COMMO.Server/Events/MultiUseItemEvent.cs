// <copyright file="MultiUseItemEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using COMMO.Data.Contracts;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Server.Events
{
    internal class MultiUseItemEvent : BaseItemEvent
    {
        public ushort ItemToUseId { get; }

        public ushort ItemToUseOnId { get; }

        public MultiUseItemEvent(IList<string> conditionSet, IList<string> actionSet)
            : base(conditionSet, actionSet)
        {
            // Look for a IsType condition.
            var isTypeConditions = Conditions.Where(func => IsTypeFunctionName.Equals(func.FunctionName));

            var typeConditionsList = isTypeConditions as IList<IItemEventFunction> ?? isTypeConditions.ToList();
            var firstTypeCondition = typeConditionsList.FirstOrDefault();
            var secondTypeCondition = typeConditionsList.Skip(1).FirstOrDefault();

            if (firstTypeCondition == null)
            {
                throw new ArgumentNullException($"Unable to find first {IsTypeFunctionName} function.");
            }

            if (secondTypeCondition == null)
            {
                throw new ArgumentNullException($"Unable to find second {IsTypeFunctionName} function.");
            }

            ItemToUseId = Convert.ToUInt16(firstTypeCondition.Parameters[1]);
            ItemToUseOnId = Convert.ToUInt16(secondTypeCondition.Parameters[1]);
        }

        public override ItemEventType Type => ItemEventType.MultiUse;
    }
}