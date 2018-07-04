// <copyright file="MapConstants.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Contracts
{
    public class MapConstants
    {
        public const byte DefaultMapWindowSizeX = 18;
        public const byte DefaultMapWindowSizeY = 14;

        public const int MaxViewInX = 11;         // min value: maxClientViewportX + 1
        public const int MaxViewInY = 11;         // min value: max
        public const int MaxClientViewportX = 8;
        public const int MaxClientViewportY = 6;
    }
}
