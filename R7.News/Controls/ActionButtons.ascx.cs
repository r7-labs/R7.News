//
//  ActionButtons.ascx.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2017-2019 Roman M. Yagodin
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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.UI.Modules;
using R7.Dnn.Extensions.ViewModels;
using R7.News.Components;
using R7.News.Controls.Models;
using R7.News.Controls.ViewModels;
using R7.Dnn.Extensions.Controls;

namespace R7.News.Controls
{
    public class ActionButtons: UserControl
    {
        #region Controls

        protected ListView listActionButtons;

        #endregion

        #region Public properties

        public string CssClass { get; set; }

        public IList<NewsEntryAction> Actions { get; set; }

        #endregion

        ViewModelContext viewModelContext;
        ViewModelContext ViewModelContext {
            get { return viewModelContext ?? (viewModelContext = new ViewModelContext (this, this.FindParentOfType<IModuleControl> ())); }
        }

        public override void DataBind ()
        {
            if (Actions != null && Actions.Count > 0) {
                listActionButtons.DataSource = Actions.Select (a => new NewsEntryActionViewModel (a, ViewModelContext));
            }

            base.DataBind ();
        }

        protected void linkActionButton_Command (object sender, CommandEventArgs e)
        {
            var actionHandler = new ActionHandler ();
            var action = JsonExtensionsWeb.FromJson<NewsEntryAction> ((string) e.CommandArgument);
            actionHandler.ExecuteAction (action,
                                         portalId: PortalSettings.Current.PortalId,
                                         // TODO: Get superuser
                                         userId: 1); // as superuser
        }
    }
}

