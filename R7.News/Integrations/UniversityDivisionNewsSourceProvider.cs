//
//  UniversityDivisionNewsSourceProvider.cs
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
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using R7.News.Models;
using R7.News.Models.Data;
using R7.News.Components;

namespace R7.News.Integrations
{
    // TODO: Move to separate assembly?
    [TableName ("University_Divisions")]
    [PrimaryKey ("DivisionID", AutoIncrement = false)]
    public class UniversityDivisionInfo
    {
        public int DivisionID { get; set; }

        public string Title { get; set; }
            
        public string HomePage { get; set; }
    }

    public class UniversityDivisionNewsSourceProvider: INewsSourceProvider
    {
        private const string cacheKey = "r7_NewsSources_UniversityDivisions";

        #region INewsSourceProvider implementation

        public INewsSource GetSelfSource ()
        {
            return NewsDataProvider.Instance.GetObjects<NewsSourceInfo> (
                "WHERE Type = N'R7.News.Integrations.UniversityDivisionNewsSourceProvider'").Single ();
        }

        public INewsSource GetSource (int? sourceItemId)
        {
            if (sourceItemId != null) {
                return DataCache.GetCachedData<IEnumerable<INewsSource>> (
                    new CacheItemArgs (cacheKey, NewsConfig.Instance.DataCacheTime, CacheItemPriority.Normal),
                    c => GetSourcesInternal ()
                ).SingleOrDefault (ns => ns.SourceItemId == sourceItemId);
            }

            return null;
        }

        public IEnumerable<INewsSource> GetSources ()
        {
            return DataCache.GetCachedData<IEnumerable<INewsSource>> (
                new CacheItemArgs (cacheKey, NewsConfig.Instance.DataCacheTime, CacheItemPriority.Normal),
                c => GetSourcesInternal ()
            );
        }

        protected IEnumerable<INewsSource> GetSourcesInternal ()
        {
            return NewsDataProvider.Instance.GetObjects <UniversityDivisionInfo> ()
                .Select (d => new NewsSourceInfo {
                    SourceItemId = d.DivisionID,
                    Title = d.Title,
                    Url = d.HomePage
                });
        }

        #endregion
    }
}

