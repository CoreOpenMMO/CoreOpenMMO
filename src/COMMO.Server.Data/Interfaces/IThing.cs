// <copyright file="IThing.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data.Models.Structs;

namespace COMMO.Server.Data.Interfaces
{
    public delegate void OnThingStateChanged(IThing thingThatChanged, ThingStateChangedEventArgs eventArgs);

    public interface IThing
    {
        event OnThingStateChanged OnThingChanged;

        ushort ThingId { get; }

        byte Count { get; }

        Location Location { get; }

        ITile Tile { get; }

        string InspectionText { get; }

        string CloseInspectionText { get; }

        bool CanBeMoved { get; }

        void Added();

        void Removed();
    }
}
