using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using R7.News.Models;

namespace R7.News.Feeds
{
    // TODO: Add description to the RSS feed
    // TODO: Implement https://validator.w3.org/feed/docs/warning/MissingAtomSelfLink.html
    public class RssFeed : FeedBase
    {
        protected string Rfc822DateTime (DateTime dateTime) => dateTime.ToUniversalTime ().ToString ("ddd, dd MMM yyyy HH:mm:ss K");

        protected override string FormatDateTime (DateTime dateTime) => Rfc822DateTime (dateTime);

        public override void Render (XmlWriter writer, IEnumerable<NewsEntry> newsEntries, ModuleInfo module,
            PortalSettings portalSettings, string requestUrl, bool withImages)
        {
            var authorityDate = portalSettings.PortalAlias.CreatedOnDate.ToUniversalTime ().ToString ("yyyy-MM-dd");
            var updatedDate = newsEntries.Any () ? newsEntries.First ().PublishedOnDate () : module.LastModifiedOnDate;

            writer.WriteStartDocument ();
            writer.WriteStartElement ("rss");
            writer.WriteAttributeString ("version", "2.0");

            writer.WriteStartElement ("channel");

            writer.WriteElementString ("title", module.ModuleTitle);

            writer.WriteElementString ("description", module.ModuleTitle);

            writer.WriteElementString ("generator", portalSettings.PortalName);

            writer.WriteElementString ("link", requestUrl);
            writer.WriteElementString ("pubDate", FormatDateTime (updatedDate));

            foreach (var n in newsEntries) {
                var permalink = n.GetPermalinkFriendly (ModuleController.Instance, module.ModuleID, module.TabID);

                writer.WriteStartElement ("item");
                writer.WriteElementString ("title", n.Title);

                writer.WriteElementString ("link", Uri.EscapeUriString (permalink));

                writer.WriteStartElement ("guid");
                writer.WriteAttributeString ("isPermaLink", "false");
                writer.WriteValue ($"tag:{portalSettings.PortalAlias.HTTPAlias},{authorityDate}:entry#{n.EntryId}");
                writer.WriteEndElement ();

                writer.WriteElementString ("pubDate", FormatDateTime (n.PublishedOnDate ()));

                RenderEntrySummary (writer, n);

                writer.WriteEndElement ();
            }

            writer.WriteEndElement ();
            writer.WriteEndElement ();
            writer.WriteEndDocument ();

            writer.Close ();
        }

        protected virtual void RenderEntrySummary (XmlWriter writer, INewsEntry newsEntry)
        {
            writer.WriteElementString ("description", HttpUtility.HtmlDecode (HtmlUtils.StripTags (HttpUtility.HtmlDecode (newsEntry.Description), true)).Trim ());
        }
    }
}
