//
//  NewsEntryViewModelBase.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016-2017 Roman M. Yagodin
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
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using R7.DotNetNuke.Extensions.ViewModels;
using R7.News.Components;
using R7.News.Controls;
using R7.News.Data;
using R7.News.Models;

namespace R7.News.ViewModels
{
    public class NewsEntryViewModelBase: INewsEntry
    {
        protected ViewModelContext Context;

        protected INewsEntry NewsEntry;

        public NewsEntryViewModelBase (INewsEntry newsEntry, ViewModelContext context)
        {
            NewsEntry = newsEntry;
            Context = context;
        }

        #region INewsEntry implementation

        public int EntryId
        {
            get { return NewsEntry.EntryId; }
            set { throw new InvalidOperationException (); }
        }

        public int PortalId
        {
            get { return NewsEntry.PortalId; }
            set { throw new InvalidOperationException (); }
        }

        public int ContentItemId
        {
            get { return NewsEntry.ContentItemId; }
            set { throw new InvalidOperationException (); }
        }

        public int? AgentModuleId
        {
            get { return NewsEntry.AgentModuleId; }
            set { throw new InvalidOperationException (); }
        }

        public string Url
        {
            get { return NewsEntry.Url; }
            set { throw new InvalidOperationException (); }
        }

        public DateTime? StartDate
        {
            get { return NewsEntry.StartDate; }
            set { throw new InvalidOperationException (); }
        }

        public DateTime? EndDate
        {
            get { return NewsEntry.EndDate; }
            set { throw new InvalidOperationException (); }
        }

        public DateTime? ThresholdDate
        {
            get { return NewsEntry.ThresholdDate; }
            set { throw new InvalidOperationException (); }
        }

        public DateTime? DueDate
        {
            get { return NewsEntry.DueDate; }
            set { throw new InvalidOperationException (); }
        }

        public string Title
        {
            get { return NewsEntry.Title; }
            set { throw new InvalidOperationException (); }
        }

        public string Description
        {
            get { return NewsEntry.Description; }
            set { throw new InvalidOperationException (); }
        }

        public int ThematicWeight
        { 
            get { return NewsEntry.ThematicWeight; }
            set { throw new InvalidOperationException (); }
        }

        public int StructuralWeight
        { 
            get { return NewsEntry.StructuralWeight; }
            set { throw new InvalidOperationException (); }
        }

        public ContentItem ContentItem
        {
            get { return NewsEntry.ContentItem; }
            set { throw new InvalidOperationException (); }
        }

        public ModuleInfo AgentModule
        {
            get { return NewsEntry.AgentModule; }
            set { throw new InvalidOperationException (); }
        }

        public ICollection<INewsEntry> Group
        {
            get { return NewsEntry.Group; }
            set { throw new InvalidOperationException (); }
        }

        #endregion

        public bool HasImage
        {
            get { return NewsEntry.GetImage () != null; }
        }

        public string Link
        {
            get {
                if (!string.IsNullOrWhiteSpace (Url)) {
                    return Globals.LinkClick (Url, Context.Module.TabId, Context.Module.ModuleId);
                }

                return this.GetPermalink (PermalinkMode.Friendly,
                                          NewsDataProvider.Instance.ModuleController,
                                          PortalSettings.Current.PortalAlias,
                                          Context.Module.ModuleId, Context.Module.TabId);
            }
        }

        public string TitleLink
        {
            get {
                if (!string.IsNullOrEmpty (Link)) {
                    return string.Format ("<a href=\"{0}\">{1}</a>", Link, Title);
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
            get {
                var user = ContentItem.CreatedByUser (Context.Module.PortalId);
                if (user != null) {
                    return user.DisplayName;        
                }

                return Localization.GetString ("SystemUser.Text", Context.LocalResourceFile);
            }
        }

        public List<Badge> Badges
        {
            get { 
                if (Context.Module.IsEditable) {
                    var badges = new List<Badge> ();
                    var now = HttpContext.Current.Timestamp;

                    if (!NewsEntry.IsPublished (now)) {
                        if (NewsEntry.HasBeenExpired (now)) {
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
