// <copyright file="TileNode.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data.Interfaces;
using COMMO.Server.Data.Models.Structs;
using COMMO.Utilities;
using System;
using System.Collections.Generic;

namespace COMMO.Server.Algorithms {
	internal class TileNode : INode {
		private bool _isInClosedList;

		public string SearchId { get; }

		public ITile Tile { get; }

		public int TotalCost => MovementCost + EstimatedCost;

		public int MovementCost { get; private set; }

		public int EstimatedCost { get; private set; }

		public INode Parent { get; set; }

		public IEnumerable<INode> Children {
			get {
				var rng = new Random();
				var children = new List<TileNode>();

				var currentLoc = Tile.Location;

				var offsets = new List<Location>();

				// look at adjacent tiles.
				for (var dx = -1; dx <= 1; dx++) {
					for (var dy = -1; dy <= 1; dy++) {
						// skip the current tile.
						if (dx == 0 && dy == 0) {
							continue;
						}

						offsets.Insert(rng.Next(offsets.Count), new Location { X = dx, Y = dy, Z = 0 });
					}
				}

				foreach (var locOffset in offsets) {
					var tile = Game.Instance.GetTileAt(currentLoc + locOffset);

					if (tile == null) {
						continue;
					}

					children.Add(TileNodeCache.Create(SearchId, tile));
				}

				return children;
			}
		}

		public bool IsInOpenList { get; set; }

		public bool IsInClosedList {
			get {
				// TODO: handle damage types to avoid
				return _isInClosedList || MovementCost > 0 && !Tile.CanBeWalked();
			}

			set {
				_isInClosedList = value;
			}
		}

		public TileNode(string searchId, ITile tile) {
			if (string.IsNullOrWhiteSpace(searchId))
				throw new ArgumentException(nameof(searchId) + " can't be null or whitespace.");
			if (tile == null)
				throw new ArgumentNullException(nameof(tile));

			Tile = tile;
			_isInClosedList = false;
			SearchId = searchId;
		}

		public void SetMovementCost(INode parent) {
			var parentNode = parent as TileNode;

			if (Tile == null || parentNode?.Tile == null) {
				return;
			}

			var locationDiff = Tile.Location - parentNode.Tile.Location;
			var isDiagonal = Math.Min(Math.Abs(locationDiff.X), 1) + Math.Min(Math.Abs(locationDiff.Y), 1) == 2;

			var newCost = parent.MovementCost + (isDiagonal ? 3 : 1);
			MovementCost = MovementCost > 0 ? Math.Min(MovementCost, newCost) : newCost;
		}

		public void SetEstimatedCost(INode goal) {
			var goalNode = goal as TileNode;

			if (Tile == null || goalNode?.Tile == null) {
				return;
			}

			var locationDiff = Tile.Location - goalNode.Tile.Location;

			EstimatedCost = Math.Abs(locationDiff.X) + Math.Abs(locationDiff.Y);
		}

		public bool IsGoal(INode goal) {
			var goalNode = goal as TileNode;

			if (goalNode?.Tile == null) {
				// true to stop ASAP.
				return true;
			}

			var locationDiff = Tile.Location - goalNode.Tile.Location;

			return Math.Abs(locationDiff.X) <= 1 && Math.Abs(locationDiff.Y) <= 1;
		}
	}
}
