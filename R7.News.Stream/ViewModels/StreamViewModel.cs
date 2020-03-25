//
//  StreamViewModel.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016-2020  Roman M. Yagodin
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
using System.Linq;
using System.Web;
using System.Web.Caching;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.UI.Modules;
using R7.Dnn.Extensions.ViewModels;
using R7.News.Components;
using R7.News.Data;
using R7.News.Models;
using R7.News.Stream.Models;

namespace R7.News.Stream.ViewModels
{
    public class StreamViewModel : ViewModelContext<StreamSettings>
    {
        public StreamViewModel (IModuleControl module, StreamSettings settings) : base (module, settings)
        {
        }

        // TODO: Move to base library
        string Base64ToCanonicalForm (string base64String) => base64String.Replace ("%3d", "%3D");

        public string FeedUrl => Globals.AddHTTP (Module.PortalAlias.HTTPAlias
                + Globals.DesktopModulePath
                + "R7.News.Stream/API/Feed/Atom?key=" + Base64ToCanonicalForm (UrlUtils.EncryptParameter ($"{Module.TabId}-{Module.ModuleId}")));

        public StreamNewsEntryViewModelPage GetPage (int pageIndex, int pageSize)
        {
            var now = Module.IsEditable ? null : (DateTime?) HttpContext.Current.Timestamp;

            // TODO: Should 'now' value be used in the cache key? 
            // var today = DateTime.Today; 
            // var now = today + new TimeSpan (today.Hour, 0, 0);

            // TODO: Check for sorting options also
            if (pageIndex == 0 && pageSize == Settings.PageSize) {
                
                // we cache viewmodels for first page, so there are no need to implement caching
                // in the GetNewsEntries..._Count and GetNewsEntries..._FirstPage repository methods

                // if we will cache models instead of viewmodels, then we need to vary cache key by ModuleId, not TabModuleId
                var cacheKey = NewsRepository.NewsCacheKeyPrefix + "TabModuleId=" + Module.TabModuleId
                               + "&PageIndex=0&PageSize=" + pageSize + "&CheckNow=" + !Module.IsEditable;
                
                return DataCache.GetCachedData<StreamNewsEntryViewModelPage> (
                    new CacheItemArgs (cacheKey, NewsConfig.Instance.DataCacheTime, CacheItemPriority.Normal),
                    c => GetFirstPageInternal (pageSize, now)
                );
            }

            return GetPageInternal (pageIndex, pageSize, now);
        }

        protected StreamNewsEntryViewModelPage GetFirstPageInternal (int pageSize, DateTime? now)
        {
            var baseItems = NewsRepository.Instance.GetNewsEntries_FirstPage (Module.PortalId, Settings.PageSize, now,
                new WeightRange (Settings.MinThematicWeight, Settings.MaxThematicWeight),
                new WeightRange (Settings.MinStructuralWeight, Settings.MaxStructuralWeight),
                Settings.ShowAllNews, Settings.IncludeTerms, out int baseItemsCount);

            var streamModuleConfig = NewsConfig.Instance.StreamModule;

            return new StreamNewsEntryViewModelPage (
                baseItemsCount,
                baseItems.Select (ne => new StreamNewsEntryViewModel (ne, this, streamModuleConfig))
                .ToList ()
            );
        }

        protected StreamNewsEntryViewModelPage GetPageInternal (int pageIndex, int pageSize, DateTime? now)
        {
            // check for pageIndex < 0
            if (pageIndex < 0) {
                return StreamNewsEntryViewModelPage.Empty;
            }

            var baseItems = NewsRepository.Instance.GetNewsEntries_Page (Module.ModuleId, Module.PortalId,
                new WeightRange (Settings.MinThematicWeight, Settings.MaxThematicWeight),
                new WeightRange (Settings.MinStructuralWeight, Settings.MaxStructuralWeight),
                Settings.ShowAllNews, Settings.IncludeTerms);

            // check for no data available
            if (baseItems == null || !baseItems.Any ()) {
                return StreamNewsEntryViewModelPage.Empty;
            }

            // get only published items
            IList<NewsEntryInfo> items = baseItems
                .Where (ne => now == null || ne.IsPublished (now.Value))
                .ToList ();
            
            // check for no data available
            var totalItems = items.Count;
            if (totalItems == 0) {
                return StreamNewsEntryViewModelPage.Empty;
            }

            // check for pageIndex > totalPages
            var totalPages = totalItems / pageSize + ((totalItems % pageSize == 0) ? 0 : 1);
            if (pageIndex > totalPages) {
                return StreamNewsEntryViewModelPage.Empty;
            }

            var streamModuleConfig = NewsConfig.Instance.StreamModule;

            return new StreamNewsEntryViewModelPage (
                totalItems,
                items.OrderByDescending (ne => ne.PublishedOnDate ())
                    .Skip (pageIndex * pageSize)
                    .Take (pageSize)
                    .Select (ne => new StreamNewsEntryViewModel (ne, this, streamModuleConfig))
                    .ToList ()
            );
        }
    }
}
