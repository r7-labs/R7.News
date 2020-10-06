//
//  TabSynchronizer.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2019 Roman M. Yagodin
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
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.FileSystem;
using R7.News.Components;
using R7.News.Models;

namespace R7.News.Data
{
    public class TabSynchronizer
    {
        public void UpdateTabFromNewsEntry (TabInfo activeTab, INewsEntry newsEntry)
        {
            activeTab.TabName = HtmlUtils.Shorten (newsEntry.Title, 199, "");
            activeTab.Title = HtmlUtils.Shorten (newsEntry.Title, 199, "\u2026");
            activeTab.Description = HtmlUtils.Shorten (
                HttpUtility.HtmlDecode (HtmlUtils.StripTags (HttpUtility.HtmlDecode (newsEntry.Description), true)).Trim (),
                499, "\u2026"
            );

            activeTab.StartDate = newsEntry.StartDate.GetValueOrDefault ();
            activeTab.EndDate = (newsEntry.EndDate != null) ? newsEntry.EndDate.Value : DateTime.MaxValue;

            var tabCtrl = new TabController ();
            tabCtrl.UpdateTab (activeTab);

            var termCtrl = new TermController ();
            termCtrl.RemoveTermsFromContent (activeTab);
            foreach (var term in newsEntry.ContentItem.Terms) {
                termCtrl.AddTermToContent (term, activeTab);
            }
        }

        public INewsEntry AddNewsEntryFromTabData (TabInfo activeTab, int moduleId)
        {
            // add default news entry based on tab data
            var newsEntry = new NewsEntry {
                Title = activeTab.TabName,
                Description = HttpUtility.HtmlEncode ("<p>" + activeTab.Description + "</p>"),
                AgentModuleId = moduleId,
                PortalId = activeTab.PortalID,
                StartDate = (activeTab.StartDate == default (DateTime) || activeTab.StartDate == DateTime.MaxValue) ? null : (DateTime?) activeTab.StartDate,
                // if no end date is set, make news entry expired by default
                EndDate = (activeTab.EndDate == default (DateTime) || activeTab.EndDate == DateTime.MaxValue) ? DateTime.Today : activeTab.EndDate,
                ThematicWeight = NewsConfig.Instance.NewsEntry.DefaultThematicWeight,
                StructuralWeight = NewsConfig.Instance.NewsEntry.DefaultStructuralWeight
            };

            // add news entry
            NewsRepository.Instance.AddNewsEntry (newsEntry, activeTab.Terms, new List<IFileInfo> (), moduleId, activeTab.TabID);

            return newsEntry;
        }

    }
}
