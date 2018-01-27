using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml;

using COTS.GameServer.World.Loading;

namespace COTS.GameServer.Items
{
    public class ItemManager
    {
        private static ItemManager _instance;
        public static ItemManager GetInstance()
        {
            if(_instance == null)
                _instance = new ItemManager();

            return _instance;
        }

        private ItemManager() {} // Defeat Instantiation

        private bool _loaded = false;
        public bool isLoaded
        {
            get {return _loaded;}
        }

        private UInt32 majorVersion = 0;
        private UInt32 minorVersion = 0;
        private UInt32 buildVersion = 0;

        private List<Item> _items;
        private List<SharedItem> _sharedItems;

        public Item GetItem(ushort uid)
        {
            return _items[uid];
        }

        public SharedItem GetSharedItem(ushort id) // Returns readOnly
        {
            if(id < 100 || id > _sharedItems.Capacity + 100)
                return null;

            return _sharedItems.AsReadOnly()[id - 100]; //_sharedItems[id]; // Maybe not the best way // NOT WORK
        }

        public bool PushItem(Item item) // This is to push item to be handled by the itemmanager
        {
            if(item == null) /// Maybe just create a CreateItem function here or something like that
                return false;
            
            //if(item.ID > _items.Length)
            //{ /* Resize _items to insert a new element */ }

            return (_items[item.ID] = item) == item;
        }

        public bool DeleteItem(Item item) // This should not exist? Maybe just replace by a blank element with .disposed/removed = true, as the UID cannot be reclaimed!!
        { // Finish this ///////// NOT????
            Item deletedItem;
            if(true) // _items.contains item
                deletedItem = null; // = _items.remove item
            
            return (deletedItem = null) == null;
        }

        public int GetSharedItemCount() => _sharedItems != null ? _sharedItems.Count : 0;

        // Create a LOAD function to call loadOtb and loadXml and set loaded = true

