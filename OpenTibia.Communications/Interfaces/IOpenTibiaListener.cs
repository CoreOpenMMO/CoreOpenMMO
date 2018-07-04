// <copyright file="IOpenTibiaListener.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Interfaces
{
    using System;

    /// <summary>
    /// Common interface of all open tibia listeners.
    /// </summary>
    public interface IOpenTibiaListener
    {
        /// <summary>
        /// Handles the result after the socket connection has been accepted.
        /// </summary>
        /// <param name="ar">The result of the connection.</param>
        void OnAccept(IAsyncResult ar);

        /// <summary>
        /// Begins listening for requests.
        /// </summary>
        void BeginListening();

        /// <summary>
        /// Stops listening for requests.
        /// </summary>
        void EndListening();
    }
}