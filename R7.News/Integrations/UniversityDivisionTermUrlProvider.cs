//
//  UniversityDivisionTermUrlProvider.cs
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
using System.Collections;
using System.Collections.Generic;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Common.Utilities;
using System.Web.Caching;
using R7.News.Components;
using R7.News.Providers;
using R7.News.Data;

namespace R7.News.Integrations
{
    public class UniversityDivisionTermUrlProvider: ITermUrlProvider
    {
        private const string cacheKey = Const.Prefix + "_TermUrls_UniversityDivisions";

        #region ITermUrlProvider implementation

        public string GetUrl (Term term)
        {
            var division = GetDivisions ()
                .FirstOrDefault (d => d.DivisionTermId == term.TermId);

            if (division != null) {
                return division.HomePage;
            }

            return string.Empty;
        }

        public string GetUrl (int termId, TermController termController)
        {
            var division = GetDivisions ()
                .FirstOrDefault (d => d.DivisionTermId == termId);

            if (division != null) {
                return division.HomePage;
            }

            return string.Empty;
        }

        #endregion

        protected IEnumerable<UniversityDivisionInfo> GetDivisions ()
        {
            return DataCache.GetCachedData<IEnumerable<UniversityDivisionInfo>> (
                new CacheItemArgs (cacheKey, NewsConfig.Instance.DataCacheTime, CacheItemPriority.Normal),
                c => GetDivisionsInternal ());
        }

        private IEnumerable<UniversityDivisionInfo> GetDivisionsInternal ()
        {
            return NewsDataProvider.Instance.GetObjects<UniversityDivisionInfo> ();
        }
    }
}

