//
//  Importer.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016 Roman M. Yagodin
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
using System.Threading;
using DotNetNuke.Common;
using DotNetNuke.Data;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Scheduling;
using R7.News.Components;
using R7.News.Data;

namespace R7.News.Integrations.AnnoView
{
    public class Importer: SchedulerClient
    {
        public Importer (ScheduleHistoryItem oItem)
        {
            ScheduleHistoryItem = oItem;
        }

        public override void DoWork ()
        {
            try {
                // perform required items for logging
                Progressing ();

                var itemsImported = Import (1000);

                // log result
                ScheduleHistoryItem.AddLogNote ("Items imported: " + itemsImported);

                // show success
                ScheduleHistoryItem.Succeeded = true;
                Completed ();
            }
            catch (Exception ex) {
                ScheduleHistoryItem.Succeeded = false;
                ScheduleHistoryItem.AddLogNote ("Exception: " + ex);
                Errored (ref ex);
                Exceptions.LogException (ex);
            }
        }

        /// <summary>
        /// Imports news from AnnoView modules to R7.News
        /// </summary>
        /// <param name="sleepTimeout">Sleep timeout.</param>
        protected int Import (int sleepTimeout)
        {
            var itemsImported = 0;
            var itemsImportedDiv10 = 0;

            var announcements = NewsDataProvider.Instance.GetObjects<AnnouncementInfo> ();
            if (announcements != null) {
                
                var moduleController = new ModuleController ();
                var tabController = new TabController ();
                var termController = new TermController ();

                using (var dc = DataContext.Instance ()) {
                    var repository = dc.GetRepository<NewsEntryInfo> ();
                    
                    foreach (var announcement in announcements) {
                        var module = moduleController.GetModule (announcement.ModuleId);
                        if (module != null) {
                            try {
                                // fill news entry
                                var newsEntry = new NewsEntryInfo {
                                    Title = announcement.Title,
                                    Description = announcement.Description,
                                    StartDate = announcement.PublishDate,
                                    EndDate = announcement.ExpireDate,
                                    Url = announcement.Url,
                                    PortalId = module.PortalID,
                                    ThematicWeight = (announcement.Export) ? NewsConfig.GetInstance (module.PortalID).NewsEntry.MaxWeight : 0,
                                    StructuralWeight = (announcement.Export) ? NewsConfig.GetInstance (module.PortalID).NewsEntry.MaxWeight : 0
                                };

                                // add news entry
                                NewsRepository.Instance.BulkAddNewsEntry (
                                    repository,
                                    newsEntry,
                                    GetTerms (announcement, module, tabController, termController),
                                    GetImages (announcement),
                                    module.ModuleID,
                                    module.TabID);

                                // count number of items imported,
                                // invoke Progressing method each 10 items
                                itemsImported++;
                                if (itemsImported / 10 > itemsImportedDiv10) {
                                    itemsImportedDiv10++;
                                    Progressing ();
                                }

                                Thread.Sleep (sleepTimeout);
                            }
                            catch (Exception ex) {
                                // log error on importing current announcement
                                ScheduleHistoryItem.AddLogNote ("Announcement.ItemId:" +  announcement.ItemId + "; Exception:" + ex);
                                Exceptions.LogException (ex);
                            }
                        }
                    }
                }
            }
        
            return itemsImported;
        }

        static List<IFileInfo> GetImages (AnnouncementInfo announcement)
        {
            var images = new List<IFileInfo> ();
            if (Globals.GetURLType (announcement.ImageSource) == TabType.File) {
                var imageFileId = int.Parse (announcement.ImageSource.Substring (announcement.ImageSource.IndexOf ("=") + 1));
                var image = FileManager.Instance.GetFile (imageFileId);
                if (image != null) {
                    images.Add (image);
                }
            }

            return images;
        }

        static List<Term> GetTerms (AnnouncementInfo announcement, ModuleInfo module, TabController tabController, TermController termController)
        {
            var terms = new List<Term> ();

            // try get terms from target tab
            if (Globals.GetURLType (announcement.Url) == TabType.Tab) {
                var tab = tabController.GetTab (int.Parse (announcement.Url), module.PortalID);
                if (tab != null) {
                    terms = termController.GetTermsByContent (tab.ContentItemId).ToList ();
                }
            }

            // try get terms from module tab
            if (terms == null || terms.Count == 0) {
                var tab = tabController.GetTab (module.TabID, module.PortalID);
                if (tab != null) {
                    terms = termController.GetTermsByContent (tab.ContentItemId).ToList ();
                }
            }

            return terms;
        }
   }
}

