// <copyright file="ObjectsFileItemLoader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                                    ItemFlag flagMatch;

                                    var flagName = element.Attributes.FirstOrDefault()?.Name;

                                    if (Enum.TryParse(flagName, out flagMatch))
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

        var baseDataDir = Directory.GetParent(Directory.GetCurrentDirectory()) + "/COMMO.Server/Data";
        var itemFilePath = baseDataDir + "/items/items.otb";
        var itemExtensionFilePath = baseDataDir + "/items/items.xml";

        if (!File.Exists(itemFilePath))
        {
            throw new Exception($"Failed to load {itemFilePath}.");
        }

        var fileTree = OTBDeserializer.DeserializeOTBData(new ReadOnlyMemory<byte>(File.ReadAllBytes(itemFilePath)));
        foreach (var itemChildren in fileTree.Children) {
            var current = new ItemType();
            var itemStream = new OTBParsingStream(itemChildren.Data);

            var flags = itemStream.ReadUInt32();
            current.ParseFlags(flags);

            while (!itemStream.IsOver)
            {
                var attr = itemStream.ReadByte();
                var dataSize = itemStream.ReadUInt16();

                switch (attr)
                {
                    case 0x10: // ServerID 0x10 = 16
                        current.SetId(itemStream.ReadUInt16());
                        break;

                    // ClientId 0x11 = 17 -- unused

                    /*case 0x12: // Name 0x12 = 18
                        current.SetName(itemStream.ReadString());
                        break;*/

                    /*case 0x13: // Description 0x13 = 19
                        current.SetDescription(itemStream.ReadString());
                        break;*/

                    // Speed 0x14 = 20

                    // Slot 0x15 = 21

                    // MaxItems 0x16 = 22

                    /*case 0x17: // Weight 0x17 = 23
                        current.SetAttribute(ItemAttribute.Weight, itemStream.ReadUInt16());
                        break;*/

                    // Weapon 0x18 = 24

                    // Amunition 0x19 = 25

                    // Armor 0x1A = 26

                    // MagicLevel 0x1B = 27

                    // MagicFieldType 0x1C = 28

                    // Writeable 0x1D = 29

                    // RotateTo 0x1E = 30

                    // Decay 0x1F = 31

                    // SpriteHash 0x20 = 32

                    // MinimapColor 0x21 = 33

                    // 07? 0x22 = 34

                    // 08? 0x23 = 35

                    // Light 0x24 = 36

                    //>> 1-byte aligned
                    // Decay2 0x25 = 37  -- deprecated

                    // Weapon2 0x26 = 38 -- deprecated

                    // Amunition2 0x27 = 39 -- deprecated

                    // Armor2 0x28 = 40 -- deprecated

                    // Writeable2 0x29 = 41 -- deprecated

                    /*case 0x2A: // Light2 0x2A = 42
                        current.SetAttribute(ItemAttribute.Brightness, itemStream.ReadByte());
                        current.SetAttribute(ItemAttribute.LightColor, itemStream.ReadByte());
                        break;*/

                    // TopOrder 0x2B = 43

                    // Writeable3 0x2C = 44 -- deprecated
                    //>> end of 1-byte aligned attributes

                    // WareId 0x2D = 45

                    default:
                        itemStream.Skip(dataSize);
                        break;
                }
            }
            itemDictionary.Add(current.TypeId, current);
        }

        foreach (var type in itemDictionary)
        {
            type.Value.LockChanges();
        }

        return itemDictionary;
    }

    }
}
