// <copyright file="LoginListener.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications
{
    using OpenTibia.Communications.Interfaces;

    /// <summary>
    /// Class that extends the standard <see cref="OpenTibiaListener"/> for the login protocol.
    /// </summary>
    public class LoginListener : OpenTibiaListener
    {
        private const int DefaultLoginListenerPort = 7171;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginListener"/> class.
        /// </summary>
        /// <param name="handlerFactory">The handler factory that this listener will use.</param>
        /// <param name="port">The port where this listener will listen.</param>
        public LoginListener(IHandlerFactory handlerFactory, int port = DefaultLoginListenerPort)
            : base(port, ProtocolFactory.CreateForType(OpenTibiaProtocolType.LoginProtocol, handlerFactory))
        {
        }
    }
}
