//
//  AgentController.cs
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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Search.Entities;
using R7.News.Data;
using R7.News.Models;
using R7.News.Components;

namespace R7.News.Agent.Components
{
    public class AgentController : ModuleSearchBase, IPortable
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
            foreach (var newsEntry in newsEntries) {
                var now = DateTime.Now;
                if (newsEntry.ContentItem.LastModifiedOnDate.ToUniversalTime () > beginDateUtc.ToUniversalTime ()) {
                    searchDocs.Add (new SearchDocument {
                        PortalId = moduleInfo.PortalID,
                        AuthorUserId = newsEntry.ContentItem.CreatedByUserID,
                        Title = newsEntry.Title,
                        // Description = HtmlUtils.Shorten (...);
                        Body = HtmlUtils.ConvertToText (newsEntry.Description),
                        Tags = newsEntry.ContentItem.Terms.Select (t => t.Name),
                        ModifiedTimeUtc = newsEntry.ContentItem.LastModifiedOnDate.ToUniversalTime (),
                        UniqueKey = string.Format (Const.Prefix + "_{0}", newsEntry.EntryId),
                        Url = string.Format ("/Default.aspx?tabid={0}#{1}", moduleInfo.TabID, moduleInfo.ModuleID),
                        IsActive = newsEntry.IsPublished (now)
                    });
                }
            }

            return searchDocs;
        }

        #endregion

        #region IPortable implementation

        public string ExportModule (int ModuleID)
        {
            XmlWriter xmlWriter = null;

            var moduleController = new ModuleController ();
            var module = moduleController.GetModule (ModuleID);

            try {
                var xml = new StringBuilder ();
                var xmlSerializer = new XmlSerializer (typeof (List<XmlNewsEntryInfo>));

                // get news entries
                var newsEntries = NewsRepository.Instance.GetNewsEntriesByAgent (module.ModuleID, module.PortalID)
                    .Select (ne => new XmlNewsEntryInfo (ne)).ToList ();

                if (newsEntries != null) {
                    var xmlSettings = new XmlWriterSettings ();
                    using (xmlWriter = XmlWriter.Create (xml, xmlSettings)) {
                        xmlSerializer.Serialize (xmlWriter, newsEntries);
                        xmlWriter.Close ();
                    }
                }

                return xml.ToString ();
            }
            catch (Exception ex) {
                var logController = new EventLogController ();

                var logInfo = new LogInfo {
                    Exception = new ExceptionInfo (ex),
                    LogTypeKey = EventLogController.EventLogType.ADMIN_ALERT.ToString (),
                    LogUserID = -1, // superuser
                    LogPortalID = module.PortalID
                };

                logInfo.AddProperty ("R7.News.Agent", "Cannot export module");
                logController.AddLog (logInfo);
            }
            finally {
                if (xmlWriter != null) {
                    xmlWriter.Close ();
                }
            }

            return string.Empty;
        }

        public void ImportModule (int ModuleID, string Content, string Version, int UserID)
        {
            XmlReader xmlReader = null;

            var moduleController = new ModuleController ();
            var module = moduleController.GetModule (ModuleID);

            try {
                var xmlSerializer = new XmlSerializer (typeof (List<XmlNewsEntryInfo>));
                using (xmlReader = XmlReader.Create (new StringReader (Content))) {
                    // deserialize
                    var xmlNewsEntries = (List<XmlNewsEntryInfo>) xmlSerializer.Deserialize (xmlReader);
                    xmlReader.Close ();

                    var termController = new TermController ();

                    // add news entries
                    foreach (var xmlNewsEntry in xmlNewsEntries) {
                        // get news entry and reset ids
                        var newsEntry = xmlNewsEntry.GetNewsEntryInfo ();
                        newsEntry.EntryId = 0;
                        newsEntry.AgentModuleId = ModuleID;
                        newsEntry.ContentItemId = 0;
                        newsEntry.PortalId = module.PortalID;

                        // get terms by ids
                        var terms = new List<Term> ();
                        foreach (var termId in xmlNewsEntry.TermIds) {
                            var term = termController.GetTerm (termId);
                            if (term != null) {
                                terms.Add (term);
                            }
                        }

                        // get images by ids
                        var images = new List<IFileInfo> ();
                        foreach (var imageFileId in xmlNewsEntry.ImageFileIds) {
                            var image = FileManager.Instance.GetFile (imageFileId);
                            if (image != null) {
                                images.Add (image);
                            }
                        }

                        // add news entry
                        NewsRepository.Instance.AddNewsEntry (newsEntry, terms, images,
                            ModuleID, module.TabID);
                    }
                }
            }
            catch (Exception ex) {
                var logController = new EventLogController ();

                var logInfo = new LogInfo {
                    Exception = new ExceptionInfo (ex),
                    LogTypeKey = EventLogController.EventLogType.ADMIN_ALERT.ToString (),
                    LogUserID = -1, // superuser
                    LogPortalID = module.PortalID
                };

                logInfo.AddProperty ("R7.News.Agent", "Cannot import module");
                logController.AddLog (logInfo);
            }
            finally {
                if (xmlReader != null) {
                    xmlReader.Close ();
                }
            }
        }

        #endregion
    }
}

