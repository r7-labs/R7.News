//
//  EditNewsEntry.ascx.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016-2020 Roman M. Yagodin
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
using DotNetNuke.Common;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Web.UI.WebControls;
using R7.Dnn.Extensions.Client;
using R7.Dnn.Extensions.Collections;
using R7.Dnn.Extensions.Controls;
using R7.Dnn.Extensions.FileSystem;
using R7.Dnn.Extensions.Modules;
using R7.Dnn.Extensions.Text;
using R7.Dnn.Extensions.ViewModels;
using R7.News.Components;
using R7.News.Controls;
using R7.News.Data;
using R7.News.Models;
using R7.News.Stream.Models;
using R7.News.Stream.ViewModels;

namespace R7.News.Stream
{
    public enum EditNewsEntryTab
    {
        Common,
        TermsAndWeights,
        Advanced,
        Audit
    }

    public partial class EditNewsEntry : EditPortalModuleBase<NewsEntryInfo, int>
    {
        #region Properties

        ViewModelContext viewModelContext;

        protected ViewModelContext ViewModelContext {
            get { return viewModelContext ?? (viewModelContext = new ViewModelContext (this)); }
        }

        protected EditNewsEntryTab SelectedTab {
            get {
                // get postback initiator
                var eventTarget = Request.Form ["__EVENTTARGET"];
                if (!string.IsNullOrEmpty (eventTarget)) {
                    if (eventTarget.Contains ("$" + ctlUrl.ID)) {
                        ViewState ["SelectedTab"] = EditNewsEntryTab.Advanced;
                        return EditNewsEntryTab.Advanced;
                    }

                    if (eventTarget.Contains ("$" + buttonGetModules.ID)) {
                        ViewState ["SelectedTab"] = EditNewsEntryTab.TermsAndWeights;
                        return EditNewsEntryTab.TermsAndWeights;
                    }

                    if (eventTarget.Contains ("$" + buttonClearDiscussionLink.ID)) {
                        ViewState ["SelectedTab"] = EditNewsEntryTab.Audit;
                        return EditNewsEntryTab.Audit;
                    }
                }

                // otherwise, get active tab from viewstate
                var obj = ViewState ["SelectedTab"];
                return (obj != null) ? (EditNewsEntryTab)obj : EditNewsEntryTab.Common;
            }
            set { ViewState ["SelectedTab"] = value; }
        }

        #endregion

        public EditNewsEntry () : base ("entryid", null)
        {
        }

        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            JavaScript.RequestRegistration ("Select2");
            var select2Library = JavaScriptLibraryHelper.GetHighestVersionLibrary ("Select2");
            if (select2Library != null) {
                JavaScriptLibraryHelper.RegisterStyleSheet (select2Library, Page, "css/select2.min.css");
            }

            TermSelector.InitTerms (selTerms);

            pickerImage.FileFilter = Globals.glbImageFileTypes;
            pickerImage.FolderPath = GetImagesFolderPath ();

            // setup weight sliders
            sliderThematicWeight.Attributes.Add ("data-max", NewsConfig.Instance.NewsEntry.MaxWeight.ToString ());
            sliderStructuralWeight.Attributes.Add ("data-max", NewsConfig.Instance.NewsEntry.MaxWeight.ToString ());
            sliderThematicWeight.Text = NewsConfig.Instance.NewsEntry.DefaultThematicWeight.ToString ();
            sliderStructuralWeight.Text = NewsConfig.Instance.NewsEntry.DefaultStructuralWeight.ToString ();

            txtAgentModuleId.ReadOnly = !IsAdmin ();

            // localize column headers in the gridview
            gridModules.LocalizeColumnHeaders (LocalResourceFile);
        }

        string GetUrl (DnnUrlControl ctlUrl)
        {
            if (chkCurrentPage.Checked) {
                return TabId.ToString ();
            }

            return ctlUrl.Url;
        }

        bool IsAdmin ()
        {
            return Request.IsAuthenticated && (UserInfo.IsSuperUser || UserInfo.IsInRole ("Administrators"));
        }

        string GetImagesFolderPath ()
        {
            var folderId = FolderHistory.GetLastFolderId (Request, PortalId);
            if (folderId != null) {
                var folder = FolderManager.Instance.GetFolder (folderId.Value);
                if (folder != null) {
                    return folder.FolderPath;
                }
            }

            return NewsConfig.Instance.DefaultImagesPath;
        }

