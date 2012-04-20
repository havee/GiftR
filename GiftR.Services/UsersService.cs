using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GiftR.Model;
using GiftR.Repository;

namespace GiftR.Services
{
    public static class UsersService
    {
        public static bool Exists(long id)
        {
            UsersRepository usersRepo = new UsersRepository();
            return usersRepo.Exists(id);            
        }

        public static Users Save(Users user)
        {
            UsersRepository usersRepo = new UsersRepository();
            return usersRepo.Save(user);
        }

        public static Users GetUserByExternalId(long id)
        {
            var usersRepo = new UsersRepository();
            return usersRepo.GetUserByExternalId(id);
        }
    }
}
