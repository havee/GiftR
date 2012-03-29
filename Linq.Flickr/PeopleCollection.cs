using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Linq.Flickr.Authentication;
using LinqExtender;
using Linq.Flickr.Repository.Abstraction;
using Linq.Flickr.Repository;

namespace Linq.Flickr
{
    public class PeopleCollection : Query<People>
    {
        private readonly IRepositoryFactory repositoryFactory;

        public PeopleCollection()
        {
            repositoryFactory = new DefaultRepositoryFactory();
        }

        public PeopleCollection(AuthInfo authenticationInformation)
        {
            repositoryFactory = new AuthInfoRepository(authenticationInformation);
        }

        protected override bool AddItem()
        {
            throw new Exception("Add item not supported for People");
        }

        protected override bool RemoveItem()
        {
            throw new Exception("Remove item not supported for People");
        }

        private static class PeopleColumns
        {
            public const string Id = "Id";
            public const string Username = "Username";
        }

        protected override void Process(LinqExtender.Interface.IModify<People> items)
        {
            using (IPeopleRepository peopleRepositoryRepo = repositoryFactory.CreatePeopleRepository())
            {
                string userId = (string)Bucket.Instance.For.Item(PeopleColumns.Id).Value;
                string username = (string)Bucket.Instance.For.Item(PeopleColumns.Username).Value;

                People people = null;

                if (!string.IsNullOrEmpty(userId))
                {
                    people = peopleRepositoryRepo.GetInfo(userId);
                }
                else if (!string.IsNullOrEmpty(username))
                {
                    people = peopleRepositoryRepo.GetByUsername(username);
                }
                else
                {
                    // try to get autheticated person
                    AuthToken token = peopleRepositoryRepo.GetAuthenticatedToken();

                    if (token != null)
                        people = peopleRepositoryRepo.GetInfo(token.UserId);
                    else     
                        throw new Exception("Query must contain a valid user id or name");
                }

                items.Add(people);
            }
        }
    }
}
