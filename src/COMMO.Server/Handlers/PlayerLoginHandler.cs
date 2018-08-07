// <copyright file="PlayerLoginHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Linq;
using COMMO.Communications;
using COMMO.Communications.Packets.Incoming;
using COMMO.Communications.Packets.Outgoing;
using COMMO.Configuration;
using COMMO.Data;
using COMMO.Data.Contracts;
using COMMO.Server.Data;

namespace COMMO.Server.Handlers
{
    internal class PlayerLoginHandler : IncomingPacketHandler
    {
        public override void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var playerLoginPacket = new PlayerLoginPacket(message);
            connection.SetXtea(playerLoginPacket.XteaKey);

            var gameConfig = ServiceConfiguration.GetConfiguration();

			if (playerLoginPacket.Version < gameConfig.ClientMinVersionInt || playerLoginPacket.Version > gameConfig.ClientMaxVersionInt) {
				ResponsePackets.Add(new GameServerDisconnectPacket {
					Reason = $"You need client version in between {gameConfig.ClientMinVersionString} and {gameConfig.ClientMaxVersionString} to connect to this server."
				});

				return;
			}

			gameConfig.ReceivedClientVersionInt = playerLoginPacket.Version;

            if (Game.Instance.Status == WorldState.Creating)
            {
                ResponsePackets.Add(new GameServerDisconnectPacket
                {
                    Reason = "The game is just starting.\nPlease try again in a few minutes."
                });

                return;
            }

            using (var otContext = new OpenTibiaDbContext())
            {
                var failure = LoginFailureReason.None;

                var userRecord = otContext.Users.FirstOrDefault(u => u.Login == playerLoginPacket.AccountNumber && u.Passwd.Equals(playerLoginPacket.Password));
                var playerRecord = otContext.Players.FirstOrDefault(p => p.Account_Nr == playerLoginPacket.AccountNumber && p.Charname.Equals(playerLoginPacket.CharacterName));

                if (userRecord == null || playerRecord == null)
                {
                    failure = LoginFailureReason.AccountOrPasswordIncorrect;
                }
                else
                {
                    // Check bannishment.
                    if (userRecord.Banished > 0 || userRecord.Bandelete > 0)
                    {
                        // Lift if time is up
                        if (userRecord.Bandelete > 0 || DateTime.FromFileTimeUtc(userRecord.Banished_Until) > DateTime.Now)
                        {
                            failure = LoginFailureReason.Bannished;
                        }
                        else
                        {
                            userRecord.Banished = 0;
                        }
                    }

                    // Check that no other characters from this account are logged in.
                    var anotherCharacterIsLoggedIn = otContext.Players.Any(p => p.Account_Nr == playerLoginPacket.AccountNumber && p.Online > 0 && !p.Charname.Equals(playerLoginPacket.CharacterName));

                    if (anotherCharacterIsLoggedIn)
                    {
                        failure = LoginFailureReason.AnotherCharacterIsLoggedIn;
                    }

                    // Check if game is open to public
                    if (Game.Instance.Status != WorldState.Open)
                    {
                        ResponsePackets.Add(new GameServerDisconnectPacket
                        {
                            Reason = "The game is not open to the public yet.\nCheck for news on our webpage."
                        });

                        return;
                    }
                }

                if (failure == LoginFailureReason.None)
                {
                    try
                    {
                        // Set player status to online.
                        // playerRecord.online = 1;

                        // otContext.SaveChanges();
                        var player = Game.Instance.Login(playerRecord, connection);

                        // set this to allow future packets from this connection.
                        connection.IsAuthenticated = true;
                        connection.PlayerId = player.CreatureId;

                        ResponsePackets.Add(new SelfAppearPacket
                        {
                            CreatureId = player.CreatureId,
                            IsLogin = true,
                            Player = player
                        });

                        // Add MapDescription
                        ResponsePackets.Add(new MapDescriptionPacket
                        {
                            Origin = player.Location,
                            DescriptionBytes = Game.Instance.GetMapDescriptionAt(player, player.Location)
                        });

                        ResponsePackets.Add(new MagicEffectPacket
                        {
                            Location = player.Location,
                            Effect = EffectT.BubbleBlue
                        });

                        ResponsePackets.Add(new PlayerInventoryPacket { Player = player });
                        ResponsePackets.Add(new PlayerStatusPacket { Player = player });
                        ResponsePackets.Add(new PlayerSkillsPacket { Player = player });

                        ResponsePackets.Add(new WorldLightPacket { Level = Game.Instance.LightLevel, Color = Game.Instance.LightColor });

                        ResponsePackets.Add(new CreatureLightPacket { Creature = player });

                        // Adds a text message
                        ResponsePackets.Add(new TextMessagePacket
                        {
                            Type = MessageType.StatusDefault,
                            Message = "This is a test message"
                        });

                        // std::string tempstring = g_config.getString(ConfigManager::LOGIN_MSG);
                        // if (tempstring.size() > 0)
                        // {
                        //    AddTextMessage(msg, MSG_STATUS_DEFAULT, tempstring.c_str());
                        // }

                        // if (player->getLastLoginSaved() != 0)
                        // {
                        //    tempstring = "Your last visit was on ";
                        //    time_t lastLogin = player->getLastLoginSaved();
                        //    tempstring += ctime(&lastLogin);
                        //    tempstring.erase(tempstring.length() - 1);
                        //    tempstring += ".";

                        // AddTextMessage(msg, MSG_STATUS_DEFAULT, tempstring.c_str());
                        // }
                        // else
                        // {
                        //    tempstring = "Welcome to ";
                        //    tempstring += g_config.getString(ConfigManager::SERVER_NAME);
                        //    tempstring += ". Please choose an outfit.";
                        //    sendOutfitWindow(player);
                        // }

                        // Add any Vips here.

                        // for (VIPListSet::iterator it = player->VIPList.begin(); it != player->VIPList.end(); it++)
                        // {
                        //    bool online;
                        //    std::string vip_name;
                        //    if (IOPlayer::instance()->getNameByGuid((*it), vip_name))
                        //    {
                        //        online = (g_game.getPlayerByName(vip_name) != NULL);
                        //
                        // msg->AddByte(0xD2);
                        // msg->AddU32(guid);
                        // msg->AddString(name);
                        // msg->AddByte(isOnline ? 1 : 0);
                        //    }
                        // }

                        // Send condition icons
                        ResponsePackets.Add(new PlayerConditionsPacket { Player = player });

                        return;
                    }
                    catch (Exception ex)
                    {
                        // TODO: propper logging
                        Console.WriteLine(ex);

                        failure = LoginFailureReason.InternalServerError;
                    }
                }

                if (failure != LoginFailureReason.None)
                {
                    ResponsePackets.Add(new GameServerDisconnectPacket
                    {
                        Reason = failure.ToString() // TODO: implement correctly.
                    });
                }
            }
        }
    }
}