using COMMO.IO;
using System.Collections.Generic;

namespace COMMO.FileFormats.Dat {
	public class DatFile {
		public static DatFile Load(string path) {
			using (var stream = new ByteArrayFileStream(path)) {
				var reader = new ByteArrayStreamReader(stream);

				var file = new DatFile();

				file.Signature = reader.ReadUInt();

				ushort items = reader.ReadUShort();

				ushort outfits = reader.ReadUShort();

				ushort effects = reader.ReadUShort();

				ushort projectiles = reader.ReadUShort();

				file.Items = new List<Item>(items);

				for (ushort itemId = 100; itemId <= items; itemId++) {
					var item = Item.Load(reader);

					item.TibiaId = itemId;

					file.Items.Add(item);
				}

				file.Outfits = new List<Item>(outfits);

				for (ushort outfitId = 0; outfitId < outfits; outfitId++) {
					var item = Item.Load(reader);

					item.TibiaId = outfitId;

					file.Outfits.Add(item);
				}

				file.Effects = new List<Item>(effects);

				for (ushort effectId = 0; effectId < effects; effectId++) {
					var item = Item.Load(reader);

					item.TibiaId = effectId;

					file.Effects.Add(item);
				}

				file.Projectiles = new List<Item>(projectiles);

				for (ushort projectileId = 0; projectileId < projectiles; projectileId++) {
					var item = Item.Load(reader);

					item.TibiaId = projectileId;

					file.Projectiles.Add(item);
				}

				return file;
			}
		}

        public uint Signature { get; private set; }

        public List<Item> Items { get; private set; }

        public List<Item> Outfits { get; private set; }

        public List<Item> Effects { get; private set; }

        public List<Item> Projectiles { get; private set; }
    }
}