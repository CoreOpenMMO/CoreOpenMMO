// <copyright file="GameListener.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications
{
    using OpenTibia.Communications.Interfaces;

    /// <summary>
    /// Class that extends the standard <see cref="OpenTibiaListener"/> for the game protocol.
    /// </summary>
    public class GameListener : OpenTibiaListener
    {
        private const int DefaultGameListenerPort = 7172;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameListener"/> class.
        /// </summary>
        /// <param name="handlerFactory">The handler factory that this listener will use.</param>
        /// <param name="port">The port where this listener will listen.</param>
        public GameListener(IHandlerFactory handlerFactory, int port = DefaultGameListenerPort)
            : base(port, ProtocolFactory.CreateForType(OpenTibiaProtocolType.GameProtocol, handlerFactory))
        {
        }
    }
}
