using System;
using System.Collections.Generic;
using System.Linq;

namespace COMMO.Common.Objects
{
    public class Inventory : IContainer
    {
        public Inventory(Player player)
        {
            Player = player;
        }

        public Player Player { get; }

        private readonly IContent[] _contents = new IContent[11];

		public byte AddContent(IContent content) => throw new NotSupportedException();

		public void AddContent(byte index, IContent content)
        {
            _contents[index] = content;

            content.Container = this;
        }

        public void RemoveContent(byte index)
        {
            var content = GetContent(index);

            _contents[index] = null;

            content.Container = null;
        }

        public byte GetIndex(IContent content)
        {
            for (byte index = 0; index < _contents.Length; index++)
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
            for (byte index = 0; index < _contents.Length; index++)
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
            if (index < 0 || index > _contents.Length - 1)
            {
                return null;
            }

            return _contents[index];
        }

        public IEnumerable<IContent> GetContents()
        {
            foreach (var content in _contents)
            {
                if (content != null)
                {
                    yield return content;
                }
            }
        }

		public IEnumerable<Item> GetItems() => _contents.OfType<Item>();

		public IEnumerable< KeyValuePair<byte, IContent> > GetIndexedContents()
        {
            for (byte index = 0; index < _contents.Length; index++)
            {
                if (_contents[index] != null)
                {
                    yield return new KeyValuePair<byte, IContent>( index, _contents[index] );
                }
            }
        }
    }
}