// <copyright file="CustomConvertersFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Scripting
{
    using System;
    using OpenTibia.Server.Data.Models.Structs;

    public static class CustomConvertersFactory
    {
        internal static IConverter GetConverter(Type newType)
        {
            if (newType == typeof(Location))
            {
                return new LocationConverter();
            }

            return null;
        }
    }
}