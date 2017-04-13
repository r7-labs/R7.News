//
//  NewsModuleBase.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2017 Roman M. Yagodin
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

using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Icons;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using R7.DotNetNuke.Extensions.Modules;
using R7.News.Controls;
using R7.News.Models;
using R7.News.ViewModels;

namespace R7.News.Modules
{
    public abstract class NewsModuleBase<TSettings>: PortalModuleBase<TSettings>, IActionable
        where TSettings: class, new ()
    {
        #region IActionable implementation

        public virtual ModuleActionCollection ModuleActions {
            get {
                var actions = new ModuleActionCollection ();
                actions.Add (
                    GetNextActionID (),
                    LocalizeString ("AddNewsEntry.Action"),
                    ModuleActionType.AddContent,
                    "",
                    IconController.IconURL ("Add"),
                    EditUrl ("EditNewsEntry"),
                    false,
                    SecurityAccessLevel.Edit,
                    true,
                    false
                );

                return actions;
            }
        }

        #endregion

        protected void BindChildControls (NewsEntryViewModelBase item, Control itemControl)
        {
            var linkEdit = (HyperLink) itemControl.FindControl ("linkEdit");
            var iconEdit = (Image) itemControl.FindControl ("imageEdit");

            // edit link
            if (IsEditable) {
                linkEdit.NavigateUrl = EditUrl ("entryid", item.EntryId.ToString (), "EditNewsEntry");
            }

            // make edit link visible in edit mode
            linkEdit.Visible = IsEditable;
            iconEdit.Visible = IsEditable;

            // visibility badges
            var listBadges = (BadgeList) itemControl.FindControl ("listBadges");
            if (item.Badges != null && item.Badges.Count > 0) {
                listBadges.DataSource = item.Badges;
                listBadges.DataBind ();
            } else {
                listBadges.Visible = false;
            }

            // show term links
            var termLinks = (TermLinks) itemControl.FindControl ("termLinks");
            if (item.ContentItem.Terms.Count > 0) {
                termLinks.Module = this;
                termLinks.DataSource = item.ContentItem.Terms;
                termLinks.DataBind ();
            } else {
                termLinks.Visible = false;
            }

            // action buttons
            var actionButtons = (ActionButtons) itemControl.FindControl ("actionButtons");
            var actions = item.GetActions ();

            if (actions.Count > 0) {
                actionButtons.Actions = actions;
                actionButtons.DataBind ();
            } else {
                actionButtons.Visible = false;
            }
        }
    }
}
