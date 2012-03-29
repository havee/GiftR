using System;
using Linq.Flickr.Repository.Abstraction;
using System.Net;
using System.IO;

namespace Linq.Flickr.Proxies
{
    /// <summary>
    /// Contains proxy methods for making web requests.
    /// </summary>
    public class WebRequestProxy : IWebRequest
    {
        WebRequest IWebRequest.Create(string url)
        {
            return WebRequest.Create(url);
        }

        System.IO.Stream IWebRequest.GetResponseStream(WebRequest request)
        {
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            return stream;
        }

        /// <summary>
        /// Makes an HttpPost request to the specfied url and return the response from server.
        /// </summary>
        /// <param name="requestUrl">Target request url.</param>
        /// <returns>Xml containing server response.</returns>
        public string DoHttpPost(string requestUrl)
        {
            // Create a request using a URL that can receive a post. 
            WebRequest request = (this as IWebRequest).Create(requestUrl);

            // Set the Method property of the request to POST.
            request.Method = "POST";
            //// Set the ContentType property of the WebRequest.
            request.ContentType = "charset=UTF-8";
            request.ContentLength = 0;
            // Get the request stream.
            Stream dataStream;
            // Get the response.
            WebResponse response = request.GetResponse();

            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Clean up the streams.
            reader.Close();
            // validate the response.
            // clean up garbage charecters.
            responseFromServer = responseFromServer.Replace("\r", string.Empty).Replace("\n", string.Empty);
            return responseFromServer;
        }
    }
}
