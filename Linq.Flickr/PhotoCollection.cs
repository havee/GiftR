using System;
using System.Collections.Specialized;
using LinqExtender.Attribute;
using LinqExtender.Interface;
using LinqExtender;
using Linq.Flickr.Repository;
using Linq.Flickr.Authentication;
using Linq.Flickr.Repository.Abstraction;

namespace Linq.Flickr
{
    public class PhotoCollection : Query<Photo>
    {
        public PhotoCollection(IFlickrElement elementProxy)
        {
            this.elementProxy = elementProxy;
            repositoryFactory = new DefaultRepositoryFactory();
        }

        public PhotoCollection(IFlickrElement elementProxy, AuthInfo authenticationInformation)
            :this(elementProxy)
        {
            repositoryFactory = new AuthInfoRepository(authenticationInformation);
        }

        // Comments can not stay alone, it is a part of photo.
        public CommentCollection Comments
        {
            get
            {
                if (commentCollection == null)
                {
                    commentCollection = new CommentCollection(elementProxy);
                }

                return commentCollection;
            }
        }

        protected override Photo GetItem()
        {
            // default values
            PhotoSize size = Bucket.Instance.For.Item(PhotoColumns.Photosize).Value == null ? PhotoSize.Square : (PhotoSize)Bucket.Instance.For.Item(PhotoColumns.Photosize).Value;
            ViewMode viewMode = Bucket.Instance.For.Item(PhotoColumns.Viewmode).Value == null ? ViewMode.Public : (ViewMode)Bucket.Instance.For.Item(PhotoColumns.Viewmode).Value;
         
            Photo photo = null;
            using (IAuthRepository authRepository = repositoryFactory.CreateAuthRepository())
            {
                GenerateToken(viewMode, authRepository);
           
                if (Bucket.Instance.For.Item(PhotoColumns.ID).Value != null)
                {
                    using (IPhotoRepository photoRepository = repositoryFactory.CreatePhotoRepository())
                    {
                        photo = photoRepository.GetPhotoDetail((string)Bucket.Instance.For.Item(PhotoColumns.ID).Value, size);
                    }

                }
            }
            return photo;
        }

        protected override bool AddItem()
        {
            using (IPhotoRepository flickr = repositoryFactory.CreatePhotoRepository())
            {
                if (people == null)
                    people = flickr.GetUploadStatus();
              
                var nvCollection = new NameValueCollection();

                ViewMode viewMode = Bucket.Instance.For.Item(PhotoColumns.Viewmode).Value == null
                                        ? ViewMode.Public
                                        : (ViewMode) Bucket.Instance.For.Item(PhotoColumns.Viewmode).Value;

                int isPublic = viewMode == ViewMode.Public ? 1 : 0;
                int isFriend = viewMode == ViewMode.Friends || viewMode == ViewMode.FriendsFamily ? 1 : 0;
                int isFamily = viewMode == ViewMode.Family || viewMode == ViewMode.FriendsFamily ? 1 : 0;


                nvCollection.Add("is_public", isPublic.ToString());
                nvCollection.Add("is_friend", isFriend.ToString());
                nvCollection.Add("is_family", isFamily.ToString());
                nvCollection.Add(Bucket.Instance.For.Item(PhotoColumns.Title).Name, Convert.ToString(Bucket.Instance.For.Item(PhotoColumns.Title).Value));
                nvCollection.Add(Bucket.Instance.For.Item(PhotoColumns.Desc).Name, Convert.ToString(Bucket.Instance.For.Item(PhotoColumns.Desc).Value));
                
                var fileName = (string) Bucket.Instance.For.Item(PhotoColumns.Filename).Value;

                if (string.IsNullOrEmpty(fileName))
                    throw new Exception("Please key in the filename for the photo");

                byte[] postContnet = (byte[]) Bucket.Instance.For.Item(PhotoColumns.PhotoContent).Value;

                if (postContnet == null || postContnet.Length == 0)
                    throw new Exception("Zero photo length detected, please key in a valid photo file");

                // check if the user has any storage.
                int kbTobUploaded = (int) Math.Ceiling((postContnet.Length/1024f));
                if (people.BandWidth != null)
                {
                    int currentByte = people.BandWidth.UsedKb + kbTobUploaded;
                    if (currentByte >= people.BandWidth.RemainingKb)
                    {
                        throw new Exception("Storage limit excceded, try pro account!!");
                    }
                }

                string [] attributes = new string[nvCollection.Count * 2];


                int index = 0;

                foreach (string result in nvCollection.AllKeys)
                {
                    attributes[index++] = result;
                    attributes[index++] = nvCollection[result];
                }


                string photoId = flickr.Upload(attributes, fileName, postContnet);
                // set the id.
                Bucket.Instance.For.Item(PhotoColumns.ID).Value = photoId;

                // do the math
                if (people.BandWidth != null)
                {
                    people.BandWidth.UsedKb += kbTobUploaded;
                    people.BandWidth.RemainingKb -= kbTobUploaded;
                }

                return (string.IsNullOrEmpty(photoId) == false);
            }
        }

