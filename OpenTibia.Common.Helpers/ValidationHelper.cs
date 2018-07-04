// <copyright file="ValidationHelper.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Common.Helpers
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class that provides extension methods for common validation operations.
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Throws am <see cref="ArgumentNullException"/> if the analyzed string is null or white space only.
        /// </summary>
        /// <param name="str">The string to analyze.</param>
        /// <param name="paramName">The parameter name to use.</param>
        public static void ThrowIfNullOrWhiteSpace(this string str, string paramName = "")
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentNullException(string.IsNullOrWhiteSpace(paramName) ? nameof(str) : paramName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the analyzed object reference is null.
        /// </summary>
        /// <param name="obj">The object reference to analyze.</param>
        /// <param name="paramName">The parameter name to use.</param>
        public static void ThrowIfNull(this object obj, string paramName = "")
        {
            if (obj == null)
            {
                throw new ArgumentNullException(string.IsNullOrWhiteSpace(paramName) ? nameof(obj) : paramName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the analyzed variable has the default value for it's type.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IConvertible"/>.</typeparam>
        /// <param name="obj">The variable to analyze.</param>
        /// <param name="paramName">The parameter name to use.</param>
        public static void ThrowIfDefaultValue<T>(this T obj, string paramName = "")
            where T : IConvertible
        {
            obj.ThrowIfNull();

            if (EqualityComparer<T>.Default.Equals(obj, default(T)))
            {
                throw new ArgumentException($"Parameter {(string.IsNullOrWhiteSpace(paramName) ? nameof(obj) : paramName)} has the default value.");
            }
        }
    }
}
