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
using R7.News.Stream.Components;
using DotNetNuke.R7.Entities.Modules;

namespace R7.News.Stream
{
    public partial class ModuleSettings : ModuleSettingsBase<StreamSettings>
    {
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

                ModuleController.SynchronizeModule (ModuleId);
            }
            catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }
    }
}

