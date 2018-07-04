// <copyright file="TileNode.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Algorithms
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;
    using OpenTibia.Utilities;

    internal class TileNode : INode
    {
        private bool isInClosedList;

        public string SearchId { get; }

        public ITile Tile { get; }

        public int TotalCost => this.MovementCost + this.EstimatedCost;

        public int MovementCost { get; private set; }

        public int EstimatedCost { get; private set; }

        public INode Parent { get; set; }

        public IEnumerable<INode> Children
        {
            get
            {
                var rng = new Random();
                var children = new List<TileNode>();

                var currentLoc = this.Tile.Location;

                var offsets = new List<Location>();

                // look at adjacent tiles.
                for (var dx = -1; dx <= 1; dx++)
                {
                    for (var dy = -1; dy <= 1; dy++)
                    {
                        // skip the current tile.
                        if (dx == 0 && dy == 0)
                        {
                            continue;
                        }

                        offsets.Insert(rng.Next(offsets.Count), new Location { X = dx, Y = dy, Z = 0 });
                    }
                }

                foreach (var locOffset in offsets)
                {
                    var tile = Game.Instance.GetTileAt(currentLoc + locOffset);

                    if (tile == null)
                    {
                        continue;
                    }

                    children.Add(TileNodeCache.Create(this.SearchId, tile));
                }

                return children;
            }
        }

        public bool IsInOpenList { get; set; }

        public bool IsInClosedList
        {
            get
            {
                // TODO: handle damage types to avoid
                return this.isInClosedList || this.MovementCost > 0 && !this.Tile.CanBeWalked();
            }

            set
            {
                this.isInClosedList = value;
            }
        }

        public TileNode(string searchId, ITile tile)
        {
            searchId.ThrowIfNullOrWhiteSpace(nameof(searchId));
            tile.ThrowIfNull(nameof(tile));

            this.Tile = tile;
            this.isInClosedList = false;
            this.SearchId = searchId;
        }

        public void SetMovementCost(INode parent)
        {
            var parentNode = parent as TileNode;

            if (this.Tile == null || parentNode?.Tile == null)
            {
                return;
            }

            var locationDiff = this.Tile.Location - parentNode.Tile.Location;
            var isDiagonal = Math.Min(Math.Abs(locationDiff.X), 1) + Math.Min(Math.Abs(locationDiff.Y), 1) == 2;

            var newCost = parent.MovementCost + (isDiagonal ? 3 : 1);
            this.MovementCost = this.MovementCost > 0 ? Math.Min(this.MovementCost, newCost) : newCost;
        }

        public void SetEstimatedCost(INode goal)
        {
            var goalNode = goal as TileNode;

            if (this.Tile == null || goalNode?.Tile == null)
            {
                return;
            }

            var locationDiff = this.Tile.Location - goalNode.Tile.Location;

            this.EstimatedCost = Math.Abs(locationDiff.X) + Math.Abs(locationDiff.Y);
        }

        public bool IsGoal(INode goal)
        {
            var goalNode = goal as TileNode;

            if (goalNode?.Tile == null)
            {
                // true to stop ASAP.
                return true;
            }

            var locationDiff = this.Tile.Location - goalNode.Tile.Location;

            return Math.Abs(locationDiff.X) <= 1 && Math.Abs(locationDiff.Y) <= 1;
        }
    }
}
