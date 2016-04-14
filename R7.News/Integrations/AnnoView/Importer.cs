//
//  Importer.cs
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
using DotNetNuke.Common;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.FileSystem;
using R7.News.Data;
using R7.News.Components;

namespace R7.News.Integrations.AnnoView
{
    public class Importer
    {
        /// <summary>
        /// Imports news from AnnoView modules on specifed portal to R7.News
        /// </summary>
        /// <param name="moduleId">Module identifier.</param>
        /// <param name="tabId">Tab identifier.</param>
        /// <param name="portalId">Portal identifier.</param>
        public int Import (int moduleId, int tabId, int portalId)
        {
            var itemsImported = 0;

            var announcements = NewsDataProvider.Instance.GetObjects<AnnouncementInfo> ();
            if (announcements != null) {
                
                var moduleController = new ModuleController ();
                var tabController = new TabController ();
                var termController = new TermController ();

                foreach (var announcement in announcements) {
                    var module = moduleController.GetModule (announcement.ModuleId);
                    if (module.PortalID == portalId) {

                        // fill news entry
                        var newsEntry = new NewsEntryInfo {
                            Title = announcement.Title,
                            Description = announcement.Description,
                            StartDate = announcement.PublishDate,
                            EndDate = announcement.ExpireDate,
                            Url = announcement.Url,
                            PortalId = portalId,
                            ThematicWeight = NewsConfig.GetInstance (portalId).NewsEntry.MaxWeight,
                            StructuralWeight = NewsConfig.GetInstance (portalId).NewsEntry.MaxWeight
                        };
                            
                        // fill image
                        var images = new List<IFileInfo> ();
                        if (Globals.GetURLType (announcement.ImageSource) == TabType.File) {
                            var imageFileId = int.Parse (announcement.ImageSource.Substring (announcement.ImageSource.IndexOf ("=") + 1));
                            var image = FileManager.Instance.GetFile (imageFileId);
                            if (image != null) {
                                images.Add (image);
                            }
                        }

                        // try get tab
                        TabInfo tab;
                        if (Globals.GetURLType (announcement.Url) == TabType.Tab) {
                            // get link target tab
                            tab = tabController.GetTab (int.Parse (announcement.Url), portalId);
                        }
                        else {
                            // get module tab
                            tab = tabController.GetTab (module.TabID, portalId);
                        }

                        // fill terms
                        var terms = new List<Term> ();
                        if (tab != null) {
                            terms = termController.GetTermsByContent (tab.ContentItemId).ToList ();
                        }

                        // create news entry
                        NewsRepository.Instance.AddNewsEntry (newsEntry, terms, images, moduleId, tabId);
                        itemsImported++;
                    }
                }
            }
        
            return itemsImported;
        }
    }
}

