using System.Linq;
using SharpServer.Domain.Entities;

namespace SharpServer.Database
{
    public class AccountRepository
    {
        public AccountRepository()
        {
            Init();
        }

        public bool CheckAccountLogin(string username, string password)
        {
            using (var db = new ServerContext())
            {
                return db.Account.Any(c => c.UserName.Equals(username) && c.Password.Equals(password));
            }
        }
        
        public void Init()
        {
            using (var db = new ServerContext())
            {
                if (!db.Account.Any())
                {
                    db.Account.Add(new Account()
                    {
                        Password = "1",
                        UserName = "1"
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


