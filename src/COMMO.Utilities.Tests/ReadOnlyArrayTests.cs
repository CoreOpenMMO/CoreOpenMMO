namespace COMMO.Utilities.Tests {
	using COMMO.Utilities;
	using System;
	using Xunit;

	public sealed class ReadOnlyArrayTests {

		[Fact]
		public void WrapCollection_Throws_OnNullReference() {
			Assert.ThrowsAny<ArgumentNullException>(() => {
				var readOnlyArray = ReadOnlyArray<int>.WrapCollection(null);
			});
		}
		
		[Theory]
		[InlineData(0, 0)]
		[InlineData(0, -1)]
		[InlineData(0, 1)]
		[InlineData(1, -1)]
		[InlineData(1, 1)]
		public void Indexer_Throws_OnOutOfBoundsIndex(int arrayLength, int elementIndex) {
			var array = new int[arrayLength];
			var wrapper = ReadOnlyArray<int>.WrapCollection(array);

			Assert.ThrowsAny<IndexOutOfRangeException>(() => {
				var values = wrapper[elementIndex];
			});
		}
	}
}
