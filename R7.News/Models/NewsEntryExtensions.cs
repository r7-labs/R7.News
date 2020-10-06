using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.FileSystem;
using R7.News.Components;
using R7.News.Controls.Models;
using R7.News.Data;

namespace R7.News.Models
{
    public static class NewsEntryExtensions
    {
        public static INewsEntry WithContentItem (this INewsEntry newsEntry)
        {
            newsEntry.ContentItem = NewsDataProvider.Instance.ContentController.GetContentItem (newsEntry.ContentItemId);
            return newsEntry;
        }

        public static INewsEntry WithText (this INewsEntry newsEntry)
        {
            if (newsEntry.EntryTextId != null) {
                newsEntry.Text = NewsDataProvider.Instance.Get<NewsEntryText, int> (newsEntry.EntryTextId.Value).Text;
            }
            return newsEntry;
        }

        public static IEnumerable<INewsEntry> WithContentItems (this IEnumerable<INewsEntry> newsEntries)
        {
            var contentController = new ContentController ();
            var contentItems = contentController.GetContentItemsByContentType (NewsDataProvider.Instance.NewsContentType);

            return newsEntries.Join (contentItems.DefaultIfEmpty (),
                ne => ne.ContentItemId,
                ci => ci.ContentItemId,
                (ne, ci) => {
                    ne.ContentItem = ci;
                    return ne;
                }
            );
        }

        public static IEnumerable<INewsEntry> WithContentItemsOneByOne (this IEnumerable<INewsEntry> newsEntries)
        {
            foreach (var newsEntry in newsEntries) {
                yield return newsEntry.WithContentItem ();
            }
        }

        public static INewsEntry WithAgentModule (this INewsEntry newsEntry, IModuleController moduleController)
        {
            if (newsEntry.AgentModuleId != null) {
                newsEntry.AgentModule = GetAgentModule (moduleController, newsEntry.AgentModuleId.Value);
            }

            return newsEntry;
        }

        private static ModuleInfo GetAgentModule (IModuleController moduleController, int agentModuleId)
        {
            return moduleController.GetTabModulesByModule (agentModuleId)
                .FirstOrDefault (am => !am.IsDeleted);
        }

        private static ModuleInfo GetAgentModuleOnce (IModuleController moduleController, INewsEntry newsEntry)
        {
            if (newsEntry.AgentModule == null && newsEntry.AgentModuleId != null) {
                return GetAgentModule (moduleController, newsEntry.AgentModuleId.Value);
            }

            return newsEntry.AgentModule;
        }

        public static IEnumerable<INewsEntry> WithAgentModules (this IEnumerable<INewsEntry> newsEntries,
                                                                IModuleController moduleController)
        {
            foreach (var newsEntry in newsEntries) {
                yield return newsEntry.WithAgentModule (moduleController);
            }
        }

        public static IFileInfo GetImage (this INewsEntry newsEntry)
        {
            return newsEntry.ContentItem.Images.FirstOrDefault ();
        }

        public static string GetImageUrl (this INewsEntry newsEntry, int width)
        {
            var image = newsEntry.GetImage ();
            if (image != null) {
                return Globals.AddHTTP (PortalSettings.Current.PortalAlias.HTTPAlias)
                + "/dnnimagehandler.ashx"
                + "?mode=securefile"
                + "&fileid=" + image.FileId
                + "&filter=resize"
                + "&w=" + width
                // this helps external services to understand mimetype
                + "&ext=." + image.Extension;
            }

            return string.Empty;
        }

        public static string GetRawImageUrl (this INewsEntry newsEntry)
        {
            var image = newsEntry.GetImage ();
            if (image != null) {
                return Globals.AddHTTP (PortalSettings.Current.PortalAlias.HTTPAlias) + FileManager.Instance.GetUrl (image);
            }

            return string.Empty;
        }

        public static bool IsPublished (this INewsEntry newsEntry, DateTime now)
        {
            return ModelHelper.IsPublished (now, newsEntry.StartDate, newsEntry.EndDate);
        }

        public static bool HasBeenExpired (this INewsEntry newsEntry, DateTime now)
        {
            return ModelHelper.HasBeenExpired (now, newsEntry.StartDate, newsEntry.EndDate);
        }

        public static DateTime PublishedOnDate (this INewsEntry newsEntry)
        {
            return ModelHelper.PublishedOnDate (newsEntry.StartDate, newsEntry.ContentItem.CreatedOnDate);
        }

