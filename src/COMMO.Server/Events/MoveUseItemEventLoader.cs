// <copyright file="MoveUseItemEventLoader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using COMMO.Data.Contracts;
using COMMO.Server.Data.Interfaces;
using COMMO.Server.Items;
using Sprache;
using static COMMO.Server.Parsing.Grammar.EventGrammar;

namespace COMMO.Server.Events
{
    public class MoveUseItemEventLoader : IItemEventLoader
    {
        /*
            An item definition starts and ends with blank lines.

            TypeID      = 1 # body container
            Name        = ""
            Flags       = {Container,Take}
            Attributes  = {Capacity=1,Weight=0

         */

        public const char CommentSymbol = '#';
        public const char PropertyValueSeparator = '=';

        public IDictionary<ItemEventType, HashSet<IItemEvent>> Load(string moveUseFileName)
        {
            if (string.IsNullOrWhiteSpace(moveUseFileName))
            {
                throw new ArgumentNullException(nameof(moveUseFileName));
            }

            var moveUseFilePath = "COMMO.Server.Data." + ServerConfiguration.DataFilesDirectory + "." + moveUseFileName;

            var assembly = Assembly.GetExecutingAssembly();

            var eventDictionary = new Dictionary<ItemEventType, HashSet<IItemEvent>>
            {
                { ItemEventType.Use, new HashSet<IItemEvent>() },
                { ItemEventType.MultiUse, new HashSet<IItemEvent>() },
                { ItemEventType.Movement, new HashSet<IItemEvent>() },
                { ItemEventType.Collision, new HashSet<IItemEvent>() },
                { ItemEventType.Separation, new HashSet<IItemEvent>() }
            };

            //using (var stream = assembly.GetManifestResourceStream(moveUseFilePath))
            //{
            //    if (stream == null)
            //    {
            //        throw new Exception($"Failed to load {moveUseFilePath}.");
            //    }

            //    using (var reader = new StreamReader(stream))
            //    {
            //        foreach (var readLine in reader.ReadToEnd().Split("\r\n".ToCharArray()))
            //        {
            //            var inLine = readLine?.Split(new[] { ObjectsFileItemLoader.CommentSymbol }, 2).FirstOrDefault();

            //            // ignore comments and empty lines.
            //            if (string.IsNullOrWhiteSpace(inLine) || inLine.StartsWith("BEGIN") || inLine.StartsWith("END"))
            //            {
            //                continue;
            //            }

            //            try
            //            {
            //                var moveUseEventParsed = Event.Parse(inLine);

            //                eventDictionary[moveUseEventParsed.Type].Add(ItemEventFactory.Create(moveUseEventParsed));
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine(ex.Message);
            //                Console.WriteLine(ex.StackTrace);
            //            }
            //        }
            //    }
            //}

            return eventDictionary;
        }
    }
}
