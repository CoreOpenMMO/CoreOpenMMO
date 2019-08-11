using COMMO.Common.Structures;

namespace COMMO.Common.Objects
{
    public interface IClient 
    {
        ICreatureCollection CreatureCollection { get; }

        IContainerCollection ContainerCollection { get; }

        IWindowCollection WindowCollection { get; }

        Player Player { get; set;  }

        IConnection Connection { get; set; }

        FightMode FightMode { get; set; }

        ChaseMode ChaseMode { get; set; }

        SafeMode SafeMode { get; set; }
    }
}