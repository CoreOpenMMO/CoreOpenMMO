namespace COMMO.Utilities.Tests {
	using COMMO.Utilities;
	using System;
	using Xunit;

	public sealed class ReadOnlyArrayTests {

		/// <summary>
		/// Checks whether the WrapCollection accepts null arguments.
		/// It shouldn't.
		/// </summary>
		[Fact]
		public void WrapNullReference() {
			Assert.ThrowsAny<ArgumentNullException>(() => {
				var readOnlyArray = ReadOnlyArray<int>.WrapCollection(null);
			});
		}
		
		/// <summary>
		/// Checks whether the indexer throws when provided with out-of-bounds argument.
		/// </summary>
		[Theory]
		[InlineData(0, 0)]
		[InlineData(0, -1)]
		[InlineData(0, 1)]
		[InlineData(1, -1)]
		[InlineData(1, 1)]
		public void AccessElementsOutOfBounds(int arrayLength, int elementIndex) {
			var array = new int[arrayLength];
			var wrapper = ReadOnlyArray<int>.WrapCollection(array);

			Assert.ThrowsAny<IndexOutOfRangeException>(() => {
				var values = wrapper[elementIndex];
			});
		}
	}
}
