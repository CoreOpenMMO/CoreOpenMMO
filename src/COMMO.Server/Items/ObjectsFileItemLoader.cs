// <copyright file="ObjectsFileItemLoader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Reflection;
using COMMO.Data.Contracts;
using COMMO.Server.Parsing;
using COMMO.OTB;

namespace COMMO.Server.Items
{
    public class ObjectsFileItemLoader : IItemLoader
    {
        /*
            An item definition starts and ends with blank lines.

            TypeID      = 1 # body container
            Name        = ""
            Flags       = {Container,Take}
            Attributes  = {Capacity=1,Weight=0

         */

        public const char CommentSymbol = '#';
        public const char PropertyValueSeparator = '=';

        public Dictionary<ushort, ItemType> Load(string objectsFileName)
        {
            if (string.IsNullOrWhiteSpace(objectsFileName))
            {
                throw new ArgumentNullException(nameof(objectsFileName));
            }

            var itemDictionary = new Dictionary<ushort, ItemType>();
            var objectsFilePath = "COMMO.Server.Data." + ServerConfiguration.DataFilesDirectory + "." + objectsFileName;

            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(objectsFilePath))
            {
                if (stream == null)
                {
                    throw new Exception($"Failed to load {objectsFilePath}.");
                }

                using (var reader = new StreamReader(stream))
                {
                    var current = new ItemType();

                    foreach (var readLine in reader.ReadToEnd().Split("\r\n".ToCharArray()))
                    {
                        if (readLine == null)
                        {
                            continue;
                        }

                        var inLine = readLine.Split(new[] { CommentSymbol }, 2).FirstOrDefault();

                        // ignore comments and empty lines.
                        if (string.IsNullOrWhiteSpace(inLine))
                        {
                            // wrap up the current ItemType and add it if it has enought properties set:
                            if (current.TypeId == 0 || string.IsNullOrWhiteSpace(current.Name))
                            {
                                continue;
                            }

                            current.LockChanges();
                            itemDictionary.Add(current.TypeId, current);

                            current = new ItemType();
                            continue;
                        }

                        var data = inLine.Split(new[] { PropertyValueSeparator }, 2);

                        if (data.Length != 2)
                        {
                            throw new Exception($"Malformed line [{inLine}] in objects file: [{objectsFilePath}]");
                        }

                        var propName = data[0].ToLower().Trim();
                        var propData = data[1].Trim();

                        switch (propName)
                        {
                            case "typeid":
                                current.SetId(Convert.ToUInt16(propData));
                                break;
                            case "name":
                                current.SetName(propData.Substring(Math.Min(1, propData.Length), Math.Max(0, propData.Length - 2)));
                                break;
                            case "description":
                                current.SetDescription(propData);
                                break;
                            case "flags":
                                foreach (var element in CipParser.Parse(propData))
                                {

									var flagName = element.Attributes.FirstOrDefault()?.Name;

									if (Enum.TryParse(flagName, out ItemFlag flagMatch))
                                    {
                                        current.SetFlag(flagMatch);
                                    }
                                    else
                                    {
                                        // TODO: proper logging.
                                        Console.WriteLine($"Unknown flag [{flagName}] found on item with TypeID [{current.TypeId}].");
                                    }
                                }

                                break;
                            case "attributes":
                                foreach (var attrStr in propData.Substring(Math.Min(1, propData.Length), Math.Max(0, propData.Length - 2)).Split(','))
                                {
                                    var attrPair = attrStr.Split('=');

                                    if (attrPair.Length != 2)
                                    {
                                        throw new InvalidDataException($"Invalid attribute {attrStr}.");
                                    }

                                    current.SetAttribute(attrPair[0], Convert.ToInt32(attrPair[1]));
                                }

                                break;
                        }
                    }

                    // wrap up the last ItemType and add it if it has enought properties set:
                    if (current.TypeId != 0 && string.IsNullOrWhiteSpace(current.Name))
                    {
                        current.LockChanges();
                        itemDictionary.Add(current.TypeId, current);
                    }
                }
            }

            return itemDictionary;
        }

