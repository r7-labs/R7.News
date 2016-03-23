//
//  ViewAgent.ascx.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.R7;
using DotNetNuke.R7.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using R7.News.Agent.Components;
using R7.News.Agent.ViewModels;
using R7.News.Components;
using R7.News.Controls;
using R7.News.Models;
using R7.News.Models.Data;
using R7.News.ViewModels;

namespace R7.News.Agent
{
    public partial class ViewAgent : PortalModuleBase<AgentSettings>, IActionable
    {
        #region Properties

        ViewModelContext<AgentSettings> viewModelContext;
        protected ViewModelContext<AgentSettings> ViewModelContext
        {
            get { return viewModelContext ?? (viewModelContext = new ViewModelContext<AgentSettings> (this, Settings)); }
        }

        #endregion

        #region Handlers

        /// <summary>
        /// Handles Load event for a control
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnLoad (EventArgs e)
        {
            base.OnLoad (e);
            
            try {
                if (!IsPostBack) {
                    var items = NewsRepository.Instance.GetNewsEntriesByAgent (ModuleId);

                    // check if we have some content to display, 
                    // otherwise display a message for module editors.
                    if ((items == null || !items.Any ()) && IsEditable) {

                        // show panel with add button
                        panelAddDefaultEntry.Visible = true;
                    }
                    else {
                        // create viewmodels
                        var viewModels = items
                            .OrderByDescending (ne => ne.ContentItem.CreatedOnDate)
                            .GroupByAgentModule (Settings.EnableGrouping)
                            .Select (ne => new AgentModuleNewsEntryViewModel (ne, ViewModelContext));

                        // bind the data
                        listAgent.DataSource = viewModels;
                        listAgent.DataBind ();
                    }
                }
            }
            catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }

        #endregion

        #region IActionable implementation

        public ModuleActionCollection ModuleActions
        {
            get {
                // create a new action to add an item, this will be added 
                // to the controls dropdown menu
                var actions = new ModuleActionCollection ();

                // TODO: Add action to create news entry from tab data

                actions.Add (
                    GetNextActionID (), 
                    Localization.GetString (ModuleActionType.AddContent, this.LocalResourceFile),
                    ModuleActionType.AddContent, 
                    "", 
                    "", 
                    EditUrl ("EditNewsEntry"),
                    false, 
                    DotNetNuke.Security.SecurityAccessLevel.Edit,
                    true, 
                    false
                );

                return actions;
            }
        }

        #endregion

        /// <summary>
        /// Handles the items being bound to the listview control. In this method we merge the data with the
        /// template defined for this control to produce the result to display to the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void listAgent_ItemDataBound (object sender, ListViewItemEventArgs e)
        {
            var item = (AgentModuleNewsEntryViewModel) e.Item.DataItem;
            
            var linkEdit = (HyperLink) e.Item.FindControl ("linkEdit");
            var iconEdit = (Image) e.Item.FindControl ("imageEdit");
            
            // read module settings (may be useful in a future)
            // var settings = new R7.News.AgentSettings (this);            
            
            // edit link
            if (IsEditable) {
                linkEdit.NavigateUrl = EditUrl ("entryid", item.EntryId.ToString (), "EditNewsEntry");
            }

            // make edit link visible in edit mode
            linkEdit.Visible = IsEditable;
            iconEdit.Visible = IsEditable;

            // show image
            var imageImage = (Image) e.Item.FindControl ("imageImage");
            imageImage.Visible = item.GetImage () != null;

            // show term links
            var termLinks = (TermLinks) e.Item.FindControl ("termLinks");
            termLinks.Module = this;
            termLinks.DataSource = item.ContentItem.Terms;
            termLinks.DataBind ();

            // show grouped news
            var listGroup = (ListView) e.Item.FindControl ("listGroup");

            if (item.Group != null && item.Group.Count > 0) {
                listGroup.DataSource = item.Group
                    .OrderByDescending (ne => ne.ContentItem.CreatedOnDate)
                    .Select (ne => new AgentModuleNewsEntryViewModel (ne, ViewModelContext));
                listGroup.DataBind ();
            }
        }

        /// <summary>
        /// Handles the items being bound to the listview control. In this method we merge the data with the
        /// template defined for this control to produce the result to display to the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void listGroup_ItemDataBound (object sender, ListViewItemEventArgs e)
        {
            var item = (AgentModuleNewsEntryViewModel) e.Item.DataItem;

            var linkEdit = (HyperLink) e.Item.FindControl ("linkEdit");
            var iconEdit = (Image) e.Item.FindControl ("imageEdit");

            // edit link
            if (IsEditable) {
                linkEdit.NavigateUrl = EditUrl ("entryid", item.EntryId.ToString (), "EditNewsEntry");
            }

            // make edit link visible in edit mode
            linkEdit.Visible = IsEditable;
            iconEdit.Visible = IsEditable;

            // show image
            var imageImage = (Image) e.Item.FindControl ("imageImage");
            imageImage.Visible = item.GetImage () != null;
        }

        protected void buttonAddFromTabData_Click (object sender, EventArgs e)
        {
            var newsEntry = AddFromTabData ();
            UpdateModuleTitle (newsEntry.Title);

            Response.Redirect (Globals.NavigateURL (), true);
        }

        protected INewsEntry AddFromTabData ()
        {
            var tabController = new TabController ();
            var activeTab = tabController.GetTab (TabId, PortalId);

            // add default news entry based on tab data
            var newsEntry = new ModuleNewsEntryInfo {
                Title = activeTab.TabName,
                Description = activeTab.Description,
                AgentModuleId = ModuleId,
                EndDate = DateTime.Today
            };

            // add news entry
            NewsRepository.Instance.AddNewsEntry (newsEntry, activeTab.Terms, new List<IFileInfo> (), ModuleId, TabId);

            return newsEntry;
        }

        protected void UpdateModuleTitle (string title)
        {
            var moduleController = NewsDataProvider.Instance.ModuleController;
            var module = moduleController.GetModule (ModuleId, TabId);
            if (module.ModuleTitle != title) {
                module.ModuleTitle = title;
                moduleController.UpdateModule (module);
            }
        }
    }
}

