using System;

namespace COMMO.GameServer.OTBParsing {

    public sealed class OldOTBTree {

        /// <summary>
        /// To save memory, the <see cref="OldOTBNode"/>s don't actually store their information.
        /// Instead, they just keep a `offset' and a `byte count' to this array.
        /// </summary>
        public readonly byte[] Data;

        /// <summary>
        /// World info is store in a "adhoc xml".
        /// This is the root node of such pseudo-xmml.
        /// </summary>
        public readonly OldOTBNode Root;

        public OldOTBTree(byte[] data, OldOTBNode root) {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            Data = data;
            Root = root;
        }
    }
}