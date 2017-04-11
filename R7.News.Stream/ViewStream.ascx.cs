//
//  ViewStream.ascx.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016-2017 Roman M. Yagodin
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
using System.Web.UI.WebControls;
using DotNetNuke.Services.Exceptions;
using R7.DotNetNuke.Extensions.Controls;
using R7.DotNetNuke.Extensions.ModuleExtensions;
using R7.News.Controls;
using R7.News.Models;
using R7.News.Modules;
using R7.News.Stream.Components;
using R7.News.Stream.ViewModels;

namespace R7.News.Stream
{
    public partial class ViewStream: NewsModuleBase<StreamSettings>
    {
        StreamViewModel viewModel;

        protected StreamViewModel ViewModel
        {
            get { return viewModel ?? (viewModel = new StreamViewModel (this, Settings)); }
        }

        protected int PageSize
        {
            get { 
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

            // hide horizontal rule in signature if "Show more" button is used
            agplSignature.ShowRule = !Settings.UseShowMore;
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

                    ToggleStreamControls (page.TotalItems, PageSize);

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

        protected void ToggleStreamControls (int totalItems, int pageSize)
        {
            if (totalItems > 0) {
                // show module content
                panelStream.Visible = true;

                // setup paging controls
                pagerTop.PageSize = pageSize;
                pagerBottom.PageSize = pageSize;
                pagerTop.TotalRecords = totalItems;
                pagerBottom.TotalRecords = totalItems;

                var canShowPager = totalItems > pageSize;
                pagerTop.Visible = canShowPager && Settings.ShowTopPager;
                pagerBottom.Visible = canShowPager && Settings.ShowBottomPager;

                buttonShowMore.Visible = Settings.UseShowMore && (totalItems - pageSize) > 0;
            }
            else {
                if (IsEditable) {
                    // hide module content
                    panelStream.Visible = false;
                    // display message for editors
                    this.Message ("NothingToDisplay.Text", MessageType.Info, true);
                }
                else {
                    // hide module from non-editors
                    ContainerControl.Visible = false;
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

            ToggleStreamControls (page.TotalItems, PageSize);

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

            ToggleStreamControls (page.TotalItems, PageSize);

            if (page.TotalItems > 0) {
                // bind the data
                listStream.DataSource = page.Page;
                listStream.DataBind ();
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
            BindChildControls ((StreamNewsEntryViewModel) e.Item.DataItem, e.Item);
        }
    }
}
