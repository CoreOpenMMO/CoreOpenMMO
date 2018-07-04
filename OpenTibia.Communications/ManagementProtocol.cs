// <copyright file="ManagementProtocol.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications
{
    using OpenTibia.Communications.Interfaces;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    internal class ManagementProtocol : OpenTibiaProtocol
    {
        public override bool KeepConnectionOpen => true;

        public ManagementProtocol(IHandlerFactory handlerFactory)
            : base(handlerFactory)
        {
        }

        public override void ProcessMessage(Connection connection, NetworkMessage inboundMessage)
        {
            LoginOrManagementIncomingPacketType packetType = (LoginOrManagementIncomingPacketType)inboundMessage.GetByte();

            // TODO: move this validation?
            if (packetType != LoginOrManagementIncomingPacketType.AuthenticationRequest && !connection.IsAuthenticated)
            {
                connection.Close();
                return;
            }

            var handler = this.HandlerFactory.CreateIncommingForType((byte)packetType);

            handler?.HandleMessageContents(inboundMessage, connection);

            if (handler?.ResponsePackets != null)
            {
                // Send any responses prepared for this.
                NetworkMessage message = new NetworkMessage();

                foreach (var outPacket in handler.ResponsePackets)
                {
                    message.AddPacket(outPacket);
                }

                connection.Send(message);
            }
        }
    }
}
