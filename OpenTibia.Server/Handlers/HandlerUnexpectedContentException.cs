// <copyright file="HandlerUnexpectedContentException.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class HandlerUnexpectedContentException : Exception
    {
        public HandlerUnexpectedContentException()
        {
        }

        public HandlerUnexpectedContentException(string message)
            : base(message)
        {
        }

        public HandlerUnexpectedContentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected HandlerUnexpectedContentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}