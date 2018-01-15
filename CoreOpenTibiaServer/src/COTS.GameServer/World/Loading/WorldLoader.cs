﻿using System;

namespace COTS.GameServer.World.Loading {

    public static partial class WorldLoader {

        public static World ParseWorld(byte[] serializedWorldData) {
            if (serializedWorldData == null)
                throw new ArgumentNullException(nameof(serializedWorldData));
            if (serializedWorldData.Length < MinimumWorldSize)
                throw new MalformedWorldException();

            var stream = new ByteArrayReadStream(serializedWorldData);
            var rootNode = ParseTree(stream);

            throw new NotImplementedException();
        }
    }
}