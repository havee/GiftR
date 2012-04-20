using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GiftR.Model;

namespace GiftR.Repository
{
    public class UsersRepository : BaseRepository
    {
        public UsersRepository() 
            : base()
        {
        }

        public List<Users> GetUsers()
        {
            List<Users> users = db.Users.ToList();

            return users;
        }

        public bool Exists(Users user)
        {
            return Exists(user.id);
        }

        public bool Exists(long id)
        {
            var query = from p in db.Users
                        where id == p.userid
                        select p.id;

            return query.Count() > 0;
        }

        public Users GetUserByExternalId(long externalId)
        {
            var query = from p in db.Users
                        where p.userid == externalId
                        select p;

            return query.Count() > 0 ? query.First() : null;
        }

        public Users Save(Users user)
        {
            if (user.EntityState == System.Data.EntityState.Added)
            {
                db.Users.AddObject(user);
            }
            db.SaveChanges();

            return user;
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }
    }
}
