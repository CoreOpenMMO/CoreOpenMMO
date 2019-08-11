// <copyright file="GenericEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Scheduling.Contracts;
using System;

namespace COMMO.Server {
	internal class GenericEventCondition : IEventCondition {
		private readonly Func<bool> _condition;

		public GenericEventCondition(Func<bool> condition, string errorMsg = "") {
			_condition = condition ?? throw new ArgumentNullException(nameof(condition));
			ErrorMessage = errorMsg;
		}

		public string ErrorMessage { get; }

		public bool Evaluate() => _condition();
	}
}