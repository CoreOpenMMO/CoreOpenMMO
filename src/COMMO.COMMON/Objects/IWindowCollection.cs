using System.Collections.Generic;

namespace COMMO.Common.Objects
{
    public interface IWindowCollection
    {
        byte OpenWindow(Window window);

        void ReplaceWindow(byte windowId, Window window);

        void CloseWindow(byte windowId);

        Window GetWindow(byte windowId);

        IEnumerable<Window> GetWindows();

        IEnumerable< KeyValuePair<byte, Window> > GetIndexedWindows();
    }
}