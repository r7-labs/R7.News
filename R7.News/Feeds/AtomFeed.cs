﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using R7.News.Models;

namespace R7.News.Feeds
{
    public class AtomFeed : IFeed
    {
        string IsoDateTime (DateTime datetime) => datetime.ToUniversalTime ().ToString ("s") + "Z";

        string Base64ToCanonicalForm (string base64String) => base64String.Replace ("%3d", "%3D");

        public void Render (XmlWriter writer, IEnumerable<NewsEntry> newsEntries, ModuleInfo module,
            PortalSettings portalSettings, string requestUrl, bool withImages)
        {
            var authorityDate = portalSettings.PortalAlias.CreatedOnDate.ToUniversalTime ().ToString ("yyyy-MM-dd");
            var updatedDate = newsEntries.Any () ? newsEntries.First ().PublishedOnDate () : module.LastModifiedOnDate;

            writer.WriteStartDocument ();
            writer.WriteStartElement ("feed", "http://www.w3.org/2005/Atom");

            writer.WriteElementString ("title", module.ModuleTitle);
            writer.WriteStartElement ("author");
            writer.WriteElementString ("name", portalSettings.PortalName);
            writer.WriteEndElement ();

            writer.WriteElementString ("id", $"tag:{portalSettings.PortalAlias.HTTPAlias},{authorityDate}:feed#"
                + Base64ToCanonicalForm (UrlUtils.EncryptParameter ($"{module.TabID}-{module.ModuleID}")));

            writer.WriteStartElement ("link");
            writer.WriteAttributeString ("rel", "self");
            writer.WriteAttributeString ("href", requestUrl);
            writer.WriteEndElement ();

            writer.WriteStartElement ("link");
            writer.WriteAttributeString ("rel", "alternate");
            writer.WriteAttributeString ("href", Uri.EscapeUriString (Globals.NavigateURL (module.TabID)));
            writer.WriteEndElement ();

            writer.WriteElementString ("updated", IsoDateTime (updatedDate));

            foreach (var n in newsEntries) {
                var permalink = n.GetPermalinkFriendly (ModuleController.Instance, module.ModuleID, module.TabID);

                writer.WriteStartElement ("entry");
                writer.WriteElementString ("title", n.Title);

                writer.WriteStartElement ("link");
                writer.WriteAttributeString ("rel", "alternate");
                writer.WriteAttributeString ("href", Uri.EscapeUriString (permalink));
                writer.WriteEndElement ();

                writer.WriteElementString ("id", $"tag:{portalSettings.PortalAlias.HTTPAlias},{authorityDate}:entry#{n.EntryId}");
                writer.WriteElementString ("updated", IsoDateTime (n.PublishedOnDate ()));
                writer.WriteElementString ("summary", HttpUtility.HtmlDecode (HtmlUtils.StripTags (HttpUtility.HtmlDecode (n.Description), true)).Trim ());

                writer.WriteStartElement ("content");

                writer.WriteAttributeString ("type", "html");

                var htmlContent = HttpUtility.HtmlDecode (n.Description);

                // HACK: Temporary workaround for GH-113
                htmlContent = htmlContent.Replace ("&mdash;", "&ndash;");

                if (withImages && n.ContentItem.Images.Count > 0) {
                    var imageUrl = n.GetRawImageUrl ();
                    if (!string.IsNullOrEmpty (imageUrl)) {
                        htmlContent = $"<img src=\"{imageUrl}\" alt=\"{n.Title}\" />" + htmlContent;
                    }
                }

                writer.WriteCData (htmlContent);

                writer.WriteEndElement ();
                writer.WriteEndElement ();
            }

            writer.WriteEndElement ();
            writer.WriteEndDocument ();

            writer.Close ();
        }
    }
}