        protected void buttonGetModules_Click (object sender, EventArgs e)
        {
            var thematicWeight = int.Parse (sliderThematicWeight.Text);
            var structuralWeight = int.Parse (sliderStructuralWeight.Text);
            var terms = TermSelector.GetSelectedTerms (selTerms);

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

        protected void buttonClearDiscussionLink_Click (object sender, EventArgs e)
        {
            hiddenDiscussProviderKey.Value = null;
            hiddenDiscussEntryId.Value = null;
            textDiscussionLink.Text = string.Empty;
        }

        protected void btnDefaultImagesPath_Click (object sender, EventArgs e)
        {
            // HACK: Setting FolderPath does nothing, so trying to set FilePath - but folder should contain images
            var defaultFolder = FolderManager.Instance.GetFolder (PortalId, NewsConfig.Instance.DefaultImagesPath);
            if (defaultFolder != null) {
                var file = FolderManager.Instance.GetFiles (defaultFolder)
                    .FirstOrDefault (f => Globals.glbImageFileTypes.Contains (f.Extension));
                if (file != null) {
                    pickerImage.FilePath = file.RelativePath;
                }
            }
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

                txtAgentModuleId.Text = ModuleId.ToString ();
            }

            if (!terms.IsNullOrEmpty ()) {
                TermSelector.SelectTerms (selTerms, terms);
            }

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

            TermSelector.SelectTerms (selTerms, item.ContentItem.Terms);

            ctlUrl.Url = item.Url;

            textPermalinkRaw.Text = item.GetPermalinkRaw (NewsDataProvider.Instance.ModuleController,
                                                          PortalAlias, ModuleId, TabId);
            textPermalinkFriendly.Text = item.GetPermalinkFriendly (NewsDataProvider.Instance.ModuleController,
                                                                    ModuleId, TabId);

            // REVIEW: Check for max value?
            sliderThematicWeight.Text = item.ThematicWeight.ToString ();
            sliderStructuralWeight.Text = item.StructuralWeight.ToString ();

            hiddenDiscussProviderKey.Value = item.DiscussProviderKey;
            hiddenDiscussEntryId.Value = item.DiscussEntryId;

            if (!string.IsNullOrEmpty (item.DiscussProviderKey)) {
                buttonClearDiscussionLink.Visible = IsAdmin ();
                var discussProvider = NewsConfig.Instance.GetDiscussProviders ().FirstOrDefault (dp => dp.ProviderKey == item.DiscussProviderKey);
                if (discussProvider != null) {
                    textDiscussionLink.Text = discussProvider.GetDiscussUrl (item.DiscussEntryId);
                }
            }

            txtAgentModuleId.Text = item.AgentModuleId.ToString ();

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

        protected override void BeforeUpdateItem (NewsEntryInfo item, bool isNew)
        {
            if (ItemKey == null) {
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

            item.Url = GetUrl (ctlUrl);

            item.ThematicWeight = int.Parse (sliderThematicWeight.Text);
            item.StructuralWeight = int.Parse (sliderStructuralWeight.Text);

            item.DiscussProviderKey = (hiddenDiscussProviderKey.Value.Length > 0)? hiddenDiscussProviderKey.Value : null;
            item.DiscussEntryId = (hiddenDiscussEntryId.Value.Length > 0) ? hiddenDiscussEntryId.Value : null; ;

            var agentModuleId = ParseHelper.ParseToNullable<int> (txtAgentModuleId.Text);
            if (agentModuleId == null) {
                // unbind news entry from Agent
                item.AgentModuleId = null;
            }
            else {
                // bind news entry to Agent, if it exists
                var module = ModuleController.Instance.GetModule (agentModuleId.Value, -1, false);
                if (module != null && module.ModuleDefinition.DefinitionName == Const.AgentModuleDefinitionName) {
                    item.AgentModuleId = agentModuleId;
                }
            }
        }

        void RememberFolder (IFileInfo image)
        {
            var file = FileManager.Instance.GetFile (image.FileId);
            if (file != null) {
                var folder = FolderManager.Instance.GetFolder (file.FolderId);
                if (folder != null) {
                    var defaultFolder = FolderManager.Instance.GetFolder (PortalId, NewsConfig.Instance.DefaultImagesPath);
                    if (folder.FolderID != defaultFolder.FolderID) {
                        FolderHistory.RememberFolder (Request, Response, folder.FolderID, PortalId);
                    }
                }
            }
        }

        protected override NewsEntryInfo GetItem (int itemKey)
        {
            return NewsRepository.Instance.GetNewsEntry (itemKey, PortalId);
        }

        protected override void AddItem (NewsEntryInfo item)
        {
            NewsRepository.Instance.AddNewsEntry (item, TermSelector.GetSelectedTerms (selTerms), images, ModuleId, TabId);
            if (images.Count > 0) {
                RememberFolder (images [0]);
            }
        }

        protected override void UpdateItem (NewsEntryInfo item)
        {
            NewsRepository.Instance.UpdateNewsEntry (item, TermSelector.GetSelectedTerms (selTerms), ModuleId, TabId);
            if (images.Count > 0) {
                RememberFolder (images [0]);
            }
        }

        protected override void DeleteItem (NewsEntryInfo item)
        {
            NewsRepository.Instance.DeleteNewsEntry (item);
        }

        #endregion
    }
}
