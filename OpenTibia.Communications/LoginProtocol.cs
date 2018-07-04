// <copyright file="LoginProtocol.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using OpenTibia.Communications.Interfaces;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Communications.Packets.Incoming;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Configuration;
    using OpenTibia.Data;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    internal class LoginProtocol : OpenTibiaProtocol
    {
        public override bool KeepConnectionOpen => false;

        public LoginProtocol(IHandlerFactory handlerFactory)
            : base(handlerFactory)
        {
        }

        public override void ProcessMessage(Connection connection, NetworkMessage inboundMessage)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (inboundMessage == null)
            {
                throw new ArgumentNullException(nameof(inboundMessage));
            }

            LoginOrManagementIncomingPacketType packetType = (LoginOrManagementIncomingPacketType)inboundMessage.GetByte();

            if (packetType != LoginOrManagementIncomingPacketType.LoginServerRequest)
            {
                // This packet should NOT have been routed to this protocol.
                Trace.TraceWarning("Non LoginServerRequest packet routed to LoginProtocol. Packet was ignored.");
                return;
            }

            NewConnectionPacket newConnPacket = new NewConnectionPacket(inboundMessage);
            var gameConfig = ServiceConfiguration.GetConfiguration();

            if (newConnPacket.Version != gameConfig.ClientVersionInt)
            {
                // TODO: hardcoded messages.
                this.SendDisconnect(connection, $"You need client version {gameConfig.ClientVersionString} to connect to this server.");
                return;
            }

            // Make a copy of the message in case we fail to decrypt using the first set of keys.
            var messageCopy = NetworkMessage.Copy(inboundMessage);

            inboundMessage.RsaDecrypt(useCipKeys: gameConfig.UsingCipsoftRsaKeys);

            if (inboundMessage.GetByte() != 0) // means the RSA decrypt was unsuccessful, lets try with the other set of RSA keys...
            {
                inboundMessage = messageCopy;

                inboundMessage.RsaDecrypt(useCipKeys: !gameConfig.UsingCipsoftRsaKeys);

                if (inboundMessage.GetByte() != 0)
                {
                    // These RSA keys are also unsuccessful... give up.
                    // loginPacket = new AccountLoginPacket(inboundMessage);

                    // connection.SetXtea(loginPacket?.XteaKey);

                    //// TODO: hardcoded messages.
                    // if (gameConfig.UsingCipsoftRSAKeys)
                    // {
                    //    this.SendDisconnect(connection, $"The RSA encryption keys used by your client cannot communicate with this game server.\nPlease use an IP changer that does not replace the RSA Keys.\nWe recommend using Tibia Loader's 7.7 client.\nYou may also download the client from out website.");
                    // }
                    // else
                    // {
                    //    this.SendDisconnect(connection, $"The RSA encryption keys used by your client cannot communicate with this game server.\nPlease use an IP changer that replaces the RSA Keys.\nWe recommend using OTLand's IP changer with a virgin 7.7 client.\nYou may also download the client from out website.");
                    // }
                    return;
                }
            }

            IAccountLoginInfo loginPacket = new AccountLoginPacket(inboundMessage);

            connection.SetXtea(loginPacket.XteaKey);

            using (OpenTibiaDbContext otContext = new OpenTibiaDbContext())
            {
                // validate credentials.
                var user = otContext.Users.FirstOrDefault(u => u.Login == loginPacket.AccountNumber && u.Passwd.Equals(loginPacket.Password));

                if (user == null)
                {
                    // TODO: hardcoded messages.
                    this.SendDisconnect(connection, "Please enter a valid account number and password.");
                }
                else
                {
                    var charactersFound = otContext.Players.Where(p => p.Account_Nr == user.Login);

                    if (!charactersFound.Any())
                    {
                        // TODO: hardcoded messages.
                        this.SendDisconnect(connection, $"Your account does not have any characters.\nPlease create a new character in our web site first: {gameConfig.WebsiteUrl}");
                    }
                    else
                    {
                        var charList = new List<ICharacterListItem>();

                        foreach (var character in charactersFound)
                        {
                            charList.Add(new CharacterListItem(character.Charname, gameConfig.PublicGameIpAddress, gameConfig.PublicGamePort, gameConfig.WorldName));
                        }

                        // TODO: motd
                        this.SendCharacterList(connection, gameConfig.MessageOfTheDay, (ushort)Math.Min(user.Premium_Days + user.Trial_Premium_Days, ushort.MaxValue), charList);
                    }
                }
            }
        }

        private void SendDisconnect(Connection connection, string reason)
        {
            NetworkMessage message = new NetworkMessage(4);
            message.AddPacket(new LoginServerDisconnectPacket
            {
                Reason = reason
            });

            connection.Send(message);
        }

        private void SendCharacterList(Connection connection, string motd, ushort premiumDays, IEnumerable<ICharacterListItem> chars)
        {
            NetworkMessage message = new NetworkMessage(4);

            if (motd != string.Empty)
            {
                message.AddPacket(new MessageOfTheDayPacket
                {
                    MessageOfTheDay = motd
                });
            }

            message.AddPacket(new CharacterListPacket
            {
                Characters = chars,
                PremiumDaysLeft = premiumDays
            });

            connection.Send(message);
        }
    }
}
