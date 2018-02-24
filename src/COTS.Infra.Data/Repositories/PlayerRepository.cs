using System.Collections.Generic;
using System.Linq;

namespace COTS.Data.Repositories
{
    using Context;
    using Domain.Entities;
    using Domain.Interfaces.Repositories;

    public class PlayerRepository : BaseRepository<Player>, IPlayerRepository
    {
        public PlayerRepository(COTSContext context) : base (context)
        {
            Init();
        }

        public IEnumerable<string> GetCharactersListByAccountId(int id)
        {
            using (var db = new COTSContext())
            {
                return FindBy(x => x.AccountId.Equals(id)).Select(x => x.Name);
            }
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
                        AccountId = 1,
                        WorldId = 1
                    });
                    db.Player.Add(new Player()
                    {
                        Name = "Player 2",
                        AccountId = 1,
                        WorldId = 1
                    });
                    db.Player.Add(new Player()
                    {
                        Name = "Player 3",
                        AccountId = 1,
                        WorldId = 1
                    });
                    db.Player.Add(new Player()
                    {
                        Name = "Player 4",
                        AccountId = 1,
                        WorldId = 1
                    });

                    db.SaveChanges();
                }
            }
        }

    }
}


