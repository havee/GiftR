using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GiftR.Model;
using DotNetOpenAuth.ApplicationBlock.Facebook;

namespace GiftR.Common
{
    public class UsersManager
    {
        public static Users ConvertFacebookUser(FacebookGraph user)
        {
            return new Users()
            {
                email = "",
                firstname = user.FirstName,
                lastname = user.LastName,
                userid = user.Id,
                fullname = user.Name,
                source = "facebook",
            };
        }
    }
}
