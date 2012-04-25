using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GiftR.Repository;
using GiftR.Model;

namespace GiftR.Services
{
    public static class SitesService
    {
        public static Sites GetSiteByCode(string code)
        {
            var sitesRepo = new SitesRepository();
            Sites site;
            if (sitesRepo.Exists(code, out site))
            {
                return site;
            }

            return null;
        }

        public static List<Sites> GetSiteByUserId(long userId)
        {
            var sitesRepo = new SitesRepository();
            
            return sitesRepo.GetSiteByUserId(userId);            
        }

        public static Sites Save(int userid, string email, Sites site)
        {
            SitesRepository sitesRepo = new SitesRepository();

            return sitesRepo.Save(userid, email, site);
        }

        public static void Delete(int siteId)
        {
            SitesRepository sitesRepo = new SitesRepository();

            sitesRepo.DeleteSite(siteId);
        }

        public static List<Sites> GetSiteById(int id)
        {
            var sitesRepo = new SitesRepository();

            return sitesRepo.GetSiteByUserId(id); 
        }
    }
}
