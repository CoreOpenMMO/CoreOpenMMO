using System;
using System.Net.Sockets;
using COTS.Domain.Entities;
using COTS.Infra.CrossCutting.Security;
using COTS.Infra.CrossCutting.Security.Enums;

namespace COTS.GameServer.Network
{
    public class Connection
    {
        private Socket _socket;
        private NetworkStream _stream;
        private NetworkMessage _networkMessage;

        public void LoginListenerCallback(IAsyncResult ar)
        {
            TcpListener clientListener = (TcpListener)ar.AsyncState;
            _socket = clientListener.EndAcceptSocket(ar);
            _stream = new NetworkStream(_socket);

            _stream.BeginRead(_networkMessage.Buffer, 0, 2, ClientReadFirstCallBack, null);
        }

        public void GameListenerCallback(IAsyncResult ar)
        {
            TcpListener gameListener = (TcpListener)ar.AsyncState;
            _socket = gameListener.EndAcceptSocket(ar);
            _stream = new NetworkStream(_socket);

            SendConnectionPacket();

            _stream.BeginRead(_networkMessage.Buffer, 0, 2,
                new AsyncCallback(ClientReadFirstCallBack), null);
        }

        private void ClientReadFirstCallBack(IAsyncResult ar)
        {
            if (!EndRead(ar)) return;

            try
            {
                byte protocol = _networkMessage.GetByte(); // protocol id (1 = login, 2 = game)

                if (protocol == 1)
                {
                    
                }
                else if (protocol == 0x0A)
                {
                    
                }
            }
            catch (Exception ex)
            {
                // Invalid data from the client
                Close();
            }
        }

        private void ClientReadCallBack(IAsyncResult ar)
        {
            if (!EndRead(ar))
            {
                // Client crashed or disconnected
                Game.PlayerLogout(Player);
                return;
            }

            inMessage.XteaDecrypt(xteaKey);
            ushort length = inMessage.GetUInt16();
            byte type = inMessage.GetByte();

            ParseClientPacket((ClientPacketType)type, inMessage);

            if (!remove)
            {
                _stream.BeginRead(inMessage.Buffer, 0, 2,
                    new AsyncCallback(ClientReadCallBack), null);
            }
        }

        private bool EndRead(IAsyncResult ar)
        {
            try
            {
                int read = _stream.EndRead(ar);

                if (read == 0)
                {
                    // client disconnected
                    Close();
                    return false;
                }

                int size = (int)BitConverter.ToUInt16(inMessage.Buffer, 0) + 2;

                while (read < size)
                {
                    if (_stream.CanRead)
                        read += _stream.Read(inMessage.Buffer, read, size - read);
                }
                inMessage.Length = size;

                inMessage.Position = 0;

                inMessage.GetUInt16(); // total length
                inMessage.GetUInt32(); // adler

                return true;
            }
            catch
            {
                Close();
                return false;
            }
        }

        #endregion

        #region Parse

        private void ParseLoginPacket(NetworkMessage message)
        {
            LoginPacket loginPacket = LoginPacket.Parse(message);
            xteaKey = loginPacket.XteaKey;

            long accountId = Game.CheckLoginInfo(this, loginPacket, false);

            if (accountId >= 0)
            {
                this.AccountId = accountId;
                Game.ProcessLogin(this, loginPacket.CharacterName);
            }
            else
            {
                Close();
            }
        }

        private void ParseClientPacket(ClientPacketType type, NetworkMessage message)
        {
            switch (type)
            {
                //case ClientPacketType.Logout:
                //    ParseLogout();
                //    break;
                //case ClientPacketType.ItemMove:
                //    ParseItemMove(message);
                //    break;
                //default:
                //    //Server.Log("Unhandled packet from {0}: {1}", Player.ToString(), type);
                //    break;
            }
        }

    }
}
