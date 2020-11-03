using System;
using System.Web.UI.WebControls;
using DotNetNuke.Services.Exceptions;
using R7.Dnn.Extensions.Modules;
using R7.Dnn.Extensions.ViewModels;
using R7.Dnn.Extensions.Controls.PagingControl;
using R7.News.Modules;
using R7.News.ViewModels;
using R7.News.Stream.ViewModels;
using R7.News.Stream.Models;
using R7.News.Components;

namespace R7.News.Stream
{
    public partial class ViewStream: NewsModuleBase<StreamSettings>
    {
        StreamViewModel viewModel;

        protected StreamViewModel ViewModel
        {
            get { return viewModel ?? (viewModel = new StreamViewModel (this, Settings)); }
        }

        ViewModelContext<StreamSettings> viewModelContext;
        protected ViewModelContext<StreamSettings> ViewModelContext {
            get { return viewModelContext ?? (viewModelContext = new ViewModelContext<StreamSettings> (this, Settings)); }
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
                InitPager (pagerTop);
                pagerTop.CssClass = "news-pager news-pager-top";
            }
            else {
                pagerTop.Visible = false;
            }

            // setup bottom paging control
            if (Settings.ShowBottomPager) {
                InitPager (pagerBottom);
                pagerBottom.CssClass = "news-pager news-pager-bottom";
            }
            else {
                pagerBottom.Visible = false;
            }

            // hide horizontal rule in signature if "Show more" button is used
            agplSignature.ShowRule = !Settings.UseShowMore;
        }

        protected void InitPager (PagingControl pager)
        {
            pager.CurrentPage = CurrentPage;
            pager.TabID = TabId;
            pager.PageSize = PageSize;
            pager.PageLinksPerPage = Settings.MaxPageLinks;
            pager.Mode = PagingControlMode.PostBack;
            pager.QuerystringParams = "pagingModuleId=" + ModuleId;

            pager.ShowStatus = Settings.PagerShowStatus ?? NewsConfig.Instance.StreamModule.PagerShowStatus;
            pager.ShowFirstLast = Settings.PagerShowFirstLast ?? NewsConfig.Instance.StreamModule.PagerShowFirstLast;

            pager.ListCssClass = "pagination";
            pager.ItemCssClass = "page-item";
            pager.LinkCssClass = "page-link";
            pager.StatusCssClass = "news-pager-status";
            pager.CurrentItemCssClass = "active";
            pager.InactiveItemCssClass = "disabled";

            pager.PrevText = LocalizeString ("Pager_Prev.Text");
            pager.NextText = LocalizeString ("Pager_Next.Text");
            pager.FirstText = LocalizeString ("Pager_First.Text");
            pager.LastText = LocalizeString ("Pager_Last.Text");
            pager.CurrentText = LocalizeString ("Pager_Current.Text");
            pager.AriaLabel = LocalizeString ("Pager_ArialLabel.Text");
            pager.StatusFormat = LocalizeString ("Pager_StatusFormat.Text");
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
            BindChildControls ((NewsEntryViewModelBase) e.Item.DataItem, e.Item);
        }
    }
}
