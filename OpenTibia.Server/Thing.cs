// <copyright file="Thing.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server
{
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

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

        public Location Location
        {
            get
            {
                return this.location;
            }

            protected set
            {
                var oldValue = this.location;
                this.location = value;
                if (oldValue != this.location)
                {
                    this.OnThingChanged?.Invoke(this, new ThingStateChangedEventArgs() { PropertyChanged = nameof(this.Location) });
                }
            }
        }

        public ITile Tile
        {
            get
            {
                return this.tile;
            }

            set
            {
                if (value != null)
                {
                    this.Location = value.Location;
                }
                this.tile = value;
            }
        }

        public void Added()
        {
            // this.OnThingAdded?.Invoke();
        }

        public void Removed()
        {
            // this.OnThingRemoved?.Invoke();
        }
    }
}