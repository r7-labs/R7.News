using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Web.DDRMenu;
using R7.News.Components;
using R7.News.Data;
using R7.News.Models;
using R7.News.Stream.Models;

namespace R7.News.Stream.Integrations.DDRMenu
{
    public class StreamNodeManipulator: INodeManipulator
    {
        #region INodeManipulator implementation

        // TODO: Pass extra parameters via specific node instead of config, remove that node after processing?
        public List<MenuNode> ManipulateNodes (List<MenuNode> nodes, PortalSettings portalSettings)
        {
            try {
                var config = NewsConfig.GetInstance (portalSettings.PortalId).NodeManipulator;
                if (config.ParentNodeTabId <= 0) {
                    return nodes;
                }
                var parentNode = FindNodeByTabId (nodes, config.ParentNodeTabId);
                if (parentNode == null) {
                    return nodes;
                }

                var streamModule = ModuleController.Instance.GetModule (config.StreamModuleId, config.StreamModuleTabId, false);
                if (streamModule == null) {
                    LogAdminAlert ($"Could not find Stream module with ModuleID={config.StreamModuleId} on page with TabID={config.StreamModuleTabId}.", portalSettings.PortalId);
                }
                else {
                    var settingsRepository = new StreamSettingsRepository ();
                    var settings = settingsRepository.GetSettings (streamModule);
                    var newsEntries = GetNewsEntries_Cached (streamModule.ModuleID, portalSettings.PortalId, settings);
                    foreach (var newsEntry in newsEntries) {
                        parentNode.Children.Add (CreateMenuNode (newsEntry, parentNode, streamModule));
                    }
                }
            }
            catch (Exception ex) {
                Exceptions.LogException (ex);
            }

            return nodes;
        }

        #endregion

        protected MenuNode FindNodeByTabId (IList<MenuNode> nodes, int tabId)
        {
            foreach (var node in nodes) {
                var parentNode = node.FindById (tabId);
                if (parentNode != null) {
                    return parentNode;
                }
            }

            return null;
        }

        protected IEnumerable<INewsEntry> GetNewsEntries_Cached (int moduleId, int portalId, StreamSettings settings)
        {
            // TODO: Reuse cached data from StreamViewModel or (better) move caching to repository
            var cacheKey = "//" + Const.Prefix + "/NodeManipulator?ModuleId=" + moduleId;
            return DataCache.GetCachedData<IEnumerable<INewsEntry>> (
                new CacheItemArgs (cacheKey, NewsConfig.GetInstance (portalId).DataCacheTime, CacheItemPriority.Normal),
                c => GetNewsEntries (portalId, settings)
            );
        }

        protected IEnumerable<INewsEntry> GetNewsEntries (int portalId, StreamSettings settings)
        {
            return NewsRepository.Instance.GetNewsEntries_FirstPage (
                portalId, settings.PageSize, DateTime.Now,
                new WeightRange (settings.MinThematicWeight, settings.MaxThematicWeight),
                new WeightRange (settings.MinStructuralWeight, settings.MaxStructuralWeight),
                settings.ShowAllNews, settings.IncludeTerms, out int newsEntriesCount
            );
        }

        protected MenuNode CreateMenuNode (INewsEntry newsEntry, MenuNode parentNode, ModuleInfo streamModule)
        {
            var node = new MenuNode ();
            node.Enabled = true;
            node.Parent = parentNode;
            node.Text = newsEntry.Title;
            node.Title = newsEntry.Title;
            node.Description = HtmlUtils.StripTags (HttpUtility.HtmlDecode (newsEntry.Description), false);
            node.Url = newsEntry.GetUrl (streamModule.TabID, streamModule.ModuleID);

            node.CommandName = "X-Date";
            node.CommandArgument = newsEntry.PublishedOnDate ().ToString ();

            if (newsEntry.AgentModule != null) {
                node.TabId = newsEntry.AgentModule.TabID;
            }

            return node;
        }

        void LogAdminAlert (string message, int portalId)
        {
            var log = new LogInfo ();
            log.LogPortalID = portalId;
            log.LogTypeKey = EventLogController.EventLogType.ADMIN_ALERT.ToString ();
            log.AddProperty (GetType ().ToString (), message);
            EventLogController.Instance.AddLog (log);
        }
    }
}
