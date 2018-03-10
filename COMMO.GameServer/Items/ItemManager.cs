using COMMO.GameServer.OTBParsing;
using COMMO.GameServer.World.Loading;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace COMMO.GameServer.Items {

    public class ItemManager {
        private static ItemManager _instance;

        public static ItemManager GetInstance() {
            if (_instance == null)
                _instance = new ItemManager();

            return _instance;
        }

        private ItemManager() {
        } // Defeat Instantiation

        private bool _loaded = false;

        public bool IsLoaded {
            get { return _loaded; }
        }

        private UInt32 _majorVersion = 0;
        private UInt32 _minorVersion = 0;
        private UInt32 _buildVersion = 0;

        private List<Item> _items = new List<Item>();
        private List<SharedItem> _sharedItems;

        public Item GetItem(ushort uid) {
            return _items[uid];
        }

        public SharedItem GetSharedItem(ushort id) // Returns readOnly
        {
            if (id < 100 || id > _sharedItems.Capacity + 100)
                return null;

            return _sharedItems.AsReadOnly()[id - 100]; //_sharedItems[id]; // Maybe not the best way // NOT WORK
        }

        public bool PushItem(Item item) // This is to push item to be handled by the itemmanager
        {
            if (item == null) /// Maybe just create a CreateItem function here or something like that
                return false;

            //if(item.ID > _items.Length)
            //{ /* Resize _items to insert a new element */ }

            return (_items[item.ID] = item) == item;
        }

        public int GetSharedItemCount() => _sharedItems != null ? _sharedItems.Count : 0;

        public void LoadItems() {
            LoadFromOTB(FileManager.GetFilePath(FileManager.FileType.ITEMS_OTB));
            LoadFromXML(FileManager.GetFilePath(FileManager.FileType.ITEMS_XML));
            _loaded = true;
        }

        private void LoadFromXML(string path) // For now lets use XML
        {
            var rootElement = XElement.Load(path, LoadOptions.SetLineInfo);
            if (rootElement.Name != "items")
                throw new MalformedWorldException(); // Just for now

            foreach (var element in rootElement.Elements()) {
                var firstAttr = element.FirstAttribute; //Maybe just check element.Attribute(name) != null
                var secondAttr = element.FirstAttribute.NextAttribute;
                var multiple = firstAttr.Name == "fromid" && secondAttr.Name == "toid";

                if (element.Name != "item" || (firstAttr.Name != "id" && !multiple))
                    continue;

                var iValues = new int[2]; // Maybe change this
                if (!int.TryParse(firstAttr.Value, out iValues[0])
                    || (multiple && !int.TryParse(secondAttr.Value, out iValues[1])))
                    continue;

                var max = GetSharedItemCount();
                if ((!multiple && (iValues[0] < 100 || iValues[0] > max) || (multiple
                    && (iValues[0] < 100 || iValues[1] > max))))
                    continue; // Invalid range id

                var items = _sharedItems.GetRange(iValues[0] - 100, (!multiple ? 1 : iValues[1] - iValues[0] + 1)); // Maybe - 100 its not working as should. Check it on load OTB, the first 100 elements
                foreach (var item in items) {
                    foreach (var attribute in element.Attributes()) {
                        if (attribute.Name == "name")
                            item.Name = attribute.Value;
                        else if (attribute.Name == "article")
                            item.Article = attribute.Value;
                        else if (attribute.Name == "plural")
                            item.PluralName = attribute.Value;
                    }

                    foreach (var child in element.Elements()) {
                        if (child.Name != "attribute")
                            continue;

                        var key = child.FirstAttribute;
                        var value = key.NextAttribute;
                        if (key.Name != "key" || value.Name != "value")
                            continue;

                        switch (key.Value) {// Maybe use key.Value.asLower and compare this way. Simple and better to peoples. Why force this?
                            case "description":
                            item.Description = value.Value;
                            break;

                            case "type":
                            item.Type = value.Value;
                            break;

                            case "floorchange":
                            case "floorChange":
                            item.FloorChange = value.Value;
                            break;

                            case "effect":
                            item.Effect = value.Value;
                            break;

                            case "field":
                            item.Field = value.Value;
                            break;

                            case "fluidsource":
                            case "fluidSource":
                            item.FluidSource = value.Value;
                            break;

                            case "weaponType":
                            item.WeaponType = value.Value;
                            break;

                            case "shootType":
                            item.ShootType = value.Value;
                            break;

                            case "ammoType":
                            item.AmmoType = value.Value;
                            break;

                            case "partnerDirection":
                            item.PartnerDirection = value.Value;
                            break;

                            case "corpseType":
                            item.CorpseType = value.Value;
                            break;

                            case "slotType":
                            item.SlotType = value.Value;
                            break;

                            case "containerSize":
                            ParseByte(ref item.ContainerSize, value);
                            break;
                            // TODO: Continue and do the rest use the parse methods too
                            case "decayTo":
                            ParseUShort(ref item.DecaytTo, value);
                            break;

                            case "rotateTo":
                            ParseUShort(ref item.RotateTo, value);
                            break;

                            case "destroyTo":
                            ParseUShort(ref item.DestroyTo, value);
                            break;

                            case "writeOnceItemId":
                            ushort.TryParse(value.Value, out item.WriteOnceItemId);
                            break;

                            case "maleSleeper":
                            ushort.TryParse(value.Value, out item.MaleSleeperId);
                            break;

                            case "femaleSleeper":
                            ushort.TryParse(value.Value, out item.FemaleSleeperId);
                            break;

                            case "maxTextLen":
                            case "maxTextLength":
                            ushort.TryParse(value.Value, out item.MaxTextLength);
                            break;

                            case "attack":
                            ushort.TryParse(value.Value, out item.Attack);
                            break;

                            case "defense":
                            ushort.TryParse(value.Value, out item.Defense);
                            break;

                            case "maxHitChance":
                            ushort.TryParse(value.Value, out item.MaxHitChance);
                            break;

                            case "range":
                            ushort.TryParse(value.Value, out item.Range);
                            break;

                            case "leveldoor":
                            case "levelDoor":
                            ushort.TryParse(value.Value, out item.LevelDoor);
                            break;

                            case "weight":
                            uint.TryParse(value.Value, out item.Weight);
                            break;

                            case "duration":
                            uint.TryParse(value.Value, out item.Duration);
                            break;

                            case "damage":
                            uint.TryParse(value.Value, out item.Damage);
                            break;

                            case "damageTicks":
                            uint.TryParse(value.Value, out item.DamageTicks);
                            break;

                            case "damageCount":
                            uint.TryParse(value.Value, out item.DamageCount);
                            break;

                            case "damageStart":
                            uint.TryParse(value.Value, out item.DamageStart);
                            break;

                            case "writeable":
                            bool.TryParse(value.Value, out item.IsWriteable);
                            break;

                            case "replaceable":
                            bool.TryParse(value.Value, out item.IsRepleaceable);
                            break;

                            case "allowDistRead":
                            bool.TryParse(value.Value, out item.AllowDistRead);
                            break;

                            case "allowpickupable":
                            case "allowPickupable":
                            bool.TryParse(value.Value, out item.AllowPickupable);
                            break;

                            case "blocking":
                            case "blockSolid":
                            bool.TryParse(value.Value, out item.BlockSolid);
                            break;

                            case "blockprojectile":
                            case "blockProjectile":
                            bool.TryParse(value.Value, out item.BlockProjectile);
                            break;

                            case "walkStack":
                            bool.TryParse(value.Value, out item.WalkStack);
                            break;
                            /// MAYBE CREATE A SetBool method to tryParse and output a warning if can't.Pass out and value
                            default:
                            Console.WriteLine("[Items.XML] - Invalid attribute: " + key.Value);
                            continue; // in future use a log to output as warning
                        }
                    }
                    _sharedItems[item.Id - 100] = item;
                }
            }
        }

        private void ShowParseWarning(XAttribute attr, String type) {
            var lineInfo = (IXmlLineInfo)attr;
            var info = "";
            if (lineInfo.HasLineInfo())
                info = "at " + lineInfo.LineNumber + ":" + lineInfo.LinePosition;

            Console.WriteLine("[Items.XML] " + info + " - Invalid type in " + attr.Name
                + " = " + attr.Value + ". Expected " + type);
        }

        private void ParseBool(ref bool value, XAttribute attr) {
            if (attr.Value == "0" || attr.Value == "1")
                value = (attr.Value == "1" ? true : false);
            else if (!bool.TryParse(attr.Value, out value))
                ShowParseWarning(attr, "Boolean");
        }

        private void ParseUInt(ref uint value, XAttribute attr) {
            if (!uint.TryParse(attr.Value, out value))
                ShowParseWarning(attr, "UInt32");
        }

        private void ParseUShort(ref ushort value, XAttribute attr) {
            if (!ushort.TryParse(attr.Value, out value))
                ShowParseWarning(attr, "UInt16");
        }

        private void ParseByte(ref byte value, XAttribute attr) {
            if (!byte.TryParse(attr.Value, out value))
                ShowParseWarning(attr, "Byte");
        }

        private void LoadFromOTB(string path) {
            var data = FileManager.ReadFileToByteArray(path);
            var parsingTree = TFSWorldLoader.ParseWorld(data);

            var rootNode = parsingTree.Root;
            var stream = new OTBNodeParsingStream(parsingTree, rootNode);

            ParseOTBVersion(ref stream);
            _sharedItems = new List<SharedItem>(rootNode.Children.Count);

            foreach (var itemNode in rootNode.Children) {
                var itemStream = new OTBNodeParsingStream(parsingTree, itemNode);
                ParseItemNode(ref itemStream);
            }
            _sharedItems.TrimExcess();
        }

        private void ParseOTBVersion(ref OTBNodeParsingStream stream) {
            stream.UnderlayingStream.Skip(4); // Skip flags
            var attr = (OTBAttributes)stream.ReadByte();
            if (attr == OTBAttributes.ROOT_VERSION) {
                var dataSize = stream.ReadUInt16();
                if (dataSize != 140) // VersionInfo : 4 UInt32(4bytes) = 12 + 128 * 1 byte = 140
                    throw new MalformedWorldException(); // Lets use this for now

                _majorVersion = stream.ReadUInt32(); // Otb version
                _minorVersion = stream.ReadUInt32(); // Client version
                _buildVersion = stream.ReadUInt32(); // Build version
                stream.UnderlayingStream.Skip(128); // Skip CSD version
            }
        }

        private void ParseItemNode(ref OTBNodeParsingStream stream) {
            var flags = (SharedItemFlags)stream.ReadUInt32();
            var item = new SharedItem(flags);

            while (!stream.IsOver) {
                var attr = (OTBAttributes)stream.ReadByte();
                var dataSize = stream.ReadUInt16();

                switch (attr) {
                    case OTBAttributes.ITEM_SERVERID:
                    if (dataSize != sizeof(UInt16))
                        throw new MalformedItemNodeException();

                    item.Id = stream.ReadUInt16();

                    if (item.Id > 30000 && item.Id < 30100)
                        item.Id -= 30000; // Correct ID in wrong range I guess

                    break;

                    case OTBAttributes.ITEM_CLIENTID:
                    if (dataSize != sizeof(UInt16))
                        throw new MalformedItemNodeException();

                    item.ClientId = stream.ReadUInt16();
                    break;

                    case OTBAttributes.ITEM_SPEED:
                    if (dataSize != sizeof(UInt16))
                        throw new MalformedItemNodeException();

                    item.Speed = stream.ReadUInt16();
                    break;

                    case OTBAttributes.ITEM_LIGHT2:
                    if (dataSize != 2 * sizeof(UInt16)) // 2 UInt16 = 4 bytes
                        throw new MalformedItemNodeException();

                    item.LightLevel = (Byte)stream.ReadUInt16(); // Read UInt16
                    item.LightColor = (Byte)stream.ReadUInt16(); // But range is only to 255
                    break;

                    case OTBAttributes.ITEM_TOPORDER:
                    if (dataSize != sizeof(Byte))
                        throw new MalformedItemNodeException();

                    item.AlwaysOnTopOrder = stream.ReadByte();
                    break;

                    case OTBAttributes.ITEM_WAREID:
                    if (dataSize != sizeof(UInt16))
                        throw new MalformedItemNodeException();

                    item.WareId = stream.ReadUInt16();
                    break;

                    default:
                    stream.Skip(dataSize);
                    break;
                }
            }

            if (item.Id > _sharedItems.Capacity + 100 || item.Id < 100)
                throw new IndexOutOfRangeException();

            _sharedItems.Insert(item.Id - 100, item);
        }
    }
}