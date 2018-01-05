using System.Linq;
using COTS.Data.Context;
using COTS.Domain.Entities;
using COTS.Domain.Interfaces.Repositories;

namespace COTS.Data.Repositories
{
    public class PlayerRepository : BaseRepository<Player>, IPlayerRepository
    {
        public PlayerRepository(COTSContext context) : base (context)
        {
            Init();
        }

        public void Init()
        {
            using (var db = new COTSContext())
            {
                if (!db.Player.Any())
                {
                    db.Player.Add(new Player()
                    {
                        Name = "Player 1",
                        UserId = 1,
                        WorldId = 1
                    });
                    db.Player.Add(new Player()
                    {
                        Name = "Player 2",
                        UserId = 1,
                        WorldId = 1
                    });

                    db.SaveChanges();
                }
            }
        }
    }
}


