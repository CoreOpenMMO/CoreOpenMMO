// <copyright file="ICipCreature.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Contracts
{
    public interface ICipCreature
    {
        byte Id { get; set; }

        string Race { get; set; }

        string Plural { get; set; }

        string Description { get; set; }
    }
}
