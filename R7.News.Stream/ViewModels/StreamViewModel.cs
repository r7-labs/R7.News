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
using System.Web.Caching;
using DotNetNuke.Common.Utilities;
using DotNetNuke.UI.Modules;
using R7.News.Components;
using R7.News.Data;
using R7.News.Models;
using R7.News.Stream.Components;
using R7.News.ViewModels;

namespace R7.News.Stream.ViewModels
{
    public class StreamViewModel: ViewModelContext<StreamSettings>
    {
        public StreamViewModel (IModuleControl module, StreamSettings settings): base (module, settings)
        {
        }

        public StreamModuleNewsEntryViewModelPage GetPage (int pageIndex, int pageSize)
        {
            if (!Module.IsEditable && pageIndex == 0 && pageSize == Settings.PageSize) {
                var cacheKey = NewsRepository.NewsCacheKeyPrefix + "ModuleId=" + Module.ModuleId
                    + "&PageIndex=0&PageSize=" + pageSize;
                
                return DataCache.GetCachedData<StreamModuleNewsEntryViewModelPage> (
                    new CacheItemArgs (cacheKey, NewsConfig.Instance.DataCacheTime, CacheItemPriority.Normal),
                    c => GetPageInternal (pageIndex, pageSize)
                );
            }

            return GetPageInternal (pageIndex, pageSize);
        }

        protected StreamModuleNewsEntryViewModelPage GetPageInternal (int pageIndex, int pageSize)
        {
            IEnumerable<NewsEntryInfo> baseItems;

            // check for pageIndex < 0
            if (pageIndex < 0) {
                return StreamModuleNewsEntryViewModelPage.Empty;
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
                return StreamModuleNewsEntryViewModelPage.Empty;
            }

            // get only published items
            IList<NewsEntryInfo> items = baseItems
                .Where (ne => (ne.IsPublished () && ne.IsVisible (
                    Settings.MinThematicWeight, Settings.MaxThematicWeight, 
                    Settings.MinStructuralWeight, Settings.MaxStructuralWeight)) || Module.IsEditable)
                .ToList ();
            
            // check for no data available
            var totalItems = items.Count;
            if (totalItems == 0) {
                return StreamModuleNewsEntryViewModelPage.Empty;
            }

            // check for pageIndex > totalPages
            var totalPages = totalItems / pageSize + ((totalItems % pageSize == 0) ? 0 : 1);
            if (pageIndex > totalPages) {
                return StreamModuleNewsEntryViewModelPage.Empty;
            }

            return new StreamModuleNewsEntryViewModelPage (
                totalItems,
                items.OrderByDescending (ne => ne.PublishedOnDate ())
                    .Skip (pageIndex * pageSize)
                    .Take (pageSize)
                    .Select (ne => new StreamModuleNewsEntryViewModel (ne, this))
                    .ToList ()
            );
        }

    }
}

