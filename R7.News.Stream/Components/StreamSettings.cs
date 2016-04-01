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
using System.Linq;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuke.UI.Modules;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.R7;
using DotNetNuke.R7.Entities.Modules;

namespace R7.News.Stream.Components
{
    /// <summary>
    /// Provides strong typed access to settings used by module
    /// </summary>
    public class StreamSettings : SettingsWrapper
    {
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
            get
            { 
                var termController = new TermController ();

                return ReadSetting<string> ("r7_News_Stream_IncludeTerms", string.Empty)
                    .Split (new [] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select (ti => termController.GetTerm (int.Parse (ti)))
                    .ToList ();
            }

            set
            {
                WriteModuleSetting<string> ("r7_News_Stream_IncludeTerms", 
                    TextUtils.FormatList (";", value.Select (t => t.TermId)));
            }
        }

        public bool ShowAllNews
        {
            get { return ReadSetting<bool> ("r7_News_Stream_ShowAllNews", false); }
            set { WriteModuleSetting<bool> ("r7_News_Stream_ShowAllNews", value); }
        }

        public int MinThematicWeight
        {
            // TODO: Define default values in config
            get { return ReadSetting<int> ("r7_News_Stream_MinThematicWeight", 0); }
            set { WriteModuleSetting<int> ("r7_News_Stream_MinThematicWeight", value); }
        }

        public int MinStructuralWeight
        {
            // TODO: Define default values in config
            get { return ReadSetting<int> ("r7_News_Stream_MinStructuralWeight", 0); }
            set { WriteModuleSetting<int> ("r7_News_Stream_MinStructuralWeight", value); }
        }

        public int MaxThematicWeight
        {
            // TODO: Define default values in config
            get { return ReadSetting<int> ("r7_News_Stream_MaxThematicWeight", 10); }
            set { WriteModuleSetting<int> ("r7_News_Stream_MaxThematicWeight", value); }
        }

        public int MaxStructuralWeight
        {
            // TODO: Define default values in config
            get { return ReadSetting<int> ("r7_News_Stream_MaxStructuralWeight", 10); }
            set { WriteModuleSetting<int> ("r7_News_Stream_MaxStructuralWeight", value); }
        }

        #endregion

        #region Tab-specific module settings

        public bool UseShowMore
        {
            get { return ReadSetting<bool> ("r7_News_Stream_UseShowMore", false); }
            set { WriteTabModuleSetting<bool> ("r7_News_Stream_UseShowMore", value); }
        }

        public bool ShowTopPager
        {
            get { return ReadSetting<bool> ("r7_News_Stream_ShowTopPager", true); }
            set { WriteTabModuleSetting<bool> ("r7_News_Stream_ShowTopPager", value); }
        }

        public bool ShowBottomPager
        {
            get { return ReadSetting<bool> ("r7_News_Stream_ShowBottomPager", true); }
            set { WriteTabModuleSetting<bool> ("r7_News_Stream_ShowBottomPager", value); }
        }

        public int PageSize
        {
            get { return ReadSetting<int> ("r7_News_Stream_PageSize", 3); }
            set { WriteTabModuleSetting<int> ("r7_News_Stream_PageSize", value); }
        }

        public int MaxPageLinks
        {
            get { return ReadSetting<int> ("r7_News_Stream_MaxPageLinks", 3); }
            set { WriteTabModuleSetting<int> ("r7_News_Stream_MaxPageLinks", value); }
        }

        public int ThumbnailWidth
        {
            // TODO: Get default thumbnail width from config
            get { return ReadSetting<int> ("r7_News_Stream_ThumbnailWidth", 192); }
            set { WriteTabModuleSetting<int> ("r7_News_Stream_ThumbnailWidth", value); }
        }

        #endregion
    }
}

