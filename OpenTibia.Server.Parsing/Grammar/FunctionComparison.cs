// <copyright file="FunctionComparison.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Parsing.Grammar
{
    using OpenTibia.Data.Contracts;

    public class FunctionComparison
    {
        public string Name { get; }

        public string[] Parameters { get; }

        public FunctionComparisonType Type { get; }

        public string CompareToIdentifier { get; }

        public FunctionComparison(string name, string comparisonType, string comparingTo, params string[] parameters)
        {
            this.Name = name;
            this.Parameters = parameters;
            this.CompareToIdentifier = comparingTo;
            this.Parameters = parameters;

            if (comparisonType == ">=")
            {
                this.Type = FunctionComparisonType.GreaterThanOrEqual;
            }
            else if (comparisonType == "<=")
            {
                this.Type = FunctionComparisonType.LessThanOrEqual;
            }
            else if (comparisonType == ">")
            {
                this.Type = FunctionComparisonType.GreaterThan;
            }
            else if (comparisonType == "<")
            {
                this.Type = FunctionComparisonType.LessThan;
            }
            else if (comparisonType == "==")
            {
                this.Type = FunctionComparisonType.Equal;
            }
        }
    }
}