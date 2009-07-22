using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BistroApi;
using BistroModel;
namespace NDjango.BistroIntegration
{
    [Bind("?", ControllerBindType=BindType.Payload)]
    [TemplateMapping(".django")]
    public class DjangoController : RenderingController
    {
        protected override Type EngineType
        {
            get { return typeof(DjangoEngine); }
        }
    }
}
