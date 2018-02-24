using System;

namespace COMMO.GameServer {

    public sealed class DeprecatedItemGroupException : Exception {

        public DeprecatedItemGroupException()
            : base() { }

        public DeprecatedItemGroupException(string message)
            : base(message) { }
    }
}