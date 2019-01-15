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
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using R7.Dnn.Extensions.Text;
using R7.News.Components;
using R7.News.Data;
using R7.News.Feeds;
using R7.News.Models;
using R7.News.Stream.Components;

namespace R7.News.Stream
{
    public class Feed : PageBase
    {
        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            Response.ClearContent ();

            // should be "application/atom+xml"
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.UTF8;
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
            try {
                var moduleId = ParseHelper.ParseToNullable<int> (Request.QueryString ["mid"]) ?? -1;
                var tabId = ParseHelper.ParseToNullable<int> (Request.QueryString ["tabid"]) ?? -1;

                var settings = default (StreamSettings);

                var isValidModule = false;
                var module = ModuleController.Instance.GetModule (moduleId, tabId, false);
                if (module != null) {
                    settings = GetModuleSettings (module);
                    isValidModule = (settings != null);
                }

                if (!isValidModule) {
                    Response.StatusCode = (int) HttpStatusCode.NotFound;
                    Response.End ();
                    // TODO: Log error
                    return;
                }

                if (!settings.EnableFeed) {
                    Response.StatusCode = (int) HttpStatusCode.Forbidden;
                    Response.End ();
                    return;
                }

                if (!ModulePermissionController.CanViewModule (module)) {
                    // FIXME: Unauthorized leads to invalid redirect to login page
                    Response.StatusCode = (int) HttpStatusCode.Forbidden;
                    Response.End ();
                    return;
                }

                var newsEntries = GetNewsEntries (module, settings);
                if (newsEntries != null) {
                    var feed = new AtomFeed ();
                    feed.Render (writer, newsEntries, module, this);
                }
            }
            catch (Exception ex) {
                Exceptions.ProcessPageLoadException (ex);
            }
        }
    }
}
