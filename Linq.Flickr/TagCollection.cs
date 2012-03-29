using System;
using System.Collections.Generic;
using System.Linq;
using Linq.Flickr.Authentication;
using LinqExtender;
using Linq.Flickr.Repository.Abstraction;
using Linq.Flickr.Repository;

namespace Linq.Flickr
{
    /// <summary>
    /// Depends on photo or gets list of popular tags.
    /// </summary>
    public class TagCollection : Query<Tag>
    {
        private IRepositoryFactory repositoryFactory;

        public TagCollection()
        {
            repositoryFactory = new DefaultRepositoryFactory();
        }

        public TagCollection(AuthInfo authenticationInformation)
        {
            repositoryFactory = new AuthInfoRepository(authenticationInformation);
        }

        protected override bool AddItem()
        {
            var photoId = (string)Bucket.Instance.For.Item(TagColums.PhotoId).Value;

            if (string.IsNullOrEmpty(photoId))
            {
                throw new Exception("Must provide a valid photo id");
            }

            var tags = (string)Bucket.Instance.For.Item(TagColums.Text).Value;

            if (string.IsNullOrEmpty(tags))
                return false;

            using (ITagRepository repository = repositoryFactory.CreateTagRepository())
            {
                return repository.AddTags(photoId, tags);
            }
        }

        protected override bool RemoveItem()
        {
            string tagId = (string)Bucket.Instance.For.Item(TagColums.Id).Value;

            if (string.IsNullOrEmpty(tagId))
            {
                throw  new Exception("Must provide a valid tag id");
            }

            using (ITagRepository repository = repositoryFactory.CreateTagRepository())
            {
                return repository.RemovTag(tagId);
            }
        }

        protected override void Process(LinqExtender.Interface.IModify<Tag> items)
        {
            object tagList = Bucket.Instance.For.Item(TagColums.ListMode).Value;
            TagListMode tagListMode = tagList == null ? TagListMode.Popular : (TagListMode)tagList;


            if (tagListMode == TagListMode.Popular)
            {
                object tagsPeriod = Bucket.Instance.For.Item(TagColums.Period).Value;
                TagPeriod period = tagsPeriod == null ? TagPeriod.Day : (TagPeriod) tagsPeriod;

                int score = Convert.ToInt32(Bucket.Instance.For.Item(TagColums.Score).Value ?? "0");

                int count = (int) Bucket.Instance.For.Item(TagColums.Count).Value;

                if (count > 200)
                {
                    throw new Exception("Tag count should be less than 200");
                }

                using (ITagRepository tagRepositoryRepo = repositoryFactory.CreateTagRepository())
                {
                    IEnumerable<Tag> tags = tagRepositoryRepo.GetPopularTags(period, count);
                    // do the filter on score.

                    if (score > 0)
                    {
                        tags = tags.Where(tag => tag.Score == score).Select(tag => tag);
                    }

                    items.AddRange(tags, true);
                }
            }
            else
            {
                object photoId = Bucket.Instance.For.Item(TagColums.PhotoId).Value;

                if (photoId == null)
                    throw new Exception("Must provide a valid photoId");

                using (ITagRepository tagRepositoryRepo = repositoryFactory.CreateTagRepository())
                {
                    items.AddRange(tagRepositoryRepo.GetTagsForPhoto((string)photoId));   
                }
            }
        }

        public class TagColums
        {
            public const string Id = "Id";
            public const string Period = "Period";
            public const string Count = "Count";
            public const string Score = "Score";
            public const string ListMode = "ListMode";
            public const string PhotoId = "PhotoId";
            public const string Text = "Text";
        }
    }
}
