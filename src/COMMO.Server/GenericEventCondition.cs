// <copyright file="GenericEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using COMMO.Common.Helpers;
using COMMO.Scheduling.Contracts;

namespace COMMO.Server
{
    internal class GenericEventCondition : IEventCondition
    {
        private Func<bool> condition;

        public GenericEventCondition(Func<bool> condition, string errorMsg = "")
        {
            condition.ThrowIfNull();

            condition = condition;
            ErrorMessage = errorMsg;
        }

        public string ErrorMessage { get; }

        public bool Evaluate()
        {
            return condition();
        }
    }
}