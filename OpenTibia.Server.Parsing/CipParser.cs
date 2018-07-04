// <copyright file="CipParser.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Parsing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class CipParser
    {
        public const char Quote = '"';
        public const char Space = ' ';
        public const char Backslash = '\\';

        public const char OpenParenthesis = '(';
        public const char CloseParenthesis = ')';
        public const char OpenCurly = '{';
        public const char CloseCurly = '}';

        public static Stack<string> GetEnclosedButPreserveQuoted(string str, Dictionary<char, char> openClosePairs)
        {
            var stack = new Stack<string>();

            if (string.IsNullOrWhiteSpace(str))
            {
                return stack;
            }

            var openEnclosure = new Stack<char>();
            var buffers = new Stack<StringBuilder>();

            var main = new StringBuilder();
            buffers.Push(main);

            var inQuote = false;
            for (var i = 0; i < str.Length; i++)
            {
                if (str[i] == Quote && (i > 0 && str[i - 1] != '\\'))
                {
                    inQuote = !inQuote;
                }

                var c = str[i];

                if (openClosePairs.ContainsValue(c) && !inQuote)
                {
                    openEnclosure.Push(c);
                    buffers.Push(new StringBuilder());
                }
                else if (openClosePairs.ContainsKey(c) && !inQuote)
                {
                    if (openEnclosure.Peek() != openClosePairs[c])
                    {
                        throw new Exception("Malformed string.");
                    }

                    openEnclosure.Pop();
                    stack.Push(buffers.Pop().ToString());
                }
                else
                {
                    buffers.Peek().Append(c);
                }
            }

            while (buffers.Count > 0)
            {
                stack.Push(buffers.Pop().ToString());
            }

            return stack;
        }

        public static IEnumerable<string> SplitByTokenPreserveQuoted(string str, char token = Space)
        {
            var queue = new Queue<string>();

            if (string.IsNullOrWhiteSpace(str))
            {
                return queue;
            }

            var inQuote = false;
            var sb = new StringBuilder();

            for (var i = 0; i < str.Length; i++)
            {
                if (str[i] == Quote && (i > 0 && str[i - 1] != Backslash))
                {
                    inQuote = !inQuote;
                }

                if (str[i] == token && !inQuote)
                {
                    var currentString = sb.ToString();

                    if (!string.IsNullOrWhiteSpace(currentString))
                    {
                        queue.Enqueue(currentString);
                        sb.Clear();
                    }
                }
                else
                {
                    sb.Append(str[i]);
                }
            }

            queue.Enqueue(sb.ToString());

            return queue;
        }

        public static IEnumerable<CipElement> Parse(string inputStr)
        {
            if (string.IsNullOrWhiteSpace(inputStr))
            {
                return null;
            }

            var enclosingChars = new Dictionary<char, char> { { CloseCurly, OpenCurly }, { CloseParenthesis, OpenParenthesis} };

            inputStr = inputStr.Trim(' '); // remove extra leading and trailing spaces.
            inputStr = TrimEnclosures(inputStr, enclosingChars);

            var enclosures = GetEnclosedButPreserveQuoted(inputStr, enclosingChars);
            var root = new CipElement(-1);
            root.Attributes.Add(new CipAttribute());

            var pendingContent = new Stack<CipAttribute>();

            // root is guaranteed to have at least one attribute.
            pendingContent.Push(root.Attributes.First());

            while (enclosures.Count > 0)
            {
                // comma separate but watch for strings in quotes ("").
                var elements = SplitByTokenPreserveQuoted(enclosures.Pop(), ',').Select(ParseElement).ToList();

                var currentAttribute = pendingContent.Pop();

                foreach (var element in elements)
                {
                    foreach (var attr in element.Attributes.Where(a => a.Name.Equals("Content")))
                    {
                        pendingContent.Push(attr);
                    }
                }

                currentAttribute.Value = elements;
            }

            return root.Attributes.First().Value as IEnumerable<CipElement>;
        }

        private static string TrimEnclosures(string inputStr, Dictionary<char, char> enclosingChars)
        {
            foreach (var encl in enclosingChars)
            {
                if (inputStr.StartsWith(encl.Value.ToString()) && inputStr.EndsWith(encl.Key.ToString()))
                {
                    inputStr = inputStr.Substring(1, inputStr.Length - 1);
                    inputStr = inputStr.Substring(0, inputStr.Length - 1);
                }
            }

            return inputStr;
        }

        private static CipElement ParseElement(string eString)
        {
            var attrs = SplitByTokenPreserveQuoted(eString);
            int intValue;
            var attributesList = attrs as IList<string> ?? attrs.ToList();
            var hasIdData = int.TryParse(attributesList.FirstOrDefault(), out intValue);

            Func<string, CipAttribute> extractAttribute = str =>
            {
                var sections = str.Split(new[] { '=' }, 2);

                if (sections.Length < 2)
                {
                    return new CipAttribute
                    {
                        Name = sections[0].EndsWith("=") ? sections[0].Substring(0, sections[0].Length - 1) : sections[0]
                    };
                }

                int numericValue;

                return new CipAttribute
                {
                    Name = sections[0],
                    Value = int.TryParse(sections[1], out numericValue) ? (IConvertible)numericValue : sections[1]
                };
            };

            var element = new CipElement(hasIdData ? intValue : -1)
            {
                Attributes = attributesList.Skip(hasIdData ? 1 : 0).Select(a => extractAttribute(a)).ToList()
            };

            return element;
        }
    }
}
