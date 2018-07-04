// <copyright file="ScriptFunction.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Parsing.Grammar
{
    public class ScriptFunction
    {
        public string Name { get; }

        public object[] Parameters { get; }

        public ScriptFunction(string name, params object[] parameters)
        {
            this.Name = name;
            this.Parameters = parameters;
        }
    }
}