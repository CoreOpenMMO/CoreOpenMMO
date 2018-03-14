using COMMO.GameServer.OTBParsing;
using System;
using System.Linq;
using System.Text;
using Xunit;

namespace COMMO.GameServer.Tests {

	/// <summary>
	/// In this classe we are using '/', '(' and ')' to represent
	/// the Escape, Start and End markup bytes, respectively.
	/// </summary>
	public sealed class OTBMParsingTests {

		[Fact]
		public void Parse_RootlessTree() {
			var markupBytes = ConvertStringToOTBMarkupBytes("()()");

			Assert.Throws<InvalidOperationException>(() => ParseOTBTree(markupBytes));
		}

		[Theory]
		[InlineData("(((()))")]
		[InlineData("((())))")]
		[InlineData("")]
		[InlineData("(")]
		[InlineData(")")]
		public void Parse_MalformedTree(string markersAsParenthesis) {
			var markupBytes = ConvertStringToOTBMarkupBytes(markersAsParenthesis);

			Assert.Throws<InvalidOperationException>(() => ParseOTBTree(markupBytes));
		}

		[Fact]
		public void Parsing_SingleNode_NoData() {
			var markupBytes = ConvertStringToOTBMarkupBytes("()");
			var root = ParseOTBTree(markupBytes);

			var expectedDataLength = 0;
			var actualNodeCount = root.Data.Length;

			Assert.Equal(expected: expectedDataLength, actual: actualNodeCount);
		}

		[Fact]
		public void Parsing_SingleNode_WithData() {
			var markupBytes = ConvertStringToOTBMarkupBytes("(data)");
			var root = ParseOTBTree(markupBytes);

			var expected = new ASCIIEncoding().GetBytes("data");
			var actual = root.Data.Span;

			Assert.Equal(
				expected: expected.Length,
				actual: actual.Length);

			for (int i = 0; i < actual.Length; i++) {
				Assert.Equal(
					expected: expected[i],
					actual: actual[i]);
			}
		}

		public static byte[] ConvertStringToOTBMarkupBytes(string parenthesis) {
			if (parenthesis == null)
				throw new ArgumentNullException(nameof(parenthesis));

			var bytes = new ASCIIEncoding().GetBytes(parenthesis);

			for (int i = 0; i < bytes.Length; i++) {
				if (bytes[i] == '(')
					bytes[i] = (byte)OTBMarkupByte.Start;
				else if (bytes[i] == ')')
					bytes[i] = (byte)OTBMarkupByte.End;
			}

			return bytes;
		}

		public static OTBNode ParseOTBTree(byte[] serializedTreeData) {
			var treeBuilder = new OTBTreeBuilder(serializedTreeData);

			for (int i = 0; i < serializedTreeData.Length; i++) {
				var markupByte = (OTBMarkupByte)serializedTreeData[i];
				switch (markupByte) {
					case OTBMarkupByte.Escape:
					i++;
					break;

					case OTBMarkupByte.Start:
					treeBuilder.AddNodeDataBegin(i + 1, OTBNodeType.NotSetYet);
					break;

					case OTBMarkupByte.End:
					treeBuilder.AddNodeEnd(i);
					break;
				}
			}

			return treeBuilder.BuildTree();
		}

		public static int CountNodesInTree(OTBNode root) {
			if (root.Children.Count == 0)
				return 1;

			return 1 + root
				.Children
				.Select(node => CountNodesInTree(node))
				.Sum();
		}
	}
}
