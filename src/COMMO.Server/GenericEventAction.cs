// <copyright file="GenericEventAction.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Scheduling.Contracts;
using System;

namespace COMMO.Server {
	internal class GenericEventAction : IEventAction {
		private readonly Action _action;

		public GenericEventAction(Action action) {
			if (action == null)
				throw new ArgumentNullException(nameof(action));

			_action = action;
		}

		public void Execute() {
			_action();
		}
	}
}