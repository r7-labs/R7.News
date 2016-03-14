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
using R7.News.Models;
using R7.News.Models.Data;
using DotNetNuke.Entities.Content;
using DotNetNuke.R7;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;

namespace R7.News.Stream.ViewModels
{
    public class StreamModuleNewsEntryViewModel: IModuleNewsEntry
    {
        protected ViewModelContext Context;

        protected IModuleNewsEntry NewsEntry;

        public StreamModuleNewsEntryViewModel (IModuleNewsEntry newsEntry, ViewModelContext context)
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

        public int SortIndex
        {
            get { return NewsEntry.SortIndex; }
            set {}
        }

        public bool IsSticky
        {
            get { return NewsEntry.IsSticky; }
            set {}
        }

        public int? SourceId
        {
            get { return NewsEntry.SourceId; }
            set {}
        }

        public int? SourceItemId
        {
            get { return NewsEntry.SourceItemId; }
            set {}
        }

        public ContentItem ContentItem
        {
            get { return NewsEntry.ContentItem; }
            set {}
        }

        public INewsSource Source
        {
            get { return NewsEntry.Source; }
            set {}
        }

        public int? ModuleId
        {
            get { return NewsEntry.ModuleId; }
            set {}
        }

        public int? Visibility
        {
            get { return NewsEntry.Visibility; }
            set {}
        }

        #endregion

        public string ImageUrl
        {
            get { return NewsEntry.GetImageUrl (width: 192); }
        }

        public string TitleLink
        {
            get
            { 
                string url = null;
                if (!string.IsNullOrWhiteSpace (Url)) {
                    url = Url;
                }
                else if (AgentModuleId != null) {
                    var moduleController = new ModuleController ();
                    var agentModule = moduleController.GetModule (AgentModuleId.Value);
                    url = agentModule.TabID.ToString ();
                }

                if (!string.IsNullOrEmpty (url)) {
                    return string.Format ("<a href=\"{0}\">{1}</a>", 
                        Globals.LinkClick (url, Context.Module.TabId,  Context.Module.ModuleId),
                        Title);
                }

                return Title;
            }
        }

        public string CreatedOnDateString
        {
            get 
            {
                return ContentItem.CreatedOnDate.ToString (Localization.GetString (
                    "CreatedOnDate.Format",
                    Context.LocalResourceFile));
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
    }
}

