using System;
using System.IO;
using System.Drawing;
using Linq.Flickr.Attribute;
using LinqExtender;
using LinqExtender.Attribute;
using LinqExtender.Interface;

namespace Linq.Flickr
{
    /// <summary>
    ///  AND means the result will ANDED with tags, And OR means it will be ORED.
    /// </summary>
    public enum TagMode
    {
        AND = 0,
        OR
    }
    public enum PhotoSize
    {
        Square,
        Thumbnail,
        Small,
        Medium,
        Original,
        Default,
        Big
    }

    public enum ViewMode
    {
        Owner,
        Public = 1,
        Friends,
        Family,
        FriendsFamily,
        Private
    }

    public enum SearchMode
    {
        FreeText,
        TagsOnly
    }

    public enum PhotoOrder
    {
        Date_Posted,
        Date_Taken,
        Interestingness
    }

    public enum FilterMode
    {
        Safe = 1,
        Moderate = 2,
        Restricted = 3
    }

    [Flags]
    public enum ExtrasOption
    {
        None = 0,
        License = 1,
        Date_Upload = 2,
        Date_Taken = 4,
        Owner_Name = 8,
        Icon_Server = 16,
        Original_Format = 32,
        Last_Update = 64,
        Geo = 128,
        Tags = 256,
        Machine_Tags = 512,
        Views = 1024,
        Media = 2048,
        All = License | Date_Upload | Date_Taken | Owner_Name | Icon_Server | Original_Format | Last_Update | Geo | Tags | Machine_Tags | Views | Media,
    }

    [Serializable, XElement("photo")]
    public class Photo : IQueryObject
    {
        public Photo()
        {
            IsPublic = true;
            this.SortOrder = PhotoOrder.Date_Posted;
            this.SearchMode = SearchMode.FreeText;
            this.FilterMode = FilterMode.Safe;
            this.ExtrasResult = new ExtraOptions();
        }

        [OriginalFieldName("title"), XAttribute("title")]
        public string Title { get; set; }
        [OriginalFieldName("description")]
        public string Description { get; set; }
        [OriginalFieldName("photo_id"), UniqueIdentifier, XAttribute("id")]
        public string Id { get; set; }
        /// <summary>
        ///  text on which to search on flickr.
        /// </summary>
        [OriginalFieldName("text")]
        public string SearchText { get; internal set; }
        /// <summary>
        /// Use to query user in flickr, is filled up only when a photo is get by photoId.
        /// </summary>
        [OriginalFieldName("user")]        
        public string User { get; internal set; }
        /// <summary>
        /// This is the unique Id aginst username, availble only when photos are get by Id explictly.
        /// This can be used in where clause for getting photo by nsId
        /// </summary>
        [XAttribute("owner"), OriginalFieldName("user_id")]
        public string NsId { get; internal set; }    

        /// <summary>
        /// The original url of the photo in flickr page.
        /// </summary>
        public string WebUrl
        {
            get
            {
                webUrl = string.Format("http://www.flickr.com/photos/{0}/{1}/", NsId, Id);
                return webUrl;
            }
            internal set
            {
                webUrl = value;   
            }
        }

        [OriginalFieldName("safe_search")]
        public FilterMode FilterMode
        {
            get
            {
                return (FilterMode)filterMode;
            }
            internal set
            {
                filterMode = (int)value;
            }      
        }

        [OriginalFieldName("photo")]
        public string FileName
        {
            set
            {
                uploadFilename = value;
            }
            get
            {
                if (string.IsNullOrEmpty(uploadFilename))
                {
                    uploadFilename = Guid.NewGuid().ToString();
                }
                return uploadFilename;
            }
        }
        /// <summary>
        /// points to physical path where photo is located
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// Contains the stream for the photo to be uploaded.
        /// </summary>
        public Stream File { get; set; }

        public byte[] PhotoContent
        {
            get
            {
                if (photoContent == null)
                {
                    photoContent = GetBytesFromPhysicalFile();
                }
                return photoContent;
            }
      
        }

        
        public PhotoSize PhotoSize
        {
            get
            {
                return (PhotoSize)size;
            }
            internal set
            {
                size = (int)value;
            }
        }

        /// <summary>
        /// Defines the tag condition ANY or AND , default is ANY
        /// </summary>
        [OriginalFieldName("tag_mode")]
        public TagMode TagMode
        {
            get
            {
                return (TagMode)tagMode;
            }
            internal set
            {
                tagMode = (int)value;
            }
        }

        /// <summary>
        /// Defines in which mode you will get the photos , Currenlty supported FreeText or TagsOnly
        /// </summary>
        
        public SearchMode SearchMode
        {
            get
            {
                return (SearchMode)searchMode;
            }
            internal set
            {
                searchMode = (int)value;
            }
        }

        /// <summary>
        /// A comma-delimited list of extra information to fetch for each returned record. 
        /// Currently supported fields are: license, date_upload, date_taken, owner_name, icon_server, 
        /// original_format, last_update, geo, tags, machine_tags, views, media. 
        /// Use ExtrasOption enum with  | to set your options. Ex 
        /// p.Extras == (ExtrasOption.Views | ExtrasOption.Date_Taken | ExtrasOption.Date_Upload), dont forget to use parenthesis.
        /// </summary>
        [OriginalFieldName("extras")]
        public ExtrasOption Extras
        {
            get
            {
                return extras;
            }
            set
            {
                extras = value;
            }
        }

