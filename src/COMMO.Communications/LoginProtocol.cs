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

			//var version = 1098;

			//if (version > 772) 
			//{
			//	inboundMessage.SkipBytes(4);
			//}

			var packetType = (LoginOrManagementIncomingPacketType) inboundMessage.GetByte();

			if (packetType != LoginOrManagementIncomingPacketType.LoginServerRequest) {

				inboundMessage.SkipBytes(3);

				packetType = (LoginOrManagementIncomingPacketType) inboundMessage.GetByte();

				if (packetType != LoginOrManagementIncomingPacketType.LoginServerRequest) {
					// This packet should NOT have been routed to this protocol.
					Trace.TraceWarning("Non LoginServerRequest packet routed to LoginProtocol. Packet was ignored.");
					return;
				}
			}

			var newConnPacket = new NewConnectionPacket(inboundMessage);
            var gameConfig = ServiceConfiguration.GetConfiguration();

			if (gameConfig.ReceivedClientVersionInt < gameConfig.ClientMinVersionInt || gameConfig.ReceivedClientVersionInt > gameConfig.ClientMaxVersionInt) {

				//ResponsePackets.Add(new GameServerDisconnectPacket {
				//	Reason = $"You need client version in between {gameConfig.ClientMinVersionString} and {gameConfig.ClientMaxVersionString} to connect to this server."
				//});

				return;
			}

			// Make a copy of the message in case we fail to decrypt using the first set of keys.

			var messageCopy = NetworkMessage.Copy(inboundMessage);
				
			inboundMessage.RsaDecrypt(useRsa2: true);

            if (inboundMessage.GetByte() != 0)
            {

				inboundMessage = messageCopy;

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
						Premium_Days = 0
		            };

		            otContext.Users.Add(u);
		            otContext.SaveChanges();

		            var p = new PlayerModel()
		            {
			            Account_Id = 1,
			            Player_Id = 1,
			            Account_Nr = 1,
			            Charname = "Player 1",
						Level = 10,
						Comment = ""
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
			            Premium_Days = 0
		            };

		            otContext.Users.Add(u2);
		            otContext.SaveChanges();

		            var p2 = new PlayerModel()
		            {
			            Account_Id = 2,
			            Player_Id = 2,
			            Account_Nr = 2,
			            Charname = "Player 2",
			            Level = 10,
			            Comment = ""
		            };

		            otContext.Players.Add(p2);
		            otContext.SaveChanges();

					var u3 = new User() {
						Id = 3,
						Email = "3",
						Login = 3,
						Passwd = "3",
						Userlevel = 50,
						Premium = 1,
						Premium_Days = 100
					};

					otContext.Users.Add(u3);
					otContext.SaveChanges();

					var p3 = new PlayerModel() {
						Account_Id = 3,
						Player_Id = 3,
						Account_Nr = 3,
						Charname = "TESTE",
						Level = 50,
						Comment = "Teste"
					};

					otContext.Players.Add(p3);
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
                        SendCharacterList(connection, user.Login, user.Passwd, gameConfig.MessageOfTheDay, (ushort)Math.Min(user.Premium_Days + user.Trial_Premium_Days, ushort.MaxValue), charList);
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

        private void SendCharacterList(Connection connection, int login, string password, string motd, ushort premiumDays, IEnumerable<ICharacterListItem> chars)
        {
            var message = new NetworkMessage(4);

			long ticks = DateTime.Now.Ticks / 30;

			var token = "";

			if (ServiceConfiguration.GetConfiguration().ReceivedClientVersionInt > 1071) {

				message = new NetworkMessage(8);

				//Add session key
				if (!string.IsNullOrEmpty(password)) {
					//    if (token.empty() || !(token == generateToken(account.key, ticks) || token == generateToken(account.key, ticks - 1) || token == generateToken(account.key, ticks + 1)))
					//    {
					//        output->addByte(0x0D);
					//        output->addByte(0);
					//        send(output);
					//        disconnect();
					//        return;
					//    }
					message.AddByte(0x0C);
					message.AddByte(0);
				}
			}

			////Update premium days
			//Game::updatePremium(account);

			if (motd != string.Empty) {
				message.AddPacket(new MessageOfTheDayPacket {
					MessageOfTheDay = motd
				});
			}

			if (ServiceConfiguration.GetConfiguration().ReceivedClientVersionInt > 1071) { //Add session key
				message.AddByte(0x28);
				message.AddString(login + "\n" + password + "\n" + token + "\n" + ticks);
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
