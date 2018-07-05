// <copyright file="ItemEventFunctionComparison.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Server.Events
{
    using COMMO.Data.Contracts;
    using COMMO.Server.Data.Interfaces;

    internal class ItemEventFunctionComparison : IItemEventFunction
    {
        public string FunctionName { get; }

        public object[] Parameters { get; }

        public FunctionComparisonType Type { get; }

        public string CompareToIdentifier { get; }

        public ItemEventFunctionComparison(string name, FunctionComparisonType type, string compareIdentifier, object[] parameters)
        {
            FunctionName = name;
            Type = type;
            CompareToIdentifier = compareIdentifier;
            Parameters = parameters;
        }
    }
}