//
//  ViewNewsEntry.ascx.cs
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
using System.Net;
using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke.Services.Exceptions;
using R7.DotNetNuke.Extensions.Entities.Modules;
using R7.DotNetNuke.Extensions.ModuleExtensions;
using R7.DotNetNuke.Extensions.ViewModels;
using R7.News.Controls;
using R7.News.Data;
using R7.News.Models;
using R7.News.Stream.Components;
using R7.News.Stream.ViewModels;

namespace R7.News.Stream
{
    public partial class ViewNewsEntry : PortalModuleBase<StreamSettings>
    {
        #region Properties
        
        ViewModelContext<StreamSettings> viewModelContext;
        protected ViewModelContext<StreamSettings> ViewModelContext
        {
            get { return viewModelContext ?? (viewModelContext = new ViewModelContext<StreamSettings> (this, Settings)); }
        }

        int? entryId;
        protected int? EntryId
        {
            get
            {
                if (entryId == null) {
                    var strEntryId = Request.QueryString ["entryId"];
                    int outEntryId;
                    if (int.TryParse (strEntryId, out outEntryId)) {
                        entryId = outEntryId;
                    }
                }

                return entryId;
            }
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
                    // get news entry
                    NewsEntryInfo newsEntry = null;
                    if (EntryId != null) {
                        newsEntry = NewsRepository.Instance.GetNewsEntry (EntryId.Value, PortalId);
                    }

                    if (newsEntry != null) {
                        var newsEntries = new List<StreamModuleNewsEntryViewModel> ();
                        newsEntries.Add (new StreamModuleNewsEntryViewModel (newsEntry, ViewModelContext));

                        formNewsEntry.DataSource = newsEntries;
                        formNewsEntry.DataBind ();
                    }
                    else {
                        if (!IsEditable) {
                            // throw 404 exception, which would result in showing 404 page
                            throw new HttpException ((int) HttpStatusCode.NotFound, string.Empty);
                        }

                        // display warning for editors instead
                        this.Message ("NewsEntryNotFound.Text", MessageType.Warning, true);
                    }
                }
            }
            catch (HttpException ex) {
                Exceptions.ProcessHttpException (ex);
            }
            catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }

        protected void formNewsEntry_DataBound (object sender, EventArgs e)
        {
            var item = (StreamModuleNewsEntryViewModel) formNewsEntry.DataItem;

            var linkEdit = (HyperLink) formNewsEntry.FindControl ("linkEdit");
            var iconEdit = (Image) formNewsEntry.FindControl ("imageEdit");

            // edit link
            if (IsEditable) {
                linkEdit.NavigateUrl = EditUrl ("entryid", item.EntryId.ToString (), "EditNewsEntry");
            }

            // make edit link visible in edit mode
            linkEdit.Visible = IsEditable;
            iconEdit.Visible = IsEditable;

            // visibility badges
            var listBadges = (BadgeList) formNewsEntry.FindControl ("listBadges");
            listBadges.DataSource = item.Badges;
            listBadges.DataBind ();

            // show image
            var imageImage = (Image) formNewsEntry.FindControl ("imageImage");
            imageImage.Visible = item.GetImage () != null;

            // show term links
            var termLinks = (TermLinks) formNewsEntry.FindControl ("termLinks");
            termLinks.Module = this;
            termLinks.DataSource = item.ContentItem.Terms;
            termLinks.DataBind ();
        }
    }

    #endregion
}