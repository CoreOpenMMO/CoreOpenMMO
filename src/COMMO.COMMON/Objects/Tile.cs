using COMMO.Common.Structures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace COMMO.Common.Objects
{
    public class Tile : IContainer
    {
        public Tile(Position position)
        {
            Position = position;
        }

        public Position Position { get; }

        public Item Ground
        {
            get
            {
                return GetItems()
                    .Where(i => i.TopOrder == TopOrder.Ground)
                    .FirstOrDefault();
            }
        }

        private readonly List<IContent> _contents = new List<IContent>(1);
        
        public byte AddContent(IContent content)
        {
            //10 Other
            //11 Other
            //12 Other
            //9 Creature
            //8 Creature
            //7 LowPriority
            //6 LowPriority
            //5 MediumPriority
            //4 MediumPriority
            //3 HighPriority
            //2 HighPriority   
            //1 Ground
            //0 Ground

            byte index = 0;
            
            if (content.TopOrder == TopOrder.Other)
	        {
                while (index < _contents.Count && _contents[index].TopOrder != content.TopOrder)
                {
                    index++;
                }
	        }
            else
            {
                while (index < _contents.Count && _contents[index].TopOrder <= content.TopOrder)
                {
                    index++;
                }
            }

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

		public IEnumerable<Creature> GetCreatures() => _contents.OfType<Creature>();

		public IEnumerable<Monster> GetMonsters() => _contents.OfType<Monster>();

		public IEnumerable<Npc> GetNpcs() => _contents.OfType<Npc>();

		public IEnumerable<Player> GetPlayers() => _contents.OfType<Player>();

		public IEnumerable< KeyValuePair<byte, IContent> > GetIndexedContents()
        {
            for (byte index = 0; index < _contents.Count; index++)
            {
                yield return new KeyValuePair<byte, IContent>( index, _contents[index] );
            }
        }
    }
}