        protected override bool RemoveItem()
        {
            using (IPhotoRepository flickr = repositoryFactory.CreatePhotoRepository())
            {
                if (!string.IsNullOrEmpty((string)Bucket.Instance.For.Item(PhotoColumns.ID).Value))
                {
                    return flickr.Delete((string) Bucket.Instance.For.Item(PhotoColumns.ID).Value);
                }
                else
                {
                    throw new Exception("Must have valid photo id to perform delete operation");
                }
            }
        }
        protected override bool UpdateItem()
        {
            string photoId = (string)Bucket.Instance.For.Item(PhotoColumns.ID).Value;

            if (string.IsNullOrEmpty(photoId))
                throw new Exception("Must provide a valid photoId");

            string title = (string)Bucket.Instance.For.Item(PhotoColumns.Title).Value;
            string description = (string)Bucket.Instance.For.Item(PhotoColumns.Desc).Value;

            if (string.IsNullOrEmpty(title))
            {
                throw new Exception("photo title can not be empty");
            }

            using (IPhotoRepository photo = repositoryFactory.CreatePhotoRepository())
            {
                return photo.SetMeta(photoId, title, string.IsNullOrEmpty(description) ? " " : description);
            }
        }

        private static class PhotoColumns
        {
            public const string ID = "Id";
            public const string User = "User";
            public const string Nsid = "NsId";
            public const string Searchtext = "SearchText";
            public const string Photosize = "PhotoSize";
            public const string Viewmode = "ViewMode";
            public const string Title = "Title";
            public const string Desc = "Description";
            public const string Filename = "FileName";
            public const string PhotoContent = "PhotoContent";
            public const string SearchMode = "SearchMode";

            public static string Extras
            {
                get { return "Extras"; }
            }
        }

        protected override void Process(IModify<Photo> items)
        {
            using (IPhotoRepository flickr = repositoryFactory.CreatePhotoRepository())
            {
                PhotoSize size = Bucket.Instance.For.Item(PhotoColumns.Photosize).Value == null
                                     ? PhotoSize.Square
                                     : (PhotoSize) Bucket.Instance.For.Item(PhotoColumns.Photosize).Value;

                ViewMode viewMode = Bucket.Instance.For.Item(PhotoColumns.Viewmode).Value == null
                                        ? ViewMode.Public
                                        : (ViewMode) Bucket.Instance.For.Item(PhotoColumns.Viewmode).Value;
         
                int index = Bucket.Instance.Entity.ItemsToSkipFromStart + 1;
                if (index == 0) index = index + 1;

                int itemsToTake = 100;

                if (Bucket.Instance.Entity.ItemsToFetch != null)
                {
                    itemsToTake = Bucket.Instance.Entity.ItemsToFetch.Value;
                }
               
                bool fetchRecent = true;

                /// if there is not tag text, tag or id methioned in search , also want to get my list of images,
                fetchRecent &= string.IsNullOrEmpty((string) Bucket.Instance.For.Item(PhotoColumns.Searchtext).Value);
                fetchRecent &= viewMode != ViewMode.Owner;
                fetchRecent &= string.IsNullOrEmpty((string)Bucket.Instance.For.Item(PhotoColumns.User).Value);
                fetchRecent &= string.IsNullOrEmpty((string)Bucket.Instance.For.Item(PhotoColumns.Nsid).Value);

                bool unique = Bucket.Instance.For.Item(PhotoColumns.ID).Unique &&
                              !string.IsNullOrEmpty((string) Bucket.Instance.For.Item(PhotoColumns.ID).Value);

                Bucket.Instance.Entity.OrderBy.IfUsed.Process(delegate 
                {
                    fetchRecent = false;
                });
              
                /// unique property has higher precendence over general search query.
                if (unique)
                {
                    using (IAuthRepository authRepository = repositoryFactory.CreateAuthRepository())
                    {
                        GenerateToken(viewMode, authRepository);
                        Photo photo = flickr.GetPhotoDetail((string) Bucket.Instance.For.Item(PhotoColumns.ID).Value,
                                                            size);

                        if (photo != null)
                        {
                            items.Add(photo);
                        }
                    }
                }
                else if (fetchRecent)
                {
                    items.AddRange(flickr.GetMostInteresting(index, itemsToTake, size));
                    //items.AddRange();
                }
                else
                {
                    using (IAuthRepository authRepository = repositoryFactory.CreateAuthRepository())
                    {
                        AuthToken token = GenerateToken(viewMode, authRepository);
                        items.AddRange(flickr.Search(index, itemsToTake, size, token == null ? string.Empty : token.Id,
                                                     ProcessSearchQuery(flickr, viewMode)));
                    }
                }
           }
        }

