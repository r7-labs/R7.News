//
//  NewsEntityExtensions.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016-2017 Roman M. Yagodin
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
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
                + "/imagehandler.ashx?fileticket=" + UrlUtils.EncryptParameter (image.FileId.ToString ())
                + "&width=" + width + "&ext=." + image.Extension;
            }

            return string.Empty;
        }

        public static IEnumerable<INewsEntry> GroupByAgentModule (this IEnumerable<INewsEntry> newsEntries,
                                                                  bool enableGrouping)
        {
            if (enableGrouping) {
                var newsList = new List<INewsEntry> ();
                foreach (var newsEntry in newsEntries) {

                    // find group entry
                    var groupEntry = newsList
                        .SingleOrDefault (ne => ne.AgentModuleId != null && ne.AgentModuleId == newsEntry.AgentModuleId);

                    // add current entry to the group
                    if (groupEntry != null) {
                        if (groupEntry.Group == null) {
                            groupEntry.Group = new Collection<INewsEntry> ();
                        }
                        groupEntry.Group.Add (newsEntry);
                        continue;
                    }

                    // add current entry as group entry
                    newsEntry.Group = null;
                    newsList.Add (newsEntry);
                }

                return newsList;
            }
            else {
                // clear group references
                foreach (var newsEntry in newsEntries) {
                    newsEntry.Group = null;
                }

                return newsEntries;
            }
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

        public static IList<NewsEntryAction> GetActions (this INewsEntry newsEntry)
        {
            var actions = new List<NewsEntryAction> ();
            var discussionStarted = !string.IsNullOrEmpty (newsEntry.DiscussProviderKey);
            if (!discussionStarted) {
                var discussProvider = NewsConfig.Instance.GetDiscussProviders ().FirstOrDefault ();
                if (discussProvider != null) {
                    actions.Add (new NewsEntryAction {
                        EntryId = newsEntry.EntryId,
                        Action = NewsEntryActions.StartDiscussion,
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

