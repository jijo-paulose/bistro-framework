using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Bistro.Controllers.OutputHandling
{
    /// <summary>
    /// Interface for serializing and deserializing object graphs into a web-friendly
    /// format (e.g. JSON or XML)
    /// </summary>
    public interface IWebFormatter
    {
        /// <summary>
        /// Serializes the specified object graph to the given stream.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="outputStream">The output stream.</param>
        void Serialize(Object graph, Stream outputStream);

        /// <summary>
        /// Serialzies the specified object graph into a string.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <returns></returns>
        string Serialize(Object graph);

        /// <summary>
        /// Deserializes the specified target type.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        Object Deserialize(Type targetType, string source);

        /// <summary>
        /// Deserializes the specified target type.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="inputStream">The input stream.</param>
        /// <returns></returns>
        Object Deserialize(Type targetType, Stream inputStream);
    }
}
