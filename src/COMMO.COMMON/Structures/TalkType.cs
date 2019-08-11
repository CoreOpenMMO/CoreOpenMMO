namespace COMMO.Common.Structures
{
    public enum TalkType : byte
    {
        Say = 1,

        Whisper = 2,

        Yell = 3,

        PrivatePlayerToNpc = 4,

        PrivateNpcToPlayer = 5,

        Private = 6, 

        ChannelYellow = 7,

        ChannelWhite = 8,

        ReportRuleViolationOpen = 9,

        ReportRuleViolationAnswer = 10,

        ReportRuleViolationQuestion = 11,

        Broadcast = 12,

        ChannelRed = 13,

        PrivateRed = 14,

        ChannelOrange = 15,

        ChannelRedAnonymous = 17,

        MonsterSay = 19,

        MonsterYell = 20
    }
}