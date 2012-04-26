using System;
using GiftR.Model;

namespace GiftR.Services
{
    public interface IUsersService
    {
        bool Exists(long id);

        Users GetUserByExternalId(long id);

        Users Save(GiftR.Model.Users user);
    }
}
