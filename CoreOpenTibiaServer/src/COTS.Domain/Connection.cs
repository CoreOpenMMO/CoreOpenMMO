using System.Net.Sockets;

namespace COTS.GameServer.Network
{
    /*public sealed class Connection {

        #region Properties
        private readonly int _id;
        private readonly TcpClient _client;
        private readonly NetworkStream _networkStream;
        private readonly NetworkMessage _networkMessage;

        public int ID { get { return _id; } }
        #endregion

        #region Constructors
        public Connection(TcpClient client, NetworkStream stream, NetworkMessage message)
        {
            _client = client;
            _networkStream = stream;
            _networkMessage = message;
        }
        #endregion

        #region Methods
        public int Read(int offset, int size) => _networkStream.Read(_networkMessage.Buffer, offset, size);
        #endregion
    }*/
}
