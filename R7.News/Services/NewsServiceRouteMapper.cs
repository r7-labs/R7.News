
using DotNetNuke.Web.Api;

namespace R7.News.Services
{
    public class NewsServiceRouteMapper: IServiceRouteMapper
    {
        public void RegisterRoutes (IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute ("R7.News", "r7_News_Map1", "{controller}/{action}", new [] { "R7.News.Services" });
        }
    }
}
