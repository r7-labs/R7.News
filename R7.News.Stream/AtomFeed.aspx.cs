//
//  AtomFeed.aspx.cs
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
using System.Text;
using System.Web;
using System.Web.UI;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using R7.Dnn.Extensions.Text;
using R7.News.Components;
using R7.News.Data;
using R7.News.Models;
using R7.News.Stream.Components;

namespace R7.News.Stream
{
    public class FeedParams
    {
        public int ModuleId { get; set; }

        public int TabId { get; set; }
    }

    public class AtomFeed : PageBase
    {
        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            Response.ClearContent ();

            // should be "application/atom+xml"
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.UTF8;
        }

        FeedParams ParseFeedParams ()
        {
            return new FeedParams {
                ModuleId = ParseHelper.ParseToNullable<int> (Request.QueryString ["mid"]) ?? -1
                ,TabId = ParseHelper.ParseToNullable<int> (Request.QueryString ["tabid"]) ?? -1
            };
        }

        IEnumerable<NewsEntryInfo> GetNewsEntries (ModuleInfo module, StreamSettings settings)
        {
            // TODO: Provide separate value via config/settings
            var numOfEntries = settings.PageSize;

            return NewsRepository.Instance.GetNewsEntries_FirstPage (PortalSettings.PortalId, numOfEntries,
                HttpContext.Current.Timestamp,
                new WeightRange (settings.MinThematicWeight, settings.MaxThematicWeight),
                new WeightRange (settings.MinStructuralWeight, settings.MaxStructuralWeight),
                settings.ShowAllNews, settings.IncludeTerms, out int newsEntriesCount);
        }

        StreamSettings GetModuleSettings (ModuleInfo module)
        {
            if (module != null && module.ModuleDefinition.DefinitionName == Const.StreamModuleDefinitionName) {
                return new StreamSettingsRepository ().GetSettings (module);
            }

            return null;
        }

        protected override void Render (HtmlTextWriter writer)
        {
            var feed = ParseFeedParams ();

            var module = ModuleController.Instance.GetModule (feed.ModuleId, feed.TabId, false);
            if (module == null) {
                Response.StatusCode = 500;
                // TODO: Log error
                return;
            }

            var settings = GetModuleSettings (module);
            if (settings == null) {
                Response.StatusCode = 500;
                // TODO: Log error
                return;
            }

            var newsEntries = GetNewsEntries (module, settings);
            if (newsEntries != null) {
                RenderAtomFeed (writer, newsEntries, module, settings);
            }
        }

        string IsoDateTime (DateTime datetime)
        {
            return datetime.ToUniversalTime ().ToString ("s") + "Z";
        }

        void RenderAtomFeed (HtmlTextWriter writer, IEnumerable<NewsEntryInfo> newsEntries, ModuleInfo module, StreamSettings settings)
        {
            var authorityDate = PortalSettings.CreatedOnDate.ToString ("yyyy-MM-dd");
            var updatedDate = newsEntries.Any () ? newsEntries.First ().PublishedOnDate () : module.LastModifiedOnDate;

            writer.WriteLine ("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            writer.WriteLine ($"<feed xmlns=\"http://www.w3.org/2005/Atom\">");
            writer.WriteLine ($"  <title>{module.ModuleTitle}</title>");
            writer.WriteLine ("  <author>");
            writer.WriteLine ($"    <name>{PortalSettings.PortalName}</name>");
            writer.WriteLine ("  </author>");
            writer.WriteLine ($"  <id>tag:{PortalSettings.PortalAlias.HTTPAlias},{authorityDate}:stream#{module.TabModuleID}</id>");
            writer.WriteLine ($"  <link rel=\"self\" href=\"{HttpUtility.HtmlAttributeEncode (Request.Url.ToString ())}\" />");
            writer.WriteLine ($"  <link rel=\"alternate\" href=\"{HttpUtility.HtmlAttributeEncode (Globals.NavigateURL (module.TabID))}\" />");
            writer.WriteLine ($"  <updated>{IsoDateTime (updatedDate)}</updated>");

            foreach (var n in newsEntries) {
                var permalink = n.GetPermalinkFriendly (ModuleController.Instance, module.ModuleID, module.TabID);

                writer.WriteLine ("  <entry>");
                writer.WriteLine ($"    <title>{n.Title}</title>");
                writer.WriteLine ($"    <link rel=\"alternate\" href=\"{HttpUtility.HtmlAttributeEncode (permalink)}\" />");
                writer.WriteLine ($"    <id>tag:{PortalSettings.PortalAlias.HTTPAlias},{authorityDate}:entry#{n.EntryId}</id>");
                writer.WriteLine ($"    <updated>{IsoDateTime (n.PublishedOnDate ())}</updated>");
                writer.WriteLine ($"    <summary>{HtmlUtils.StripTags (HttpUtility.HtmlDecode (n.Description), true).Trim ()}</summary>");
                writer.WriteLine ($"    <content type=\"html\">{n.Description}</content>");
                writer.WriteLine ("  </entry>");
            }

            writer.Write ("</feed>");
        }
    }
}
