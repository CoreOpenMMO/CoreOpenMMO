﻿// <copyright file="IConverter.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Server.Scripting
{
    internal interface IConverter
    {
        object Convert(string value);
    }
}