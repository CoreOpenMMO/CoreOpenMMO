// <copyright file="ManagementListener.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications
{
    using OpenTibia.Communications.Interfaces;

    /// <summary>
    /// Class that extends the standard <see cref="OpenTibiaListener"/> for the management protocol.
    /// </summary>
    public class ManagementListener : OpenTibiaListener
    {
        private const int DefaultManagementListenerPort = 17778;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementListener"/> class.
        /// </summary>
        /// <param name="handlerFactory">The handler factory that this listener will use.</param>
        /// <param name="port">The port where this listener will listen.</param>
        public ManagementListener(IHandlerFactory handlerFactory, int port = DefaultManagementListenerPort)
            : base(port, ProtocolFactory.CreateForType(OpenTibiaProtocolType.ManagementProtocol, handlerFactory))
        {
        }
    }
}
