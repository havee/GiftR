using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GiftR.Model;
using GiftR.Repository;

namespace GiftR.Services
{
    public class UsersService : GiftR.Services.IUsersService
    {
        public bool Exists(long id)
        {
            UsersRepository usersRepo = new UsersRepository();
            return usersRepo.Exists(id);            
        }

        public Users Save(Users user)
        {
            UsersRepository usersRepo = new UsersRepository();
            return usersRepo.Save(user);
        }

        public Users GetUserByExternalId(long id)
        {
            var usersRepo = new UsersRepository();
            return usersRepo.GetUserByExternalId(id);
        }
    }
}
