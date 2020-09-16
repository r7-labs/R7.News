﻿//
//  ModuleSettings.ascx.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016-2020 Roman M. Yagodin
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
using R7.Dnn.Extensions.Controls;
using R7.Dnn.Extensions.Modules;
using R7.Dnn.Extensions.Text;
using R7.News.Components;
using R7.News.Data;
using R7.News.Stream.Models;

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

            rblPagerShowFirstLast.AddItem (LocalizeString ("DefaultValue.Text"), "null");
            rblPagerShowFirstLast.AddItem (LocalizeString ("Yes"), "true");
            rblPagerShowFirstLast.AddItem (LocalizeString ("No"), "false");
            rblPagerShowFirstLast.SelectedIndex = 0;

            rblPagerShowStatus.AddItem (LocalizeString ("DefaultValue.Text"), "null");
            rblPagerShowStatus.AddItem (LocalizeString ("Yes"), "true");
            rblPagerShowStatus.AddItem (LocalizeString ("No"), "false");
            rblPagerShowStatus.SelectedIndex = 0;
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

                    chkEnableFeed.Checked = Settings.EnableFeed;
                    txtFeedMaxEntries.Text = Settings.FeedMaxEntries?.ToString ();

                    txtImageCssClass.Text = Settings.ImageCssClass;
                    txtTextCssClass.Text = Settings.TextCssClass;
                    txtImageColumnCssClass.Text = Settings.ImageColumnCssClass;
                    txtTextColumnCssClass.Text = Settings.TextColumnCssClass;

                    if (Settings.PagerShowFirstLast != null) {
                        rblPagerShowFirstLast.SelectedIndex = Settings.PagerShowFirstLast.Value ? 1 : 2;
                    }

                    if (Settings.PagerShowStatus != null) {
                        rblPagerShowStatus.SelectedIndex = Settings.PagerShowStatus.Value ? 1 : 2;
                    }
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

                Settings.ThumbnailWidth = ParseHelper.ParseToNullable<int> (textThumbnailWidth.Text);

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

                // TODO: Implement custom validator for this?
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

                Settings.EnableFeed = chkEnableFeed.Checked;
                Settings.FeedMaxEntries = ParseHelper.ParseToNullable<int> (txtFeedMaxEntries.Text);

                Settings.ImageCssClass = !string.IsNullOrEmpty (txtImageCssClass.Text) ? txtImageCssClass.Text : null;
                Settings.TextCssClass = !string.IsNullOrEmpty (txtTextCssClass.Text) ? txtTextCssClass.Text : null;
                Settings.ImageColumnCssClass = !string.IsNullOrEmpty (txtImageColumnCssClass.Text) ? txtImageColumnCssClass.Text : null;
                Settings.TextColumnCssClass = !string.IsNullOrEmpty (txtTextColumnCssClass.Text) ? txtTextColumnCssClass.Text : null;

                Settings.PagerShowStatus = ParseHelper.ParseToNullable<bool> (rblPagerShowStatus.SelectedValue);
                Settings.PagerShowFirstLast = ParseHelper.ParseToNullable<bool> (rblPagerShowFirstLast.SelectedValue);

                SettingsRepository.SaveSettings (ModuleConfiguration, Settings);

                NewsRepository.Instance.ClearModuleCache (ModuleId, TabModuleId);

                ModuleController.SynchronizeModule (ModuleId);
            }
            catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }
    }
}

