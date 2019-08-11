using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

//using COMMO.FileFormats.Spr;
using COMMO.IO;

namespace COMMO.FileFormats.Dat {
	public class Item {
		public static Item Load(ByteArrayStreamReader reader) {
			var item = new Item();

			while (true) {
				switch ((DatAttribute) reader.ReadByte()) {
					case DatAttribute.IsGround:

						item.IsGround = true;

						item.Speed = reader.ReadUShort();

						break;

					case DatAttribute.AlwaysOnTop1:

						item.AlwaysOnTop1 = true;

						break;

					case DatAttribute.AlwaysOnTop2:

						item.AlwaysOnTop2 = true;

						break;

					case DatAttribute.AlwaysOnTop3:

						item.AlwaysOnTop3 = true;

						break;

					case DatAttribute.IsContainer:

						item.IsContainer = true;

						break;

					case DatAttribute.Stackable:

						item.Stackable = true;

						break;

					case DatAttribute.Useable:

						item.Useable = true;

						break;

					case DatAttribute.Writeable:

						item.MaxReadWriteChars = reader.ReadUShort();

						break;

					case DatAttribute.Readable:

						item.MaxReadChars = reader.ReadUShort();

						break;

					case DatAttribute.IsFluid:

						item.IsFluid = true;

						break;

					case DatAttribute.IsSplash:

						item.IsSplash = true;

						break;

					case DatAttribute.NotWalkable:

						item.NotWalkable = true;

						break;

					case DatAttribute.NotMoveable:

						item.NotMoveable = true;

						break;

					case DatAttribute.BlockProjectile:

						item.BlockProjectile = true;

						break;

					case DatAttribute.BlockPathFinding:

						item.BlockPathFinding = true;

						break;

					case DatAttribute.Pickupable:

						item.Pickupable = true;

						break;

					case DatAttribute.Hangable:

						item.Hangable = true;

						break;

					case DatAttribute.Horizontal:

						item.Horizontal = true;

						break;

					case DatAttribute.Vertical:

						item.Vertical = true;

						break;

					case DatAttribute.Rotatable:

						item.Rotatable = true;

						break;

					case DatAttribute.Light:

						item.LightLevel = reader.ReadUShort();

						item.LightColor = reader.ReadUShort();

						break;

					case DatAttribute.Offset:

						item.OffsetX = reader.ReadUShort();

						item.OffsetY = reader.ReadUShort();

						break;

					case DatAttribute.HasHeight:

						item.ItemHeight = reader.ReadUShort();

						break;

					case DatAttribute.IdleAnimation:

						item.IdleAnimation = true;

						break;

					case DatAttribute.MinimapColor:

						item.MinimapColor = reader.ReadUShort();

						break;

					case DatAttribute.ExtraInfo:

						item.ExtraInfo = (ExtraInfo) reader.ReadUShort();

						break;

					case DatAttribute.SolidGround:

						item.SolidGround = true;

						break;

					case DatAttribute.LookThrough:

						item.LookThrough = true;

						break;

					case DatAttribute.End:

						item.Width = reader.ReadByte();

						item.Height = reader.ReadByte();

						if (item.Width > 1 || item.Height > 1) {
							item.CropSize = reader.ReadByte();
						}

						item.Layers = reader.ReadByte();

						item.XRepeat = reader.ReadByte();

						item.YRepeat = reader.ReadByte();

						item.ZRepeat = reader.ReadByte();

						item.Animations = reader.ReadByte();

						int sprites = item.Width * item.Height * item.Layers * item.XRepeat * item.YRepeat * item.ZRepeat * item.Animations;

						item.SpriteIds = new List<ushort>(sprites);

						for (int i = 0; i < sprites; i++) {
							item.SpriteIds.Add(reader.ReadUShort());
						}

						return item;
				}
			}
		}

		public ushort TibiaId { get; set; }

		public bool IsGround { get; set; }

		public ushort Speed { get; set; }

		public bool AlwaysOnTop1 { get; set; }

		public bool AlwaysOnTop2 { get; set; }

		public bool AlwaysOnTop3 { get; set; }

		public bool IsContainer { get; set; }

		public bool Stackable { get; set; }

		public bool Useable { get; set; }

		public ushort MaxReadWriteChars { get; set; }

		public ushort MaxReadChars { get; set; }

		public bool IsFluid { get; set; }

		public bool IsSplash { get; set; }

		public bool NotWalkable { get; set; }

		public bool NotMoveable { get; set; }

		public bool BlockProjectile { get; set; }

		public bool BlockPathFinding { get; set; }

		public bool Pickupable { get; set; }

		public bool Hangable { get; set; }

		public bool Horizontal { get; set; }

		public bool Vertical { get; set; }

		public bool Rotatable { get; set; }

		public ushort LightLevel { get; set; }

		public ushort LightColor { get; set; }

		public ushort OffsetX { get; set; }

		public ushort OffsetY { get; set; }

		public ushort ItemHeight { get; set; }

		public bool IdleAnimation { get; set; }

		public ushort MinimapColor { get; set; }

		public ExtraInfo ExtraInfo { get; set; }

		public bool SolidGround { get; set; }

		public bool LookThrough { get; set; }

		public byte Width { get; set; }

		public byte Height { get; set; }

		public byte CropSize { get; set; }

		public byte Layers { get; set; }

		public byte XRepeat { get; set; }

		public byte YRepeat { get; set; }

		public byte ZRepeat { get; set; }

		public byte Animations { get; set; }

        public List<ushort> SpriteIds { get; private set; }

        //public Bitmap GetImage(List<Sprite> sprites, int animation, int z, int y, int x, int layer) {
        //	animation = Math.Min(Animations - 1, animation);

        //	z = Math.Min(ZRepeat - 1, z);

        //	y = Math.Min(YRepeat - 1, y);

        //	x = Math.Min(XRepeat - 1, x);

        //	layer = Math.Min(Layers - 1, layer);

        //	Bitmap bitmap = new Bitmap(32 * Width, 32 * Height);

        //	using (Graphics graphics = Graphics.FromImage(bitmap)) {
        //		/*
        //              int index = ZRepeat * YRepeat * XRepeat * Layers * Width * Height * animation +

        //                          YRepeat * XRepeat * Layers * Width * Height * z +

        //                          XRepeat * Layers * Width * Height * y +

        //                          Layers * Width * Height * x +

        //                          Width * Height * layer;
        //              */

        //		int index = Width * Height * (Layers * (XRepeat * (YRepeat * (ZRepeat * animation + z) + y) + x) + layer);

        //		for (int j = Height - 1; j >= 0; j--) {
        //			for (int i = Width - 1; i >= 0; i--) {
        //				ushort spriteId = _spriteIds[index++];

        //				if (spriteId > 0) {
        //					graphics.DrawImage(sprites.First(sprite => sprite.Id == spriteId).GetImage(), 32 * i, 32 * j);
        //				}
        //			}
        //		}
        //	}

        //	return bitmap;
        //}
    }
}