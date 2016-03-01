//
//  EditNewsEntry.ascx.cs
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
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Services.Exceptions;
using R7.News.Stream.Components;
using DotNetNuke.R7;
using R7.News.Models;
using R7.News.Models.Extensions;
using R7.News.Components;
using System.Reflection;

namespace R7.News.Stream
{
    public partial class EditNewsEntry : EditModuleBase<NewsDataProvider,StreamSettings,NewsEntryInfo>
    {
        public EditNewsEntry () : base ("entryid")
        {
        }

        #region Implemented abstract members of EditModuleBase

        protected override void OnInitControls ()
        {
            InitControls (buttonUpdate, buttonDelete, linkCancel, ctlAudit); 
        }

        protected override void OnLoadNewItem ()
        {
            termsTerms.PortalId = PortalId;
            termsTerms.Terms = new List<Term> ();
            termsTerms.DataBind ();

            buttonUpdate.Text = LocalizeString ("Add.Text");
        }

        protected override void OnLoadItem (NewsEntryInfo item)
        {
            // load also content item
            item = (NewsEntryInfo) item.WithContentItem ();

            textTitle.Text = item.Title;
            textDescription.Text = item.Description;
            textSortIndex.Text = item.SortIndex.ToString ();

            datetimeThresholdDate.SelectedDate = item.ThresholdDate;
            datetimeDueDate.SelectedDate = item.DueDate;
            datetimeStartDate.SelectedDate = item.StartDate;
            datetimeEndDate.SelectedDate = item.EndDate;

            termsTerms.PortalId = PortalId;
            termsTerms.Terms = item.ContentItem.Terms;
            termsTerms.DataBind ();

            ctlAudit.CreatedDate = item.ContentItem.CreatedOnDate.ToLongDateString ();
            ctlAudit.LastModifiedDate = item.ContentItem.LastModifiedOnDate.ToLongDateString ();
            ctlAudit.CreatedByUser = item.ContentItem.CreatedByUser (PortalId).DisplayName;
            ctlAudit.LastModifiedByUser = item.ContentItem.LastModifiedByUser (PortalId).DisplayName;

            buttonUpdate.Text = LocalizeString ("Update.Text");
        }
    
        protected override void OnUpdateItem (NewsEntryInfo item)
        {
            // empty, entire OnButtonUpdateClick is overriden
        }

        #endregion

        protected override void OnButtonUpdateClick (object sender, EventArgs e)
        {
            try {
                NewsEntryInfo item;

                if (ItemId == null) {
                    item = new NewsEntryInfo ();
                }
                else {
                    item = NewsRepository.Instance.GetNewsEntry (ItemId.Value, PortalId);
                }

                // fill the object
                item.Title = textTitle.Text.Trim ();
                item.Description = textDescription.Text.Trim ();
                item.SortIndex = int.Parse (textSortIndex.Text);

                item.ThresholdDate = datetimeThresholdDate.SelectedDate;
                item.DueDate = datetimeDueDate.SelectedDate;
                item.StartDate = datetimeStartDate.SelectedDate;
                item.EndDate = datetimeEndDate.SelectedDate;
                item.PortalId = PortalId;

                if (ModuleConfiguration.ModuleDefinition.DefinitionName == "R7.News.Agent") {
                    item.AgentModuleId = ModuleId;
                } 

                if (ItemId == null) {
                    NewsRepository.Instance.AddNewsEntry (item, termsTerms.Terms, ModuleId, TabId);
                }
                else {
                    NewsRepository.Instance.UpdateNewsEntry (item, termsTerms.Terms);
                }

                ModuleController.SynchronizeModule (ModuleId);

                Response.Redirect (Globals.NavigateURL (), true);
            }
            catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }    
        }

        protected override void OnButtonDeleteClick (object sender, EventArgs e)
        {
            try
            {
                if (ItemId.HasValue)
                {
                    var item = NewsRepository.Instance.GetNewsEntry (ItemId.Value, PortalId);
                    if (item != null && CanDeleteItem (item))
                    {
                        NewsRepository.Instance.DeleteNewsEntry (item);

                        Response.Redirect (Globals.NavigateURL (), true);
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }

    }
}

