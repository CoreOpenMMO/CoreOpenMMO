// <copyright file="INewConnectionInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets
{
    public interface INewConnectionInfo
    {
        ushort Os { get; set; }

        ushort Version { get; set; }
    }
}
