// <copyright file="CipElement.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Parsing
{
    using System.Collections.Generic;

    public class CipElement
    {
        public CipElement(int data)
        {
            this.Data = data;
            this.Attributes = new List<CipAttribute>();
        }

        public int Data { get; set; }

        public IList<CipAttribute> Attributes { get; set; }
    }
}
