//
//  NewsRepository.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016 Roman M. Yagodin
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Linq;
using System.Web.Caching;
using System.Collections.Generic;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Common.Utilities;
using R7.News.Components;
using R7.News.Models;

namespace R7.News.Models.Data
{
    public class NewsRepository
    {
        #region Singleton implementation

        private static readonly Lazy<NewsRepository> instance = new Lazy<NewsRepository> ();

        public static NewsRepository Instance
        {
            get { return instance.Value; }
        }

        #endregion

        private const string newsCacheKeyPrefix = "r7_News_";

        public NewsEntryInfo GetNewsEntry (int entryId, int portalId)
        {
            return (NewsEntryInfo) NewsDataProvider.Instance.Get<NewsEntryInfo> (entryId, portalId)
                .WithContentItem ();
        }

        public ModuleNewsEntryInfo GetModuleNewsEntry (int entryId, int moduleId)
        {
            return (ModuleNewsEntryInfo) NewsDataProvider.Instance.GetObjectsFromSp<ModuleNewsEntryInfo> (
                    "r7_News_GetModuleNewsEntry", entryId, moduleId)
                .Single ()
                .WithContentItem ();
        }

        public NewsEntryInfo GetNewsEntryByContentItem (ContentItem contentItem)
        {
            return NewsDataProvider.Instance.Get<NewsEntryInfo> (int.Parse (contentItem.ContentKey));
        }

        public int AddModuleNewsEntry (ModuleNewsEntryInfo newsEntry, List<Term> terms, List<IFileInfo> images, int moduleId, int tabId)
        {
            var entryId = AddNewsEntry (newsEntry, terms, images, moduleId, tabId);

            if (newsEntry.Visibility != null) {
                NewsDataProvider.Instance.Add<ModuleRuleInfo> (new ModuleRuleInfo { 
                    EntryId = entryId,
                    ModuleId = moduleId,
                    Visibility = newsEntry.Visibility
                });
            }

            return entryId;
        }

        public int AddNewsEntry (ModuleNewsEntryInfo newsEntry, List<Term> terms, List<IFileInfo> images, int moduleId, int tabId)
        {
            var contentItem = new ContentItem {
                ContentTitle = newsEntry.Title,
                ContentTypeId = NewsDataProvider.Instance.NewsContentType.ContentTypeId,
                Indexed = false,
                // Content = newsEntry.Description,
                // ContentKey = "mid=" + moduleId + "&ctl=DetailControlKey" + "&entryId=" + newsEntry.EntryId,
                ModuleID = newsEntry.AgentModuleId ?? moduleId,
                TabID = tabId,
            };

            // add content item and news entry
            newsEntry.ContentItemId = NewsDataProvider.Instance.ContentController.AddContentItem (contentItem);
            NewsDataProvider.Instance.Add<ModuleNewsEntryInfo> (newsEntry);

            // update content item after EntryId get its value
            contentItem.ContentKey = newsEntry.EntryId.ToString ();
            NewsDataProvider.Instance.ContentController.UpdateContentItem (contentItem);

            // add images to content item
            if (images.Count > 0) {
                var attachmentController = new AttachmentController (NewsDataProvider.Instance.ContentController);
                attachmentController.AddImagesToContent (contentItem.ContentItemId, images);
            }

            // add terms to content item
            var termController = new TermController ();
            foreach (var term in terms) {
                termController.AddTermToContent (term, contentItem);
            }

            CacheHelper.RemoveCacheByPrefix (newsCacheKeyPrefix);

            return newsEntry.EntryId;
        }

        public void UpdateModuleNewsEntry (ModuleNewsEntryInfo newsEntry, List<Term> terms, int moduleId)
        {
            UpdateNewsEntry (newsEntry, terms);

            var entryId = newsEntry.EntryId;
            var moduleRule = NewsDataProvider.Instance.Get<ModuleRuleInfo> (
                                 "WHERE ModuleID = @0 AND EntryID = @1",
                                 moduleId,
                                 entryId);
            
            if (moduleRule != null) {
                if (newsEntry.Visibility == null) {
                    NewsDataProvider.Instance.Delete<ModuleRuleInfo> (moduleRule);
                }
                else {
                    moduleRule.Visibility = newsEntry.Visibility;
                    NewsDataProvider.Instance.Update<ModuleRuleInfo> (moduleRule);
                }
            }
            else {
                if (newsEntry.Visibility != null) {
                    NewsDataProvider.Instance.Add<ModuleRuleInfo> (new ModuleRuleInfo { 
                        EntryId = entryId,
                        ModuleId = moduleId,
                        Visibility = newsEntry.Visibility
                    });
                }
            }
        }

