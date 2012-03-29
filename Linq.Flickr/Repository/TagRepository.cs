using System.Collections.Generic;
using System.Xml;
using Linq.Flickr.Authentication;
using Linq.Flickr.Repository.Abstraction;

namespace Linq.Flickr.Repository
{
    public class TagRepository : CommonRepository, ITagRepository
    {
        public TagRepository(IFlickrElement elementProxy) : base(elementProxy, typeof(ITagRepository)) 
        {
            this.elementProxy = elementProxy;
        }

        public TagRepository(IFlickrElement elementProxy, AuthInfo authenticationInformation, IAuthRepository authRepository)
            : base(elementProxy, authenticationInformation, typeof(ITagRepository))
        {
            this.elementProxy = elementProxy;
            this.authRepository = authRepository;
        }

        #region ITagRepository Members

        IEnumerable<Tag> ITagRepository.GetPopularTags(TagPeriod period, int count)
        {
            IList<Tag> list = new List<Tag>();

            string method = Helper.GetExternalMethodName();
            string requestUrl = BuildUrl(method, "period", period.ToString().ToLower(), "count", count.ToString());

            XmlElement element = elementProxy.GetResponseElement(requestUrl);

            foreach (var xmlElement in element.Descendants("tag"))
            {
                Tag tag = new Tag(XmlToObject<PopularTag>.Deserialize(xmlElement.OuterXml));

                tag.Period = period;
                tag.Count = count;
                
                list.Add(tag);
            }
            return list;
        }

        IEnumerable<Tag> ITagRepository.GetTagsForPhoto(string photoId)
        {
            IList<Tag> list =new List<Tag>();

            string method = Helper.GetExternalMethodName();
            string requestUrl = BuildUrl(method, "photo_id", photoId);
           
            XmlElement element = elementProxy.GetResponseElement(requestUrl);

            foreach (var xmlElement in element.Descendants("tag"))
            {
                Tag tag = XmlToObject<Tag>.Deserialize(xmlElement.OuterXml);

                tag.PhotoId = photoId;
                tag.ListMode = TagListMode.PhotoSpecific;

                list.Add(tag);   
            }

            return list;
        }

        bool ITagRepository.RemovTag(string tagId)
        {
            string authenitcatedToken = authRepository.Authenticate(Permission.Delete);
            string method = Helper.GetExternalMethodName();

            string sig = GetSignature(method, true, "tag_id", tagId, "auth_token", authenitcatedToken);
            string requestUrl = BuildUrl(method, "tag_id", tagId, "auth_token", authenitcatedToken, "api_sig", sig);

            try
            {
                elementProxy.SendPostRequest(requestUrl);
                return true;
            }
            catch
            {
                return false;
            }
        }

        bool ITagRepository.AddTags(string photoId, string tags)
        {
            string authenitcatedToken = authRepository.Authenticate(Permission.Delete);
            string method = Helper.GetExternalMethodName();

            string sig = GetSignature(method, true, "photo_id", photoId, "tags", tags, "auth_token", authenitcatedToken);
            string requestUrl = BuildUrl(method, "photo_id", photoId, "tags", tags, "auth_token", authenitcatedToken, "api_sig", sig);

            try
            {
                elementProxy.SendPostRequest(requestUrl);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        private IAuthRepository authRepository;

        #region IDisposable Members

        public void Dispose()
        {
            // nothing here.
        }
        #endregion

        private IFlickrElement elementProxy;
    }
}
