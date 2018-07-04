// <copyright file="SimpleDoSDefender.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Security
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class SimpleDoSDefender : IDoSDefender
    {
        // To prevent a memory attack... just blacklist a maximum of 1M addresses.
        private const int ListSizeLimit = 1000000;

        // Count to reach within 30 seconds (reduces count every 5 seconds);
        private const int BlockAtCount = 20;

        public HashSet<string> BlockedIpAddresses { get; }

        public ConcurrentDictionary<string, int> ConnectionCount { get; }

        public CancellationTokenSource CancellationTokenSource { get; }

        private static SimpleDoSDefender singleton;

        private static readonly object SingletonLock = new object();
        private Task cleaningTask;

        public static SimpleDoSDefender Instance
        {
            get
            {
                if (singleton == null)
                {
                    lock (SingletonLock)
                    {
                        if (singleton == null)
                        {
                            singleton = new SimpleDoSDefender();
                        }
                    }
                }

                return singleton;
            }
        }

        public SimpleDoSDefender()
        {
            this.BlockedIpAddresses = new HashSet<string>();
            this.ConnectionCount = new ConcurrentDictionary<string, int>();
            this.CancellationTokenSource = new CancellationTokenSource();

            this.cleaningTask = Task.Factory.StartNew(() =>
            {
                while (!this.CancellationTokenSource.Token.IsCancellationRequested)
                {
                    const int secondsToWait = 5;
                    Thread.Sleep(TimeSpan.FromSeconds(secondsToWait));
                    var cleaningList = this.ConnectionCount.ToList();

                    foreach (var kvp in cleaningList)
                    {
                        if (kvp.Value < secondsToWait)
                        {
                            int count;
                            this.ConnectionCount.TryRemove(kvp.Key, out count);
                        }
                        else
                        {
                            this.ConnectionCount.TryUpdate(kvp.Key, kvp.Value - secondsToWait, kvp.Value);
                        }
                    }
                }
            });
        }

        public void AddToBlocked(string addressStr)
        {
            if (this.BlockedIpAddresses.Count >= ListSizeLimit || string.IsNullOrWhiteSpace(addressStr))
            {
                return;
            }

            this.AddInternal(addressStr);
        }

        private void AddInternal(string addressStr)
        {
            try
            {
                this.BlockedIpAddresses.Add(addressStr);
            }
            catch
            {
                // this will be thrown if there is already an element in there, so just ignore.
            }
        }

        public bool IsBlockedAddress(string addressStr)
        {
            return this.BlockedIpAddresses.Contains(addressStr);
        }

        public void LogConnectionAttempt(string addressStr)
        {
            this.ConnectionCount.AddOrUpdate(addressStr, 0, (key, prev) => { return prev + 1; });

            try
            {
                if (this.ConnectionCount[addressStr] == BlockAtCount)
                {
                    this.AddInternal(addressStr);

                    int count;
                    this.ConnectionCount.TryRemove(addressStr, out count);
                }
            }
            catch
            {
                // happens if the key was removed exactly at the time we were querying. Just ignore.
            }
        }
    }
}
