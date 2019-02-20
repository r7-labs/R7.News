//
//  AtomFeed.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2019 Roman M. Yagodin
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Affero General Public License for more details.
//
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
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

        // TODO: Move to base library
        string Base64ToCanonicalForm (string base64String) => base64String.Replace ("%3d", "%3D");

        public void Render (XmlWriter writer, IEnumerable<NewsEntryInfo> newsEntries, ModuleInfo module, PortalSettings portalSettings, string requestUrl)
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
                writer.WriteString (n.Description);
                writer.WriteEndElement ();

                writer.WriteEndElement ();
            }

            writer.WriteEndElement ();
            writer.WriteEndDocument ();

            writer.Close ();
        }
    }
}