        /// <summary>
        /// Defines which type photo you want to get , Private or others, if you want get things from your stream
        /// use ViewMode.Owner.
        /// </summary>
        [OriginalFieldName("privacy_filter")]
        public ViewMode ViewMode
        {
            get
            {
                if (!IsPublic)
                    visibility = (int)ViewMode.Private;
                else if (IsFriend)
                    visibility = (int)ViewMode.Friends;
                else if (IsFamily)
                    visibility = (int)ViewMode.Family;
                else
                    visibility = (int)ViewMode.Public;

                return (ViewMode)visibility;
            }
            set
            {
                visibility = (int)value;
            }
        }


        public PhotoOrder SortOrder
        {
            get
            {
                return (PhotoOrder)sortOrder;
            }
            set
            {
                sortOrder = (int)value;
            }
        }
        /// <summary>
        /// date when photo is uploaded in flickr
        /// </summary>
        public DateTime UploadedOn
        {
            get
            {
                return DateUploaded.GetDate();
            }
        }
        /// <summary>
        /// date when photo is updated in flickr
        /// </summary>
        public DateTime UpdatedOn
        {
            get
            {
                return LastUpdated.GetDate();
            }
        }

        /// <summary>
        /// the date when the photo is taken.
        /// </summary>
        public DateTime TakeOn
        {
            get
            {
                DateTime date;
                DateTime.TryParse(DateTaken, out date);
                return date;
            }
        }

        /// <summary>
        ///  array of photo tags.
        /// </summary>
        public string[] Tags
        {
            get
            {
                return tags;
            }
            set
            {
                tags = value;
            }
        }

        [XAttribute("secret")]
        internal string SecretId { get; set; }
        [XAttribute("server")]
        internal string ServerId { get; set; }
        [XAttribute("farm")]
        internal string FarmId { get; set; }
        [XAttribute("dateupload")]
        internal string DateUploaded { get; set; }
        /// <summary>
        /// tied to Extras option
        /// </summary>
        [XAttribute("lastupdate")]
        internal string LastUpdated { get; set; }
        /// <summary>
        /// tied to Extras option
        /// </summary>
        [XAttribute("datetaken")]
        internal string DateTaken { get; set; }
        [XAttribute("ispublic")]
        internal bool IsPublic { get; set; }
        [XAttribute("isfriend")]
        internal bool IsFriend { get; set; }
        [XAttribute("isfamily")]
        internal bool IsFamily { get; set; }
        [XAttribute("views")]
        public int Views { get; internal set; }

        internal byte[] GetBytesFromPhysicalFile()
        {
            Stream stream = null;
       
            try
            {
                if (File != null)
                {
                    stream = File;
                }
                else
                {
                    stream = new FileStream(FilePath, FileMode.Open);
                }

                using (Bitmap bitmap = new Bitmap(stream))
                {
                    //bitmap.v(stream, ImageFormat.Jpeg);
                }

                byte[] image = new byte[stream.Length];

                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(image, 0, image.Length);

                return image;
            }
            catch
            {
                return null;
            }
        }

        [XAttribute("tags")]
        internal string PTags 
        {
            set
            {
                tags = value.Split(new char[] { ',', ';', ' '}, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        private string GetSizePostFix(PhotoSize size)
        {
            string sizePostFx;

            switch (size)
            {
                case PhotoSize.Square:
                    sizePostFx = "_s";
                    break;
                case PhotoSize.Small:
                    sizePostFx = "_m";
                    break;
                case PhotoSize.Thumbnail:
                    sizePostFx = "_t";
                    break;
                case PhotoSize.Big:
                    sizePostFx = "_b";
                    break;
                default:
                    sizePostFx = string.Empty;
                    break;

            }
            return sizePostFx;
        }


        public string Url
        {
            get
            {
                if (string.IsNullOrEmpty(url))
                {
                    url = "http://farm" + FarmId + ".static.flickr.com/" + ServerId + "/" + Id + "_" + SecretId + GetSizePostFix(PhotoSize) + ".jpg?v=0";
                }
                return url;
            }
            set
            {
                url = value;
            }
        }
        /// <summary>
        /// holds out the common propeties like page , total page count and total item count
        /// </summary>
        public CommonAttribute SharedProperty { get; set; }
        /// <summary>
        /// Contains the retult for the <c>Extras</c> option set by user.
        /// </summary>
        [XChild, Ignore]
        public ExtraOptions ExtrasResult { get; set; }

        private string webUrl = string.Empty;
        private string url = string.Empty;
        private int filterMode;
        private string uploadFilename = string.Empty;
        private byte[] photoContent = null;
        private int size = 0;
        private int tagMode = 0;
        private int searchMode = 0;
        private ExtrasOption extras = ExtrasOption.None;
        private int visibility = 0;
        private int sortOrder = 0;
        private string[] tags = new string[0];
    }

    [XElement("photos")]
    public class CommonAttribute : IDisposable
    {
        [XAttribute("page")]
        public int Page { get; set; }
        [XAttribute("pages")]
        public int Pages { get; set; }
        [XAttribute("perpage")]
        public int Perpage { get; set; }
        [XAttribute("total")]
        public int Total { get; set; }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            //throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// holds extras settings for photo
    /// </summary>
    public class ExtraOptions : IDisposable
    {
        [XAttribute("license")]
        public string License { get; internal set; }
        [XAttribute("ownername")]
        public string OwnerName { get; internal set; }
        [XAttribute("media")]
        public string Media { get; internal set; }
        [XAttribute("original_format")]
        public string OriginalFormat { get; internal set; }
        [XAttribute("latitude")]
        public string Latitude { get; internal set; }
        [XAttribute("longitude")]
        public string Longitude { get; internal set; }
        [XAttribute("accuracy")]
        public string Accuracy { get; internal set; }
        [XAttribute("machine_tags")]
        public string MachineTags { get; internal set; }
        [XAttribute("icon_server")]
        public string IconServer { get; internal set; }
        

        #region IDisposable Members

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}
