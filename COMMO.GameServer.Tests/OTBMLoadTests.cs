using System;
using COMMO.GameServer.OTBParsing;
using System.Linq;
using Xunit;

namespace COMMO.GameServer.Tests
{
	public class OTBMLoadTests {

		[Theory]
		[InlineData("(((()))")]
		[InlineData("((())))")]
		[InlineData("")]
		[InlineData("(")]
		[InlineData(")")]
		[InlineData("()()")]
		public void OTBMLoad_Wrong(String map)
        {
			var newmap = map.Select(c => c == '(' ? (byte)OTBMarkupByte.Start : (byte)OTBMarkupByte.End).ToArray();
			Assert.Throws<InvalidOperationException>(() => {
				var treeBuilder = new OTBTreeBuilder(newmap);

				for (var i=0; i< newmap.Length; i++){ 
					switch (newmap[i]) {
						case (byte)OTBMarkupByte.Start: //(
							treeBuilder.AddNodeStart(start: i, type: OTBNodeType.Item);
							break;

						case (byte)OTBMarkupByte.End: //)
							treeBuilder.AddNodeEnd(i);
							break;
					}
				}

				return treeBuilder.BuildTree();
			});
		}

		[Theory]
		[InlineData("()")]
		[InlineData("(())")]
		[InlineData("(()())")]
		[InlineData("((())())")]
		[InlineData("(()(())((()))(()()(()))())")]
		public void OTBMLoad_Right(String map) {
			var newmap = map.Select(c => c == '(' ? (byte)OTBMarkupByte.Start : (byte)OTBMarkupByte.End).ToArray();
			var treeBuilder = new OTBTreeBuilder(newmap);

			for (var i = 0; i < newmap.Length; i++) {
				switch (newmap[i]) {
					case (byte)OTBMarkupByte.Start: //(
						treeBuilder.AddNodeStart(start: i, type: OTBNodeType.Item);
						break;

					case (byte)OTBMarkupByte.End: //)
						treeBuilder.AddNodeEnd(i);
						break;
				}
			}

			Assert.IsType<OTBNode>(treeBuilder.BuildTree());
		}
	}
}
