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
using System.Web.UI;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using R7.News.Models;

namespace R7.News.Feeds
{
    public class AtomFeed: IFeed
    {
        string IsoDateTime (DateTime datetime) => datetime.ToUniversalTime ().ToString ("s") + "Z";

        public void Render (HtmlTextWriter writer, IEnumerable<NewsEntryInfo> newsEntries, ModuleInfo module, PageBase page)
        {
            var authorityDate = page.PortalSettings.CreatedOnDate.ToString ("yyyy-MM-dd");
            var updatedDate = newsEntries.Any () ? newsEntries.First ().PublishedOnDate () : module.LastModifiedOnDate;

            writer.WriteLine ("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            writer.WriteLine ($"<feed xmlns=\"http://www.w3.org/2005/Atom\">");
            writer.WriteLine ($"  <title>{module.ModuleTitle}</title>");
            writer.WriteLine ("  <author>");
            writer.WriteLine ($"    <name>{page.PortalSettings.PortalName}</name>");
            writer.WriteLine ("  </author>");
            writer.WriteLine ($"  <id>tag:{page.PortalSettings.PortalAlias.HTTPAlias},{authorityDate}:stream#{module.TabModuleID}</id>");
            writer.WriteLine ($"  <link rel=\"self\" href=\"{HttpUtility.HtmlAttributeEncode (page.Request.Url.ToString ())}\" />");
            writer.WriteLine ($"  <link rel=\"alternate\" href=\"{HttpUtility.HtmlAttributeEncode (Globals.NavigateURL (module.TabID))}\" />");
            writer.WriteLine ($"  <updated>{IsoDateTime (updatedDate)}</updated>");

            foreach (var n in newsEntries) {
                var permalink = n.GetPermalinkFriendly (ModuleController.Instance, module.ModuleID, module.TabID);

                writer.WriteLine ("  <entry>");
                writer.WriteLine ($"    <title>{n.Title}</title>");
                writer.WriteLine ($"    <link rel=\"alternate\" href=\"{HttpUtility.HtmlAttributeEncode (permalink)}\" />");
                writer.WriteLine ($"    <id>tag:{page.PortalSettings.PortalAlias.HTTPAlias},{authorityDate}:entry#{n.EntryId}</id>");
                writer.WriteLine ($"    <updated>{IsoDateTime (n.PublishedOnDate ())}</updated>");
                writer.WriteLine ($"    <summary>{HtmlUtils.StripTags (HttpUtility.HtmlDecode (n.Description), true).Trim ()}</summary>");
                writer.WriteLine ($"    <content type=\"html\">{n.Description}</content>");
                writer.WriteLine ("  </entry>");
            }

            writer.Write ("</feed>");
        }
    }
}
