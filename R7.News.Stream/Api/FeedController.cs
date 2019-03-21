//
//  FeedController.cs
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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Xml;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Web.Api;
using R7.News.Components;
using R7.News.Data;
using R7.News.Feeds;
using R7.News.Models;
using R7.News.Stream.Models;

namespace R7.News.Stream.Api
{
    // TODO: Move to the base library
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }

    public class FeedController : DnnApiController
    {
        IEnumerable<NewsEntryInfo> GetNewsEntries (ModuleInfo module, StreamSettings settings)
        {
            return NewsRepository.Instance.GetNewsEntries_FirstPage (PortalSettings.PortalId,
                settings.FeedMaxEntries ?? NewsConfig.Instance.Feed.DefaultMaxEntries,
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

        void DecryptParameters (string key, out int tabId, out int moduleId)
        {
            var keyParts = UrlUtils.DecryptParameter (key).Split ('-');
            tabId = int.Parse (keyParts [0]);
            moduleId = int.Parse (keyParts [1]);
        }

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage Atom (string key, bool withImages = false)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            var logType = EventLogController.EventLogType.HOST_ALERT;
            var tabId = -1;
            var moduleId = -1;

            try {
                DecryptParameters (key, out tabId, out moduleId);
                var module = ModuleController.Instance.GetModule (moduleId, tabId, false);
                var settings = GetModuleSettings (module);

                if (settings == null) {
                    statusCode = HttpStatusCode.BadRequest;
                    logType = EventLogController.EventLogType.ADMIN_ALERT;
                    throw new Exception ("Stream module not found.");
                }

                if (!settings.EnableFeed) {
                    Request.CreateResponse (HttpStatusCode.Forbidden);
                }

                if (!ModulePermissionController.CanViewModule (module)) {
                    Request.CreateResponse (HttpStatusCode.Unauthorized);
                }

                var newsEntries = GetNewsEntries (module, settings);
                if (newsEntries == null) {
                    throw new Exception ("Error reading news entries for module.");
                }

                var writer = new Utf8StringWriter ();
                var xmlWriter = XmlWriter.Create (writer, new XmlWriterSettings {
                    Indent = true,
                    IndentChars = "  ",
                    Encoding = Encoding.UTF8
                });

                var feed = new AtomFeed ();
                feed.Render (xmlWriter, newsEntries, module, PortalSettings, Request.RequestUri.ToString (), withImages);

                return new HttpResponseMessage {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent (writer.ToString (), Encoding.UTF8, "text/xml")
                };
            }
            catch (Exception ex) {
                var log = new LogInfo ();
                log.AddProperty ("Source", GetType ().FullName);
                log.AddProperty ("PortalId", PortalSettings.PortalId.ToString ());
                log.AddProperty ("TabId", tabId.ToString ());
                log.AddProperty ("ModuleId", moduleId.ToString ());
                log.AddProperty ("RawUrl", Request.GetHttpContext ().Request.RawUrl);
                log.AddProperty ("Referrer", Request.GetHttpContext ().Request.UrlReferrer?.ToString ());
                log.LogPortalID = PortalSettings.PortalId;
                log.LogUserID = UserInfo?.UserID ?? -1;
                log.LogUserName = UserInfo?.Username ?? "Unknown";
                log.LogTypeKey = logType.ToString ();
                log.Exception = new ExceptionInfo (ex);
                EventLogController.Instance.AddLog (log);

                return Request.CreateResponse (statusCode);
            }
        }
    }
}
