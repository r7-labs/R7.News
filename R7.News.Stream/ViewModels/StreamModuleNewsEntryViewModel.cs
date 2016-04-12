//
//  StreamModuleNewsEntryViewModel.cs
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
using DotNetNuke.Common;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Modules;
using DotNetNuke.R7;
using DotNetNuke.R7.ViewModels;
using DotNetNuke.Services.Localization;
using R7.News.Controls;
using R7.News.Models;
using R7.News.Stream.Components;

namespace R7.News.Stream.ViewModels
{
    public class StreamModuleNewsEntryViewModel: INewsEntry
    {
        protected ViewModelContext<StreamSettings> Context;

        protected INewsEntry NewsEntry;

        public StreamModuleNewsEntryViewModel (INewsEntry newsEntry, ViewModelContext<StreamSettings> context)
        {
            NewsEntry = newsEntry;
            Context = context;
        }

        #region INewsEntry implementation

        public int EntryId
        {
            get { return NewsEntry.EntryId; }
            set {}
        }

        public int PortalId
        {
            get { return NewsEntry.PortalId; }
            set {}
        }

        public int ContentItemId
        {
            get { return NewsEntry.ContentItemId; }
            set {}
        }

        public int? AgentModuleId
        {
            get { return NewsEntry.AgentModuleId; }
            set {}
        }

        public string Url
        {
            get { return NewsEntry.Url; }
            set {}
        }

        public DateTime? StartDate
        {
            get { return NewsEntry.StartDate; }
            set {}
        }

        public DateTime? EndDate
        {
            get { return NewsEntry.EndDate; }
            set {}
        }

        public DateTime? ThresholdDate
        {
            get { return NewsEntry.ThresholdDate; }
            set {}
        }

        public DateTime? DueDate
        {
            get { return NewsEntry.DueDate; }
            set {}
        }

        public string Title
        {
            get { return NewsEntry.Title; }
            set {}
        }

        public string Description
        {
            get { return NewsEntry.Description; }
            set {}
        }

        public int ThematicWeight
        { 
            get { return NewsEntry.ThematicWeight; }
            set { }
        }

        public int StructuralWeight
        { 
            get { return NewsEntry.StructuralWeight; }
            set { }
        }

        public ContentItem ContentItem
        {
            get { return NewsEntry.ContentItem; }
            set {}
        }

        public ModuleInfo AgentModule
        {
            get { return NewsEntry.AgentModule; }
            set {}
        }

        public ICollection<INewsEntry> Group
        {
            get { return NewsEntry.Group; }
            set {}
        }

        #endregion

        public string ImageUrl
        {
            get { return NewsEntry.GetImageUrl (width: Context.Settings.ThumbnailWidth); }
        }

        public string TitleLink
        {
            get
            { 
                string url = null;
                if (!string.IsNullOrWhiteSpace (Url)) {
                    url = Url;
                }
                else if (AgentModule != null) {
                    url = AgentModule.TabID.ToString ();
                }

                if (!string.IsNullOrEmpty (url)) {
                    return string.Format ("<a href=\"{0}\">{1}</a>", 
                        Globals.LinkClick (url, Context.Module.TabId,  Context.Module.ModuleId),
                        Title);
                }

                return Title;
            }
        }

        public string PublishedOnDateString
        {
            get { 
                return this.PublishedOnDate ().ToString (
                    Localization.GetString ("PublishedOnDate.Format", Context.LocalResourceFile));
            }
        }

        public string CreatedByUserName
        {
            get 
            {
                var user = ContentItem.CreatedByUser (Context.Module.PortalId);
                if (user != null) {
                    return user.DisplayName;        
                }

                return string.Empty;
            }
        }

        public List<Badge> Badges
        {
            get
            { 
                if (Context.Module.IsEditable) {
                    var badges = new List<Badge> ();
                    var now = DateTime.Now;

                    if (!NewsEntry.IsPublished (now)) {
                        if (NewsEntry.HasBeenExpired ()) {
                            badges.Add (new Badge {
                                CssClass = "expired",
                                Text = string.Format (Localization.GetString (
                                    "Visibility_Expired.Format", Context.LocalResourceFile), NewsEntry.EndDate)
                            });
                        }
                        else {
                            badges.Add (new Badge {
                                CssClass = "not-published",
                                Text = string.Format (Localization.GetString (
                                    "Visibility_NotPublished.Format", Context.LocalResourceFile), NewsEntry.StartDate)
                            });
                        }
                    }

                    return badges;
                }

                return null;
            }
        }
    }
}

