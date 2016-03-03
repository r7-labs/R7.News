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
using System.Linq;
using System.Collections.Generic;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Common.Utilities;
using DotNetNuke.R7;
using R7.News.Models;
using R7.News.Models.Data;
using R7.News.Components;
using R7.News.Stream.Components;
using DotNetNuke.UI.UserControls;

namespace R7.News.Stream
{
    public partial class EditNewsEntry : EditModuleBase<NewsDataProvider,StreamSettings,NewsEntryInfo>
    {
        public EditNewsEntry () : base ("entryid", true)
        {
        }

        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            var newsSourceProviders = NewsSourceRepository.Instance.GetSelfSources ();
            newsSourceProviders.Insert (0, new NewsSourceInfo { 
                SourceId = Null.NullInteger, 
                Title = LocalizeString ("NotSelected.Text")
            });

            comboNewsSourceProvider.DataSource = newsSourceProviders;
            comboNewsSourceProvider.DataBind ();

            UpdateNewsSources ();

            pickerImage.FileFilter = Globals.glbImageFileTypes;
        }

        protected void comboNewsSourceProvider_SelectedIndexChanged (object sender, EventArgs e)
        {
            // disable handler when loading item
            if (IsPostBack) {
                UpdateNewsSources ();
            }
        }

        protected void UpdateNewsSources ()
        {
            var sourceId = TypeUtils.ParseToNullableInt (comboNewsSourceProvider.SelectedValue);

            var newsSources = NewsSourceRepository.Instance.GetSources (sourceId)
                .OrderBy (ns => ns.Title)
                .ToList ();
            
            newsSources.Insert (0, new NewsSourceInfo { 
                SourceItemId = Null.NullInteger, 
                Title = LocalizeString ("NotSelected.Text")
            });

            comboNewsSource.DataSource = newsSources;
            comboNewsSource.DataBind ();
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

            var image = item.ContentItem.Images.FirstOrDefault ();
            if (image != null) {
                pickerImage.FileID = image.FileId;
            }

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

            comboNewsSourceProvider.SelectByValue (item.SourceId);
            UpdateNewsSources ();
            comboNewsSource.SelectByValue (item.SourceItemId);
           
            urlUrl.Url = item.Url;

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
                List<IFileInfo> images;

                if (ItemId == null) {
                    item = new NewsEntryInfo ();
                    images = new List<IFileInfo> ();
                }
                else {
                    item = NewsRepository.Instance.GetNewsEntry (ItemId.Value, PortalId);
                    images = item.ContentItem.Images;
                }

                // fill the object
                var imageFile = FileManager.Instance.GetFile (pickerImage.FileID);
                if (imageFile != null) {

                    if (images.Count == 0) {
                        images.Add (imageFile);
                    }
                    else {
                        images.Clear ();
                        images.Add (imageFile);
                    }
                }
                else if (images.Count > 0) {
                    images.Clear ();
                }

                item.Title = textTitle.Text.Trim ();
                item.Description = textDescription.Text.Trim ();
                item.SortIndex = int.Parse (textSortIndex.Text);

                item.ThresholdDate = datetimeThresholdDate.SelectedDate;
                item.DueDate = datetimeDueDate.SelectedDate;
                item.StartDate = datetimeStartDate.SelectedDate;
                item.EndDate = datetimeEndDate.SelectedDate;
                item.PortalId = PortalId;

                item.SourceId = TypeUtils.ParseToNullableInt (comboNewsSourceProvider.SelectedValue);
                item.SourceItemId = TypeUtils.ParseToNullableInt (comboNewsSource.SelectedValue);

                item.Url = urlUrl.Url;

                if (ModuleConfiguration.ModuleDefinition.DefinitionName == "R7.News.Agent") {
                    item.AgentModuleId = ModuleId;
                } 

                if (ItemId == null) {
                    NewsRepository.Instance.AddNewsEntry (item, termsTerms.Terms, images, ModuleId, TabId);
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

