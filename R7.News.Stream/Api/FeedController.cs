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
using DotNetNuke.Web.Api;
using R7.News.Components;
using R7.News.Data;
using R7.News.Feeds;
using R7.News.Models;
using R7.News.Stream.Components;

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

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage Atom (string key)
        {
            var keyParts = UrlUtils.DecryptParameter (key).Split ('-');
            var tabId = int.Parse (keyParts [0]);
            var moduleId = int.Parse (keyParts [1]);

            var settings = default (StreamSettings);

            var isValidModule = false;
            var module = ModuleController.Instance.GetModule (moduleId, tabId, false);
            if (module != null) {
                settings = GetModuleSettings (module);
                isValidModule = (settings != null);
            }

            if (!isValidModule) {
                // TODO: Log error
                return Request.CreateResponse (HttpStatusCode.BadRequest);
            }

            if (!settings.EnableFeed) {
                return Request.CreateResponse (HttpStatusCode.Forbidden);
            }

            if (!ModulePermissionController.CanViewModule (module)) {
                return Request.CreateResponse (HttpStatusCode.Unauthorized);
            }

            var newsEntries = GetNewsEntries (module, settings);
            if (newsEntries != null) {
                var feed = new AtomFeed ();

                var writer = new Utf8StringWriter ();
                var xmlWriter = XmlWriter.Create (writer, new XmlWriterSettings {
                    Indent = true,
                    IndentChars = "  ",
                    Encoding = Encoding.UTF8
                });

                feed.Render (xmlWriter, newsEntries, module, PortalSettings, Request.RequestUri.ToString ());

                // media type should be "application/atom+xml", but IIS serve it as file to download
                return Request.CreateResponse (HttpStatusCode.OK, writer.ToString (),
                    new StringPassThroughMediaTypeFormatter (), "text/xml");
            }

            // TODO: Log error
            return Request.CreateResponse (HttpStatusCode.InternalServerError);
        }
    }
}
