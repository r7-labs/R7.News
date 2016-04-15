//
//  SettingsStream.ascx.cs
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
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using R7.DotNetNuke.Extensions.ControlExtensions;
using R7.DotNetNuke.Extensions.Modules;
using R7.News.Components;
using R7.News.Data;
using R7.News.Stream.Components;

namespace R7.News.Stream
{
    public partial class ModuleSettings : ModuleSettingsBase<StreamSettings>
    {
        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            // fill weight comboboxes
            for (var i = 0; i <= NewsConfig.Instance.NewsEntry.MaxWeight; i++) {
                comboMinThematicWeight.Items.Add (i.ToString ());
                comboMaxThematicWeight.Items.Add (i.ToString ());
                comboMinStructuralWeight.Items.Add (i.ToString ());
                comboMaxStructuralWeight.Items.Add (i.ToString ());
            }
        }

        /// <summary>
        /// Handles the loading of the module setting for this control
        /// </summary>
        public override void LoadSettings ()
        {
            try {
                if (!IsPostBack) {

                    textThumbnailWidth.Text = Settings.ThumbnailWidth.ToString ();

                    checkUseShowMore.Checked = Settings.UseShowMore;
                    checkShowTopPager.Checked = Settings.ShowTopPager;
                    checkShowBottomPager.Checked = Settings.ShowBottomPager;
                    textPageSize.Text = Settings.PageSize.ToString ();
                    textMaxPageLinks.Text = Settings.MaxPageLinks.ToString ();

                    checkShowAllNews.Checked = Settings.ShowAllNews;
                    termsIncludeTerms.PortalId = PortalId;
                    termsIncludeTerms.Terms = Settings.IncludeTerms;
                    termsIncludeTerms.DataBind ();

                    comboMinThematicWeight.SelectByValue (Settings.MinThematicWeight);
                    comboMaxThematicWeight.SelectByValue (Settings.MaxThematicWeight);
                    comboMinStructuralWeight.SelectByValue (Settings.MinStructuralWeight);
                    comboMaxStructuralWeight.SelectByValue (Settings.MaxStructuralWeight);
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

                Settings.ThumbnailWidth = int.Parse (textThumbnailWidth.Text);

                Settings.UseShowMore = checkUseShowMore.Checked;
                Settings.ShowTopPager = checkShowTopPager.Checked;
                Settings.ShowBottomPager = checkShowBottomPager.Checked;
                Settings.PageSize = int.Parse (textPageSize.Text);
                Settings.MaxPageLinks = int.Parse (textMaxPageLinks.Text);

                Settings.ShowAllNews = checkShowAllNews.Checked;
                Settings.IncludeTerms = termsIncludeTerms.Terms;

                var minThematicWeight = int.Parse (comboMinThematicWeight.SelectedValue);
                var maxThematicWeight = int.Parse (comboMaxThematicWeight.SelectedValue);
                var minStructuralWeight = int.Parse (comboMinStructuralWeight.SelectedValue);
                var maxStructuralWeight = int.Parse (comboMaxStructuralWeight.SelectedValue);

                // HACK: Implement custom validator for this
                if (minThematicWeight > maxThematicWeight) {
                    minThematicWeight = maxThematicWeight;
                }

                if (minStructuralWeight > maxStructuralWeight) {
                    minStructuralWeight = maxStructuralWeight;
                }

                Settings.MinThematicWeight = minThematicWeight;
                Settings.MaxThematicWeight = maxThematicWeight;
                Settings.MinStructuralWeight = minStructuralWeight;
                Settings.MaxStructuralWeight = maxStructuralWeight;

                // clear module-specific cache
                NewsRepository.Instance.RemoveModuleCache (ModuleId);

                ModuleController.SynchronizeModule (ModuleId);
            }
            catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }

        protected void buttonImport_Click (object sender, EventArgs e)
        {
            try {
                var importer = new R7.News.Integrations.AnnoView.Importer ();
                var itemsImported = importer.Import (ModuleId, TabId, PortalId);
                buttonImport.Text = string.Format ("Imported {0} items", itemsImported);
                buttonImport.Enabled = false;
            }
            catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }
    }
}

