using System.Xml;
using System.Net;
using System.IO;
using Linq.Flickr.Repository.Abstraction;

namespace Linq.Flickr.Proxies
{
    /// <summary>
    /// Provides interface implementation for Doing HTTP calls.
    /// </summary>
    public class FlickrElementProxy : IFlickrElement
    {

        /// <summary>
        /// Initializes a new instance of <see cref="XmlElementProxy"/> class.
        /// </summary>
        public FlickrElementProxy()
        {
            this.webRequest = new WebRequestProxy();
        }
 
        /// <summary>
        /// Initializes a new instance of <see cref="XmlElementProxy"/> class.
        /// </summary>
        /// <param name="webProxy">Target web proxy to make http post requests</param>
        public FlickrElementProxy(IWebRequest webRequest)
        {
            this.webRequest = webRequest;
        }

        public XmlElement GetResponseElement(string requestUrl)
        {
            WebRequest request = WebRequest.Create(requestUrl);
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();

            XmlReaderSettings readerSettings = new XmlReaderSettings();

            readerSettings.IgnoreWhitespace = true;
            readerSettings.IgnoreProcessingInstructions = true;
            readerSettings.IgnoreComments = true;
      
            XmlReader reader = XmlReader.Create(stream, readerSettings);

            try{
                return (this as IFlickrElement).GetResponseElement(reader);
            }
            finally{
                // close the reader explicitly.
                reader.Close();
                stream.Close();
                response.Close();
            }
        }

        public XmlElement GetResponseElement(XmlReader reader)
        {
            return RestExtension.Load(reader).ValidateResponse();
        }

        public XmlElement GetElementFromResponse(string response)
        {
            XmlElement element = RestExtension.Parse(response);
            return element.ValidateResponse();
        }

        /// <summary>
        /// Sends a request xml request to flickr for specified url.
        /// </summary>
        /// <param name="response"></param>
        /// <exception cref="FlickrException">Throws exception for invalid response.</exception>
        /// <returns>Xml element containing response from server.</returns>
        public XmlElement SendPostRequest(string url)
        {
            string responseFromServer = webRequest.DoHttpPost(url);
            XmlElement element = RestExtension.Parse(responseFromServer);
            
            element.ValidateResponse();

            return element;
        }

        private IWebRequest webRequest;
    }
}
