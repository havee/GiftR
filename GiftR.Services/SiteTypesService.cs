using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GiftR.Repository;
using GiftR.Model;

namespace GiftR.Services
{
    public class SiteTypesService : GiftR.Services.ISiteTypesService
    {
        public List<SitesTypes> ListAll()
        {
            var repo = new SiteTypesRepository();
            return repo.ListAll();
        }
    }
}