    public Dictionary<ushort, ItemType> LoadOTItems()
    {
        var itemDictionary = new Dictionary<UInt16, ItemType>();
			
		var attrsNotSuported = 0;
		var attrsNotValid = 0;
			
        var fileTree = OTBDeserializer.DeserializeOTBData(new ReadOnlyMemory<byte>(ServerResourcesManager.GetItemsBytes("items.otb")), 4);
        foreach (var itemChildren in fileTree.Children)
        {
            var current = new ItemType();
            var itemStream = new OTBParsingStream(itemChildren.Data);

            var flags = itemStream.ReadUInt32();
            current.ParseOTFlags(flags);

            while (!itemStream.IsOver)
            {
                var attr = itemStream.ReadByte();
                var dataSize = itemStream.ReadUInt16();

                switch ((ItemAttributes)attr)
                {
					case ItemAttributes.ServerId:
                        var serverId = itemStream.ReadUInt16();

						if(serverId == 4535) {
							serverId = 4535;
						}

                        if (serverId > 30000 && serverId < 30100)
                            serverId -= 30000;

						current.SetId(serverId);
                        break;

                    case ItemAttributes.ClientId:
                        current.SetClientId(itemStream.ReadUInt16());
                        break;

                    default:
                        itemStream.Skip(dataSize);
                        break;
                }
            }
            itemDictionary.Add(current.TypeId, current);
        }

       var rootElement = XElement.Load(ServerResourcesManager.GetItems("items.xml"), LoadOptions.SetLineInfo);

        foreach (var element in rootElement.Elements("item"))
        {
            var id = element.Attribute("id");
            var fromId = element.Attribute("fromid");
            var toId = element.Attribute("toid");

            // Malformed element, missing id information, ignore it
            if (id == null && (fromId == null || toId == null))
                continue;

            ushort serverId = 0;
            ushort aplyTo = 1;
            if (id == null)
            {
                // Ignore if can't parse the values or if fromId >= toId
                if (!ushort.TryParse(fromId.Value, out serverId) || !ushort.TryParse(toId.Value, out aplyTo) || serverId >= aplyTo)
                    continue;

                aplyTo -= serverId;
            }
            else
            {
                if (!ushort.TryParse(id.Value, out serverId))
                    continue;
            }

            for (ushort key = serverId; key < serverId + aplyTo; key++)
            {
				if (!itemDictionary.TryGetValue(key, out ItemType current))
					continue;

				var name = element.Attribute("name");
                if (name != null)
                    current.SetName(name.Value);

                foreach (var attribute in element.Elements("attribute"))
                {
                    var attrName = attribute.Attribute("key");
                    var attrValue = attribute.Attribute("value");

                    if (attrName == null || attrValue == null)
                        continue;

                    if (attrName.Value == "description")
                    {
                        current.SetDescription(attrValue.Value);
                        continue;
                    }

                    var lineInfo = (IXmlLineInfo) attribute;
						var attr = OpenTibiaTranslationMap.TranslateAttributeName(attrName.Value, out bool success);

						if (success)
                    {
                        int value = -1;
                        bool setAttr = true;
                        switch (attrName.Value)
                        {
                            case "weaponType":
                                success = current.ParseOTWeaponType(attrValue.Value);
                                setAttr = false;
                                break;

                            case "fluidSource":
                                value = OpenTibiaTranslationMap.TranslateLiquidType(attrValue.Value, out success);
                                break;

                            case "corpseType":
                                value = OpenTibiaTranslationMap.TranslateCorpseType(attrValue.Value, out success);
                                break;

                            case "slotType":
                                value = OpenTibiaTranslationMap.TranslateSlotType(attrValue.Value, out success);
                                break;

                            default:
                                success = int.TryParse(attrValue.Value, out value);
                                break;
                        }

                        if (!success)
						{ 
							attrsNotValid++;
							//Console.WriteLine($"[{Path.GetFileName(itemExtensionFilePath)}:{lineInfo.LineNumber}] \"{attrValue.Value}\" is not a valid value for attribute \"{attrName.Value}\"");
						}
                        else if (setAttr)
                            current.SetAttribute(attr, value);

                    }
					else {
						attrsNotSuported++;
						//Console.WriteLine($"[{Path.GetFileName(itemExtensionFilePath)}:{lineInfo.LineNumber}] Attribute \"{attrName.Value}\" is not supported!");
					} 
                }

            }
        }

        foreach (var type in itemDictionary)
        {
            type.Value.LockChanges();
        }

		Console.WriteLine($"Items with attributes not supported: {attrsNotSuported}");
		Console.WriteLine($"Not valid attributes: {attrsNotSuported}");

        return itemDictionary;
    }
		
    }

	public enum ItemAttributes : byte
    {
        ServerId = 0x10,
        ClientId,
        Name,
        Description,
        Speed,
        Slot,
        MaxItems,
        Weight,
        Weapon,
        Ammunition,
        Armor,
        MagicLevel,
        MagicFieldType,
        Writeable,
        RotateTo,
        Decay,
        SpriteHash,
        MiniMapColor,
        Attr07,
        Attr08,
        Light,

        //1-byte aligned
        Decay2, //deprecated
        Weapon2, //deprecated
        Ammunition2, //deprecated
        Armor2, //deprecated
        Writeable2, //deprecated
        Light2,
        TopOrder,
        Writeable3, //deprecated

        WareId
    }
}
