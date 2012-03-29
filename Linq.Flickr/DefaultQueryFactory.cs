using Linq.Flickr.Repository.Abstraction;

namespace Linq.Flickr
{
    public class DefaultQueryFactory : IQueryFactory
    {
        /// <summary>
        /// Initializes a new instance of  <see cref="DefaultQueryFactory"/> class.
        /// </summary>
        /// <param name="elementProxy"></param>
        public DefaultQueryFactory(IFlickrElement elementProxy)
        {
            this.elementProxy = elementProxy;
        }

        /// <summary>
        /// Creates the tag query entry point.
        /// </summary>
        /// <returns></returns>
        public TagCollection CreateTagQuery()
        {
            return new TagCollection();
        }

        /// <summary>
        /// Creates people query entry point
        /// </summary>
        /// <returns></returns>
        public PeopleCollection CreatePeopleQuery()
        {
            return new PeopleCollection();
        }

        /// <summary>
        /// Creates photo query entry point.
        /// </summary>
        /// <returns></returns>
        public PhotoCollection CreatePhotoQuery()
        {
            return new PhotoCollection(elementProxy);
        }

        private IFlickrElement elementProxy;
    }
}