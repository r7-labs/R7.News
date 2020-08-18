using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Icons;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Security;
using R7.Dnn.Extensions.Modules;
using R7.Dnn.Extensions.Text;
using R7.News.Controls;
using R7.News.Data;
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
            var listBadges = (BadgeList) itemControl.FindControl ("listBadges");
            if (item.Badges != null && item.Badges.Count > 0) {
                listBadges.DataSource = item.Badges;
                listBadges.DataBind ();
            }
            else {
                listBadges.Visible = false;
            }

            var termLinks = (TermLinks) itemControl.FindControl ("termLinks");
            if (item.ContentItem.Terms.Count > 0) {
                termLinks.Module = this;
                termLinks.DataSource = item.ContentItem.Terms;
                termLinks.DataBind ();
            }
            else {
                termLinks.Visible = false;
            }

            var actionButtons = ((ActionsControl) itemControl.FindControl ("ctlActions")).ActionButtons;
            var actions = item.GetActions (ModuleId);
            if (actions.Count > 0) {
                actionButtons.Actions = actions;
                actionButtons.DataBind ();
            } else {
                actionButtons.Visible = false;
            }
        }
    }
}
