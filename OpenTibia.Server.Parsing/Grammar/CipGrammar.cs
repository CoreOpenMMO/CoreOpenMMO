// <copyright file="CipGrammar.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Parsing.Grammar
{
    using System.Collections.Generic;
    using System.Linq;
    using Sprache;

    public class CipGrammar
    {
        private static readonly Parser<char> EqualSign = Parse.Char('=');
        private static readonly Parser<char> DoubleQuote = Parse.Char('"');
        private static readonly Parser<char> Backslash = Parse.Char('\\');
        private static readonly Parser<char> OpenParenthesis = Parse.Char('(');
        private static readonly Parser<char> CloseParenthesis = Parse.Char(')');
        private static readonly Parser<char> OpenBracket = Parse.Char('[');
        private static readonly Parser<char> CloseBracket = Parse.Char(']');
        private static readonly Parser<char> Comma = Parse.Char(',');

        private static readonly Parser<IEnumerable<char>> ConditionsActionsSeparator = Parse.String("->");

        private static readonly Parser<string> GreaterThanComparison = Parse.String(">").Text();
        private static readonly Parser<string> LessThanComparison = Parse.String("<").Text();
        private static readonly Parser<string> GreaterThanOrEqualToComparison = Parse.String(">=").Text();
        private static readonly Parser<string> LessThanOrEqualToComparison = Parse.String("<=").Text();
        private static readonly Parser<string> EqualToComparison = Parse.String("==").Text();

        private static readonly Parser<char> Quoted = Parse.AnyChar.Except(DoubleQuote);
        private static readonly Parser<char> Escaped =
            from blackSlash in Backslash
            from c in Parse.AnyChar
            select c;

        public static readonly Parser<string> QuotedMessage =
            from open in DoubleQuote
            from text in Quoted.Or(Escaped).Many().Text()
            from close in DoubleQuote
            select open + text + close; // we want the string in "quotes"

        private static readonly Parser<string> Text =
            from text in Parse.AnyChar.Except(ConditionsActionsSeparator).Except(Comma).Except(EqualSign).AtLeastOnce().Text()
            select text.Trim();

        private static readonly Parser<string> FunctionOrComparisonString =
            from functionName in Parse.AnyChar.Except(OpenParenthesis).Except(Comma).Except(EqualSign).Many().Text()
            from open in OpenParenthesis
            from oneOrMoreArguments in Parse.AnyChar.Except(CloseParenthesis).Many().Text().Or(QuotedMessage)
            from close in CloseParenthesis
            from functionComparison in Parse.AnyChar.Except(Parse.WhiteSpace).Except(Comma).Many().Text()
            select functionName.Trim() + open + oneOrMoreArguments + close + functionComparison.Trim();

        public static readonly Parser<string> LocationString =
            from leadingSpaces in Parse.WhiteSpace.Many().Text()
            from open in OpenBracket
            from negX in Parse.Char('-').Optional()
            from x in Parse.Number
            from firstComma in Comma
            from negY in Parse.Char('-').Optional()
            from y in Parse.Number
            from secondComma in Comma
            from negZ in Parse.Char('-').Optional()
            from z in Parse.Number
            from close in CloseBracket
            select $"{open}{(negX.IsEmpty ? string.Empty : "-")}{x},{(negY.IsEmpty ? string.Empty : "-")}{y},{(negZ.IsEmpty ? string.Empty : "-")}{z}{close}";

        private static readonly Parser<string> KeyValStr =
            from key in Text
            from eq in EqualSign
            from value in Text
            select key + eq + value; // we want the whole thing .. key=val

        private static readonly Parser<IEnumerable<string>> Conditions = FunctionOrComparisonString.Or(QuotedMessage).Or(KeyValStr).Or(Text).DelimitedBy(Comma);
        private static readonly Parser<IEnumerable<string>> Actions = FunctionOrComparisonString.Or(QuotedMessage).Or(KeyValStr).Or(Text).DelimitedBy(Comma);

        public static readonly Parser<ConditionalActionRule> ConditionalActionRule =
            from conditions in Conditions
            from leading in Parse.WhiteSpace.Optional().Many()
            from separator in ConditionsActionsSeparator
            from trailing in Parse.WhiteSpace.Many()
            from actions in Actions
            select new ConditionalActionRule(conditions, actions);

        public static readonly Parser<ScriptFunction> Function =
            from functionName in Parse.AnyChar.Except(OpenParenthesis).Except(Comma).Except(EqualSign).Many().Text()
            from open in OpenParenthesis
            from oneOrMoreArguments in LocationString.Or(QuotedMessage).Or(Parse.AnyChar.Except(CloseParenthesis).Except(Comma).Many().Text()).DelimitedBy(Comma)
            from close in CloseParenthesis
            select new ScriptFunction(functionName.Trim(), oneOrMoreArguments.ToArray());

        public static readonly Parser<FunctionComparison> Comparison =
            from functionName in Parse.AnyChar.Except(OpenParenthesis).Except(Comma).Except(EqualSign).Many().Text()
            from open in OpenParenthesis
            from oneOrMoreArguments in LocationString.Or(QuotedMessage).Or(Parse.AnyChar.Except(CloseParenthesis).Except(Comma).Many().Text()).DelimitedBy(Comma)
            from close in CloseParenthesis
            from functionComparison in GreaterThanComparison.Or(GreaterThanOrEqualToComparison).Or(LessThanComparison).Or(LessThanOrEqualToComparison).Or(EqualToComparison)
            from identifier in Parse.AnyChar.Except(Comma).Many().Text()
            select new FunctionComparison(functionName.Trim(), functionComparison, identifier, oneOrMoreArguments.ToArray());
    }
}