        //TODO:
        // Look at link from discord chat conversation and implement better things. Also use of Explicit and line number information
        public void LoadFromXML(string path) // For now lets use XML
        {
            if (!isLoaded)
                throw new MalformedWorldException(); // Just for now

            XElement rootElement = XElement.Load(@"/home/carlos/forgottenserver/data/items/items-min.xml", LoadOptions.SetLineInfo);
            if (rootElement.Name != "items")
                throw new MalformedWorldException(); // Just for now

            foreach (var element in rootElement.Elements())
            {
                var firstAttr = element.FirstAttribute; //Maybe just check element.Attribute(name) != null
                var secondAttr = element.FirstAttribute.NextAttribute;
                var multiple = firstAttr.Name == "fromid" && secondAttr.Name == "toid";

                if (element.Name != "item" || (firstAttr.Name != "id" && !multiple))
                    continue;

                int[] iValues = new int[2]; // Maybe change this
                if (!int.TryParse(firstAttr.Value, out iValues[0])
                    || (multiple && !int.TryParse(secondAttr.Value, out iValues[1])))
                    continue;

                var max = GetSharedItemCount();
                if ((!multiple && (iValues[0] < 100 || iValues[0] > max) || (multiple 
                    && (iValues[0] < 100 || iValues[1] > max))))
                    continue; // Invalid range id

                var items = _sharedItems.GetRange(iValues[0] - 100, (!multiple ? 1 : iValues[1] - iValues[0] + 1)); // Maybe - 100 its not working as should. Check it on load OTB, the first 100 elements
                foreach (var item in items) {
                    foreach (var attribute in element.Attributes())
                    {
                        if (attribute.Name == "name")
                            item.name = attribute.Value;
                        else if (attribute.Name == "article")
                            item.article = attribute.Value;
                        else if (attribute.Name == "plural")
                            item.pluralName = attribute.Value;
                    }

                    foreach (var child in element.Elements())
                    {
                        if (child.Name != "attribute")
                            continue;

                        var key = child.FirstAttribute;
                        var value = key.NextAttribute;
                        if (key.Name != "key" || value.Name != "value")
                            continue;

                        switch (key.Value)
                        {// Maybe use key.Value.asLower and compare this way. Simple and better to peoples. Why force this?
                            case "description":         item.description = value.Value;       break;
                            case "type":                item.type = value.Value;              break;
                            case "floorchange":
                            case "floorChange":         item.floorChange = value.Value;       break;
                            case "effect":              item.effect = value.Value;            break;
                            case "field":               item.field = value.Value;             break;
                            case "fluidsource":
                            case "fluidSource":         item.fluidSource = value.Value;       break;
                            case "weaponType":          item.weaponType = value.Value;        break;
                            case "shootType":           item.shootType = value.Value;         break;
                            case "ammoType":            item.ammoType = value.Value;          break;
                            case "partnerDirection":    item.partnerDirection = value.Value;  break;
                            case "corpseType":          item.corpseType = value.Value;        break;
                            case "slotType":            item.slotType = value.Value;          break;

                            case "containerSize":       ParseByte(ref item.containerSize, value);     break;
                            // TODO: Continue and do the rest use the parse methods too
                            case "decayTo":             ParseUShort(ref item.decaytTo, value);          break;
                            case "rotateTo":            ParseUShort(ref item.rotateTo, value);          break;
                            case "destroyTo":           ParseUShort(ref item.destroyTo, value);         break;
                            case "writeOnceItemId":     ushort.TryParse(value.Value, out item.writeOnceItemId);   break;
                            case "maleSleeper":         ushort.TryParse(value.Value, out item.maleSleeperId);     break;
                            case "femaleSleeper":       ushort.TryParse(value.Value, out item.femaleSleeperId);   break;
                            case "maxTextLen":
                            case "maxTextLength":       ushort.TryParse(value.Value, out item.maxTextLength);     break;
                            case "attack":              ushort.TryParse(value.Value, out item.attack);            break;
                            case "defense":             ushort.TryParse(value.Value, out item.defense);           break;
                            case "maxHitChance":        ushort.TryParse(value.Value, out item.maxHitChance);      break;
                            case "range":               ushort.TryParse(value.Value, out item.range);             break;
                            case "leveldoor":
                            case "levelDoor":           ushort.TryParse(value.Value, out item.levelDoor);         break;

                            case "weight":              uint.TryParse(value.Value, out item.weight);            break;
                            case "duration":            uint.TryParse(value.Value, out item.duration);          break;
                            case "damage":              uint.TryParse(value.Value, out item.damage);            break;
                            case "damageTicks":         uint.TryParse(value.Value, out item.damageTicks);       break;
                            case "damageCount":         uint.TryParse(value.Value, out item.damageCount);       break;
                            case "damageStart":         uint.TryParse(value.Value, out item.damageStart);       break;

                            case "writeable":           bool.TryParse(value.Value, out item.isWriteable);       break;
                            case "replaceable":         bool.TryParse(value.Value, out item.isRepleaceable);    break;
                            case "allowDistRead":       bool.TryParse(value.Value, out item.allowDistRead);     break;
                            case "allowpickupable":
                            case "allowPickupable":     bool.TryParse(value.Value, out item.allowPickupable);   break;
                            case "blocking":
                            case "blockSolid":          bool.TryParse(value.Value, out item.blockSolid);        break;
                            case "blockprojectile":
                            case "blockProjectile":     bool.TryParse(value.Value, out item.blockProjectile);   break;
                            case "walkStack":           bool.TryParse(value.Value, out item.walkStack);         break;
                            /// MAYBE CREATE A SetBool method to tryParse and output a warning if can't.Pass out and value
                            default:    Console.WriteLine("[Items.XML] - Invalid attribute: " + key.Value);     continue; // in future use a log to output as warning
                        }
                    }
                    _sharedItems[item.id - 100] = item;
                }
            }
        }

        private void showParseWarning(XAttribute attr, String type)
        {
            var lineInfo = (IXmlLineInfo) attr;
            var info = "";
            if (lineInfo.HasLineInfo())
                info = "at " + lineInfo.LineNumber + ":" + lineInfo.LinePosition;

            Console.WriteLine("[Items.XML] " + info + " - Invalid type in " + attr.Name
                + " = " + attr.Value + ". Expected " + type);
        }

        private void ParseBool(ref bool value, XAttribute attr)
        {
            if (attr.Value == "0" || attr.Value == "1")
                value = (attr.Value == "1" ? true : false);
            else if (!bool.TryParse(attr.Value, out value))
                showParseWarning(attr, "Boolean");
        }

        private void ParseUInt(ref uint value, XAttribute attr)
        {
            if (!uint.TryParse(attr.Value, out value))
                showParseWarning(attr, "UInt32");
        }

