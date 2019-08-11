using System.Collections.Generic;

namespace COMMO.Common.Objects
{
    public interface IContainerCollection
    {
        byte OpenContainer(Container container);

        void ReplaceContainer(byte containerId, Container container);

        void CloseContainer(byte containerId);

        Container GetContainer(byte containerId);

        IEnumerable<Container> GetContainers();

        IEnumerable<KeyValuePair<byte, Container> > GetIndexedContainers();
    }
}