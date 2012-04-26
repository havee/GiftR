using System;
using System.Collections.Generic;
using GiftR.Model;

namespace GiftR.Services
{
    public interface ISitesService
    {
        void Delete(int siteId);

        Sites GetSiteByCode(string code);

        List<Sites> GetSiteById(int id);

        List<Sites> GetSiteByUserId(long userId);

        Sites Save(int userid, string email, GiftR.Model.Sites site);
    }
}
