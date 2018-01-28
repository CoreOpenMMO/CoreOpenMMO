using System.Net.Sockets;

namespace COTS.Infra.CrossCutting.Network
{
    public sealed class PendingRequest {

        #region Properties
        //private readonly int _id;
        private readonly TcpClient _socket;

        //public int ID { get { return _id; } }
        public TcpClient Socket { get { return _socket; } }
        #endregion

        #region Constructors
        public PendingRequest(TcpClient socket) {
            _socket = socket;
        }
        #endregion
    }
}
