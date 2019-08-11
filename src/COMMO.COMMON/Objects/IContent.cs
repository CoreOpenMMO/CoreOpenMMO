using COMMO.Common.Structures;

namespace COMMO.Common.Objects
{
    public interface IContent
    {
        TopOrder TopOrder { get; }

        IContainer Container { get; set; }
    }
}