namespace COMMO.Utilities.Tests {
	using COMMO.Utilities;
	using NUnit.Framework;
	using System;

	[TestFixture]
	public sealed class ReadOnlyArrayTests {

		[Test]
		public void WrapCollection_Throws_OnNullReference() {
			Assert.Throws(Is.InstanceOf<ArgumentNullException>(),
				() => {
					var readOnlyArray = ReadOnlyArray<int>.WrapCollection(null);
				});
		}

		[Test, Sequential]
		public void Indexer_Throws_OnOutOfBoundsIndex(
			[Values(0, +0, 0, +1, 1)] int arrayLength,
			[Values(0, -1, 1, -1, 1)] int elementIndex
			) {
			var array = new int[arrayLength];
			var wrapper = ReadOnlyArray<int>.WrapCollection(array);

			Assert.Throws(Is.InstanceOf<IndexOutOfRangeException>(),
				() => {
					var values = wrapper[elementIndex];
				});
		}
	}
}
