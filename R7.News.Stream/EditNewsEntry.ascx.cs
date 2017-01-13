//
//  EditNewsEntry.ascx.cs
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
using System.Linq;
using DotNetNuke.Common;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.FileSystem;
using R7.DotNetNuke.Extensions.ControlExtensions;
using R7.DotNetNuke.Extensions.Modules;
using R7.DotNetNuke.Extensions.ViewModels;
using R7.News.Components;
using R7.News.Data;
using R7.News.Models;
using R7.News.Stream.Components;
using R7.News.Stream.Data;
using R7.News.Stream.ViewModels;

namespace R7.News.Stream
{
    public enum EditNewsEntryTab
    {
        Common,
        TermsAndWeights,
        Advanced
    }

    public partial class EditNewsEntry : EditPortalModuleBase<NewsEntryInfo,int>
    {
        #region Properties

        ViewModelContext viewModelContext;

        protected ViewModelContext ViewModelContext
        {
            get { return viewModelContext ?? (viewModelContext = new ViewModelContext (this)); }
        }

        protected EditNewsEntryTab SelectedTab
        {
            get {
                // get postback initiator
                var eventTarget = Request.Form ["__EVENTTARGET"];
                if (!string.IsNullOrEmpty (eventTarget)) {
                    if (eventTarget.Contains ("$" + urlUrl.ID)) {
                        // urlURL control is on Advanced tab
                        ViewState ["SelectedTab"] = EditNewsEntryTab.Advanced;
                        return EditNewsEntryTab.Advanced;
                    }

                    if (eventTarget.Contains ("$" + buttonGetModules.ID)) {
                        // buttonGetModules control is on Citation tab
                        ViewState ["SelectedTab"] = EditNewsEntryTab.TermsAndWeights;
                        return EditNewsEntryTab.TermsAndWeights;
                    }
                }
                    
                // otherwise, get active tab from viewstate
                var obj = ViewState ["SelectedTab"];
                return (obj != null) ? (EditNewsEntryTab) obj : EditNewsEntryTab.Common;
            }
            set { ViewState ["SelectedTab"] = value; }
        }

        #endregion

        public EditNewsEntry () : base ("entryid", true)
        {
        }

        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            pickerImage.FolderPath = NewsConfig.Instance.DefaultImagesPath;
            pickerImage.FileFilter = Globals.glbImageFileTypes;

            // setup weight sliders
            sliderThematicWeight.Attributes.Add ("data-max", NewsConfig.Instance.NewsEntry.MaxWeight.ToString ());
            sliderStructuralWeight.Attributes.Add ("data-max", NewsConfig.Instance.NewsEntry.MaxWeight.ToString ());
            sliderThematicWeight.Text = NewsConfig.Instance.NewsEntry.DefaultThematicWeight.ToString ();
            sliderStructuralWeight.Text = NewsConfig.Instance.NewsEntry.DefaultStructuralWeight.ToString ();

            // localize column headers in the gridview
            gridModules.LocalizeColumnHeaders (LocalResourceFile);
        }

        protected void buttonGetModules_Click (object sender, EventArgs e)
        {
            var thematicWeight = int.Parse (sliderThematicWeight.Text);
            var structuralWeight = int.Parse (sliderStructuralWeight.Text);
            var terms = termsTerms.Terms;

            gridModules.DataSource = GetStreamModules (thematicWeight, structuralWeight, terms);
            gridModules.DataBind ();
        }

        // REVIEW: Move to business logic layer
        protected IEnumerable<StreamModuleViewModel> GetStreamModules (
            int thematicWeight,
            int structuralWeight,
            IList<Term> terms)
        {
            var moduleController = new ModuleController ();
            var settingsRepository = new StreamSettingsRepository ();
            return moduleController.GetModulesByDefinition (PortalId, Const.StreamModuleDefinitionName)
                .Cast<ModuleInfo> ()
                .Where (m => !m.IsDeleted)
                .Where (m => StreamModuleViewModel.IsNewsEntryWillBePassedByModule (settingsRepository.GetSettings (m), 
                thematicWeight, structuralWeight, terms))
                .Select (m => new StreamModuleViewModel (
                m,
                settingsRepository.GetSettings (m),
                ViewModelContext,
                thematicWeight,
                structuralWeight,
                terms))
                .OrderBy (m => m.ModuleTitle);
        }

        #region Implemented abstract and overriden members of EditPortalModuleBase

        protected override void InitControls ()
        {
            InitControls (buttonUpdate, buttonDelete, linkCancel, ctlAudit); 
        }

        protected override void LoadNewItem ()
        {
            List<Term> terms = null;

            // Stream: get terms from module settings
            if (ModuleConfiguration.ModuleDefinition.DefinitionName == Const.StreamModuleDefinitionName) {
                var settings = new StreamSettingsRepository ().GetSettings (ModuleConfiguration);
                terms = settings.IncludeTerms;
            }

            // Agent: get terms from current tab
            if (ModuleConfiguration.ModuleDefinition.DefinitionName == Const.AgentModuleDefinitionName) {
                terms = PortalSettings.ActiveTab.Terms;
            }

            termsTerms.Terms = terms ?? new List<Term> ();
            termsTerms.PortalId = PortalId;
            termsTerms.DataBind ();

            buttonUpdate.Text = LocalizeString ("Add.Text");
        }

