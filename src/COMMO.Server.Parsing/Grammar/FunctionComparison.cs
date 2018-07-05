// <copyright file="FunctionComparison.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Server.Parsing.Grammar
{
    using COMMO.Data.Contracts;

    public class FunctionComparison
    {
        public string Name { get; }

        public string[] Parameters { get; }

        public FunctionComparisonType Type { get; }

        public string CompareToIdentifier { get; }

        public FunctionComparison(string name, string comparisonType, string comparingTo, params string[] parameters)
        {
            Name = name;
            Parameters = parameters;
            CompareToIdentifier = comparingTo;
            Parameters = parameters;

            if (comparisonType == ">=")
            {
                Type = FunctionComparisonType.GreaterThanOrEqual;
            }
            else if (comparisonType == "<=")
            {
                Type = FunctionComparisonType.LessThanOrEqual;
            }
            else if (comparisonType == ">")
            {
                Type = FunctionComparisonType.GreaterThan;
            }
            else if (comparisonType == "<")
            {
                Type = FunctionComparisonType.LessThan;
            }
            else if (comparisonType == "==")
            {
                Type = FunctionComparisonType.Equal;
            }
        }
    }
}