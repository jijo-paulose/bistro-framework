using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace BistroApi
{
    /// <summary>
    /// The Bistro response interface. 
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Returns XML as the http response, closing the output stream.
        /// </summary>
        /// <param name="xml">The XML string.</param>
        void ReturnXml(string xml);

        /// <summary>
        /// Returns XML as the http response, closing the output stream.
        /// </summary>
        /// <param name="xml">The XML fragment.</param>
        void ReturnXml(XmlNode xml);

        /// <summary>
        /// Returns the JSON as the http response, closing the output stream.
        /// </summary>
        /// <param name="json">The json.</param>
        void ReturnJSON(string json);

        /// <summary>
        /// Returns the file as the http reponse, closing the output stream
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="contentType">the content-type header value to supply.</param>
        /// <param name="fileName">Name of the file. If supplied, a content-disposition header of "attachment" will be appended.</param>
        /// <param name="encoding">The encoding. If not supplied, UTF8 will be used</param>
        void ReturnFile(Stream fileStream, string contentType, string fileName, Encoding encoding);

        /// <summary>
        /// Returns the file as the http reponse, closing the output stream
        /// </summary>
        /// <param name="fileName">The absolute path to the file.</param>
        /// <param name="contentType">the content-type header value to supply.</param>
        /// <param name="asAttachment">whether the file should be returned as an attachment</param>
        void ReturnFile(string fileName, string contentType, bool asAttachment);

        /// <summary>
        /// Returns an arbitrary string as the http response, with the supplied content type
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="contentType">the content-type header value to supply.</param>
        void ReturnFreeForm(string value, string contentType);

        /// <summary>
        /// Returns an http status code and a message. Note that this will behave the same way as
        /// the rest of the ReturnXXX methods, allowing the controller chain to finish execution.
        /// If an immediate return is required, use <see cref="WebException"/>.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="message">The message.</param>
        void ReturnStatus(StatusCode status, string message);

        /// <summary>
        /// Gets the result that was supplied to one of the ReturnXXX methods.
        /// </summary>
        /// <value>The result.</value>
        object ExplicitResult { get; }

        /// <summary>
        /// Gets the content type of the explicit result. 
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Gets the encoding to use for the explicit result
        /// </summary>
        Encoding Encoding { get; }

        /// <summary>
        /// Gets the name that will be given to an attachment. Applicable to files 
        /// returned with asAttachment set, or an explicit file names given to streams.
        /// </summary>
        string AttachmentName { get; }

        /// <summary>
        /// Gets the http status code.
        /// </summary>
        /// <value>The code.</value>
        StatusCode Code { get; }

        /// <summary>
        /// Notifies the rendering engine how to render the results of the current request
        /// </summary>
        void RenderWith(string target);

        /// <summary>
        /// Gets the render target.
        /// </summary>
        /// <value>The render target.</value>
        string RenderTarget { get; }

        /// <summary>
        /// Gets the return type that is currently set on the context
        /// </summary>
        ReturnType CurrentReturnType { get; }
    }
}
