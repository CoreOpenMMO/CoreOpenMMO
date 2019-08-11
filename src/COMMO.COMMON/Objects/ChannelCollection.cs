using System;
using System.Collections.Generic;
using System.Linq;

namespace COMMO.Common.Objects
{
    public class ChannelCollection
    {
        private readonly List<Channel> _channels = new List<Channel>
        {
            new Channel() { Id = 2, Name = "Tutor" },

            new Channel() { Id = 3, Name = "Rule Violations" },

            new Channel() { Id = 4, Name = "Gamemaster" },

            new Channel() { Id = 5, Name = "Game Chat" },

            new Channel() { Id = 6, Name = "Trade" },

            new Channel() { Id = 7, Name = "Trade-Rookgaard" },

            new Channel() { Id = 8, Name = "Real Life Chat" },

            new Channel() { Id = 9, Name = "Help" }
        };

        private ushort GenerateId()
        {
            for (ushort id = 10; id < 65535; id++)
            {
                if ( !_channels.Any(c => c.Id == id) )
                {
                    return id;
                }
            }

            throw new Exception();
        }

        public void AddChannel(Channel channel)
        {
            if (channel.Id == 0)
            {
                channel.Id = GenerateId();
            }

            _channels.Add(channel);
        }

		public void RemoveChannel(Channel channel) => _channels.Remove(channel);

		public Channel GetChannel(int channelId)
        {
            return GetChannels()
                .Where(c => c.Id == channelId)
                .FirstOrDefault();
        }

        public PrivateChannel GetPrivateChannelByOwner(Player owner)
        {
            return GetPrivateChannels()
                .Where(c => c.Owner == owner)
                .FirstOrDefault();
        }

		public IEnumerable<Channel> GetChannels() => _channels;

		public IEnumerable<PrivateChannel> GetPrivateChannels() => _channels.OfType<PrivateChannel>();

		public IEnumerable<GuildChannel> GetGuildChannels() => _channels.OfType<GuildChannel>();

		public IEnumerable<PartyChannel> GetPartyChannels() => _channels.OfType<PartyChannel>();
	}
}