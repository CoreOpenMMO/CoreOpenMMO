using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COTS.Data.Context;
using COTS.Domain.Entities;
using COTS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace COTS.Data.Repositories
{
    public class PlayerRepository : BaseRepository<Player>, IPlayerRepository
    {
        public PlayerRepository(COTSContext context) : base (context)
        {
            Init();
        }

        public async Task<List<string>> GetCharactersListByAccountId(int id)
        {
            using (var db = new COTSContext())
            {
                var characters = new List<string>();
                await db.Player.Where(c => c.AccountId.Equals(id)).ForEachAsync(c => 
                {
                    characters.Add(c.Name);
                });

                return characters;
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