        private AuthToken GenerateToken(ViewMode viewMode, IAuthRepository flickr)
        {
            bool authenticate = false;
            AuthToken token = null;
            // for private or semi-private photo do authenticate.
            if (viewMode != ViewMode.Public)
                authenticate = true;

            token = flickr.Authenticate(authenticate, Permission.Delete);
            return token;
        }

        private static string[] ProcessSearchQuery(IPhotoRepository flickr, ViewMode viewMode)
        {
            string nsId = string.Empty;
            var args = new NameValueCollection();

            Bucket.Instance.For.EachItem.Process(delegate(BucketItem item)
            {
                /// must be mapped to flickr.
                if (item.FindAttribute(typeof(OriginalFieldNameAttribute)) != null)
                {
                    if (item.Value != null)
                    {
                        string value = Convert.ToString(item.Value);
                        // fix for tagMode 
                        if (string.Compare(item.Name, "tag_mode") == 0)
                        {
                            TagMode tagMode = (TagMode)item.Value;
                            value = tagMode == TagMode.AND ? "all" : "any";
                        }

                        if (!string.IsNullOrEmpty(value))
                        {
                            string key = string.Empty;

                            if (string.Compare(item.Name, "user") == 0)
                            {
                                key = "user_id";
                                if (value.IsValidEmail())
                                {
                                    nsId = flickr.GetNsidByEmail(value);
                                }
                                else
                                {
                                    nsId = flickr.GetNsidByUsername(value);
                                }
                                // set the new nslid
                                if (!string.IsNullOrEmpty(nsId))
                                {
                                    value = nsId;
                                }
                            }
                            else if (string.Compare(item.Name, "text") == 0)
                            {
                                SearchMode searchMode = Bucket.Instance.For.Item(PhotoColumns.SearchMode).Value == null ? SearchMode.FreeText : (SearchMode)Bucket.Instance.For.Item(PhotoColumns.SearchMode).Value;
                                key = searchMode == SearchMode.TagsOnly ? "tags" : item.Name;
                            }

                            else if (string.Compare(item.Name, "extras") == 0)
                            {
                                ExtrasOption extras = Bucket.Instance.For.Item(PhotoColumns.Extras).Value == null ? ExtrasOption.None : (ExtrasOption)Bucket.Instance.For.Item(PhotoColumns.Extras).Value;
                                key = item.Name;
                                value = extras.ToExtrasString();
                            }
                            else
                            {
                                key = item.Name;
                            }
                            args[key] = value;
                        }
                    } // end if (item.Value != null))
                }// end if (item.Name != PhotoColumns.Photosize)

            });

            bool sortUsed = false;

            Bucket.Instance.Entity.OrderBy.IfUsed.Process(delegate (string fieldName, bool isAscending)
            {
                args["sort"] = GetSortOrder(fieldName, isAscending);
                sortUsed = true;
            });

            if (!sortUsed)
            {
                /// default is relevance
                args["sort"] = GetSortOrder(string.Empty, false);
            }


            /// not user id is provided and , owner is specified then get my photos.
            if (viewMode == ViewMode.Owner && string.IsNullOrEmpty((string)Bucket.Instance.For.Item(PhotoColumns.User).Value))
            {
                args["user_id"] = "me";
            }

            var results = new string[args.Count * 2];

            int index = 0;

            foreach (string result in  args.AllKeys)
            {
                results[index++] = result;
                results[index++] = args[result];
            }

            return results;
        }

        private static string GetSortOrder(string orderBy, bool asc)
        {
            orderBy = orderBy ?? string.Empty;
            if (!string.IsNullOrEmpty(orderBy))
            {
                if (orderBy.Equals(PhotoOrder.Date_Taken.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    return "date-taken-" + ((asc) ? "asc" : "desc");
                }
                if (orderBy.Equals(PhotoOrder.Date_Posted.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    return "date-posted-" + ((asc) ? "asc" : "desc");
                }
                if (orderBy.Equals(PhotoOrder.Interestingness.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    return "interestingness-" + ((asc) ? "asc" : "desc");
                }
            }
            return "relevance";
        }

        private IRepositoryFactory repositoryFactory;

        private People people;
        private CommentCollection commentCollection;
        private IFlickrElement elementProxy;
    }
 }

