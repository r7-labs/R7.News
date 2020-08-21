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
            var newsEntry = NewsDataProvider.Instance.Get<NewsEntryInfo,int,int> (entryId, portalId);
            if (newsEntry != null) {
                return (NewsEntryInfo) newsEntry
                    .WithAgentModule (NewsDataProvider.Instance.ModuleController)
                    .WithContentItem ();
            }

            return null;
        }

        public NewsEntryText GetNewsEntryText (int entryTextId)
        {
            return NewsDataProvider.Instance.Get<NewsEntryText,int> (entryTextId);
        }

        public NewsEntryInfo GetNewsEntryByContentItem (ContentItem contentItem)
        {
            return NewsDataProvider.Instance.Get<NewsEntryInfo,int> (int.Parse (contentItem.ContentKey));
        }

        public int AddNewsEntry (NewsEntryInfo newsEntry,
                                 List<Term> terms,
                                 List<IFileInfo> images,
                                 int moduleId,
                                 int tabId)
        {
            var contentItem = AddContentItem (newsEntry, tabId, moduleId);
            newsEntry.ContentItemId = contentItem.ContentItemId;

            NewsDataProvider.Instance.Add (newsEntry);
            UpdateNewsEntryText (newsEntry);
            UpdateContentItem (contentItem, newsEntry, terms, images);

            DataCache.ClearCache (NewsCacheKeyPrefix);

            return newsEntry.EntryId;
        }

        public void UpdateNewsEntryText (NewsEntryInfo newsEntry)
        {
            // TODO: More careful handling of empty text including "empty" HTML markup like `<P>&nbsp;</P>`
            if (!string.IsNullOrEmpty (newsEntry.Text)) {
                var newsEntryText = new NewsEntryText {
                    EntryId = newsEntry.EntryId,
                    Text = newsEntry.Text
                };
                if (newsEntry.EntryTextId == null) {
                    NewsDataProvider.Instance.Add (newsEntryText);
                    newsEntry.EntryTextId = newsEntryText.EntryTextId;
                    NewsDataProvider.Instance.Update (newsEntry);
                }
                else {
                    newsEntryText.EntryTextId = newsEntry.EntryTextId.Value;
                    NewsDataProvider.Instance.Update (newsEntryText);
                }
            }
            else {
                if (newsEntry.EntryTextId != null) {
                    NewsDataProvider.Instance.Delete<NewsEntryText, int> (newsEntry.EntryTextId.Value);
                    newsEntry.EntryTextId = null;
                    NewsDataProvider.Instance.Update (newsEntry);
                }
            }
        }

        [Obsolete]
        internal int AddNewsEntry_Internal (IRepository<NewsEntryInfo> repository, NewsEntryInfo newsEntry,
            List<Term> terms,
            List<IFileInfo> images,
            int moduleId,
            int tabId)
        {
            var contentItem = AddContentItem (newsEntry, tabId, moduleId);
            newsEntry.ContentItemId = contentItem.ContentItemId;
            repository.Insert (newsEntry);

            UpdateContentItem (contentItem, newsEntry, terms, images);

            return newsEntry.EntryId;
        }

        ContentItem AddContentItem (INewsEntry newsEntry, int tabId, int moduleId)
        {
            // TODO: Add value to ContentKey
            var contentItem = new ContentItem {
                ContentTitle = newsEntry.Title,
                Content = newsEntry.Title,
                ContentTypeId = NewsDataProvider.Instance.NewsContentType.ContentTypeId,
                Indexed = false,
                ModuleID = newsEntry.AgentModuleId ?? moduleId,
                TabID = tabId
            };

            contentItem.ContentItemId = NewsDataProvider.Instance.ContentController.AddContentItem (contentItem);

            return contentItem;
        }

        public int DuplicateNewsEntry (NewsEntryInfo item, int moduleId, int tabId)
        {
            item.EntryId = 0;
            item.EntryTextId = null;

            if (item.IsPublished (DateTime.Now)) {
                item.StartDate = null;
                item.EndDate = DateTime.Today;
            }

            return AddNewsEntry (item, item.ContentItem.Terms, item.ContentItem.Images, moduleId, tabId);
        }

        // TODO: Can use IEnumerable here
        static void UpdateContentItem (ContentItem contentItem, NewsEntryInfo newsEntry, List<Term> terms, List<IFileInfo> images)
        {
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

            NewsDataProvider.Instance.Update (newsEntry);
            UpdateNewsEntryText (newsEntry);

            // update content item terms
            var termController = new TermController ();
            termController.RemoveTermsFromContent (newsEntry.ContentItem);
            foreach (var term in terms) {
                termController.AddTermToContent (term, newsEntry.ContentItem);
            }

            DataCache.ClearCache (NewsCacheKeyPrefix);
        }

        /// <summary>
        /// Updates the news entry w/o associated entities.
        /// </summary>
        /// <param name="newsEntry">News entry.</param>
        public void UpdateNewsEntry (NewsEntryInfo newsEntry)
        {
            NewsDataProvider.Instance.Update (newsEntry);
            DataCache.ClearCache (NewsCacheKeyPrefix);
        }

        public void DeleteNewsEntry (INewsEntry newsEntry)
        {
            // delete content item, related news entry and text records will be deleted by foreign key rules
            NewsDataProvider.Instance.ContentController.DeleteContentItem (newsEntry.ContentItem);

            DataCache.ClearCache (NewsCacheKeyPrefix);
        }

        public IEnumerable<NewsEntryInfo> GetAllNewsEntries (int moduleId,
                                                             int portalId,
                                                             WeightRange thematicWeights,
                                                             WeightRange structuralWeights)
        {
            var cacheKey = NewsCacheKeyPrefix + "ModuleId=" + moduleId;

            return DataCache.GetCachedData<IEnumerable<NewsEntryInfo>> (
                new CacheItemArgs (cacheKey, NewsConfig.GetInstance (portalId).DataCacheTime, CacheItemPriority.Normal),
                c => GetAllNewsEntriesInternal (portalId,
                    thematicWeights, structuralWeights)
            );
        }

        protected IEnumerable<NewsEntryInfo> GetAllNewsEntriesInternal (int portalId,
                                                                     WeightRange thematicWeights,
                                                                     WeightRange structuralWeights)
        {
            return NewsDataProvider.Instance.GetObjects<NewsEntryInfo> (
                System.Data.CommandType.StoredProcedure,
                SpNamePrefix + "GetNewsEntries", portalId,
                thematicWeights.Min, thematicWeights.Max, structuralWeights.Min, structuralWeights.Max)
                    .WithContentItems ()
                    .WithAgentModules (NewsDataProvider.Instance.ModuleController)
                    .Cast<NewsEntryInfo> ();
        }

        public int GetAllNewsEntries_Count (int portalId,
                                            DateTime? now,
                                            WeightRange thematicWeights,
                                            WeightRange structuralWeights)
        {
            return NewsDataProvider.Instance.ExecuteSpScalar<int> (
                SpNamePrefix + "GetNewsEntries_Count", portalId, now,
                thematicWeights.Min, thematicWeights.Max, structuralWeights.Min, structuralWeights.Max
            );
        }

        public IEnumerable<NewsEntryInfo> GetAllNewsEntries_FirstPage (int portalId,
                                                                       int pageSize,
                                                                       DateTime? now,
                                                                       WeightRange thematicWeights,
                                                                       WeightRange structuralWeights)
        {
            return NewsDataProvider.Instance.GetObjectsFromSp<NewsEntryInfo> (
                SpNamePrefix + "GetNewsEntries_FirstPage",
                portalId, pageSize, now,
                thematicWeights.Min, thematicWeights.Max, structuralWeights.Min, structuralWeights.Max)
                    .WithContentItems ()
                    .WithAgentModules (NewsDataProvider.Instance.ModuleController)
                    .Cast<NewsEntryInfo> ();
        }

        public IEnumerable<NewsEntryInfo> GetNewsEntriesByTerms (int moduleId,
                                                                 int portalId,
                                                                 WeightRange thematicWeights,
                                                                 WeightRange structuralWeights,
                                                                 IList<Term> terms)
        {
            var cacheKey = NewsCacheKeyPrefix + "ModuleId=" + moduleId;

            return DataCache.GetCachedData<IEnumerable<NewsEntryInfo>> (
                new CacheItemArgs (cacheKey, NewsConfig.GetInstance (portalId).DataCacheTime, CacheItemPriority.Normal),
                c => GetNewsEntriesByTermsInternal (portalId,
                    thematicWeights, structuralWeights, terms)
            );
        }

        protected IEnumerable<NewsEntryInfo> GetNewsEntriesByTermsInternal (int portalId,
                                                                            WeightRange thematicWeights,
                                                                            WeightRange structuralWeights,
                                                                            IList<Term> terms)
        {
            Contract.Requires (terms != null);

            if (terms.Count > 0) {
                return NewsDataProvider.Instance.GetObjects<NewsEntryInfo> (
                    System.Data.CommandType.StoredProcedure,
                    SpNamePrefix + "GetNewsEntriesByTerms", portalId,
                    thematicWeights.Min, thematicWeights.Max, structuralWeights.Min, structuralWeights.Max,
                    terms.Select (t => t.TermId).ToArray ())
                        .WithContentItems ()
                        .WithAgentModules (NewsDataProvider.Instance.ModuleController)
                        .Cast<NewsEntryInfo> ();
            }

            return Enumerable.Empty<NewsEntryInfo> ();
        }

        public int GetNewsEntriesByTerms_Count (int portalId,
                                                DateTime? now,
                                                WeightRange thematicWeights,
                                                WeightRange structuralWeights,
                                                IList<Term> terms)
        {
            Contract.Requires (terms != null);

            if (terms.Count > 0) {
                return NewsDataProvider.Instance.ExecuteSpScalar<int> (
                    SpNamePrefix + "GetNewsEntriesByTerms_Count", portalId, now,
                    thematicWeights.Min, thematicWeights.Max, structuralWeights.Min, structuralWeights.Max,
                    terms.Select (t => t.TermId).ToArray ()
                );
            }

            return 0;
        }

        protected IEnumerable<NewsEntryInfo> GetNewsEntriesByTerms_FirstPage (int portalId,
                                                                           int pageSize,
                                                                           DateTime? now,
                                                                           WeightRange thematicWeights,
                                                                           WeightRange structuralWeights,
                                                                           IList<Term> terms)
        {
            Contract.Requires (terms != null);

            if (terms.Count > 0) {
                return NewsDataProvider.Instance.GetObjectsFromSp<NewsEntryInfo> (SpNamePrefix + "GetNewsEntriesByTerms_FirstPage",
                    portalId, pageSize, now,
                    thematicWeights.Min, thematicWeights.Max, structuralWeights.Min, structuralWeights.Max,
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

        public IEnumerable<NewsEntryInfo> GetNewsEntries_FirstPage (int portalId, int pageSize, DateTime? now,
            WeightRange thematicRange, WeightRange structRange, bool showAllNews, IList<Term> includeTerms,
            out int newsEntriesCount)
        {
            if (showAllNews) {
                newsEntriesCount = GetAllNewsEntries_Count (portalId, now, thematicRange, structRange);
                return GetAllNewsEntries_FirstPage (portalId, pageSize, now, thematicRange, structRange);
            }

            newsEntriesCount = GetNewsEntriesByTerms_Count (portalId, now, thematicRange, structRange, includeTerms);
            return GetNewsEntriesByTerms_FirstPage (portalId, pageSize, now, thematicRange, structRange, includeTerms);
        }

        public IEnumerable<NewsEntryInfo> GetNewsEntries_Page (int moduleId, int portalId,
            WeightRange thematicRange, WeightRange structRange, bool showAllNews, IList<Term> includeTerms)
        {
            if (showAllNews) {
                return GetAllNewsEntries (moduleId, portalId, thematicRange, structRange);
            }

            return GetNewsEntriesByTerms (moduleId, portalId, thematicRange, structRange, includeTerms);
        }

        public void RemoveModuleCache (int moduleId)
        {
            DataCache.ClearCache (NewsCacheKeyPrefix + "ModuleId=" + moduleId);
        }
    }
}
