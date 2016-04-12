//
//  ViewStream.ascx.cs
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
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Icons;
using DotNetNuke.R7;
using DotNetNuke.R7.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using R7.News.Controls;
using R7.News.Models;
using R7.News.Stream.Components;
using R7.News.Stream.ViewModels;
using PagingControlMode = DotNetNuke.R7.PagingControlMode;

namespace R7.News.Stream
{
    public partial class ViewStream : PortalModuleBase<StreamSettings>, IActionable
    {
        StreamViewModel viewModel;
        protected StreamViewModel ViewModel
        {
            get { return viewModel ?? (viewModel = new StreamViewModel (this, Settings)); }
        }

        protected int PageSize
        {
            get
            { 
                var objPageSize = ViewState ["PageSize"];
                if (objPageSize != null) {
                    return (int) ViewState ["PageSize"];
                }

                return Settings.PageSize;
            }
            set { ViewState ["PageSize"] = value; }
        }

        protected int CurrentPage = 1;

        #region Handlers

        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            buttonShowMore.Visible = Settings.UseShowMore;

            // setup top paging control
            if (Settings.ShowTopPager) {
                pagerTop.CurrentPage = CurrentPage;
                pagerTop.TabID = TabId;
                pagerTop.PageSize = PageSize;
                pagerTop.PageLinksPerPage = Settings.MaxPageLinks;
                pagerTop.Mode = PagingControlMode.PostBack;
                pagerTop.QuerystringParams = "pagingModuleId=" + ModuleId;
            }
            else {
                pagerTop.Visible = false;
            }

            // setup bottom paging control
            if (Settings.ShowBottomPager) {
                pagerBottom.CurrentPage = CurrentPage;
                pagerBottom.TabID = TabId;
                pagerBottom.PageSize = PageSize;
                pagerBottom.PageLinksPerPage = Settings.MaxPageLinks;
                pagerBottom.Mode = PagingControlMode.PostBack;
                pagerBottom.QuerystringParams = "pagingModuleId=" + ModuleId;
            }
            else {
                pagerBottom.Visible = false;
            }
        }

        /// <summary>
        /// Handles Load event for a control
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnLoad (EventArgs e)
        {
            base.OnLoad (e);

            try {
                if (!IsPostBack) {
                    
                    var page = ViewModel.GetPage (CurrentPage - 1, PageSize);

                    ToggleStreamControls (page.TotalItems);

                    if (page.TotalItems > 0) {
                        // bind the data
                        listStream.DataSource = page.Page;
                        listStream.DataBind ();
                    }
                }
            }
            catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }

        protected void ToggleStreamControls (int totalItems)
        {
            if (totalItems > 0) {
                panelStream.Visible = true;

                // setup paging controls
                pagerTop.PageSize = PageSize;
                pagerBottom.PageSize = PageSize;
                pagerTop.TotalRecords = totalItems;
                pagerBottom.TotalRecords = totalItems;

                var canShowPager = totalItems > PageSize;
                pagerTop.Visible = canShowPager && Settings.ShowTopPager;
                pagerBottom.Visible = canShowPager && Settings.ShowBottomPager;
            }
            else {
                panelStream.Visible = false;

                if (IsEditable) {
                    this.Message ("NothingToDisplay.Text", MessageType.Info, true);
                }
            }
        }

        protected void pagingControl_PageChanged (object sender, EventArgs e)
        {
            var pagingControl = (PagingControl) sender;

            CurrentPage = pagingControl.CurrentPage;

            var page = ViewModel.GetPage (CurrentPage - 1, PageSize);

            // sync paging controls
            if (pagingControl == pagerTop) {
                pagerBottom.CurrentPage = CurrentPage;
            }
            else {
                pagerTop.CurrentPage = CurrentPage;
            }

            ToggleStreamControls (page.TotalItems);

            if (page.TotalItems > 0) {
                // bind the data
                listStream.DataSource = page.Page;
                listStream.DataBind ();
            }
        }

        protected void buttonShowMore_Click (object sender, EventArgs e)
        {
            // set current page to 1 and increase page size
            CurrentPage = 1;
            PageSize = PageSize + Settings.PageSize;

            var page = ViewModel.GetPage (CurrentPage - 1, PageSize);

            // hide "show more" button, if there are no more items
            if (PageSize >= page.TotalItems) {
                buttonShowMore.Visible = false;
            }

            ToggleStreamControls (page.TotalItems);

            if (page.TotalItems > 0) {
                // bind the data
                listStream.DataSource = page.Page;
                listStream.DataBind ();
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
                actions.Add (
                    GetNextActionID (),
                    LocalizeString ("AddNewsEntry.Action"),
                    ModuleActionType.AddContent,
                    "",
                    IconController.IconURL ("Add"),
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
        /// Handles the items being bound to the datalist control. In this method we merge the data with the
        /// template defined for this control to produce the result to display to the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void listStream_ItemDataBound (object sender, ListViewItemEventArgs e)
        {
            var item = (StreamModuleNewsEntryViewModel) e.Item.DataItem;

            var linkEdit = (HyperLink) e.Item.FindControl ("linkEdit");
            var iconEdit = (Image) e.Item.FindControl ("imageEdit");

            // edit link
            if (IsEditable) {
                linkEdit.NavigateUrl = EditUrl ("entryid", item.EntryId.ToString (), "EditNewsEntry");
            }

            // make edit link visible in edit mode
            linkEdit.Visible = IsEditable;
            iconEdit.Visible = IsEditable;

            // visibility badges
            var listBadges = (BadgeList) e.Item.FindControl ("listBadges");
            listBadges.DataSource = item.Badges;
            listBadges.DataBind ();

            // show image
            var imageImage = (Image) e.Item.FindControl ("imageImage");
            imageImage.Visible = item.GetImage () != null;

            // show term links
            var termLinks = (TermLinks) e.Item.FindControl ("termLinks");
            termLinks.Module = this;
            termLinks.DataSource = item.ContentItem.Terms;
            termLinks.DataBind ();
        }
    }
}

