using System.Web;
using System.Xml;
using R7.News.Models;

namespace R7.News.Feeds
{
    // TODO: Test me in production!
    public class RssVkFeed : RssFeed
    {
        protected override void RenderEntrySummary (XmlWriter writer, INewsEntry newsEntry)
        {
            var htmlContent = HttpUtility.HtmlDecode (newsEntry.Description);

            // HACK: Temporary workaround for GH-113
            htmlContent = htmlContent.Replace ("&mdash;", "&ndash;");

            if (newsEntry.ContentItem.Images.Count > 0) {
                var imageUrl = newsEntry.GetRawImageUrl ();
                if (!string.IsNullOrEmpty (imageUrl)) {
                    htmlContent = $"<img src=\"{imageUrl}\" alt=\"{newsEntry.Title}\" />" + htmlContent;
                }
            }

            writer.WriteStartElement ("description");
            writer.WriteCData (htmlContent);
            writer.WriteEndElement ();
        }
    }
}
