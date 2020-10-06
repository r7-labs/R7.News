//
//  StreamController.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016-2019 Roman M. Yagodin
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
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Search.Entities;
using R7.News.Components;
using R7.News.Data;
using R7.News.Models;
using R7.News.Stream.Models;

namespace R7.News.Stream.Components
{
    public class StreamController : ModuleSearchBase
    {
        // TODO: Move to NewsRepository
        protected IEnumerable<NewsEntry> GetNewsEntries (int moduleId,
                                                             int portalId,
                                                             WeightRange thematicWeights,
                                                             WeightRange structuralWeights,
                                                             bool showAllNews,
                                                             List<Term> includeTerms)
        {
            if (showAllNews) {
                return NewsRepository.Instance.GetAllNewsEntries (
                    moduleId, portalId, thematicWeights, structuralWeights
                );
            }

            return NewsRepository.Instance.GetNewsEntriesByTerms (
                moduleId, portalId, thematicWeights, structuralWeights, includeTerms
            );
        }

        #region ModuleSearchBase implementaion

        public override IList<SearchDocument> GetModifiedSearchDocuments (ModuleInfo moduleInfo, DateTime beginDateUtc)
        {
            var searchDocs = new List<SearchDocument> ();
            var settings = new StreamSettingsRepository ().GetSettings (moduleInfo);

            // get news entries
            var newsEntries = GetNewsEntries (
                moduleInfo.ModuleID, moduleInfo.PortalID,
                new WeightRange (settings.MinThematicWeight, settings.MaxThematicWeight),
                new WeightRange (settings.MinStructuralWeight, settings.MaxStructuralWeight),
                settings.ShowAllNews, settings.IncludeTerms
            );

            var portalAlias = PortalAliasController.Instance.GetPortalAliasesByPortalId (moduleInfo.PortalID).First (pa => pa.IsPrimary);
            var portalSettings = new PortalSettings (moduleInfo.TabID, portalAlias);

            // create search documents
            foreach (var newsEntry in newsEntries) {
                var now = DateTime.Now;
                if (newsEntry.AgentModuleId == null // get only news entries w/o agent modules
                    && newsEntry.ContentItem.LastModifiedOnDate.ToUniversalTime () > beginDateUtc.ToUniversalTime ()) {
                    searchDocs.Add (new SearchDocument {
                        PortalId = moduleInfo.PortalID,
                        AuthorUserId = newsEntry.ContentItem.CreatedByUserID,
                        Title = newsEntry.Title,
                        Body = HtmlUtils.StripTags (HttpUtility.HtmlDecode (newsEntry.Description), false),
                        Tags = newsEntry.ContentItem.Terms.Select (t => t.Name),
                        ModifiedTimeUtc = newsEntry.ContentItem.LastModifiedOnDate.ToUniversalTime (),
                        UniqueKey = string.Format (Const.Prefix + "_{0}", newsEntry.EntryId),
                        Url = Globals.NavigateURL (moduleInfo.TabID, portalSettings, "", null),
                        IsActive = newsEntry.IsPublished (now)
                    });
                }
            }

            return searchDocs;
        }

        #endregion
    }
}

