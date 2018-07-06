// <copyright file="LoginProtocol.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using COMMO.Communications.Interfaces;
using COMMO.Communications.Packets;
using COMMO.Communications.Packets.Incoming;
using COMMO.Communications.Packets.Outgoing;
using COMMO.Configuration;
using COMMO.Data;
using COMMO.Data.Models;
using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications
{
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

            var packetType = (LoginOrManagementIncomingPacketType)inboundMessage.GetByte();

            if (packetType != LoginOrManagementIncomingPacketType.LoginServerRequest)
            {
                // This packet should NOT have been routed to this protocol.
                Trace.TraceWarning("Non LoginServerRequest packet routed to LoginProtocol. Packet was ignored.");
                return;
            }

            var newConnPacket = new NewConnectionPacket(inboundMessage);
            var gameConfig = ServiceConfiguration.GetConfiguration();

            if (newConnPacket.Version != gameConfig.ClientVersionInt)
            {
                // TODO: hardcoded messages.
                SendDisconnect(connection, $"You need client version {gameConfig.ClientVersionString} to connect to this server.");
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
                    //    SendDisconnect(connection, $"The RSA encryption keys used by your client cannot communicate with this game server.\nPlease use an IP changer that does not replace the RSA Keys.\nWe recommend using Tibia Loader's 7.7 client.\nYou may also download the client from out website.");
                    // }
                    // else
                    // {
                    //    SendDisconnect(connection, $"The RSA encryption keys used by your client cannot communicate with this game server.\nPlease use an IP changer that replaces the RSA Keys.\nWe recommend using OTLand's IP changer with a virgin 7.7 client.\nYou may also download the client from out website.");
                    // }
                    return;
                }
            }

            IAccountLoginInfo loginPacket = new AccountLoginPacket(inboundMessage);

            connection.SetXtea(loginPacket.XteaKey);

            using (var otContext = new OpenTibiaDbContext())
            {
	            if (!otContext.Users.Any()) {
		            var u = new User()
		            {
						Id = 1,
			            Email = "1",
			            Login = 1,
			            Passwd = "1",
						Userlevel = 255,
						Premium = 1,
						Premium_Days = 100
		            };

		            otContext.Users.Add(u);
		            otContext.SaveChanges();

		            var p = new PlayerModel()
		            {
			            Account_Id = 1,
			            Player_Id = 1,
			            Account_Nr = 1,
			            Charname = "MUNIZ",
						Level = 10,
						Comment = "Felipe Muniz"
		            };

		            otContext.Players.Add(p);
		            otContext.SaveChanges();

		            var u2 = new User()
		            {
			            Id = 2,
			            Email = "2",
			            Login = 2,
			            Passwd = "2",
			            Userlevel = 50,
			            Premium = 1,
			            Premium_Days = 100
		            };

		            otContext.Users.Add(u2);
		            otContext.SaveChanges();

		            var p2 = new PlayerModel()
		            {
			            Account_Id = 2,
			            Player_Id = 2,
			            Account_Nr = 2,
			            Charname = "FELIPE",
			            Level = 10,
			            Comment = "Felipe"
		            };

		            otContext.Players.Add(p2);
		            otContext.SaveChanges();
	            }

                // validate credentials.
                var user = otContext.Users.FirstOrDefault(u => u.Login == loginPacket.AccountNumber && u.Passwd.Equals(loginPacket.Password));

                if (user == null)
                {
                    // TODO: hardcoded messages.
                    SendDisconnect(connection, "Please enter a valid account number and password.");
                }
                else
                {
                    var charactersFound = otContext.Players.Where(p => p.Account_Nr == user.Login);

                    if (!charactersFound.Any())
                    {
                        // TODO: hardcoded messages.
                        SendDisconnect(connection, $"Your account does not have any characters.\nPlease create a new character in our web site first: {gameConfig.WebsiteUrl}");
                    }
                    else
                    {
                        var charList = new List<ICharacterListItem>();

                        foreach (var character in charactersFound)
                        {
                            charList.Add(new CharacterListItem(character.Charname, gameConfig.PublicGameIpAddress, gameConfig.PublicGamePort, gameConfig.WorldName));
                        }

                        // TODO: motd
                        SendCharacterList(connection, gameConfig.MessageOfTheDay, (ushort)Math.Min(user.Premium_Days + user.Trial_Premium_Days, ushort.MaxValue), charList);
                    }
                }
            }
        }

        private void SendDisconnect(Connection connection, string reason)
        {
            var message = new NetworkMessage(4);
            message.AddPacket(new LoginServerDisconnectPacket
            {
                Reason = reason
            });

            connection.Send(message);
        }

        private void SendCharacterList(Connection connection, string motd, ushort premiumDays, IEnumerable<ICharacterListItem> chars)
        {
            var message = new NetworkMessage(4);

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
