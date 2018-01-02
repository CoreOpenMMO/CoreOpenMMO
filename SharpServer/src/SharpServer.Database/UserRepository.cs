using System.Linq;
using SharpServer.Domain.Entities;

namespace SharpServer.Database
{
    public class UserRepository
    {
        public UserRepository()
        {
            Init();
        }

        public bool CheckUserLogin(string username, string password)
        {
            using (var db = new ServerContext())
            {
                if (db.User.Any(c => c.UserName.Equals(username) && c.Password.Equals(password)))
                    return true;

                return false;
            }
        }
        
        public void Init()
        {
            using (var db = new ServerContext())
            {
                if (!db.User.Any())
                {
                    db.User.Add(new User()
                    {
                        Password = "1",
                        UserName = "1"
                    });
                    db.User.Add(new User()
                    {
                        Password = "2",
                        UserName = "2"
                    });
                    db.User.Add(new User()
                    {
                        Password = "3",
                        UserName = "3"
                    });

                    db.User.Add(new User()
                    {
                        Password = "4",
                        UserName = "4"
                    });
                    db.User.Add(new User()
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


