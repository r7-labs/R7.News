//
//  AgentController.cs
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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search.Entities;
using R7.News.Data;
using R7.News.Models;

namespace R7.News.Agent.Components
{
    public partial class AgentController : ModuleSearchBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="R7.News.Agent.Components.AgentController"/> class.
        /// </summary>
        public AgentController () : base ()
        {
        }

        #region ModuleSearchBase implementaion

        public override IList<SearchDocument> GetModifiedSearchDocuments (ModuleInfo moduleInfo, DateTime beginDateUtc)
        {
            var searchDocs = new List<SearchDocument> ();

            // get news entries
            var newsEntries = NewsRepository.Instance.GetNewsEntriesByAgent (moduleInfo.ModuleID, moduleInfo.PortalID);

            // create search documents
            foreach (var newsEntry in newsEntries)
            {
                var now = DateTime.Now;
                if (newsEntry.ContentItem.LastModifiedOnDate.ToUniversalTime () > beginDateUtc.ToUniversalTime ())
                {
                    searchDocs.Add (new SearchDocument {
                        PortalId = moduleInfo.PortalID,
                        AuthorUserId = newsEntry.ContentItem.CreatedByUserID,
                        Title = newsEntry.Title,
                        // Description = HtmlUtils.Shorten (...);
                        Body = HtmlUtils.ConvertToText (newsEntry.Description),
                        Tags = newsEntry.ContentItem.Terms.Select (t => t.Name),
                        ModifiedTimeUtc = newsEntry.ContentItem.LastModifiedOnDate.ToUniversalTime (),
                        UniqueKey = string.Format ("r7_News_{0}", newsEntry.EntryId),
                        Url = string.Format ("/Default.aspx?tabid={0}#{1}", moduleInfo.TabID, moduleInfo.ModuleID),
                        IsActive = newsEntry.IsPublished (now)
                    });
                }
            }

            return searchDocs;
        }
        #endregion
    }
}

