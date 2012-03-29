using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GiftR.Model;

namespace GiftR.Repository
{
    public class BaseRepository
    {
        protected GiftRModel db;

        public BaseRepository()
        {
            db = new GiftRModel();
        }
    }
}
