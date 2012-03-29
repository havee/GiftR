using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Net;
using Linq.Flickr.Authentication;
using Linq.Flickr.Repository.Abstraction;

namespace Linq.Flickr.Repository
{
    /// <summary>
    /// Defines methods to perform various photo related operations on flickr.
    /// </summary>
    public class PhotoRepository : CommonRepository, IPhotoRepository
    {
        public PhotoRepository(IFlickrElement elementProxy) : base(elementProxy, typeof(IPhotoRepository))
        {
            this.elementProxy = elementProxy;
            authRepo = new AuthRepository(elementProxy);
        }

        public PhotoRepository(IFlickrElement elementProxy, AuthInfo authenticationInformation) 
            : base (elementProxy, authenticationInformation, typeof(IPhotoRepository))
        {
            this.elementProxy = elementProxy;
            authRepo = new AuthRepository(elementProxy, authenticationInformation);
        }

        People IPhotoRepository.GetUploadStatus()
        {
            string token = authRepo.Authenticate(Permission.Delete);
            
            string method = Helper.GetExternalMethodName();
            string sig = base.GetSignature(method, true, "auth_token", token);
            string requestUrl = BuildUrl(method, "api_sig", sig, "auth_token", token);

            try
            {
                XmlElement element = elementProxy.GetResponseElement(requestUrl);

                People people = (from p in element.Descendants("user")
                select new People
                {
                    Id = p.Attribute("id").Value ?? string.Empty,
                    IsPro = Convert.ToInt32(p.Attribute("ispro").Value) == 0 ? false : true,
                    BandWidth = (from b in element.Descendants("bandwidth")
                    select new BandWidth
                    {
                        RemainingKb =  Convert.ToInt32(b.Attribute("remainingkb").Value),
                        UsedKb = Convert.ToInt32(b.Attribute("usedkb").Value)
                    }).Single<BandWidth>()
                }).Single<People>();

                return people;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        bool IPhotoRepository.SetMeta(string photoId, string title, string description)
        {
            string method = Helper.GetExternalMethodName();
            string token = authRepo.Authenticate(Permission.Delete);
            string sig = base.GetSignature(method, true,"photo_id", photoId, "title", title, "description", description, "auth_token", token);
            string requestUrl = BuildUrl(method, "photo_id", photoId, "title", title, "description", description, "auth_token", token, "api_sig", sig);

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

        IList<Photo> IPhotoRepository.GetMostInteresting(int index, int itemsPerPage, PhotoSize size)
        {
            string method = Helper.GetExternalMethodName();
            string requestUrl = BuildUrl(method, "page", index.ToString(), "per_page", itemsPerPage.ToString());

            IList<Photo> photos = new List<Photo>();

            try
            {
                photos = GetPhotos(requestUrl, size).ToList<Photo>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return photos;
        }

        string IPhotoRepository.GetNsidByUsername(string username)
        {
            string method = Helper.GetExternalMethodName();
            return base.GetNsid(method, "username", username);
        }

        string IPhotoRepository.GetNsidByEmail(string email)
        {
            string method = Helper.GetExternalMethodName();
            return base.GetNsid(method, "find_email", email);
        }

        internal class PhotoSizeWrapper
        {
            public string Label {get;set;}
            public string Url { get; set; }
        }

        internal PhotoSize PhotoSize { get; set; }
        internal ViewMode Visibility { get; set; }

        string IPhotoRepository.GetSizedPhotoUrl(string id, PhotoSize size)
        {
            if (size == PhotoSize.Original)
            {
                string method = Helper.GetExternalMethodName();
                string requestUrl = BuildUrl(method, "photo_id", id);

                XmlElement doc = elementProxy.GetResponseElement(requestUrl);

                var query = from sizes in doc.Descendants("size")
                            select new PhotoSizeWrapper
                            {
                                Label = sizes.Attribute("label").Value ?? string.Empty,
                                Url = sizes.Attribute("source").Value ?? string.Empty
                            };

                PhotoSizeWrapper[] sizeWrapper = query.ToArray<PhotoSizeWrapper>();
                try
                {
                    return sizeWrapper[(int)size].Url;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Searchs the photos for supplied name value settings.
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="pageLen">Number of items on each page</param>
        /// <param name="photoSize"></param>
        /// <param name="token">Authentication token, if logged in</param>
        /// <param name="args">Search criteria</param>
        /// <returns>list of photos</returns>
        IEnumerable<Photo> IPhotoRepository.Search(int index, int pageLen, PhotoSize photoSize, string token, params object[] args)
        {
            string method = Helper.GetExternalMethodName();

            string sig = string.Empty;

            if (!string.IsNullOrEmpty(token))
            {
                IDictionary<string, string> sorted = new Dictionary<string, string>();
                ProcessArguments(args, sorted);
                ProcessArguments(new object[] { "page", index.ToString(), "per_page", pageLen.ToString(), "auth_token", token }, sorted);
                sig = base.GetSignature(method, true, sorted);
            }

            IDictionary<string, string> dicionary = new Dictionary<string, string>();

            dicionary.Add(Helper.BASE_URL + "?method", method);
            dicionary.Add("api_key", Provider.GetCurrentFlickrSettings().ApiKey);

            ProcessArguments(args, dicionary);
            ProcessArguments( new object [] {"api_sig", sig, "page", index.ToString(), "per_page", pageLen.ToString(), "auth_token", token }, dicionary);

            string requestUrl = GetUrl(dicionary);

            if (index < 1 || index > 500)
            {
                throw new FlickrException("Index must be between 1 and 500");
            }

            return GetPhotos(requestUrl, photoSize);
        }
 
        #region PhotoGetBlock
        
        private IEnumerable<Photo> GetPhotos(string requestUrl, PhotoSize size)
        {
            XmlElement doc = elementProxy.GetResponseElement(requestUrl);
            XmlElement photosElement = doc.Element("photos");

            CollectionBuilder<Photo> builder = new CollectionBuilder<Photo>("photos");

            CollectionBuilder<CommonAttribute> commBuilder =
            new CollectionBuilder<CommonAttribute>("photos");
            CommonAttribute sharedProperty = commBuilder.ToCollection(doc, null).Single();

            return builder.ToCollection(photosElement, photo =>
             {
                 photo.Url = (this as IPhotoRepository).GetSizedPhotoUrl(photo.Id, size) ?? string.Empty;
                 photo.PhotoSize = size;
                 photo.SharedProperty = sharedProperty;
             });
        } 

        #endregion
        
        /// <summary>
        /// calls flickr.photos.getInfo to get the photo object.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="size"></param>
        /// <returns>Detail of photo</returns>
        Photo IPhotoRepository.GetPhotoDetail(string id, PhotoSize size)
        {
            this.PhotoSize = size;
        
            string method = Helper.GetExternalMethodName();

            string token = authRepo.Authenticate(Permission.Delete.ToString(), false);
            string sig = base.GetSignature(method, true, "photo_id", id, "auth_token", token);
            string requestUrl = BuildUrl(method, "photo_id", id, "auth_token", token, "api_sig", sig);

            XmlElement doc = elementProxy.GetResponseElement(requestUrl);

            var query = from photo in doc.Descendants("photo")
                        select new Photo
                                   {
                                       Id = photo.Attribute("id").Value,
                                       FarmId = photo.Attribute("farm").Value,
                                       ServerId = photo.Attribute("server").Value,
                                       SecretId = photo.Attribute("secret").Value,
                                       Title = photo.Element("title").InnerText,
                                       User = photo.Element("owner").Attribute("username").Value ?? string.Empty,
                                       NsId = photo.Element("owner").Attribute("nsid").Value ?? string.Empty,
                                       Description = photo.Element("description").InnerText ?? string.Empty,
                                       DateUploaded = photo.Element("dates").Attribute("posted").Value ?? string.Empty,
                                       DateTaken = photo.Element("dates").Attribute("taken").Value ?? string.Empty,
                                       LastUpdated =
                                           photo.Element("dates").Attribute("lastupdate").Value ?? string.Empty,
                                       Tags = (from tag in photo.Descendants("tag")
                                               select tag.InnerText ?? string.Empty).ToArray(),
                                       PhotoSize = size,
                                       WebUrl = (from photoPage in photo.Descendants("url")
                                                    where photoPage.Attribute("type").Value == "photopage"
                                                    select photoPage.InnerText
                                                   ).First(),
                                       Url = PhotoDetailUrl(photo.Attribute("id").Value, size)
                                   };

            return query.Single<Photo>();
        }

        private string PhotoDetailUrl(string photoId, PhotoSize size)
        {
            return (this as IPhotoRepository).GetSizedPhotoUrl(photoId, size);
        }

        public void Dispose()
        {

        }

        private static void EncodeAndAddItem(string boundary, ref StringBuilder baseRequest, params object [] items)
        {
            if (baseRequest == null)
            {
                baseRequest = new StringBuilder();
            }

            const string form_data = "Content-Disposition: form-data; name=\"{0}\"\r\n";
            const string photo_key = "Content-Disposition: form-data; name=\"{0}\";filename=\"{1}\"\r\n";
            const string escape = "\r\n";
            const string content_type = "Content-Type:application/octet-stream";
            const string dash = "--";

            for (int index = 0; index < items.Length; index += 2)
            {
                string key = Convert.ToString(items[index]);
                string value = string.Empty;

                if (index + 1 < items.Length)
                {
                    value = Convert.ToString(items[index + 1]);
                }

                if (!string.IsNullOrEmpty(value))
                {
                    baseRequest.Append(dash);
                    baseRequest.Append(boundary);
                    baseRequest.Append(escape);

                    if (string.Compare(key, "Photo", true) == 0)
                    {
                        baseRequest.Append(string.Format(photo_key, key, value));
                        baseRequest.Append(content_type);
                        baseRequest.Append(escape);
                        baseRequest.Append(escape);
                    }
                    else
                    {
                        baseRequest.Append(string.Format(form_data, key));
                        baseRequest.Append(escape);
                        baseRequest.Append(value);
                        baseRequest.Append(escape);
                    }
                }
            }
        
        }

        bool IPhotoRepository.Delete(string photoId)
        {
            string token = authRepo.Authenticate(Permission.Delete);
            string method = Helper.GetExternalMethodName();

            string sig = base.GetSignature(method, true, "photo_id", photoId, "auth_token", token);
            string requestUrl = BuildUrl(method, "photo_id", photoId, "auth_token", token, "api_sig", sig);

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

        /// <summary>
        /// Uploades a photo file with the fileName and byte[] data provided.
        /// </summary>
        /// <param name="args">visibility attributes</param>
        /// <param name="fileName">name of the file</param>
        /// <param name="photoData">content</param>
        /// <returns>photoId</returns>
        string IPhotoRepository.Upload(object[] args, string fileName, byte[] photoData)
        {
            string token = authRepo.Authenticate(Permission.Delete);

            const string boundary = "FLICKR_BOUNDARY";

            IDictionary<string, string> sorted = new Dictionary<string, string>();

            ProcessArguments(new object[]{ "auth_token", token }, sorted);

            string sig = base.GetSignature(Helper.UPLOAD_URL, false,sorted, args);
           
            StringBuilder builder = new StringBuilder();

            EncodeAndAddItem(boundary, ref builder, new object[] { "api_key", Provider.GetCurrentFlickrSettings().ApiKey, "auth_token", token, "api_sig", sig});
            EncodeAndAddItem(boundary, ref builder, args);
            EncodeAndAddItem(boundary, ref builder, new object[] { "photo", fileName});

            //builder = builder.Remove(builder.Length - 4, 4);
           
            // Create a request using a URL that can receive a post. 
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(Helper.UPLOAD_URL);
            // Set the Method property of the request to POST.
            request.Method = "POST";
            request.KeepAlive = true;
            // never timeout.
            request.Timeout = 300000;
            // Set the ContentType property of the WebRequest.
            request.ContentType = "multipart/form-data;charset=UTF-8;boundary=" + boundary + "";
            
            byte[] photoAttributeData = Encoding.UTF8.GetBytes(builder.ToString());
            byte[] footer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

            byte[] postContent = new byte[photoData.Length + photoAttributeData.Length + footer.Length];
            
            Buffer.BlockCopy(photoAttributeData, 0, postContent, 0, photoAttributeData.Length);
            Buffer.BlockCopy(photoData, 0, postContent, photoAttributeData.Length, photoData.Length);
            Buffer.BlockCopy(footer, 0, postContent, photoData.Length + photoAttributeData.Length, footer.Length);

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = postContent.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();

            dataStream.Write(postContent, 0, postContent.Length);
            // Close the Stream object.
            dataStream.Flush();
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Clean up the streams.
            response.Close();
            reader.Close();
            
            // get the photo id.
            XmlElement elemnent = elementProxy.GetElementFromResponse(responseFromServer);
            return elemnent.Element("photoid").InnerText ?? string.Empty;
        }

        private IAuthRepository authRepo;
        private IFlickrElement elementProxy;

    }
}
