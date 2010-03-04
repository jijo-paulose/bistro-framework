using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bistro.Controllers;
using Bistro.Controllers.Descriptor;
using Bistro.Controllers.Descriptor.Data;

namespace Controllers
{
    /// <summary>
    /// Home controller. This controller services the 'get /home/index' method.
    /// </summary>
    [Bind("get /index")]
    [RenderWith("Views/index.django")]
    public class HomeController : AbstractController
    {
        [Request]
        protected string Message = "Welcome to Bistro!";

        /// <summary>
        /// Controller implementation.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void DoProcessRequest(IExecutionContext context)
        {
        }
    }
}