        public void UpdateNewsEntry (ModuleNewsEntryInfo newsEntry, List<Term> terms)
        {
            NewsDataProvider.Instance.ContentController.UpdateContentItem (newsEntry.ContentItem);
            NewsDataProvider.Instance.Update<ModuleNewsEntryInfo> (newsEntry);

            // update content item terms
            var termController = new TermController ();
            termController.RemoveTermsFromContent (newsEntry.ContentItem);
            foreach (var term in terms) {
                termController.AddTermToContent (term, newsEntry.ContentItem);
            }

            CacheHelper.RemoveCacheByPrefix (newsCacheKeyPrefix);
        }

        public void DeleteNewsEntry (INewsEntry newsEntry)
        {
            // delete content item, related news entry will be deleted by foreign key rule
            NewsDataProvider.Instance.ContentController.DeleteContentItem (newsEntry.ContentItem);

            CacheHelper.RemoveCacheByPrefix (newsCacheKeyPrefix);
        }

        public IEnumerable<ModuleNewsEntryInfo> GetModuleNewsEntries (int moduleId, int portalId)
        {
            var cacheKey = newsCacheKeyPrefix + "ModuleId_" + moduleId;

            return DataCache.GetCachedData<IEnumerable<ModuleNewsEntryInfo>> (
                new CacheItemArgs (cacheKey, NewsConfig.Instance.DataCacheTime, CacheItemPriority.Normal),
                c => GetModuleNewsEntriesInternal (moduleId, portalId)
            );
        }

        protected IEnumerable<ModuleNewsEntryInfo> GetModuleNewsEntriesInternal (int moduleId, int portalId)
        {
            return NewsDataProvider.Instance.GetObjects<ModuleNewsEntryInfo> (System.Data.CommandType.StoredProcedure, 
                "r7_News_GetModuleNewsEntries", moduleId, portalId)
                    .WithContentItems ()
                    .WithAgentModules (NewsDataProvider.Instance.ModuleController)
                    .WithNewsSources ()
                    .Cast<ModuleNewsEntryInfo> ();
        }

        public IEnumerable<ModuleNewsEntryInfo> GetModuleNewsEntriesByTerms (int moduleId, int portalId, IList<Term> terms)
        {
            var cacheKey = newsCacheKeyPrefix + "ModuleId_" + moduleId;

            return DataCache.GetCachedData<IEnumerable<ModuleNewsEntryInfo>> (
                new CacheItemArgs (cacheKey, NewsConfig.Instance.DataCacheTime, CacheItemPriority.Normal),
                c => GetModuleNewsEntriesByTermsInternal (moduleId, portalId, terms)
            );
        }

        protected IEnumerable<ModuleNewsEntryInfo> GetModuleNewsEntriesByTermsInternal (int moduleId, int portalId, IList<Term> terms)
        {
            return NewsDataProvider.Instance.GetObjects<ModuleNewsEntryInfo> (System.Data.CommandType.StoredProcedure, 
                "r7_News_GetNewsEntriesByTerms", moduleId, portalId, terms.Select (t => t.TermId).ToArray ())
                    .WithContentItems ()
                    .WithAgentModules (NewsDataProvider.Instance.ModuleController)
                    .WithNewsSources ()
                    .Cast<ModuleNewsEntryInfo> ();
        }

        public IEnumerable<NewsEntryInfo> GetNewsEntriesByAgent (int moduleId)
        {
            var cacheKey = newsCacheKeyPrefix + "AgentModuleId_" + moduleId;
            return DataCache.GetCachedData<IEnumerable<NewsEntryInfo>> (
                new CacheItemArgs (cacheKey, NewsConfig.Instance.DataCacheTime, CacheItemPriority.Normal),
                c => GetNewsEntriesByAgentInternal (moduleId)
            );
        }

        protected IEnumerable<NewsEntryInfo> GetNewsEntriesByAgentInternal (int moduleId)
        {
            return NewsDataProvider.Instance.GetObjects<NewsEntryInfo> ("WHERE AgentModuleId = @0", moduleId)
                .WithContentItemsOneByOne ()
                // .WithAgentModules (NewsDataProvider.Instance.ModuleController)
                .WithNewsSources ()
                .Cast<NewsEntryInfo> ();
        }
    }
}
