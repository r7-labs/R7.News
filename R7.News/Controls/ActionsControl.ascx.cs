using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.UI.Modules;
using R7.Dnn.Extensions.ViewModels;
using R7.Dnn.Extensions.Controls;
using R7.News.Components;
using R7.News.Models;
using R7.News.Controls.Models;

namespace R7.News.Controls
{
    // TODO: Rename to EditActionsControl or just EditActions?
    public class ActionsControl: UserControl
    {
        public int EntryId { get; set; }

        ViewModelContext dnnContext;
        ViewModelContext DnnContext {
            get { return dnnContext ?? (dnnContext = new ViewModelContext (this, this.FindParentOfType<IModuleControl> ())); }
        }

        public ActionButtons ActionButtons => (ActionButtons) FindControl ("actionButtons");

        public bool ShowSyncTabAction { get; set; }

        public bool ShowDuplicateAction { get; set; }

        protected string LocalizeString (string text)
        {
            return DnnContext.LocalizeString (text);
        }

        protected string EditUrl ()
        {
            return DnnContext.Module.EditUrl ("entryid", EntryId.ToString (), "EditNewsEntry");
        }

        protected NewsEntryAction DuplicateAction => new NewsEntryAction {
            EntryId = EntryId,
            Action = NewsEntryActions.Duplicate,
            Enabled = true,
            Params = new string [] { DnnContext.Module.ModuleId.ToString () }
        };

        protected NewsEntryAction SyncTabAction => new NewsEntryAction {
            EntryId = EntryId,
            Action = NewsEntryActions.SyncTab,
            Enabled = true
        };

        protected bool IsEditable => DnnContext.Module.IsEditable;

        protected void btnDuplicate_Command (object sender, CommandEventArgs e)
        {
            var actionHandler = new ActionHandler ();
            var action = JsonExtensionsWeb.FromJson<NewsEntryAction> ((string) e.CommandArgument);
            var moduleId = int.Parse (action.Params [0]);
            actionHandler.Duplicate (action.EntryId, PortalSettings.Current.PortalId, PortalSettings.Current.ActiveTab.TabID, moduleId);
        }

        protected void btnSyncTab_Command (object sender, CommandEventArgs e)
        {
            var actionHandler = new ActionHandler ();
            var action = JsonExtensionsWeb.FromJson<NewsEntryAction> ((string) e.CommandArgument);
            actionHandler.SyncTab (action.EntryId, PortalSettings.Current.PortalId, PortalSettings.Current.ActiveTab);
        }
    }
}

