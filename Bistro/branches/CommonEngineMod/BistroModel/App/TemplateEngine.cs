using System.Web;
using BistroApi;

namespace BistroModel
{
    public abstract class TemplateEngine
    {
			public abstract void Render(HttpContextBase httpContext, IContext requestContext);
    }
}
