// <copyright file="Program.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Standalone
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Interfaces;
    using OpenTibia.Server.Events;
    using OpenTibia.Server.Handlers;
    using OpenTibia.Server.Handlers.Management;
    using OpenTibia.Server.Items;
    using OpenTibia.Server.Monsters;

    public class Program
    {
        private static IOpenTibiaListener loginListener;
        private static IOpenTibiaListener gameListener;
        // private static IOpenTibiaListener managementListener;
        static void Main()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            // Set the loaders to use.
            Game.Instance.Initialize(new MoveUseItemEventLoader(), new ObjectsFileItemLoader(), new MonFilesMonsterLoader());

            // TODO: load and validate external aux files.

            // Set the persistence storage source (database)

            // Initilize client listening pipeline (but reject game connections)
            loginListener = new LoginListener(new ManagementHandlerFactory(), 7171);
            gameListener = new GameListener(new GameHandlerFactory(), 7172);
            // managementListener = new ManagementListener(new ManagementHandlerFactory());
            var listeningTask = RunAsync(cancellationTokenSource.Token);

            // Initilize game
            Game.Instance.Begin(cancellationTokenSource.Token);

            // TODO: open up game connections
            listeningTask.Wait(cancellationTokenSource.Token);
        }

        private static async Task RunAsync(CancellationToken cancellationToken)
        {
            loginListener.BeginListening();
            // managementListener.BeginListening();
            gameListener.BeginListening();

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
