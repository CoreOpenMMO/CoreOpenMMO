// <copyright file="ItemEventFunction.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Server.Events
{
    using COMMO.Server.Data.Interfaces;

    internal class ItemEventFunction : IItemEventFunction
    {
        public string FunctionName { get; }

        public object[] Parameters { get; }

        public ItemEventFunction(string name, params object[] parameters)
        {
            FunctionName = name;
            Parameters = parameters;
        }
    }
}