        private void ParseUShort(ref ushort value, XAttribute attr)
        {
            if (!ushort.TryParse(attr.Value, out value))
                showParseWarning(attr, "UInt16");
        }

        private void ParseByte(ref byte value, XAttribute attr)
        {
            if (!byte.TryParse(attr.Value, out value))
                showParseWarning(attr, "Byte");
        }

        public void first100() // Delete, just for test purposes
        {
            for(int i = 0; i <= 100; i++)
            {
                SharedItem it = _sharedItems[i];
                Console.WriteLine(it.id + ":" + it.article + ":" +  it.name + ":" + it.description);
            }
        }

        public void LoadFromOTB(string path)
        {
            byte[] data = FileManager.ReadFileToByteArray(@"/home/carlos/forgottenserver/data/items/items.otb");
            var parsingTree = WorldLoader.ParseWorld(data);

            var rootNode = parsingTree.Root;
            var stream = new WorldParsingStream(parsingTree, rootNode);

            ParseOTBVersion(ref stream);
            _sharedItems = new List<SharedItem>(rootNode.Children.Count);

            foreach (var itemNode in rootNode.Children)
            {
                var itemStream = new WorldParsingStream(parsingTree, itemNode);
                ParseItemNode(ref itemStream);
            }
            _sharedItems.TrimExcess();
            _loaded = true;
        }

        private void ParseOTBVersion(ref WorldParsingStream stream)
        {
            stream.UnderlayingStream.Skip(4); // Skip flags
            var attr = (OTBAttributes) stream.ReadByte();
            if (attr == OTBAttributes.ROOT_VERSION)
            {
                var dataSize = stream.ReadUInt16();
                if (dataSize != 140) // VersionInfo : 4 UInt32(4bytes) = 12 + 128 * 1 byte = 140
                    throw new MalformedWorldException(); // Lets use this for now

                majorVersion = stream.ReadUInt32(); // Otb version
                minorVersion = stream.ReadUInt32(); // Client version
                buildVersion = stream.ReadUInt32(); // Build version
                stream.UnderlayingStream.Skip(128); // Skip CSD version
            }
        }

        private void ParseItemNode(ref WorldParsingStream stream)
        {
            var flags = (SharedItemFlags) stream.ReadUInt32();
            SharedItem item = new SharedItem(flags);

            while (!stream.IsOver)
            {
                var attr = (OTBAttributes) stream.ReadByte();
                var dataSize = stream.ReadUInt16();

                switch (attr)
                {
                    case OTBAttributes.ITEM_SERVERID:
                        if (dataSize != sizeof(UInt16))
                            throw new MalformedItemNodeException();

                        item.id = stream.ReadUInt16();

                        if (item.id > 30000 && item.id < 30100)
                            item.id -= 30000; // Correct ID in wrong range I guess

                        break;

                    case OTBAttributes.ITEM_CLIENTID:
                        if (dataSize != sizeof(UInt16))
                            throw new MalformedItemNodeException();

                        item.clientId = stream.ReadUInt16();
                        break;

                    case OTBAttributes.ITEM_SPEED:
                        if (dataSize != sizeof(UInt16))
                            throw new MalformedItemNodeException();

                        item.speed = stream.ReadUInt16();
                        break;

                    case OTBAttributes.ITEM_LIGHT2:
                        if (dataSize != 2 * sizeof(UInt16)) // 2 UInt16 = 4 bytes
                            throw new MalformedItemNodeException();

                        item.lightLevel = (Byte) stream.ReadUInt16(); // Read UInt16
                        item.lightColor = (Byte) stream.ReadUInt16(); // But range is only to 255
                        break;

                    case OTBAttributes.ITEM_TOPORDER:
                        if (dataSize != sizeof(Byte))
                            throw new MalformedItemNodeException();

                        item.alwaysOnTopOrder = stream.ReadByte();
                        break;

                    case OTBAttributes.ITEM_WAREID:
                        if (dataSize != sizeof(UInt16))
                            throw new MalformedItemNodeException();

                        item.wareId = stream.ReadUInt16();
                        break;

                    default:
                        if(!stream.Skip(dataSize))
                            throw new IndexOutOfRangeException();
                        break;
                }
            }

            if(item.id > _sharedItems.Capacity + 100 || item.id < 100)
                throw new IndexOutOfRangeException();

            _sharedItems.Insert(item.id - 100, item);
        }
    }
}