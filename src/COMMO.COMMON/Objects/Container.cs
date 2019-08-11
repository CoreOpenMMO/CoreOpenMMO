using System;
using System.Collections.Generic;
using System.Linq;

namespace COMMO.Common.Objects
{
    public class Container : Item, IContainer
    {
        public Container(ItemMetadata metadata) : base(metadata)
        {

        }

        private readonly List<IContent> _contents = new List<IContent>();
        
        public byte AddContent(IContent content)
        {
            byte index = 0;

            _contents.Insert(index, content);

            content.Container = this;

            return index;
        }

		public void AddContent(byte index, IContent content) => throw new NotSupportedException();

		public void RemoveContent(byte index)
        {
            var content = GetContent(index);

            _contents.RemoveAt(index);

            content.Container = null;
        }
        
        public byte GetIndex(IContent content)
        {
            for (byte index = 0; index < _contents.Count; index++)
            {
                if (_contents[index] == content)
                {
                    return index;
                }
            }

            throw new Exception();
        }

        public bool TryGetIndex(IContent content, out byte i)
        {
            for (byte index = 0; index < _contents.Count; index++)
            {
                if (_contents[index] == content)
                {
                    i = index;

                    return true;
                }
            }

            i = 0;

            return false;
        }

        public IContent GetContent(byte index)
        {
            if (index < 0 || index > _contents.Count - 1)
            {
                return null;
            }

            return _contents[index];
        }

		public IEnumerable<IContent> GetContents() => _contents;

		public IEnumerable<Item> GetItems() => _contents.OfType<Item>();

		public IEnumerable< KeyValuePair<byte, IContent> > GetIndexedContents()
        {
            for (byte index = 0; index < _contents.Count; index++)
            {
                yield return new KeyValuePair<byte, IContent>( index, _contents[index] );
            }
        }

        protected Dictionary<Player, int> _players = new Dictionary<Player, int>();

        public void AddPlayer(Player player)
        {

			if (!_players.TryGetValue(player, out Int32 references)) {
				references = 1;

				_players.Add(player, references);
			}
			else {
				_players[player] = references + 1;
			}
		}

        public void RemovePlayer(Player player)
        {

			if (_players.TryGetValue(player, out Int32 references)) {
				if (references == 1) {
					_players.Remove(player);
				}
				else {
					_players[player] = references - 1;
				}
			}
		}

		public bool ContainsPlayer(Player player) => _players.ContainsKey(player);

		public IEnumerable<Player> GetPlayers() => _players.Keys;
	}
}