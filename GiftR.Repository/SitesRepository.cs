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
                        where p.verificacion_code == code
                        select p;

            return query.First();
        }

        public bool Exists(string code, out Sites site)
        {
            site = null;
            var query = from p in db.Sites
                        where p.verificacion_code == code
                        select p;

            if (query != null) site = query.First();

            return query.Count() > 0;
        }

        public Sites Save(int userid, string email, Sites site)
        {
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
    }
}
