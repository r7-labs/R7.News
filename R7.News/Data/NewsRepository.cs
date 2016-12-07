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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Caching;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Services.FileSystem;
using R7.News.Components;
using R7.News.Models;

namespace R7.News.Data
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

        public const string NewsCacheKeyPrefix = "//" + Const.Prefix + "?";

        protected const string SpNamePrefix = Const.Prefix + "_";

        public NewsEntryInfo GetNewsEntry (int entryId, int portalId)
        {
            var newsEntry = NewsDataProvider.Instance.Get<NewsEntryInfo> (entryId, portalId);
            if (newsEntry != null) {
                return (NewsEntryInfo) newsEntry
                    .WithAgentModule (NewsDataProvider.Instance.ModuleController)
                    .WithContentItem ();
            }

            return null;
        }

        public NewsEntryInfo GetNewsEntryByContentItem (ContentItem contentItem)
        {
            return NewsDataProvider.Instance.Get<NewsEntryInfo> (int.Parse (contentItem.ContentKey));
        }

        public int AddNewsEntry (NewsEntryInfo newsEntry,
                                 List<Term> terms,
                                 List<IFileInfo> images,
                                 int moduleId,
                                 int tabId)
        {
            // TODO: Add value to ContentKey
            var contentItem = new ContentItem {
                ContentTitle = newsEntry.Title,
                Content = newsEntry.Title,
                ContentTypeId = NewsDataProvider.Instance.NewsContentType.ContentTypeId,
                Indexed = false,
                ModuleID = newsEntry.AgentModuleId ?? moduleId,
                TabID = tabId,
            };

            // add content item and news entry
            newsEntry.ContentItemId = NewsDataProvider.Instance.ContentController.AddContentItem (contentItem);
            NewsDataProvider.Instance.Add<NewsEntryInfo> (newsEntry);

            // update content item after EntryId get its value
            // TODO: ContentKey should allow users to view your content item directly based on links provided from the tag search results
            // more info here: http://www.dnnsoftware.com/community-blog/cid/131963/adding-core-taxonomy-to-your-module-part-2-ndash-content-items
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

            CacheHelper.RemoveCacheByPrefix (NewsCacheKeyPrefix);

            return newsEntry.EntryId;
        }

        internal int BulkAddNewsEntry (IRepository<NewsEntryInfo> repository, NewsEntryInfo newsEntry,
            List<Term> terms,
            List<IFileInfo> images,
            int moduleId,
            int tabId)
        {
            // TODO: Add value to ContentKey
            var contentItem = new ContentItem {
                ContentTitle = newsEntry.Title,
                Content = newsEntry.Title,
                ContentTypeId = NewsDataProvider.Instance.NewsContentType.ContentTypeId,
                Indexed = false,
                ModuleID = newsEntry.AgentModuleId ?? moduleId,
                TabID = tabId,
            };

            // add content item and news entry
            newsEntry.ContentItemId = NewsDataProvider.Instance.ContentController.AddContentItem (contentItem);
            repository.Insert (newsEntry);

            // update content item after EntryId get its value
            // TODO: ContentKey should allow users to view your content item directly based on links provided from the tag search results
            // more info here: http://www.dnnsoftware.com/community-blog/cid/131963/adding-core-taxonomy-to-your-module-part-2-ndash-content-items
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

            return newsEntry.EntryId;
        }

        public void UpdateNewsEntry (NewsEntryInfo newsEntry, List<Term> terms, int moduleId, int tabId)
        {
            // TODO: Update value of ContentKey
            // update content item
            newsEntry.ContentItem.ContentTitle = newsEntry.Title;
            newsEntry.ContentItem.Content = newsEntry.Title;
            newsEntry.ContentItem.ModuleID = newsEntry.AgentModuleId ?? moduleId;
            newsEntry.ContentItem.TabID = tabId;
            NewsDataProvider.Instance.ContentController.UpdateContentItem (newsEntry.ContentItem);

            NewsDataProvider.Instance.Update<NewsEntryInfo> (newsEntry);

            // update content item terms
            var termController = new TermController ();
            termController.RemoveTermsFromContent (newsEntry.ContentItem);
            foreach (var term in terms) {
                termController.AddTermToContent (term, newsEntry.ContentItem);
            }

            CacheHelper.RemoveCacheByPrefix (NewsCacheKeyPrefix);
        }

        public void DeleteNewsEntry (INewsEntry newsEntry)
        {
            // delete content item, related news entry will be deleted by foreign key rule
            NewsDataProvider.Instance.ContentController.DeleteContentItem (newsEntry.ContentItem);

            CacheHelper.RemoveCacheByPrefix (NewsCacheKeyPrefix);
        }

        public IEnumerable<NewsEntryInfo> GetNewsEntries (int moduleId,
                                                          int portalId, 
                                                          int minThematicWeight,
                                                          int maxThematicWeight,
                                                          int minStructuralWeight,
                                                          int maxStructuralWeight)
        {
            var cacheKey = NewsCacheKeyPrefix + "ModuleId=" + moduleId;

            return DataCache.GetCachedData<IEnumerable<NewsEntryInfo>> (
                new CacheItemArgs (cacheKey, NewsConfig.GetInstance (portalId).DataCacheTime, CacheItemPriority.Normal),
                c => GetNewsEntriesInternal (portalId, 
                    minThematicWeight, maxThematicWeight, minStructuralWeight, maxStructuralWeight)
            );
        }

        protected IEnumerable<NewsEntryInfo> GetNewsEntriesInternal (int portalId, 
                                                                     int minThematicWeight,
                                                                     int maxThematicWeight,
                                                                     int minStructuralWeight,
                                                                     int maxStructuralWeight)
        {
            return NewsDataProvider.Instance.GetObjects<NewsEntryInfo> (System.Data.CommandType.StoredProcedure, 
                SpNamePrefix + "GetNewsEntries", portalId, 
                minThematicWeight, maxThematicWeight, minStructuralWeight, maxStructuralWeight)
                    .WithContentItems ()
                    .WithAgentModules (NewsDataProvider.Instance.ModuleController)
                    .Cast<NewsEntryInfo> ();
        }

        public int GetNewsEntries_Count (int portalId,
                                         bool checkNow,
                                         DateTime now,
                                         int minThematicWeight,
                                         int maxThematicWeight,
                                         int minStructuralWeight,
                                         int maxStructuralWeight)
        {
            return NewsDataProvider.Instance.ExecuteSpScalar<int> (SpNamePrefix + "GetNewsEntries_Count", 
                portalId, checkNow, now,
                minThematicWeight, maxThematicWeight, minStructuralWeight, maxStructuralWeight
            );
        }

        public IEnumerable<NewsEntryInfo> GetNewsEntries_FirstPage (int portalId, 
                                                                    int pageSize,
                                                                    bool checkNow,
                                                                    DateTime now, 
                                                                    int minThematicWeight,
                                                                    int maxThematicWeight,
                                                                    int minStructuralWeight,
                                                                    int maxStructuralWeight)
        {
            return NewsDataProvider.Instance.GetObjectsFromSp<NewsEntryInfo> (SpNamePrefix + "GetNewsEntries_FirstPage",
                portalId, pageSize, checkNow, now,
                minThematicWeight, maxThematicWeight, minStructuralWeight, maxStructuralWeight)
                .WithContentItems ()
                .WithAgentModules (NewsDataProvider.Instance.ModuleController)
                .Cast<NewsEntryInfo> ();
        }

        public IEnumerable<NewsEntryInfo> GetNewsEntriesByTerms (int moduleId, int portalId,
                                                                 int minThematicWeight, int maxThematicWeight, int minStructuralWeight, int maxStructuralWeight,
                                                                 IList<Term> terms)
        {
            var cacheKey = NewsCacheKeyPrefix + "ModuleId=" + moduleId;

            return DataCache.GetCachedData<IEnumerable<NewsEntryInfo>> (
                new CacheItemArgs (cacheKey, NewsConfig.GetInstance (portalId).DataCacheTime, CacheItemPriority.Normal),
                c => GetNewsEntriesByTermsInternal (portalId, 
                    minThematicWeight, maxThematicWeight, minStructuralWeight, maxStructuralWeight, terms)
            );
        }

        protected IEnumerable<NewsEntryInfo> GetNewsEntriesByTermsInternal (int portalId, 
                                                                            int minThematicWeight, int maxThematicWeight, int minStructuralWeight, int maxStructuralWeight,
                                                                            IList<Term> terms)
        {
            Contract.Requires (terms != null);

            if (terms.Count > 0) {
                return NewsDataProvider.Instance.GetObjects<NewsEntryInfo> (System.Data.CommandType.StoredProcedure,
                    SpNamePrefix + "GetNewsEntriesByTerms", portalId, minThematicWeight, maxThematicWeight,
                    minStructuralWeight, maxStructuralWeight, terms.Select (t => t.TermId).ToArray ())
                        .WithContentItems ()
                        .WithAgentModules (NewsDataProvider.Instance.ModuleController)
                        .Cast<NewsEntryInfo> ();
            }

            return Enumerable.Empty<NewsEntryInfo> ();
        }

        public int GetNewsEntriesByTerms_Count (int portalId, bool checkNow, DateTime now,
                                                int minThematicWeight, int maxThematicWeight, int minStructuralWeight, int maxStructuralWeight,
                                                IList<Term> terms)
        {
            Contract.Requires (terms != null);

            if (terms.Count > 0) {
                return NewsDataProvider.Instance.ExecuteSpScalar<int> (SpNamePrefix + "GetNewsEntriesByTerms_Count",
                    portalId, checkNow, now,
                    minThematicWeight, maxThematicWeight, minStructuralWeight, maxStructuralWeight,
                    terms.Select (t => t.TermId).ToArray ()
                );
            }

            return 0;
        }

        public IEnumerable<NewsEntryInfo> GetNewsEntriesByTerms_FirstPage (
            int portalId, int pageSize, bool checkNow, DateTime now,
            int minThematicWeight, int maxThematicWeight, int minStructuralWeight, int maxStructuralWeight,
            IList<Term> terms)
        {
            Contract.Requires (terms != null);

            if (terms.Count > 0) {
                return NewsDataProvider.Instance.GetObjectsFromSp<NewsEntryInfo> (SpNamePrefix + "GetNewsEntriesByTerms_FirstPage",
                    portalId, pageSize, checkNow, now,
                    minThematicWeight, maxThematicWeight, minStructuralWeight, maxStructuralWeight,
                    terms.Select (t => t.TermId).ToArray ())
                    .WithContentItems ()
                    .WithAgentModules (NewsDataProvider.Instance.ModuleController)
                    .Cast<NewsEntryInfo> ();
            }

            return Enumerable.Empty<NewsEntryInfo> ();
        }

        public IEnumerable<NewsEntryInfo> GetNewsEntriesByAgent (int moduleId, int portalId)
        {
            var cacheKey = NewsCacheKeyPrefix + "AgentModuleId=" + moduleId;
            return DataCache.GetCachedData<IEnumerable<NewsEntryInfo>> (
                new CacheItemArgs (cacheKey, NewsConfig.GetInstance (portalId).DataCacheTime, CacheItemPriority.Normal),
                c => GetNewsEntriesByAgentInternal (moduleId)
            );
        }

        protected IEnumerable<NewsEntryInfo> GetNewsEntriesByAgentInternal (int moduleId)
        {
            return NewsDataProvider.Instance.GetObjects<NewsEntryInfo> ("WHERE AgentModuleId = @0", moduleId)
                .WithContentItemsOneByOne ()
            // .WithAgentModules (NewsDataProvider.Instance.ModuleController)
                .Cast<NewsEntryInfo> ();
        }

        public void RemoveModuleCache (int moduleId)
        {
            CacheHelper.RemoveCacheByPrefix (NewsRepository.NewsCacheKeyPrefix + "ModuleId=" + moduleId);
        }
    }
}
