// <copyright file="PlayerLoginHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers.Management
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Interfaces;
    using OpenTibia.Communications.Packets.Incoming;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    internal class PlayerLoginHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var playerLoginPacket = new ManagementPlayerLoginPacket(message);
            var failure = LoginFailureReason.None;

            using (var otContext = new OpenTibiaDbContext())
            {
                var userRecord = otContext.Users.Where(u => u.Login == playerLoginPacket.AccountNumber && u.Passwd.Equals(playerLoginPacket.Password)).FirstOrDefault();
                var playerRecord = otContext.Players.Where(p => p.Account_Nr == playerLoginPacket.AccountNumber && p.Charname.Equals(playerLoginPacket.CharacterName)).FirstOrDefault();

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
                    var anotherCharacterIsLoggedIn = otContext.Players.Where(p => p.Account_Nr == playerLoginPacket.AccountNumber && p.Online > 0 && !p.Charname.Equals(playerLoginPacket.CharacterName)).Any();

                    if (anotherCharacterIsLoggedIn)
                    {
                        failure = LoginFailureReason.AnotherCharacterIsLoggedIn;
                    }
                }

                if (failure == LoginFailureReason.None)
                {
                    try
                    {
                        // Set player status to online.
                        playerRecord.Online = 1;

                        // Pull guild information
                        var guildMembershipInfo = otContext.GuildMembers.Where(gm => gm.AccountId == playerRecord.Account_Id && gm.Invitation == 0).FirstOrDefault();
                        var guildInfo = guildMembershipInfo == null ? null : otContext.Guilds.Where(g => g.GuildId == guildMembershipInfo.GuildId).FirstOrDefault();
                        var guildMemberCount = guildInfo == null ? 0 : otContext.GuildMembers.Where(gm => gm.Invitation == 0 && gm.GuildId == guildInfo.GuildId).Count();

                        var rankString = string.Empty;

                        if (guildMembershipInfo != null)
                        {
                            switch (guildMembershipInfo.Rank)
                            {
                                default:
                                case 1:
                                    rankString = guildInfo.Rank1;
                                    break;
                                case 2:
                                    rankString = guildInfo.Rank2;
                                    break;
                                case 3:
                                    rankString = guildInfo.Rank3;
                                    break;
                                case 4:
                                    rankString = guildInfo.Rank4;
                                    break;
                                case 5:
                                    rankString = guildInfo.Rank5;
                                    break;
                                case 6:
                                    rankString = guildInfo.Rank6;
                                    break;
                                case 7:
                                    rankString = guildInfo.Rank7;
                                    break;
                                case 8:
                                    rankString = guildInfo.Rank8;
                                    break;
                                case 9:
                                    rankString = guildInfo.Rank9;
                                    break;
                                case 10:
                                    rankString = guildInfo.Rank10;
                                    break;
                            }
                        }

                        var vipCollection = otContext.Buddies.Where(b => b.AccountNr == playerRecord.Account_Nr).Take(100).ToList();

                        // TODO: load "privileges" from somewhere other than user level
                        var privileges = new HashSet<string>();

                        if (userRecord.Premium_Days > 0 || userRecord.Trial_Premium_Days > 0)
                        {
                            privileges.Add("PREMIUM_ACCOUNT");
                        }

                        if (userRecord.Userlevel >= 50)
                        {
                            privileges.Add("HIGHLIGHT_HELP_CHANNEL");
                            privileges.Add("READ_TUTOR_CHANNEL");
                            privileges.Add("SEND_BUGREPORTS");
                            privileges.Add("STATEMENT_ADVERT_MONEY");
                            privileges.Add("STATEMENT_ADVERT_OFFTOPIC");
                            privileges.Add("STATEMENT_CHANNEL_OFFTOPIC");
                            privileges.Add("STATEMENT_INSULTING");
                            privileges.Add("STATEMENT_NON_ENGLISH");
                            privileges.Add("STATEMENT_REPORT");
                            privileges.Add("STATEMENT_SPAMMING");
                            privileges.Add("STATEMENT_VIOLATION_INCITING");

                        }

                        if (userRecord.Userlevel >= 100)
                        {
                            privileges.Add("ALLOW_MULTICLIENT");
                            privileges.Add("BANISHMENT");
                            privileges.Add("CHEATING_ACCOUNT_SHARING");
                            privileges.Add("CHEATING_ACCOUNT_TRADING");
                            privileges.Add("CHEATING_BUG_ABUSE");
                            privileges.Add("CHEATING_GAME_WEAKNESS");
                            privileges.Add("CHEATING_HACKING");
                            privileges.Add("CHEATING_MACRO_USE");
                            privileges.Add("CHEATING_MODIFIED_CLIENT");
                            privileges.Add("CHEATING_MULTI_CLIENT");
                            privileges.Add("CLEANUP_FIELDS");
                            privileges.Add("CREATECHAR_GAMEMASTER");
                            privileges.Add("DESTRUCTIVE_BEHAVIOUR");
                            privileges.Add("FINAL_WARNING");
                            privileges.Add("GAMEMASTER_BROADCAST");
                            privileges.Add("GAMEMASTER_FALSE_REPORTS");
                            privileges.Add("GAMEMASTER_INFLUENCE");
                            privileges.Add("GAMEMASTER_PRETENDING");
                            privileges.Add("GAMEMASTER_THREATENING");
                            privileges.Add("GAMEMASTER_OUTFIT");
                            privileges.Add("HOME_TELEPORT");
                            privileges.Add("IGNORED_BY_MONSTERS");
                            privileges.Add("ILLUMINATE");
                            privileges.Add("INVALID_PAYMENT");
                            privileges.Add("INVULNERABLE");
                            privileges.Add("IP_BANISHMENT");
                            privileges.Add("KEEP_ACCOUNT");
                            privileges.Add("KEEP_CHARACTER");
                            privileges.Add("KEEP_INVENTORY");
                            privileges.Add("MODIFY_GOSTRENGTH");
                            privileges.Add("NAMELOCK");
                            privileges.Add("NAME_BADLY_FORMATTED");
                            privileges.Add("NAME_CELEBRITY");
                            privileges.Add("NAME_COUNTRY");
                            privileges.Add("NAME_FAKE_IDENTITY");
                            privileges.Add("NAME_FAKE_POSITION");
                            privileges.Add("NAME_INSULTING");
                            privileges.Add("NAME_NONSENSICAL_LETTERS");
                            privileges.Add("NAME_NO_PERSON");
                            privileges.Add("NAME_SENTENCE");
                            privileges.Add("NO_ATTACK");
                            privileges.Add("NOTATION");
                            privileges.Add("NO_BANISHMENT");
                            privileges.Add("NO_LOGOUT_BLOCK");
                            privileges.Add("NO_RUNES");
                            privileges.Add("NO_STATISTICS");
                            privileges.Add("READ_GAMEMASTER_CHANNEL");
                            privileges.Add("TELEPORT_TO_CHARACTER");
                            privileges.Add("TELEPORT_TO_MARK");
                            privileges.Add("TELEPORT_VERTICAL");
                            privileges.Add("VIEW_CRIMINAL_RECORD");
                            privileges.Add("ZERO_CAPACITY");
                        }

                        if (userRecord.Userlevel >= 255)
                        {
                            privileges.Add("ALL_SPELLS");
                            privileges.Add("ANONYMOUS_BROADCAST");
                            privileges.Add("APPOINT_CIP");
                            privileges.Add("APPOINT_JGM");
                            privileges.Add("APPOINT_SENATOR");
                            privileges.Add("APPOINT_SGM");
                            privileges.Add("ATTACK_EVERYWHERE");
                            privileges.Add("BOARD_ANONYMOUS_EDIT");
                            privileges.Add("BOARD_MODERATION");
                            privileges.Add("BOARD_PRECONFIRMED");
                            privileges.Add("BOARD_REPORT");
                            privileges.Add("CHANGE_PROFESSION");
                            privileges.Add("CHANGE_SKILLS");
                            privileges.Add("CREATE_OBJECTS");
                            privileges.Add("CIPWATCH_ADMIN");
                            privileges.Add("CIPWATCH_USER");
                            privileges.Add("CLEAR_CHARACTER_INFO");
                            privileges.Add("CLEAR_GUILDS");
                            privileges.Add("CREATECHAR_GOD");
                            privileges.Add("CREATECHAR_TEST");
                            privileges.Add("CREATE_MONEY");
                            privileges.Add("CREATE_MONSTERS");
                            privileges.Add("DELETE_GUILDS");
                            privileges.Add("ENTER_HOUSES");
                            privileges.Add("EXTRA_CHARACTER");
                            privileges.Add("KICK");
                            privileges.Add("KILLING_EXCESSIVE_UNJUSTIFIED");
                            privileges.Add("LEVITATE");
                            privileges.Add("LOG_COMMUNICATION");
                            privileges.Add("MODIFY_BANISHMENT");
                            privileges.Add("OPEN_NAMEDDOORS");
                            privileges.Add("RETRIEVE");
                            privileges.Add("SET_ACCOUNTGROUP_RIGHTS");
                            privileges.Add("SET_ACCOUNT_RIGHTS");
                            privileges.Add("SET_CHARACTERGROUP_RIGHTS");
                            privileges.Add("SET_CHARACTER_RIGHTS");
                            privileges.Add("SHOW_COORDINATE");
                            privileges.Add("SHOW_KEYHOLE_NUMBERS");
                            privileges.Add("SPECIAL_MOVEUSE");
                            privileges.Add("SPOILING_AUCTION");
                            privileges.Add("TELEPORT_TO_COORDINATE");
                            privileges.Add("UNLIMITED_CAPACITY");
                            privileges.Add("UNLIMITED_MANA");
                            privileges.Add("VIEW_ACCOUNT");
                            privileges.Add("VIEW_GAMEMASTER_RECORD");
                            privileges.Add("VIEW_LOG_FILES");
                        }

                        otContext.SaveChanges();

                        this.ResponsePackets.Add(new PlayerLoginSucessPacket
                        {
                            AccountId = playerRecord.Account_Id,
                            CharacterName = playerRecord.Charname,
                            Gender = (byte)(playerRecord.Gender == 1 ? 0x01 : 0x02),
                            Guild = guildInfo?.GuildName,
                            GuildTitle = rankString,
                            PlayerTitle = guildMembershipInfo?.PlayerTitle,
                            VipContacts = vipCollection,
                            PremiumDays = userRecord.Premium_Days + userRecord.Trial_Premium_Days,
                            RecentlyActivatedPremmium = false, // TODO: load this.
                            Privileges = privileges
                        });

                        return;
                    }
                    catch
                    {
                        failure = LoginFailureReason.InternalServerError;
                    }
                }

                this.ResponsePackets.Add(new PlayerLoginRejectionPacket
                {
                    Reason = (byte)failure
                });
            }
        }
    }
}