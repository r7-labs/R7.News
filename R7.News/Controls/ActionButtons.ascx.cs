//
//  ActionButtons.ascx.cs
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

using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Portals;
using R7.News.Components;
using R7.News.Controls.ViewModels;
using DotNetNuke.Entities.Users;

namespace R7.News.Controls
{
    public class ActionButtons: UserControl
    {
        #region Controls

        protected ListView listActionButtons;

        #endregion

        #region Public properties

        public string CssClass { get; set; }

        public List<NewsEntryAction> DataSource { get; set; }

        #endregion

        public override void DataBind ()
        {
            if (DataSource != null) {
                listActionButtons.DataSource = DataSource;
                listActionButtons.DataBind ();
            }

            base.DataBind ();
        }

        protected void linkActionButton_Command (object sender, CommandEventArgs e)
        {
            var actionHandler = new ActionHandler ();
            actionHandler.ExecuteAction (actionKey: e.CommandName,
                                         entryId: int.Parse ((string) e.CommandArgument),
                                         portalId: PortalSettings.Current.PortalId,
                                         // TODO: Get superuser
                                         userId: 1); // as superuser
        }
    }
}

