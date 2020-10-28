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

namespace R7.News.Stream.Services
{
    // TODO: Move to the base library?
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }

    public class FeedController : DnnApiController
    {
        IEnumerable<NewsEntry> GetNewsEntries (ModuleInfo module, StreamSettings settings)
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
            return RenderFeed (new AtomFeed (), key, withImages);
        }

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage Rss (string key)
        {
            return RenderFeed (new RssFeed (), key, false);
        }

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage RssVk (string key)
        {
            return RenderFeed (new RssVkFeed (), key, true);
        }

        HttpResponseMessage RenderFeed (IFeed feed, string key, bool withImages = false)
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

                feed.Render (xmlWriter, newsEntries, module, PortalSettings, Request.RequestUri.ToString (), withImages);

                return new HttpResponseMessage {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent (writer.ToString (), Encoding.UTF8, "text/xml")
                };
            }
            catch (Exception ex) {
                LogFeedException (logType, ex, tabId, moduleId);
                return Request.CreateResponse (statusCode);
            }
        }

        void LogFeedException (EventLogController.EventLogType logType, Exception ex, int tabId, int moduleId)
        {
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
        }
    }
}
