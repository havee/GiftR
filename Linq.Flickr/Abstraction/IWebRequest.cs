

using System.Net;
using System.IO;
namespace Linq.Flickr.Repository.Abstraction
{
    public interface IWebRequest
    {
        /// <summary>
        /// Creates the web request from a given url.
        /// </summary>
        /// <param name="url"></param>
        /// <returns><see cref="HttpWebRequest"/> instance.</returns>
        WebRequest Create(string url);

        /// <summary>
        /// Gets the response stream from specified web request.
        /// </summary>
        /// <returns></returns>
        Stream GetResponseStream(WebRequest request);

        /// <summary>
        /// Makes an HttpPost request to the specfied url and return the response from server.
        /// </summary>
        /// <param name="requestUrl">Target request url.</param>
        /// <returns>Xml containing server response.</returns>
        string DoHttpPost(string requestUrl);

    }
}
