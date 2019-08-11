using System;

namespace COMMO.Common.Events
{
    public enum DisconnetionType
    {
        Requested,

        SocketClosed,

        SocketException
    }

    public class DisconnectedEventArgs : EventArgs
    {
        public DisconnectedEventArgs(DisconnetionType type)
        {
            Type = type;
        }

        public DisconnetionType Type { get; }
    }
}