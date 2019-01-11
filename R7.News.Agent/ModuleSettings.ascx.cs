//
//  SettingsAgent.ascx.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016-2019 Roman M. Yagodin
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
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using R7.Dnn.Extensions.Modules;
using R7.Dnn.Extensions.ViewModels;
using R7.News.Agent.Components;
using R7.News.Data;
using R7.Dnn.Extensions.Controls;
using R7.Dnn.Extensions.Text;

namespace R7.News.Agent
{
    public partial class ModuleSettings : ModuleSettingsBase<AgentSettings>
    {
        ViewModelContext viewModelContext;
        protected ViewModelContext ViewModelContext {
            get { return viewModelContext ?? (viewModelContext = new ViewModelContext (this)); }
        }

        protected override void OnInit (EventArgs e)
        {
            comboGroupEntry.DataSource = NewsRepository.Instance.GetNewsEntriesByAgent (ModuleId, PortalId);
            comboGroupEntry.DataBind ();
            comboGroupEntry.InsertDefaultItem (LocalizeString ("NotSelected.Text"));
            comboGroupEntry.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles the loading of the module setting for this control
        /// </summary>
        public override void LoadSettings ()
        {
            try {
                if (!IsPostBack) {
                    checkEnableGrouping.Checked = Settings.EnableGrouping;
                    comboGroupEntry.SelectByValue (Settings.GroupEntryId);
                    checkHideImages.Checked = Settings.HideImages;
                    textThumbnailWidth.Text = Settings.ThumbnailWidth.ToString ();
                    textGroupThumbnailWidth.Text = Settings.GroupThumbnailWidth.ToString ();
                }
            }
            catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }

        /// <summary>
        /// handles updating the module settings for this control
        /// </summary>
        public override void UpdateSettings ()
        {
            try {
                Settings.EnableGrouping = checkEnableGrouping.Checked;
                Settings.GroupEntryId = ParseHelper.ParseToNullable<int> (comboGroupEntry.SelectedValue);
                Settings.HideImages = checkHideImages.Checked;
                Settings.ThumbnailWidth = int.Parse (textThumbnailWidth.Text);
                Settings.GroupThumbnailWidth = int.Parse (textGroupThumbnailWidth.Text);

                SettingsRepository.SaveSettings (ModuleConfiguration, Settings);

                ModuleController.SynchronizeModule (ModuleId);
            }
            catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }
    }
}

