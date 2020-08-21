using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
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

        public IList<NewsEntryAction> Actions { get; set; }

        #endregion

        ViewModelContext viewModelContext;
        protected ViewModelContext DnnContext {
            get { return viewModelContext ?? (viewModelContext = new ViewModelContext (this, this.FindParentOfType<IModuleControl> ())); }
        }

        public override void DataBind ()
        {
            if (Actions != null && Actions.Count > 0) {
                listActionButtons.DataSource = Actions.Select (a => new NewsEntryActionViewModel (a, DnnContext));
            }

            base.DataBind ();
        }

        protected void linkActionButton_Command (object sender, CommandEventArgs e)
        {
            // Cannot use DnnContext here?
            var actionHandler = new ActionHandler ();
            var action = JsonExtensionsWeb.FromJson<NewsEntryAction> ((string) e.CommandArgument);
            actionHandler.ExecuteAction (action, PortalSettings.Current.PortalId, PortalSettings.Current.ActiveTab.TabID, GetSuperUserId ());
        }

        protected int GetSuperUserId ()
        {
            var superUsers = UserController.GetUsers (false, true, -1);
            return ((UserInfo) superUsers [0]).UserID;
        }
    }
}
