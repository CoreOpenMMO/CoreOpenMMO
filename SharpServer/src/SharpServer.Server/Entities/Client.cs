using System;
using System.Linq;

namespace SharpServer.Server.Entities
{
    public class Client
    {
        public Client(string adress)
        {
            ClientId = Guid.NewGuid().ToString().ToUpper().Split('-').Last();
            Adress = adress;
        }

        public string ClientId { get; set; }
        public string Adress { get; set; }
    }
}