        /*
        protected void Trace (LogController logController, string message)
        {
            var logInfo = new LogInfo {
                LogTypeKey = EventLogController.EventLogType.ADMIN_ALERT.ToString (),
                LogUserID = -1, // superuser
                LogPortalID = PortalId
            };

            logInfo.AddProperty ("EditNewsEntry", message);
            logController.AddLog (logInfo);
        }
        */

        protected override void LoadItem (NewsEntryInfo item)
        {
            // load also content item
            item = (NewsEntryInfo) item.WithContentItem ();

            var image = item.ContentItem.Images.FirstOrDefault ();
            if (image != null) {
                pickerImage.FileID = image.FileId;
            }

            textTitle.Text = item.Title;
            textDescription.Text = item.Description;

            datetimeThresholdDate.SelectedDate = item.ThresholdDate;
            datetimeDueDate.SelectedDate = item.DueDate;
            datetimeStartDate.SelectedDate = item.StartDate;
            datetimeEndDate.SelectedDate = item.EndDate;

            termsTerms.PortalId = PortalId;
            termsTerms.Terms = item.ContentItem.Terms;
            termsTerms.DataBind ();

            urlUrl.Url = item.Url;

            textPermalink.Text = (NewsConfig.Instance.PermalinkMode == PermalinkMode.Friendly) ?
                item.GetPermalinkFriendly (NewsDataProvider.Instance.ModuleController, ModuleId, TabId) :
                item.GetPermalinkRaw (NewsDataProvider.Instance.ModuleController, PortalAlias, ModuleId, TabId);

            // REVIEW: Check for max value?
            sliderThematicWeight.Text = item.ThematicWeight.ToString ();
            sliderStructuralWeight.Text = item.StructuralWeight.ToString ();

            var auditData = new AuditData {
                CreatedDate = item.ContentItem.CreatedOnDate.ToLongDateString (),
                LastModifiedDate = item.ContentItem.LastModifiedOnDate.ToLongDateString (),
            };

            var createdByUser = item.ContentItem.CreatedByUser (PortalId);
            auditData.CreatedByUser = (createdByUser != null) ? createdByUser.DisplayName : LocalizeString ("SystemUser.Text");
            var lastModifiedByUser = item.ContentItem.LastModifiedByUser (PortalId);
            auditData.LastModifiedByUser = (lastModifiedByUser != null) ? lastModifiedByUser.DisplayName : LocalizeString ("SystemUser.Text");

            // bind audit control and store data to the viewstate
            BindAuditControl (auditData);
            ViewState ["AuditData"] = auditData;

            buttonUpdate.Text = LocalizeString ("Update.Text");
        }

        protected override void PostBack ()
        {
            // try get audit data from viewstate
            var objAuditData = ViewState ["AuditData"];
            if (objAuditData != null) {
                // bind audit control from stored data
                BindAuditControl ((AuditData) objAuditData);
            }
        }

        protected void BindAuditControl (AuditData auditData)
        {
            ctlAudit.CreatedDate = auditData.CreatedDate;
            ctlAudit.LastModifiedDate = auditData.LastModifiedDate;
            ctlAudit.CreatedByUser = auditData.CreatedByUser;
            ctlAudit.LastModifiedByUser = auditData.LastModifiedByUser;
        }

        private List<IFileInfo> images;

        protected override void BeforeUpdateItem (NewsEntryInfo item)
        {
            if (ItemId == null) {
                images = new List<IFileInfo> ();
            }
            else {
                images = item.ContentItem.Images;
            }

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

            // fill the object
            item.Title = textTitle.Text.Trim ();
            item.Description = textDescription.Text.Trim ();

            item.ThresholdDate = datetimeThresholdDate.SelectedDate;
            item.DueDate = datetimeDueDate.SelectedDate;
            item.StartDate = datetimeStartDate.SelectedDate;
            item.EndDate = datetimeEndDate.SelectedDate;
            item.PortalId = PortalId;

            item.Url = urlUrl.Url;

            item.ThematicWeight = int.Parse (sliderThematicWeight.Text);
            item.StructuralWeight = int.Parse (sliderStructuralWeight.Text);

            if (ModuleConfiguration.ModuleDefinition.DefinitionName == Const.AgentModuleDefinitionName) {
                item.AgentModuleId = ModuleId;
            }   
        }

        protected override NewsEntryInfo GetItem (int itemId)
        {
            return NewsRepository.Instance.GetNewsEntry (itemId, PortalId);
        }

        protected override void AddItem (NewsEntryInfo item)
        {
            NewsRepository.Instance.AddNewsEntry (item, termsTerms.Terms, images, ModuleId, TabId);
        }

        protected override void UpdateItem (NewsEntryInfo item)
        {
            NewsRepository.Instance.UpdateNewsEntry (item, termsTerms.Terms, ModuleId, TabId);
        }

        protected override void DeleteItem (NewsEntryInfo item)
        {
            NewsRepository.Instance.DeleteNewsEntry (item);
        }

        #endregion
    }
}
