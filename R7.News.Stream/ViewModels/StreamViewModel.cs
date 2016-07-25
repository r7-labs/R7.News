//
//  StreamViewModel.cs
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
using System.Linq;
using System.Web;
using System.Web.Caching;
using DotNetNuke.Common.Utilities;
using DotNetNuke.UI.Modules;
using R7.DotNetNuke.Extensions.ViewModels;
using R7.News.Components;
using R7.News.Data;
using R7.News.Models;
using R7.News.Stream.Components;

namespace R7.News.Stream.ViewModels
{
    public class StreamViewModel: ViewModelContext<StreamSettings>
    {
        public StreamViewModel (IModuleControl module, StreamSettings settings) : base (module, settings)
        {
        }

        public StreamNewsEntryViewModelPage GetPage (int pageIndex, int pageSize)
        {
            var checkNow = !Module.IsEditable;
            var now = HttpContext.Current.Timestamp;

            // REVIEW: Should 'now' value be used in the cache key? 
            // var today = DateTime.Today; 
            // var now = today + new TimeSpan (today.Hour, 0, 0);

            // REVIEW: Check for sorting options also
            if (pageIndex == 0 && pageSize == Settings.PageSize) {
                
                // we cache viewmodels for first page, so there are no need to implement caching
                // in the GetNewsEntries..._Count and GetNewsEntries..._FirstPage repository methods

                var cacheKey = NewsRepository.NewsCacheKeyPrefix + "ModuleId=" + Module.ModuleId
                               + "&PageIndex=0&PageSize=" + pageSize + "&CheckNow=" + !Module.IsEditable;
                
                return DataCache.GetCachedData<StreamNewsEntryViewModelPage> (
                    new CacheItemArgs (cacheKey, NewsConfig.Instance.DataCacheTime, CacheItemPriority.Normal),
                    c => GetFirstPageInternal (pageSize, checkNow, now)
                );
            }

            return GetPageInternal (pageIndex, pageSize, checkNow, now);
        }

        protected StreamNewsEntryViewModelPage GetFirstPageInternal (int pageSize, bool checkNow, DateTime now)
        {
            IEnumerable<NewsEntryInfo> baseItems;
            int baseItemsCount;

            if (Settings.ShowAllNews) {
                baseItemsCount = NewsRepository.Instance.GetNewsEntries_Count (
                    Module.PortalId, checkNow, now,
                    Settings.MinThematicWeight, Settings.MaxThematicWeight, 
                    Settings.MinStructuralWeight, Settings.MaxStructuralWeight
                );

                baseItems = NewsRepository.Instance.GetNewsEntries_FirstPage (
                    Module.PortalId, pageSize, checkNow, now,
                    Settings.MinThematicWeight, Settings.MaxThematicWeight, 
                    Settings.MinStructuralWeight, Settings.MaxStructuralWeight
                );
            }
            else {
                baseItemsCount = NewsRepository.Instance.GetNewsEntriesByTerms_Count (
                    Module.PortalId, checkNow, now,
                    Settings.MinThematicWeight, Settings.MaxThematicWeight, 
                    Settings.MinStructuralWeight, Settings.MaxStructuralWeight,
                    Settings.IncludeTerms
                );

                baseItems = NewsRepository.Instance.GetNewsEntriesByTerms_FirstPage (
                    Module.PortalId, pageSize, checkNow, now,
                    Settings.MinThematicWeight, Settings.MaxThematicWeight, 
                    Settings.MinStructuralWeight, Settings.MaxStructuralWeight,
                    Settings.IncludeTerms
                );
            } 

            return new StreamNewsEntryViewModelPage (
                baseItemsCount,
                baseItems.Select (ne => new StreamNewsEntryViewModel (ne, this))
                .ToList ()
            );
        }

        protected StreamNewsEntryViewModelPage GetPageInternal (
            int pageIndex,
            int pageSize,
            bool checkNow,
            DateTime now)
        {
            IEnumerable<NewsEntryInfo> baseItems;

            // check for pageIndex < 0
            if (pageIndex < 0) {
                return StreamNewsEntryViewModelPage.Empty;
            }

            if (Settings.ShowAllNews) {
                baseItems = NewsRepository.Instance.GetNewsEntries (Module.ModuleId, Module.PortalId,
                    Settings.MinThematicWeight, Settings.MaxThematicWeight, 
                    Settings.MinStructuralWeight, Settings.MaxStructuralWeight
                );
            }
            else {
                baseItems = NewsRepository.Instance.GetNewsEntriesByTerms (Module.ModuleId, Module.PortalId,
                    Settings.MinThematicWeight, Settings.MaxThematicWeight, 
                    Settings.MinStructuralWeight, Settings.MaxStructuralWeight,
                    Settings.IncludeTerms
                );
            }

            // check for no data available
            if (baseItems == null || !baseItems.Any ()) {
                return StreamNewsEntryViewModelPage.Empty;
            }

            // get only published items
            IList<NewsEntryInfo> items = baseItems
                .Where (ne => !checkNow || ne.IsPublished (now))
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

            return new StreamNewsEntryViewModelPage (
                totalItems,
                items.OrderByDescending (ne => ne.PublishedOnDate ())
                    .Skip (pageIndex * pageSize)
                    .Take (pageSize)
                    .Select (ne => new StreamNewsEntryViewModel (ne, this))
                    .ToList ()
            );
        }
    }
}
