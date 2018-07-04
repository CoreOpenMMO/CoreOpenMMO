// <copyright file="GCNotifier.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Utilities
{
    using System;

    public class GcNotifier
    {
        public static event EventHandler GarbageCollected;

        ~GcNotifier()
        {
            if (Environment.HasShutdownStarted || AppDomain.CurrentDomain.IsFinalizingForUnload())
            {
                return;
            }

            new GcNotifier();

            GarbageCollected?.Invoke(null, EventArgs.Empty);
        }

        public static void Start()
        {
            new GcNotifier();
        }
    }
}