        public static bool IsVisible (this INewsEntry newsEntry, int minThematicWeight, int maxThematicWeight,
                                      int minStructuralWeight, int maxStructuralWeight)
        {
            return ModelHelper.IsVisible (newsEntry.ThematicWeight, newsEntry.StructuralWeight,
                minThematicWeight, maxThematicWeight, minStructuralWeight, maxStructuralWeight);
        }

        public static string GetPermalink (this INewsEntry newsEntry,
                                           PermalinkMode permalinkMode,
                                           IModuleController moduleController,
                                           PortalAliasInfo portalAlias,
                                           int moduleId,
                                           int tabId)
        {
            return (permalinkMode == PermalinkMode.Friendly) ?
                GetPermalinkFriendly (newsEntry, moduleController, moduleId, tabId) :
                GetPermalinkRaw (newsEntry, moduleController, portalAlias, moduleId, tabId);
        }

        public static string GetPermalinkFriendly (this INewsEntry newsEntry,
                                                   IModuleController moduleController,
                                                   int moduleId,
                                                   int tabId)
        {
            var agentModule = GetAgentModuleOnce (moduleController, newsEntry);
            if (agentModule != null) {
                return Globals.NavigateURL (agentModule.TabID);
            }

            return Globals.NavigateURL (
                tabId,
                "entry",
                "mid",
                moduleId.ToString (),
                "entryid",
                newsEntry.EntryId.ToString ());
        }

        public static string GetPermalinkRaw (this INewsEntry newsEntry,
                                              IModuleController moduleController,
                                              PortalAliasInfo portalAlias,
                                              int moduleId,
                                              int tabId)
        {
            var agentModule = GetAgentModuleOnce (moduleController, newsEntry);
            if (agentModule != null) {
                return Globals.AddHTTP (portalAlias.HTTPAlias + "/default.aspx?tabid=" + agentModule.TabID);
            }

            return Globals.AddHTTP (portalAlias.HTTPAlias + "/default.aspx?tabid=" + tabId
            + "&mid=" + moduleId + "&ctl=entry&entryid=" + newsEntry.EntryId);
        }

        public static IList<NewsEntryAction> GetActions (this INewsEntry newsEntry, int moduleId)
        {
            var actions = new List<NewsEntryAction> ();
            var discussionStarted = !string.IsNullOrEmpty (newsEntry.DiscussProviderKey);
            if (!discussionStarted) {
                var discussProvider = NewsConfig.Instance.GetDiscussProviders ().FirstOrDefault ();
                if (discussProvider != null) {
                    actions.Add (new NewsEntryAction {
                        EntryId = newsEntry.EntryId,
                        Action = NewsEntryActions.StartDiscussion,
                        ModuleId = moduleId,
                        Params = new string [] { discussProvider.ProviderKey },
                        Enabled = HttpContext.Current.Request.IsAuthenticated
                    });
                }
            } else {
                var discussProvider = NewsConfig.Instance.GetDiscussProviders ()
                                                .FirstOrDefault (dp => dp.ProviderKey == newsEntry.DiscussProviderKey);
                if (discussProvider != null) {
                    actions.Add (new NewsEntryAction {
                        EntryId = newsEntry.EntryId,
                        Action = NewsEntryActions.JoinDiscussion,
                        ModuleId = moduleId,
                        Params = new string [] {
                            discussProvider.ProviderKey,
                            discussProvider.GetReplyCount (newsEntry.DiscussEntryId).ToString ()
                        },
                        Enabled = true
                    });
                }
            }

            return actions;
        }

        public static string GetUrl (this INewsEntry newsEntry, int tabId, int moduleId)
        {
            if (!string.IsNullOrWhiteSpace (newsEntry.Url)) {
                if (Globals.GetURLType (newsEntry.Url) == TabType.Url) {
                    return newsEntry.Url;
                }
                return Globals.LinkClick (newsEntry.Url, tabId, moduleId);
            }
            return newsEntry.GetPermalink (PermalinkMode.Friendly,
                                      NewsDataProvider.Instance.ModuleController,
                                      PortalSettings.Current.PortalAlias,
                                      moduleId, tabId);
        }

        public static string GetFullUrl (this INewsEntry newsEntry, int tabId, int moduleId)
        {
            var portalAlias = PortalSettings.Current.PortalAlias;
            if (!string.IsNullOrWhiteSpace (newsEntry.Url)) {
                return Globals.AddHTTP (portalAlias.HTTPAlias + Globals.LinkClick (newsEntry.Url, tabId, moduleId));
            }

            return newsEntry.GetPermalink (PermalinkMode.Friendly,
                                      NewsDataProvider.Instance.ModuleController,
                                      portalAlias,
                                      moduleId, tabId);
        }
    }
}

