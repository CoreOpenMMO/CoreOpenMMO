namespace COMMO.Common.Objects
{
    public interface IConnection 
    {
        IClient Client { get; set; }

        uint[] Keys { get; set; }

        void Send(byte[] bytes);

        void Disconnect();
    }
}