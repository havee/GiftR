using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GiftR.Model;

namespace GiftR.Repository
{
    public class UsersRepository : IDisposable
    {
        GiftRModel db;

        public UsersRepository()
        {
            db = new GiftRModel();
        }

        public List<Users> GetUsers()
        {
            List<Users> users = db.Users.ToList();

            return users;
        }

        public bool Exists(Users user)
        {
            var query = from p in db.Users
                        where user.userid == p.userid
                        select p.id;

            return query.Count() > 0;
        }

        public Users SaveUser(Users user)
        {
            db.Users.AddObject(user);
            db.SaveChanges();

            return user;
        }

        public void Dispose()
        {
        }
    }
}
