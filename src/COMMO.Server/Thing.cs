// <copyright file="Thing.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data;

using COMMO.Server.Data.Interfaces;
using COMMO.Server.Data.Models.Structs;

namespace COMMO.Server
{
    public abstract class Thing : IThing
    {
        public event OnThingStateChanged OnThingChanged;

        protected ITile tile;
        protected Location location;

        public const ushort CreatureThingId = 0x63;

        public abstract ushort ThingId { get; }

        public abstract byte Count { get; }

        public abstract string InspectionText { get; }

        public abstract string CloseInspectionText { get; }

        public abstract bool CanBeMoved { get; }

        public Location Location {
			get => location;

			protected set {
				var oldValue = location;
				location = value;
				if (oldValue != location) {
					OnThingChanged?.Invoke(this, new ThingStateChangedEventArgs() { PropertyChanged = nameof(Location) });
				}
			}
		}

		public ITile Tile {
			get => tile;

			set {
				if (value != null) {
					Location = value.Location;
				}
				tile = value;
			}
		}

		public void Added()
        {
            // OnThingAdded?.Invoke();
        }

        public void Removed()
        {
            // OnThingRemoved?.Invoke();
        }
    }
}