using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Icons;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using R7.Dnn.Extensions.Text;
using R7.Dnn.Extensions.ViewModels;
using R7.News.Agent.Models;
using R7.News.Agent.ViewModels;
using R7.News.Components;
using R7.News.Controls;
using R7.News.Data;
using R7.News.Models;
using R7.News.Modules;

namespace R7.News.Agent
{
    public partial class ViewAgent : NewsModuleBase<AgentSettings>, IActionable
    {
        #region Properties

        ViewModelContext<AgentSettings> viewModelContext;
        protected ViewModelContext<AgentSettings> ViewModelContext {
            get { return viewModelContext ?? (viewModelContext = new ViewModelContext<AgentSettings> (this, Settings)); }
        }

        #endregion

        #region Handlers

        protected override void OnInit (EventArgs e)
        {
            AddActionHandler (OnAction);
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
                    Bind ();
                }
            } catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }

        #endregion

        protected void Bind ()
        {
            var items = NewsRepository.Instance.GetNewsEntriesByAgent (ModuleId, PortalId);

            // check if we have some content to display,
            // otherwise display a message for module editors.
            if (items == null || !items.Any ()) {
                // show panel with add button
                if (IsEditable) {
                    panelAddDefaultEntry.Visible = true;
                } else {
                    // hide module from non-editors
                    ContainerControl.Visible = false;
                }
            } else {
                var now = HttpContext.Current.Timestamp;
                var agentModuleConfig = NewsConfig.Instance.AgentModule;

                // create viewmodels
                var viewModels = items
                    .Where (ne => ne.IsPublished (now) || IsEditable)
                    .OrderByDescending (ne => ne.PublishedOnDate ())
                    .Select (ne => new AgentNewsEntryViewModel (ne, ViewModelContext, agentModuleConfig))
                    .ToList ();

                if (viewModels.Count > 0) {
                    viewModels [0].IsTopEntry = true;
                }

                // bind the data
                listAgent.DataSource = viewModels;
                listAgent.DataBind ();

                agplSignature.Visible = listAgent.Items.Count > 1;
            }
        }

        #region IActionable implementation

        public override ModuleActionCollection ModuleActions {
            get {
                var actions = base.ModuleActions;

                actions.Add (
                    GetNextActionID (),
                    LocalizeString ("CreateFromPageData.Action"),
                    ModuleActionType.AddContent + "_CreateFromPageData",
                    "",
                    IconController.IconURL ("Add"),
                    "",
                    true,
                    SecurityAccessLevel.Edit,
                    true,
                    false
                );

                return actions;
            }
        }

        #endregion

        protected void OnAction (object sender, ActionEventArgs e)
        {
            if (e.Action.CommandName == ModuleActionType.AddContent + "_CreateFromPageData") {
                CreateFromPageData ();
            }
        }

        protected void buttonCreateFromPageData_Click (object sender, EventArgs e)
        {
            CreateFromPageData ();
        }

        protected void CreateFromPageData ()
        {
            var newsEntry = new TabSynchronizer ().AddNewsEntryFromTabData (PortalSettings.ActiveTab, ModuleId);
            UpdateModuleTitle (newsEntry.Title);

            Response.Redirect (Globals.NavigateURL (), true);
        }

        /// <summary>
        /// Handles the items being bound to the listview control. In this method we merge the data with the
        /// template defined for this control to produce the result to display to the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void listAgent_ItemDataBound (object sender, ListViewItemEventArgs e)
        {
            var item = (AgentNewsEntryViewModel) e.Item.DataItem;

            BindChildControls (item, e.Item);
        }

        protected void UpdateModuleTitle (string title)
        {
            var moduleController = NewsDataProvider.Instance.ModuleController;
            var module = moduleController.GetModule (ModuleId, TabId, true);
            if (module.ModuleTitle != title) {
                module.ModuleTitle = title;
                moduleController.UpdateModule (module);
            }
        }
    }
}

