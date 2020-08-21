using DotNetNuke.Web.Api;

namespace R7.News.Stream.Services
{
    public class StreamRouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes (IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute ("R7.News.Stream", "r7_News_StreamFeedRoute1", "{controller}/{action}", new [] { "R7.News.Stream.Services" });
        }
    }
}
