// <copyright file="ProtocolFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications
{
    using OpenTibia.Communications.Interfaces;

    /// <summary>
    /// Class that provides methods for <see cref="IProtocol"/> creation.
    /// </summary>
    public static class ProtocolFactory
    {
        /// <summary>
        /// Creates an instance of an implementation of <see cref="IProtocol"/> depending on the provided type.
        /// </summary>
        /// <param name="type">The type of protocol to implement.</param>
        /// <param name="handlerFactory">The handler factory to pass down to the protocol instance.</param>
        /// <returns>A new <see cref="IProtocol"/> implementation instance.</returns>
        public static IProtocol CreateForType(OpenTibiaProtocolType type, IHandlerFactory handlerFactory)
        {
            if (type == OpenTibiaProtocolType.LoginProtocol)
            {
                return new LoginProtocol(handlerFactory);
            }

            if (type == OpenTibiaProtocolType.GameProtocol)
            {
                return new GameProtocol(handlerFactory);
            }

            return new ManagementProtocol(handlerFactory);
        }
    }
}
