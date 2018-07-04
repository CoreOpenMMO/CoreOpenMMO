// <copyright file="ICombatOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Data.Interfaces
{
    using System;
    using OpenTibia.Data.Contracts;

    /// <summary>
    /// Interface for a combat operation.
    /// </summary>
    public interface ICombatOperation
    {
        /// <summary>
        /// Gets the combat operation's attack type.
        /// </summary>
        AttackType AttackType { get; }

        /// <summary>
        /// Gets the combat operation's attacker actor.
        /// </summary>
        ICombatActor Attacker { get; }

        /// <summary>
        /// Gets the combat operation's target actor.
        /// </summary>
        ICombatActor Target { get; }

        /// <summary>
        /// Gets the exhaustion cost time.
        /// </summary>
        TimeSpan ExhaustionCost { get; }

        /// <summary>
        /// Gets the absolute minimum damage that the combat operation can result in.
        /// </summary>
        int MinimumDamage { get; }

        /// <summary>
        /// Gets the absolute maximum damage that the combat operation can result in.
        /// </summary>
        int MaximumDamage { get; }

        /// <summary>
        /// Gets a value indicating whether the combat operation can be executed.
        /// </summary>
        bool CanBeExecuted { get; }

        /// <summary>
        /// Excecutes the combat operation.
        /// </summary>
        void Execute();
    }
}