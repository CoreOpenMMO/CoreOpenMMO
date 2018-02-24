namespace COMMO.GameServer.Items {

	public class SharedItem {
		public SharedItemFlags Flags;

		public ushort Id;
		public ushort ClientId;
		public ushort WareId;
		public ushort Speed = 0;

		public byte LightLevel = 0;
		public byte LightColor = 0;
		public byte AlwaysOnTopOrder = 0;

		public SharedItemAbilities Abilities;

		public string Name;
		public string Article;
		public string PluralName;
		public string Description;
		public string Type; // Maybe not string. But for now ok
		public string FloorChange; // Maybe not string. But for now ok
		public string Effect; // Maybe not string. But for now ok
		public string Field; // Maybe not string. But for now ok
		public string FluidSource; // Maybe not string. But for now ok
		public string WeaponType; // Weapons HERE??? // Maybe not string. But for now ok
		public string ShootType; // Weapons HERE??? // Maybe not string. But for now ok
		public string AmmoType; // Weapons HERE??? // Maybe not string. But for now ok
		public string PartnerDirection; // Maybe not string. But for now ok
		public string CorpseType; // Maybe not string. But for now ok
		public string SlotType; // Maybe not string. But for now ok

		public byte ContainerSize = 0; // 255 should be a good max to containzer size

		public ushort DecaytTo = 0;
		public ushort RotateTo = 0;
		public ushort DestroyTo = 0;
		public ushort WriteOnceItemId = 0;
		public ushort MaleSleeperId = 0;
		public ushort FemaleSleeperId = 0;
		public ushort MaxTextLength = 0;
		public ushort Attack = 0; // Weapons HERE???
		public ushort Defense = 0; // Weapons HERE???
		public ushort MaxHitChance = 0; // Weapons HERE???
		public ushort Range = 0; // Weapons HERE???
		public ushort LevelDoor = 0; // Whats? But Okay. // ushort should be okay if level <= 65k

		public uint Weight = 0; // Maybe not so much
		public uint Duration = 0; // In ms, ushort Is not big enough AAAAAAAAAAA. Maybe divide by 10 and make as seconds
		public uint Damage = 0;
		public uint DamageTicks = 0;
		public uint DamageCount = 0; // 65k should be enough. Change it to ushort
		public uint DamageStart = 0; // Not sure what Is. Maybe 65k Is enough too

		public bool IsUseable = false; // default true?
		public bool IsPickupable = false; // default true?
		public bool IsMoveable = false; // default true?
		public bool IsStackable = false; // default true?
		public bool IsVertical = false; // default true?
		public bool IsHorizontal = false; // default true?
		public bool IsHangable = false;
		public bool IsWriteable = false;
		public bool IsRepleaceable = false; // Maybe the default Is true
		public bool IsRotatable = false;
		public bool IsAnimation = false;
		public bool IsReadable = false;
		public bool HasHeight = false; // Maybe hasElevation
		public bool AllowDistRead = false;
		public bool AllowPickupable = false;
		public bool BlockSolid = false;
		public bool BlockProjectile = false;
		public bool BlockPathFind = false;
		public bool WalkStack = true;
		public bool AlwaysOnTop = true;
		public bool LookTrough = false;
		public bool ForceUse = false;

		public SharedItem(SharedItemFlags flags = 0) {
			Flags = flags;
			InitFlags();
			Abilities.Reset();
		}

		private bool HasFlag(SharedItemFlags flag) {
			return (Flags & flag) != 0;
		}

		private void InitFlags() {
			IsUseable = HasFlag(SharedItemFlags.USEABLE);
			IsPickupable = HasFlag(SharedItemFlags.PICKUPABLE);
			IsMoveable = HasFlag(SharedItemFlags.MOVEABLE);
			IsStackable = HasFlag(SharedItemFlags.STACKABLE);
			IsVertical = HasFlag(SharedItemFlags.VERTICAL);
			IsHorizontal = HasFlag(SharedItemFlags.HORIZONTAL);
			IsHangable = HasFlag(SharedItemFlags.HANGABLE);
			IsRotatable = HasFlag(SharedItemFlags.ROTATABLE);
			IsAnimation = HasFlag(SharedItemFlags.ANIMATION);
			IsReadable = HasFlag(SharedItemFlags.READABLE);

			HasHeight = HasFlag(SharedItemFlags.HAS_HEIGHT);

			AllowDistRead = HasFlag(SharedItemFlags.ALLOWDISTREAD);
			AlwaysOnTop = HasFlag(SharedItemFlags.ALWAYSONTOP);
			BlockSolid = HasFlag(SharedItemFlags.BLOCK_SOLID);
			BlockProjectile = HasFlag(SharedItemFlags.BLOCK_PROJECTILE);
			BlockPathFind = HasFlag(SharedItemFlags.BLOCK_PATHFIND);
			ForceUse = HasFlag(SharedItemFlags.FORCEUSE);
			LookTrough = HasFlag(SharedItemFlags.LOOKTHROUGH);
		}
	}
}