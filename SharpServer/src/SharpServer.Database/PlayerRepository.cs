using System.Linq;
using SharpServer.Domain.Entities;

namespace SharpServer.Database
{
    public class PlayerRepository
    {
        public PlayerRepository()
        {
            Init();
        }

        public Player GetPlayerByGuid(int guid)
        {
            using (var db = new ServerContext())
            {
                return db.Player.FirstOrDefault(c => c.PlayerGuid.Equals(guid));
            }
        }

        //TODO delete this method
        public string GetPlayerNameByGuid(int guid)
        {
            using (var db = new ServerContext())
            {
                return db.Player.FirstOrDefault(c => c.PlayerGuid.Equals(guid)).Name;
            }
        }

        public void Init()
        {
            using (var db = new ServerContext())
            {
                if (!db.Player.Any())
                {
                    db.Player.Add(new Player()
                    {
                        Name = "Felipe",
                        UserId = 1,
                        WorldId = 1
                    });
                    db.Player.Add(new Player()
                    {
                        Name = "Muniz",
                        UserId = 1,
                        WorldId = 1
                    });

                    db.SaveChanges();
                }
            }
        }
    }
}


