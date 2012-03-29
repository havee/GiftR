using System.Xml;

namespace Linq.Flickr.Repository.Abstraction
{
    /// <summary>
    /// Contains basic operations for making and getting responses from flickr server.
    /// </summary>
    public interface IFlickrElement
    {
        /// <summary>
        /// Gets and xml from specified url.
        /// </summary>
        /// <param name="requestUrl">Target url</param>
        /// <returns></returns>
        XmlElement GetResponseElement(string requestUrl);
        
        /// <summary>
        /// Gets xml element directly from reader.
        /// </summary>
        /// <param name="xmlReader">Target reader</param>
        /// <returns></returns>
        XmlElement GetResponseElement(XmlReader xmlReader);
        
        /// <summary>
        /// Parses the xml response.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        XmlElement GetElementFromResponse(string response);

        /// <summary>
        /// Sends a request xml request to flickr for specified url.
        /// </summary>
        /// <param name="response"></param>
        /// <exception cref="FlickrException">Throws exception for invalid response.</exception>
        /// <returns>Xml element containing response from server.</returns>
        XmlElement SendPostRequest(string url);
    }
}
