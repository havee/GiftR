using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GiftR.Model;

namespace GiftR.Repository
{
    public class SitesRepository : BaseRepository
    {
        public SitesRepository()
            : base()
        {
        }

        public Sites GetSiteByCode(string code)
        {
            var query = from p in db.Sites
                        where p.verification_code == code
                        select p;

            return query.First();
        }

        public bool Exists(string code, out Sites site)
        {
            site = null;
            var query = from p in db.Sites
                        where p.verification_code == code
                        select p;

            if (query != null) site = query.FirstOrDefault();

            return query.Count() > 0;
        }

        public bool Exists(int siteId, out Sites site)
        {
            site = null;
            var query = from p in db.Sites
                        where p.id == siteId
                        select p;

            if (query != null) site = query.First();

            return query.Count() > 0;
        }

        public Sites Save(int userid, string email, Sites site)
        {
            site.date_created = DateTime.Now;

            db.Sites.AddObject(site);
            db.SitesOwners.AddObject(new SitesOwners()
            {
                Sites = site,
                user_id = userid,
                email = email
            });

            db.SaveChanges();

            return site;
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        public List<Sites> GetSiteByUserId(long userId)
        {
            var query = from p in db.SitesOwners
                        where p.Users.userid == userId
                        select p.Sites;

            return query.ToList();
        }

        public void DeleteSite(int siteId)
        {
            Sites site = null;
            if (Exists(siteId, out site))
            {
                var siteOwners = from p in db.SitesOwners
                                 where p.site_id == siteId
                                 select p;

                foreach (var siteOwner in siteOwners)
                {
                    db.DeleteObject(siteOwner);
                }

                db.DeleteObject(site);

                db.SaveChanges();
            }
        }

        public List<Sites> GetSiteByUserId(int id)
        {
            var query = from p in db.SitesOwners
                        where p.Users.id == id
                        select p.Sites;

            return query.ToList();
        }
    }
}
