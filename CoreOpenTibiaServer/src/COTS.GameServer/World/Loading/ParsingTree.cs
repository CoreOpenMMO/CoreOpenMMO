using System;

namespace COTS.GameServer.World.Loading {

    public sealed class ParsingTree {

        /// <summary>
        /// To save memory, the <see cref="ParsingNode"/>s don't actually store their information.
        /// Instead, they just keep a `offset' and a `byte count' to this array.
        /// </summary>
        public readonly byte[] Data;

        /// <summary>
        /// World info is store in a "adhoc xml".
        /// This is the root node of such pseudo-xmml.
        /// </summary>
        public readonly ParsingNode Root;

        public ParsingTree(byte[] data, ParsingNode root) {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            Data = data;
            Root = root;
        }
    }
}