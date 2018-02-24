using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace COTS.Data.Repositories
{
    using Context;
    using Domain.Entities;
    using Domain.Interfaces.Repositories;

    public class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        public AccountRepository(COTSContext context) : base(context) {
            Init();
        }

        public async Task<Account> GetAccountByLogin(string username, string password)
        {
            using (var db = new COTSContext())
            {
                return await db.Account.FirstOrDefaultAsync(c => c.UserName.Equals(username) && c.Password.Equals(password));
            }
        }
        
        public void Init()
        {
            using (var db = new COTSContext())
            {
                if (!db.Account.Any())
                {
                    db.Account.Add(new Account()
                    {
                        Password = "",
                        UserName = "123"
                    });
                    db.Account.Add(new Account()
                    {
                        Password = "2",
                        UserName = "2"
                    });
                    db.Account.Add(new Account()
                    {
                        Password = "3",
                        UserName = "3"
                    });
                    db.Account.Add(new Account()
                    {
                        Password = "4",
                        UserName = "4"
                    });
                    db.Account.Add(new Account()
                    {
                        Password = "5",
                        UserName = "5"
                    });

                    db.SaveChanges();
                }
            }
        }
    }
}


