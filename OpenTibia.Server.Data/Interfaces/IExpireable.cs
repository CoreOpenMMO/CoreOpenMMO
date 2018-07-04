// <copyright file="IExpireable.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Data.Interfaces
{
    using System.Timers;

    public interface IExpireable
    {
        Timer Timer { get; }

        void OnExpire();

        void ExpirationSetup();
    }
}
