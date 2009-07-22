using System;
using System.Collections.Generic;
using System.Text;

namespace BistroApi
{
    /// <summary>
    /// Determines when the controller will be executed in the request execution pipeline
    /// </summary>
    public enum BindType
    {
        /// <summary>
        /// Controller will be executed on the way down the tree. This is default behavior
        /// </summary>
        Before,
        
        /// <summary>
        /// Controller will be executed as the payload of the tree (i.e. once the final leaf is reached)
        /// </summary>
        Payload,
				
				/// <summary>
        /// Controller will be executed on the way up the tree.
        /// </summary>
        After
    }
}
