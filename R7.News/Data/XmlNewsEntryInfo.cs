//
//  XmlNewsEntryInfo.cs
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
using System.Xml.Serialization;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Modules;
using R7.News.Models;

namespace R7.News.Data
{
    /// <summary>
    /// XML serialization adapter for NewsEntryInfo
    /// </summary>
    [Serializable]
    public class XmlNewsEntryInfo: INewsEntry
    {
        protected NewsEntryInfo @This;

        public XmlNewsEntryInfo ()
        {
            @This = new NewsEntryInfo ();
        }

        public XmlNewsEntryInfo (NewsEntryInfo newsEntry)
        {
            @This = newsEntry;
        }

        public NewsEntryInfo GetNewsEntryInfo ()
        {
            return @This;
        }

        #region INewsEntry implementation

        // then deserializing, new entry should be created
        public int EntryId
        {
            get { return @This.EntryId; }
            set { @This.EntryId = value; }
        }

        // then deserializing, PortalId should be set to target portal Id
        public int PortalId
        {
            get { return @This.PortalId; }
            set { @This.PortalId = value; }
        }

        // then deserializing, new content item should be created
        public int ContentItemId
        {
            get { return @This.ContentItemId; }
            set { @This.ContentItemId = value; }
        }

        // then deserializing, AgentModuleId should be set to agent's module id
        public int? AgentModuleId
        {
            get { return @This.AgentModuleId; }
            set { @This.AgentModuleId = value; }
        }

        public string Url
        {
            get { return @This.Url; }
            set { @This.Url = value; }
        }

        public DateTime? StartDate
        {
            get { return @This.StartDate; }
            set { @This.StartDate = value; }
        }

        public DateTime? EndDate
        {
            get { return @This.EndDate; }
            set { @This.EndDate = value; }
        }

        public DateTime? ThresholdDate
        {
            get { return @This.ThresholdDate; }
            set { @This.ThresholdDate = value; }
        }

        public DateTime? DueDate
        {
            get { return @This.DueDate; }
            set { @This.DueDate = value; }
        }

        public string Title
        {
            get { return @This.Title; }
            set { @This.Title = value; }
        }

        public string Description
        {
            get { return @This.Description; }
            set { @This.Description = value; }
        }

        public int SortIndex
        {
            get { return @This.SortIndex; }
            set { @This.SortIndex = value; }
        }

        public bool IsSticky
        {
            get { return @This.IsSticky; }
            set { @This.IsSticky = value; }
        }

        public int ThematicWeight
        {
            get { return @This.ThematicWeight; }
            set { @This.ThematicWeight = value; }
        }

        public int StructuralWeight
        {
            get { return @This.StructuralWeight; }
            set { @This.StructuralWeight = value; }
        }
       
        [XmlIgnore]
        public ContentItem ContentItem
        {
            get { return @This.ContentItem; }
            set { @This.ContentItem = value; }
        }

        [XmlIgnore]
        public ModuleInfo AgentModule
        {
            get { return @This.AgentModule; }
            set { @This.AgentModule = value; }
        }

        [XmlIgnore]
        public ICollection<INewsEntry> Group
        {
            get { return @This.Group; }
            set { @This.Group = value; }
        }

        #endregion

        // Terms and image files should be serialized as their ids.
        // Then deserializing, terms and files with those ids should exist -
        // if some are not, we should skip those items.

        private List<int> termIds;

        private List<int> imageFileIds;

        /// <summary>
        /// Gets the news entry terms.
        /// </summary>
        /// <value>The terms.</value>
        public List<int> TermIds
        {
            get 
            {
                if (termIds == null) {
                    if (ContentItem != null) {
                        // on export
                        termIds = ContentItem.Terms.Select (t => t.TermId).ToList ();
                    }
                    else {
                        // on import
                        termIds = new List<int> ();
                    }
                }

                return termIds;
            }
           
            set { termIds = value; }
        }

        /// <summary>
        /// Gets the news entry image file ids.
        /// </summary>
        /// <value>The terms.</value>
        public List<int> ImageFileIds
        {
            get 
            {
                if (imageFileIds == null) {
                    if (ContentItem != null) {
                        // on export
                        imageFileIds = ContentItem.Images.Select (i => i.FileId).ToList ();
                    }
                    else {
                        // on import
                        imageFileIds = new List<int> ();
                    }
                }

                return imageFileIds;
            }

            set { imageFileIds = value; }
        }
    }
}

