using System.Collections.Generic;

namespace COMMO.Common.Objects
{
    public class Channel
    {
        public ushort Id { get; set; }

        public string Name { get; set; }


        protected HashSet<Player> _players = new HashSet<Player>();

		public void AddPlayer(Player player) => _players.Add(player);

		public void RemovePlayer(Player player) => _players.Remove(player);

		public bool ContainsPlayer(Player player) => _players.Contains(player);

		public IEnumerable<Player> GetPlayers() => _players;
	}

    public class GuildChannel : Channel
    {
       
    }

    public class PartyChannel : Channel
    {
        
    }

    public class PrivateChannel : Channel
    {
        public Player Owner { get; set; }


        private readonly HashSet<Player> _invitations = new HashSet<Player>();

		public void AddInvitation(Player player) => _invitations.Add(player);

		public void RemoveInvitation(Player player) => _invitations.Remove(player);

		public bool ContainsInvitation(Player player) => _invitations.Contains(player);

		public IEnumerable<Player> GetInvitations() => _invitations;
	}
}