// <copyright file="IAuthenticationInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets
{
    /// <summary>
    /// Interface that represents authentication information.
    /// </summary>
    internal interface IAuthenticationInfo
    {
        /// <summary>
        /// Gets a byte value of currently unknown information.
        /// </summary>
        byte Unknown { get; }

        /// <summary>
        /// Gets the password.
        /// </summary>
        string Password { get; }

        /// <summary>
        /// Gets the world name.
        /// </summary>
        string WorldName { get; }
    }
}