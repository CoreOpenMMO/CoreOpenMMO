// <copyright file="InvalidModelException.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Data
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class InvalidModelException : Exception
    {
        public InvalidModelException()
        {
        }

        public InvalidModelException(string message)
            : base(message)
        {
        }

        public InvalidModelException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidModelException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}