// <copyright file="EventFiredEventArgs.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace COMMO.Scheduling.Contracts {
	/// <summary>
	/// Class that represents the event arguments of an EventFired event.
	/// </summary>
	public class EventFiredEventArgs : EventArgs {
		/// <summary>
		/// Initializes a new instance of the <see cref="EventFiredEventArgs"/> class.
		/// </summary>
		/// <param name="evt">The event to include as the event fired.</param>
		public EventFiredEventArgs(IEvent evt) {
			if (evt == null)
				throw new ArgumentNullException(nameof(evt));

			Event = evt;
		}

		/// <summary>
		/// Gets the event that was fired.
		/// </summary>
		public IEvent Event { get; }
	}
}