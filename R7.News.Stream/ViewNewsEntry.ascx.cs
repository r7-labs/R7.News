//
//  ViewNewsEntry.ascx.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Exceptions;
using R7.Dnn.Extensions.ModuleExtensions;
using R7.Dnn.Extensions.Utilities;
using R7.Dnn.Extensions.ViewModels;
using R7.News.Data;
using R7.News.Models;
using R7.News.Modules;
using R7.News.Stream.Components;
using R7.News.Stream.ViewModels;

namespace R7.News.Stream
{
    public partial class ViewNewsEntry: NewsModuleBase<StreamSettings>
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
            get {
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

        public override ModuleActionCollection ModuleActions {
            get {
                return new ModuleActionCollection ();
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

                        ReplacePageTitleAndMeta (newsEntry);

                        var newsEntries = new List<StreamNewsEntryViewModel> ();
                        newsEntries.Add (new StreamNewsEntryViewModel (newsEntry, ViewModelContext));

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

        void ReplacePageTitleAndMeta (INewsEntry newsEntry)
        {
            var page = (DotNetNuke.Framework.CDefault) Page;
            page.Title = TextUtils.FormatList (" - ", page.Title, newsEntry.Title);
            page.Description = HtmlUtils.StripTags (HttpUtility.HtmlDecode (newsEntry.Description), false);

            if (newsEntry.ContentItem.Terms.Count > 0) {
                page.KeyWords = TextUtils.FormatList (",", newsEntry.ContentItem.Terms.Select (t => t.Name));
            }
        }

        protected void formNewsEntry_DataBound (object sender, EventArgs e)
        {
            BindChildControls ((StreamNewsEntryViewModel) formNewsEntry.DataItem, formNewsEntry);
        }
    }

    #endregion
}