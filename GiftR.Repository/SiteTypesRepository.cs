using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GiftR.Model;

namespace GiftR.Repository
{
    public class SiteTypesRepository : BaseRepository
    {
        public SiteTypesRepository()
            : base()
        {
        }

        public List<SitesTypes> ListAll()
        {
            var query = from p in db.SitesTypes
                        select p;

            return query.ToList();
        }
    }
}
