// <copyright file="IAccountLoginInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets
{
    /// <summary>
    /// Interface for account login information.
    /// </summary>
    public interface IAccountLoginInfo
    {
        /// <summary>
        /// Gets the account number.
        /// </summary>
        uint AccountNumber { get; }

        /// <summary>
        /// Gets the account password.
        /// </summary>
        string Password { get; }

        /// <summary>
        /// Gets the XTea encryption bytes.
        /// </summary>
        uint[] XteaKey { get; }
    }
}
