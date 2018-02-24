using System;

namespace COMMO.GameServer.Items {

	public sealed class DeprecatedItemGroupException : Exception {

		public DeprecatedItemGroupException()
			: base() { }

		public DeprecatedItemGroupException(string message)
			: base(message) { }
	}
}