// <copyright file="ObjectsFileItemLoader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Items
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Parsing;

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
            var objectsFilePath = "OpenTibia.Server.Data." + ServerConfiguration.DataFilesDirectory + "." + objectsFileName;

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
    }
}
