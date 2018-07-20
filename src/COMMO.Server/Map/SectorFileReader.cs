// <copyright file="SectorFileReader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using COMMO.Data.Contracts;
using COMMO.Server.Data.Models.Structs;
using COMMO.Server.Parsing;

namespace COMMO.Server.Map
{
    public class SectorFileReader
    {
        public const char CommentSymbol = '#';
        public const char SectorSeparator = ':';
        public const char PositionSeparator = '-';

        public const string AttributeSeparator = ",";
        public const string AttributeDefinition = "=";

        public static IList<Tile> ReadSector(string fileName, string sectorFileContents, ushort xOffset, ushort yOffset, sbyte z)
        {
            var loadedTilesList = new List<Tile>();

            var lines = sectorFileContents.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (var readLine in lines)
            {
                var inLine = readLine?.Split(new[] { CommentSymbol }, 2).FirstOrDefault();

                // ignore comments and empty lines.
                if (string.IsNullOrWhiteSpace(inLine))
                {
                    continue;
                }

                var data = inLine.Split(new[] { SectorSeparator }, 2);

                if (data.Length != 2)
                {
                    throw new Exception($"Malformed line [{inLine}] in sector file: [{fileName}]");
                }

                var tileInfo = data[0].Split(new[] { PositionSeparator }, 2);
                var tileData = data[1];

                var newTile = new Tile(new Location
                {
                    X = (ushort)(xOffset + Convert.ToUInt16(tileInfo[0])),
                    Y = (ushort)(yOffset + Convert.ToUInt16(tileInfo[1])),
                    Z = z
                });

                // load and add tile flags and contents.
                foreach (var element in CipParser.Parse(tileData))
                {
                    foreach (var attribute in element.Attributes)
                    {
                        if (attribute.Name.Equals("Content"))
                        {
                            newTile.AddContent(attribute.Value);
                        }
                        else
                        {
							// it's a flag

							if (Enum.TryParse(attribute.Name, out TileFlag flagMatch)) {
								newTile.SetFlag(flagMatch);
							} else {
								// TODO: proper logging.
								Console.WriteLine($"Unknown flag [{attribute.Name}] found on tile at location {newTile.Location}.");
							}
						}
                    }
                }

                loadedTilesList.Add(newTile);
            }

            // TODO: proper logging.
            // Console.WriteLine($"Sector file {sectorFileContents.Name}: {loadedTilesList.Count} tiles loaded.");
            return loadedTilesList;
        }
    }
}
