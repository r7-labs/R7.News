//
//  StreamSettings.cs
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
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Modules;
using DotNetNuke.UI.Modules;
using R7.DotNetNuke.Extensions.Modules;
using R7.DotNetNuke.Extensions.Utilities;
using R7.News.Components;

namespace R7.News.Stream.Components
{
    /// <summary>
    /// Provides strong typed access to settings used by module
    /// </summary>
    public class StreamSettings : SettingsWrapper
    {
        protected const string SettingPrefix = Const.Prefix + "_Stream_";

        public StreamSettings ()
        {
        }

        public StreamSettings (IModuleControl module) : base (module)
        {
        }

        public StreamSettings (ModuleInfo module) : base (module)
        {
        }

        #region Module settings

        public List<Term> IncludeTerms
        {
            get { 
                var termController = new TermController ();

                var termIds = ReadSetting<string> (SettingPrefix + "IncludeTerms", string.Empty)
                    .Split (new [] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select (ti => int.Parse (ti));

                var terms = new List<Term> ();
                foreach (var termId in termIds) {
                    var term = termController.GetTerm (termId);
                    if (term != null) {
                        terms.Add (term);
                    }
                }

                return terms;
            }

            set {
                WriteModuleSetting<string> (SettingPrefix + "IncludeTerms", 
                    TextUtils.FormatList (";", value.Select (t => t.TermId)));
            }
        }

        public bool ShowAllNews
        {
            get { return ReadSetting<bool> (SettingPrefix + "ShowAllNews", false); }
            set { WriteModuleSetting<bool> (SettingPrefix + "ShowAllNews", value); }
        }

        // REVIEW: Separate config settings for weight filters
        public int MinThematicWeight
        {
            get { return ReadSetting<int> (SettingPrefix + "MinThematicWeight", 0); }
            set { WriteModuleSetting<int> (SettingPrefix + "MinThematicWeight", value); }
        }

        public int MaxThematicWeight
        {
            get { return ReadSetting<int> (
                    SettingPrefix + "MaxThematicWeight",
                    NewsConfig.GetInstance (PortalId).NewsEntry.MaxWeight); }
            set { WriteModuleSetting<int> (SettingPrefix + "MaxThematicWeight", value); }
        }

        public int MinStructuralWeight
        {
            get { return ReadSetting<int> (SettingPrefix + "MinStructuralWeight", 0); }
            set { WriteModuleSetting<int> (SettingPrefix + "MinStructuralWeight", value); }
        }

        public int MaxStructuralWeight
        {
            get { return ReadSetting<int> (
                    SettingPrefix + "MaxStructuralWeight",
                    NewsConfig.GetInstance (PortalId).NewsEntry.MaxWeight); }
            set { WriteModuleSetting<int> (SettingPrefix + "MaxStructuralWeight", value); }
        }

        #endregion

        #region Tab-specific module settings

        public bool UseShowMore
        {
            get { return ReadSetting<bool> (SettingPrefix + "UseShowMore", false); }
            set { WriteTabModuleSetting<bool> (SettingPrefix + "UseShowMore", value); }
        }

        public bool ShowTopPager
        {
            get { return ReadSetting<bool> (SettingPrefix + "ShowTopPager", true); }
            set { WriteTabModuleSetting<bool> (SettingPrefix + "ShowTopPager", value); }
        }

        public bool ShowBottomPager
        {
            get { return ReadSetting<bool> (SettingPrefix + "ShowBottomPager", true); }
            set { WriteTabModuleSetting<bool> (SettingPrefix + "ShowBottomPager", value); }
        }

        public int PageSize
        {
            get { return ReadSetting<int> (SettingPrefix + "PageSize", 3); }
            set { WriteTabModuleSetting<int> (SettingPrefix + "PageSize", value); }
        }

        public int MaxPageLinks
        {
            get { return ReadSetting<int> (SettingPrefix + "MaxPageLinks", 3); }
            set { WriteTabModuleSetting<int> (SettingPrefix + "MaxPageLinks", value); }
        }

        public int ThumbnailWidth
        {
            get { 
                return ReadSetting<int> (SettingPrefix + "ThumbnailWidth", 
                    NewsConfig.GetInstance (PortalId).StreamModule.DefaultThumbnailWidth); 
            }
            set { WriteTabModuleSetting<int> (SettingPrefix + "ThumbnailWidth", value); }
        }

        #endregion
    }
}

